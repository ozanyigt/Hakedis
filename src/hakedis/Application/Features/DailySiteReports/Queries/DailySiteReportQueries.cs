using Application.Features.DailySiteReports.Commands;
using Application.Features.DailySiteReports.Constants;
using Application.Features.DailySiteReports.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.DailySiteReports.Queries;

public class GetDailySiteReportByIdQuery : IRequest<DailySiteReportDto>, ISecuredRequest
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string[] Roles => [DailySiteReportsOperationClaims.Admin, DailySiteReportsOperationClaims.Read];

    public class Handler(
        IDailySiteReportRepository repository,
        DailySiteReportBusinessRules rules,
        IMapper mapper) : IRequestHandler<GetDailySiteReportByIdQuery, DailySiteReportDto>
    {
        public async Task<DailySiteReportDto> Handle(GetDailySiteReportByIdQuery request, CancellationToken cancellationToken)
        {
            Guid? tenantId = await rules.ResolveTenantAsync(request.TenantId, false, cancellationToken);
            DailySiteReport report =
                await UpdateDailySiteReportCommand.GetReport(repository, request.Id, tenantId, cancellationToken);
            return mapper.Map<DailySiteReportDto>(report);
        }
    }
}

public class GetListDailySiteReportQuery : IRequest<GetListResponse<DailySiteReportListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; } = new();
    public Guid? TenantId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? SiteId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public DailySiteReportStatus? Status { get; set; }
    public string[] Roles => [DailySiteReportsOperationClaims.Admin, DailySiteReportsOperationClaims.Read];

    public class Handler(
        IDailySiteReportRepository repository,
        DailySiteReportBusinessRules rules,
        IMapper mapper) : IRequestHandler<GetListDailySiteReportQuery, GetListResponse<DailySiteReportListItemDto>>
    {
        public async Task<GetListResponse<DailySiteReportListItemDto>> Handle(
            GetListDailySiteReportQuery request, CancellationToken cancellationToken)
        {
            Guid? tenantId = await rules.ResolveTenantAsync(request.TenantId, false, cancellationToken);
            DateTime? from = request.FromDate?.Date;
            DateTime? to = request.ToDate?.Date;
            IPaginate<DailySiteReport> reports = await repository.GetListAsync(
                predicate: x =>
                    (!tenantId.HasValue || x.TenantId == tenantId.Value)
                    && (!request.ProjectId.HasValue || x.ProjectId == request.ProjectId.Value)
                    && (!request.SiteId.HasValue || x.SiteId == request.SiteId.Value)
                    && (!from.HasValue || x.ReportDate >= from.Value)
                    && (!to.HasValue || x.ReportDate <= to.Value)
                    && (!request.Status.HasValue || x.Status == request.Status.Value),
                include: q => q.Include(x => x.Project).Include(x => x.Site).Include(x => x.CreatedByUser).Include(x => x.Photos),
                orderBy: q => q.OrderByDescending(x => x.ReportDate).ThenBy(x => x.Site.Name),
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                enableTracking: false,
                cancellationToken: cancellationToken);
            return mapper.Map<GetListResponse<DailySiteReportListItemDto>>(reports);
        }
    }
}
