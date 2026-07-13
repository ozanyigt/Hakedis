using Application.Features.DailySiteReports.Constants;
using Application.Features.DailySiteReports.Rules;
using Application.Services.CurrentUser;
using Application.Services.ImageService;
using Application.Services.Inventory;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;

namespace Application.Features.DailySiteReports.Commands;

public abstract class DailySiteReportWriteModel
{
    public Guid? TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid SiteId { get; set; }
    public DateTime ReportDate { get; set; }
    public WeatherCondition Weather { get; set; }
    public decimal? MinTemperatureCelsius { get; set; }
    public decimal? MaxTemperatureCelsius { get; set; }
    public string WorkSummary { get; set; } = null!;
    public string? WorkforceNotes { get; set; }
    public string? EquipmentNotes { get; set; }
    public string? MaterialNotes { get; set; }
    public string? BlockersNotes { get; set; }
    public string? Notes { get; set; }

    internal void ApplyTo(DailySiteReport report)
    {
        report.ProjectId = ProjectId;
        report.SiteId = SiteId;
        report.ReportDate = ReportDate.Date;
        report.Weather = Weather;
        report.MinTemperatureCelsius = MinTemperatureCelsius;
        report.MaxTemperatureCelsius = MaxTemperatureCelsius;
        report.WorkSummary = WorkSummary.Trim();
        report.WorkforceNotes = WorkforceNotes;
        report.EquipmentNotes = EquipmentNotes;
        report.MaterialNotes = MaterialNotes;
        report.BlockersNotes = BlockersNotes;
        report.Notes = Notes;
    }
}

public class CreateDailySiteReportCommand : DailySiteReportWriteModel,
    IRequest<DailySiteReportDto>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public string[] Roles =>
        [DailySiteReportsOperationClaims.Admin, DailySiteReportsOperationClaims.Write, DailySiteReportsOperationClaims.Create];

    public class Handler(
        IDailySiteReportRepository repository,
        DailySiteReportBusinessRules rules,
        IMapper mapper) : IRequestHandler<CreateDailySiteReportCommand, DailySiteReportDto>
    {
        public async Task<DailySiteReportDto> Handle(CreateDailySiteReportCommand request, CancellationToken cancellationToken)
        {
            Guid tenantId = (await rules.ResolveTenantAsync(request.TenantId, true, cancellationToken))!.Value;
            DailySiteReportBusinessRules.ValidateTemperatures(request.MinTemperatureCelsius, request.MaxTemperatureCelsius);
            await rules.ValidateProjectSiteChainAsync(tenantId, request.ProjectId, request.SiteId, cancellationToken);
            await rules.EnsureUniqueSiteDateAsync(tenantId, request.SiteId, request.ReportDate, null, cancellationToken);

            DailySiteReport report = new()
            {
                TenantId = tenantId,
                CreatedByUserId = rules.GetCurrentUserId(),
                Status = DailySiteReportStatus.Draft
            };
            request.ApplyTo(report);
            await repository.AddAsync(report);

            DailySiteReport created = (await repository.GetAsync(
                x => x.Id == report.Id,
                include: q => q.Include(x => x.Project).Include(x => x.Site).Include(x => x.CreatedByUser).Include(x => x.Photos),
                cancellationToken: cancellationToken))!;
            return mapper.Map<DailySiteReportDto>(created);
        }
    }
}

public class UpdateDailySiteReportCommand : DailySiteReportWriteModel,
    IRequest<DailySiteReportDto>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }
    public string[] Roles =>
        [DailySiteReportsOperationClaims.Admin, DailySiteReportsOperationClaims.Write, DailySiteReportsOperationClaims.Update];

    public class Handler(
        IDailySiteReportRepository repository,
        DailySiteReportBusinessRules rules,
        IMapper mapper) : IRequestHandler<UpdateDailySiteReportCommand, DailySiteReportDto>
    {
        public async Task<DailySiteReportDto> Handle(UpdateDailySiteReportCommand request, CancellationToken cancellationToken)
        {
            Guid? tenantId = await rules.ResolveTenantAsync(request.TenantId, false, cancellationToken);
            DailySiteReport report = await GetReport(repository, request.Id, tenantId, cancellationToken);
            DailySiteReportBusinessRules.EnsureEditable(report);
            DailySiteReportBusinessRules.ValidateTemperatures(request.MinTemperatureCelsius, request.MaxTemperatureCelsius);
            await rules.ValidateProjectSiteChainAsync(report.TenantId, request.ProjectId, request.SiteId, cancellationToken);
            await rules.EnsureUniqueSiteDateAsync(report.TenantId, request.SiteId, request.ReportDate, report.Id, cancellationToken);
            request.ApplyTo(report);
            await repository.UpdateAsync(report);
            return mapper.Map<DailySiteReportDto>(report);
        }
    }

    internal static async Task<DailySiteReport> GetReport(
        IDailySiteReportRepository repository, Guid id, Guid? tenantId, CancellationToken cancellationToken)
    {
        DailySiteReport? report = await repository.GetAsync(
            x => x.Id == id && (!tenantId.HasValue || x.TenantId == tenantId.Value),
            include: q => q.Include(x => x.Project).Include(x => x.Site).Include(x => x.CreatedByUser)
                .Include(x => x.Photos).Include(x => x.WorkforceSnapshots)
                .Include(x => x.MaterialLines).ThenInclude(x => x.Material),
            cancellationToken: cancellationToken);
        return report ?? throw new BusinessException("Daily site report does not exist.");
    }
}

