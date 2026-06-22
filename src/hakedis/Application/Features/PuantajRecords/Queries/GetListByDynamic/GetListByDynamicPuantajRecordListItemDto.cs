using NArchitecture.Core.Application.Dtos;
using Domain.Enums;
namespace Application.Features.PuantajRecords.Queries.GetListByDynamic;

public class GetListByDynamicPuantajRecordListItemDto : IDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? SiteId { get; set; }
    public Guid? WorkerId { get; set; }
    public DateTime WorkDate { get; set; }
    public string WorkType { get; set; }
    public decimal DayCount { get; set; }
    public decimal OvertimeHours { get; set; }
    public PuantajStatus Status { get; set; }
    public Guid? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? Notes { get; set; }
}
