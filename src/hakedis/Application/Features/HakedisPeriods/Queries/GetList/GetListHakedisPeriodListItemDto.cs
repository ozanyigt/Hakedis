using NArchitecture.Core.Application.Dtos;
using Domain.Enums;

namespace Application.Features.HakedisPeriods.Queries.GetList;

public class GetListHakedisPeriodListItemDto : IDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public int PeriodNumber { get; set; }
    public string Name { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public HakedisStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DeductionAmount { get; set; }
    public decimal NetAmount { get; set; }
    public Guid? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? Notes { get; set; }
}