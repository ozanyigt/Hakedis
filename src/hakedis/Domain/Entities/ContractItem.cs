using Domain.Enums;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

public class ContractItem : Entity<Guid>
{
    public Guid TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public MetrajKalemType KalemType { get; set; }
    public string Description { get; set; } = null!;
    public MeasurementUnit Unit { get; set; } = MeasurementUnit.M2;
    public decimal UnitPrice { get; set; }
    public decimal? ContractQuantity { get; set; }
    public int SortOrder { get; set; }

    public virtual Tenant Tenant { get; set; } = null!;
    public virtual Project Project { get; set; } = null!;
    public virtual ICollection<ProgressEntry> ProgressEntries { get; set; } = new List<ProgressEntry>();
}