public class DeleteDailySiteReportCommand : IRequest<Guid>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string[] Roles =>
        [DailySiteReportsOperationClaims.Admin, DailySiteReportsOperationClaims.Write, DailySiteReportsOperationClaims.Delete];

    public class Handler(
        IDailySiteReportRepository repository,
        IDailySiteReportPhotoRepository photoRepository,
        DailySiteReportBusinessRules rules,
        ImageServiceBase imageService) : IRequestHandler<DeleteDailySiteReportCommand, Guid>
    {
        public async Task<Guid> Handle(DeleteDailySiteReportCommand request, CancellationToken cancellationToken)
        {
            Guid? tenantId = await rules.ResolveTenantAsync(request.TenantId, false, cancellationToken);
            DailySiteReport report = await UpdateDailySiteReportCommand.GetReport(repository, request.Id, tenantId, cancellationToken);
            DailySiteReportBusinessRules.EnsureEditable(report);
            foreach (DailySiteReportPhoto photo in report.Photos)
            {
                await imageService.DeleteAsync(photo.Url);
                await photoRepository.DeleteAsync(photo);
            }
            await repository.DeleteAsync(report);
            return report.Id;
        }
    }
}

public abstract class TransitionDailySiteReportCommand : IRequest<DailySiteReportDto>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public abstract DailySiteReportStatus TargetStatus { get; }
    public abstract string[] Roles { get; }
}

public class SubmitDailySiteReportCommand : TransitionDailySiteReportCommand
{
    public override DailySiteReportStatus TargetStatus => DailySiteReportStatus.Submitted;
    public override string[] Roles => [DailySiteReportsOperationClaims.Admin, DailySiteReportsOperationClaims.Write];
    public class Handler(
        IDailySiteReportRepository repository, DailySiteReportBusinessRules rules, IMapper mapper,
        DailySiteReportSnapshotService snapshotService,
        IDailySiteReportMaterialLineRepository materialLineRepository,
        IStockPostingService stockPostingService)
        : TransitionHandler<SubmitDailySiteReportCommand>(
            repository, rules, mapper, snapshotService, materialLineRepository, stockPostingService);
}

public class ApproveDailySiteReportCommand : TransitionDailySiteReportCommand
{
    public override DailySiteReportStatus TargetStatus => DailySiteReportStatus.Approved;
    public override string[] Roles => [DailySiteReportsOperationClaims.Admin];
    public class Handler(
        IDailySiteReportRepository repository, DailySiteReportBusinessRules rules, IMapper mapper,
        DailySiteReportSnapshotService snapshotService,
        IDailySiteReportMaterialLineRepository materialLineRepository,
        IStockPostingService stockPostingService)
        : TransitionHandler<ApproveDailySiteReportCommand>(
            repository, rules, mapper, snapshotService, materialLineRepository, stockPostingService);
}

public class RejectDailySiteReportCommand : TransitionDailySiteReportCommand
{
    public string? Reason { get; set; }
    public override DailySiteReportStatus TargetStatus => DailySiteReportStatus.Rejected;
    public override string[] Roles => [DailySiteReportsOperationClaims.Admin];
    public class Handler(
        IDailySiteReportRepository repository, DailySiteReportBusinessRules rules, IMapper mapper,
        DailySiteReportSnapshotService snapshotService,
        IDailySiteReportMaterialLineRepository materialLineRepository,
        IStockPostingService stockPostingService)
        : TransitionHandler<RejectDailySiteReportCommand>(
            repository, rules, mapper, snapshotService, materialLineRepository, stockPostingService);
}

