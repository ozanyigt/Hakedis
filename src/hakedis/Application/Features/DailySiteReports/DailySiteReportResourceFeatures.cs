using Application.Features.DailySiteReports.Commands;
using Application.Features.DailySiteReports.Constants;
using Application.Features.DailySiteReports.Rules;
using Application.Services.Repositories;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Transaction;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.DailySiteReports;

public sealed class DailySiteReportMaterialLineInput
{
    public Guid MaterialId { get; set; }
    public decimal Quantity { get; set; }
    public string? Notes { get; set; }
}

public class ReplaceDailySiteReportMaterialLinesCommand :
    IRequest<IReadOnlyList<DailySiteReportMaterialLineDto>>, ISecuredRequest, ITransactionalRequest
{
    public Guid ReportId { get; set; }
    public Guid? TenantId { get; set; }
    public IReadOnlyList<DailySiteReportMaterialLineInput> Lines { get; set; } = [];
    public string[] Roles =>
        [DailySiteReportsOperationClaims.Admin, DailySiteReportsOperationClaims.Write, DailySiteReportsOperationClaims.Update];

    public class Handler(
        IDailySiteReportRepository reportRepository,
        IDailySiteReportMaterialLineRepository lineRepository,
        IMaterialRepository materialRepository,
        DailySiteReportBusinessRules rules)
        : IRequestHandler<ReplaceDailySiteReportMaterialLinesCommand, IReadOnlyList<DailySiteReportMaterialLineDto>>
    {
        public async Task<IReadOnlyList<DailySiteReportMaterialLineDto>> Handle(
            ReplaceDailySiteReportMaterialLinesCommand request, CancellationToken cancellationToken)
        {
            Guid? tenantId = await rules.ResolveTenantAsync(request.TenantId, false, cancellationToken);
            DailySiteReport report = await UpdateDailySiteReportCommand.GetReport(
                reportRepository, request.ReportId, tenantId, cancellationToken);
            DailySiteReportBusinessRules.EnsureEditable(report);
            if (request.Lines.Any(x => x.Quantity <= 0))
                throw new BusinessException("Material quantities must be greater than zero.");
            if (request.Lines.GroupBy(x => x.MaterialId).Any(x => x.Count() > 1))
                throw new BusinessException("A material can occur only once in a daily report.");

            Dictionary<Guid, Material> materials = new();
            foreach (DailySiteReportMaterialLineInput input in request.Lines)
            {
                Material? material = await materialRepository.GetAsync(
                        x => x.Id == input.MaterialId && x.TenantId == report.TenantId && x.IsActive,
                        enableTracking: false, cancellationToken: cancellationToken);
                if (material is null)
                    throw new BusinessException("Material does not exist in the report tenant.");
                materials[input.MaterialId] = material;
            }

            IPaginate<DailySiteReportMaterialLine> existing = await lineRepository.GetListAsync(
                x => x.DailySiteReportId == report.Id, size: 10000, cancellationToken: cancellationToken);
            foreach (DailySiteReportMaterialLine line in existing.Items)
                await lineRepository.DeleteAsync(line, permanent: true);
            foreach (DailySiteReportMaterialLineInput input in request.Lines)
            {
                Material material = materials[input.MaterialId];
                await lineRepository.AddAsync(new DailySiteReportMaterialLine
                {
                    DailySiteReportId = report.Id,
                    MaterialId = input.MaterialId,
                    MaterialCode = material.Code,
                    MaterialName = material.Name,
                    Unit = material.Unit,
                    Quantity = input.Quantity,
                    Notes = input.Notes?.Trim()
                });
            }
            return await LoadAsync(lineRepository, report.Id, cancellationToken);
        }
    }

    internal static async Task<IReadOnlyList<DailySiteReportMaterialLineDto>> LoadAsync(
        IDailySiteReportMaterialLineRepository repository, Guid reportId, CancellationToken cancellationToken)
    {
        IPaginate<DailySiteReportMaterialLine> page = await repository.GetListAsync(
            x => x.DailySiteReportId == reportId, include: q => q.Include(x => x.Material),
            orderBy: q => q.OrderBy(x => x.Material.Name), size: 10000, enableTracking: false,
            cancellationToken: cancellationToken);
        return page.Items.Select(x => new DailySiteReportMaterialLineDto
        {
            Id = x.Id,
            MaterialId = x.MaterialId,
            MaterialCode = x.MaterialCode,
            MaterialName = x.MaterialName,
            Unit = x.Unit,
            Quantity = x.Quantity,
            Notes = x.Notes,
            PostedUnitCost = x.PostedUnitCost,
            PostedTotalCost = x.PostedTotalCost
        }).ToList();
    }
}

