using Domain.Enums;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

public class HakedisDeductionLine : Entity<Guid>
{
    public Guid TenantId { get; set; }
    public Guid HakedisPeriodId { get; set; }
    public DeductionCategory Category { get; set; }
    public string Description { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? Notes { get; set; }

    public virtual Tenant Tenant { get; set; } = null!;
    public virtual HakedisPeriod HakedisPeriod { get; set; } = null!;
}