public abstract class TransitionHandler<TCommand>(
    IDailySiteReportRepository repository,
    DailySiteReportBusinessRules rules,
    IMapper mapper,
    DailySiteReportSnapshotService snapshotService,
    IDailySiteReportMaterialLineRepository materialLineRepository,
    IStockPostingService stockPostingService) : IRequestHandler<TCommand, DailySiteReportDto>
    where TCommand : TransitionDailySiteReportCommand
{
    public async Task<DailySiteReportDto> Handle(TCommand request, CancellationToken cancellationToken)
    {
        Guid? tenantId = await rules.ResolveTenantAsync(request.TenantId, false, cancellationToken);
        DailySiteReport report = await UpdateDailySiteReportCommand.GetReport(repository, request.Id, tenantId, cancellationToken);
        DailySiteReportBusinessRules.EnsureTransition(report, request.TargetStatus);
        if (request.TargetStatus == DailySiteReportStatus.Submitted)
            await snapshotService.CaptureAsync(report, cancellationToken);
        if (request.TargetStatus == DailySiteReportStatus.Approved)
        {
            foreach (DailySiteReportMaterialLine line in report.MaterialLines
                         .OrderBy(x => x.MaterialId)
                         .ThenBy(x => x.Id))
            {
                StockPostingResult result = await stockPostingService.PostAsync(new(
                    report.TenantId,
                    report.SiteId,
                    line.MaterialId,
                    StockMovementType.Consumption,
                    line.Quantity,
                    null,
                    StockReferenceType.DailySiteReport,
                    report.Id,
                    null,
                    $"daily-report:{report.Id:N}:material-line:{line.Id:N}",
                    line.Notes,
                    rules.GetCurrentUserId()), cancellationToken);
                line.PostedUnitCost = result.UnitCost;
                line.PostedTotalCost = result.TotalCost;
                await materialLineRepository.UpdateAsync(line);
            }
        }
        report.Status = request.TargetStatus;
        if (request.TargetStatus == DailySiteReportStatus.Approved)
        {
            report.ApprovedByUserId = rules.GetCurrentUserId();
            report.ApprovedAt = DateTime.UtcNow;
            report.RejectionReason = null;
        }
        else
        {
            report.ApprovedByUserId = null;
            report.ApprovedAt = null;
            report.RejectionReason = request is RejectDailySiteReportCommand reject
                ? reject.Reason?.Trim()
                : null;
        }
        await repository.UpdateAsync(report);
        return mapper.Map<DailySiteReportDto>(report);
    }
}

public class UploadDailySiteReportPhotoCommand : IRequest<DailySiteReportPhotoDto>,
    ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid ReportId { get; set; }
    public Guid? TenantId { get; set; }
    public IFormFile File { get; set; } = null!;
    public string? Description { get; set; }
    public string[] Roles => [DailySiteReportsOperationClaims.Admin, DailySiteReportsOperationClaims.Write, DailySiteReportsOperationClaims.Update];

    public class Handler(
        IDailySiteReportRepository reportRepository,
        IDailySiteReportPhotoRepository photoRepository,
        DailySiteReportBusinessRules rules,
        ImageServiceBase imageService,
        IMapper mapper) : IRequestHandler<UploadDailySiteReportPhotoCommand, DailySiteReportPhotoDto>
    {
        public async Task<DailySiteReportPhotoDto> Handle(UploadDailySiteReportPhotoCommand request, CancellationToken cancellationToken)
        {
            Guid? tenantId = await rules.ResolveTenantAsync(request.TenantId, false, cancellationToken);
            DailySiteReport report = await UpdateDailySiteReportCommand.GetReport(reportRepository, request.ReportId, tenantId, cancellationToken);
            DailySiteReportBusinessRules.EnsureEditable(report);
            if (request.File is null)
                throw new BusinessException("A photo file is required.");
            DailySiteReportBusinessRules.EnsurePhotoCanBeAdded(
                report.Photos.Count, request.File.Length, request.File.FileName, request.File.ContentType);

            string url = await imageService.UploadAsync(request.File);
            DailySiteReportPhoto photo = new()
            {
                DailySiteReportId = report.Id,
                Url = url,
                FileName = Path.GetFileName(request.File.FileName),
                ContentType = request.File.ContentType,
                SizeBytes = request.File.Length,
                Description = request.Description?.Trim(),
                SortOrder = report.Photos.Count
            };
            try
            {
                await photoRepository.AddAsync(photo);
            }
            catch
            {
                await imageService.DeleteAsync(url);
                throw;
            }
            return mapper.Map<DailySiteReportPhotoDto>(photo);
        }
    }
}

public class DeleteDailySiteReportPhotoCommand : IRequest<Guid>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid ReportId { get; set; }
    public Guid PhotoId { get; set; }
    public Guid? TenantId { get; set; }
    public string[] Roles => [DailySiteReportsOperationClaims.Admin, DailySiteReportsOperationClaims.Write, DailySiteReportsOperationClaims.Update];

    public class Handler(
        IDailySiteReportRepository reportRepository,
        IDailySiteReportPhotoRepository photoRepository,
        DailySiteReportBusinessRules rules,
        ImageServiceBase imageService) : IRequestHandler<DeleteDailySiteReportPhotoCommand, Guid>
    {
        public async Task<Guid> Handle(DeleteDailySiteReportPhotoCommand request, CancellationToken cancellationToken)
        {
            Guid? tenantId = await rules.ResolveTenantAsync(request.TenantId, false, cancellationToken);
            DailySiteReport report = await UpdateDailySiteReportCommand.GetReport(reportRepository, request.ReportId, tenantId, cancellationToken);
            DailySiteReportBusinessRules.EnsureEditable(report);
            DailySiteReportPhoto? photo = report.Photos.SingleOrDefault(x => x.Id == request.PhotoId);
            if (photo is null)
                throw new BusinessException("Daily site report photo does not exist.");
            await imageService.DeleteAsync(photo.Url);
            await photoRepository.DeleteAsync(photo);
            return photo.Id;
        }
    }
}