public class GetDailySiteReportMaterialLinesQuery :
    IRequest<IReadOnlyList<DailySiteReportMaterialLineDto>>, ISecuredRequest
{
    public Guid ReportId { get; set; }
    public Guid? TenantId { get; set; }
    public string[] Roles => [DailySiteReportsOperationClaims.Admin, DailySiteReportsOperationClaims.Read];
    public class Handler(
        IDailySiteReportRepository reportRepository,
        IDailySiteReportMaterialLineRepository lineRepository,
        DailySiteReportBusinessRules rules)
        : IRequestHandler<GetDailySiteReportMaterialLinesQuery, IReadOnlyList<DailySiteReportMaterialLineDto>>
    {
        public async Task<IReadOnlyList<DailySiteReportMaterialLineDto>> Handle(
            GetDailySiteReportMaterialLinesQuery request, CancellationToken cancellationToken)
        {
            Guid? tenantId = await rules.ResolveTenantAsync(request.TenantId, false, cancellationToken);
            await UpdateDailySiteReportCommand.GetReport(
                reportRepository, request.ReportId, tenantId, cancellationToken);
            return await ReplaceDailySiteReportMaterialLinesCommand.LoadAsync(
                lineRepository, request.ReportId, cancellationToken);
        }
    }
}

public class PreviewDailySiteReportWorkforceQuery :
    IRequest<DailySiteReportWorkforceDto>, ISecuredRequest
{
    public Guid? TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid SiteId { get; set; }
    public DateTime ReportDate { get; set; }
    public string[] Roles => [DailySiteReportsOperationClaims.Admin, DailySiteReportsOperationClaims.Read];
    public class Handler(
        DailySiteReportBusinessRules rules, DailySiteReportSnapshotService snapshotService)
        : IRequestHandler<PreviewDailySiteReportWorkforceQuery, DailySiteReportWorkforceDto>
    {
        public async Task<DailySiteReportWorkforceDto> Handle(
            PreviewDailySiteReportWorkforceQuery request, CancellationToken cancellationToken)
        {
            Guid tenantId = (await rules.ResolveTenantAsync(request.TenantId, true, cancellationToken))!.Value;
            await rules.ValidateProjectSiteChainAsync(
                tenantId, request.ProjectId, request.SiteId, cancellationToken);
            return await snapshotService.PreviewAsync(
                tenantId, request.ProjectId, request.SiteId, request.ReportDate, cancellationToken);
        }
    }
}

public class GetDailySiteReportWorkforceSnapshotQuery :
    IRequest<DailySiteReportWorkforceDto>, ISecuredRequest
{
    public Guid ReportId { get; set; }
    public Guid? TenantId { get; set; }
    public string[] Roles => [DailySiteReportsOperationClaims.Admin, DailySiteReportsOperationClaims.Read];
    public class Handler(
        IDailySiteReportRepository reportRepository,
        IDailySiteReportWorkforceSnapshotRepository snapshotRepository,
        DailySiteReportBusinessRules rules)
        : IRequestHandler<GetDailySiteReportWorkforceSnapshotQuery, DailySiteReportWorkforceDto>
    {
        public async Task<DailySiteReportWorkforceDto> Handle(
            GetDailySiteReportWorkforceSnapshotQuery request, CancellationToken cancellationToken)
        {
            Guid? tenantId = await rules.ResolveTenantAsync(request.TenantId, false, cancellationToken);
            await UpdateDailySiteReportCommand.GetReport(
                reportRepository, request.ReportId, tenantId, cancellationToken);
            IPaginate<DailySiteReportWorkforceSnapshot> page = await snapshotRepository.GetListAsync(
                x => x.DailySiteReportId == request.ReportId,
                orderBy: q => q.OrderBy(x => x.WorkerName), size: 10000, enableTracking: false,
                cancellationToken: cancellationToken);
            List<DailySiteReportWorkforceSnapshotDto> rows = page.Items.Select(x => new DailySiteReportWorkforceSnapshotDto
            {
                SourcePuantajRecordId = x.SourcePuantajRecordId,
                WorkerId = x.WorkerId,
                WorkerName = x.WorkerName,
                Trade = x.Trade,
                WorkType = x.WorkType,
                DayCount = x.DayCount,
                OvertimeHours = x.OvertimeHours,
                PuantajStatusAtCapture = x.PuantajStatusAtCapture,
                CapturedAt = x.CapturedAt
            }).ToList();
            return new DailySiteReportWorkforceDto
            {
                Rows = rows,
                CapturedAt = rows.Select(x => x.CapturedAt).FirstOrDefault()
            };
        }
    }
}
