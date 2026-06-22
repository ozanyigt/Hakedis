using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

public class ProgressEntry : Entity<Guid>
{
    public Guid TenantId { get; set; }
    public Guid HakedisPeriodId { get; set; }
    public Guid ContractItemId { get; set; }
    public decimal QuantityThisPeriod { get; set; }
    public decimal CumulativeQuantity { get; set; }
    public decimal AmountThisPeriod { get; set; }
    public Guid? MetrajResultId { get; set; }
    public bool IsManualEntry { get; set; }
    public string? Notes { get; set; }

    public virtual Tenant Tenant { get; set; } = null!;
    public virtual HakedisPeriod HakedisPeriod { get; set; } = null!;
    public virtual ContractItem ContractItem { get; set; } = null!;
    public virtual MetrajResult? MetrajResult { get; set; }
}
