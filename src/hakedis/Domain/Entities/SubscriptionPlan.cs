using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

public class SubscriptionPlan : Entity<Guid>
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal MonthlyPrice { get; set; }
    public decimal YearlyPrice { get; set; }
    public string EnabledModules { get; set; } = null!;
    public int MaxSiteCount { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
