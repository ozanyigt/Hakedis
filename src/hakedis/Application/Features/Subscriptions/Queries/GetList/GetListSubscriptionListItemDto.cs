using NArchitecture.Core.Application.Dtos;
using Domain.Enums;
using Domain.Enums;

namespace Application.Features.Subscriptions.Queries.GetList;

public class GetListSubscriptionListItemDto : IDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid SubscriptionPlanId { get; set; }
    public BillingCycle BillingCycle { get; set; }
    public SubscriptionStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsManualAssignment { get; set; }
    public string? Notes { get; set; }
}