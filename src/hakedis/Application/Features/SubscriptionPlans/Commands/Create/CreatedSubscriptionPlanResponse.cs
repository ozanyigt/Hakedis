using NArchitecture.Core.Application.Responses;

namespace Application.Features.SubscriptionPlans.Commands.Create;

public class CreatedSubscriptionPlanResponse : IResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal MonthlyPrice { get; set; }
    public decimal YearlyPrice { get; set; }
    public string EnabledModules { get; set; }
    public int MaxSiteCount { get; set; }
    public bool IsActive { get; set; }
}