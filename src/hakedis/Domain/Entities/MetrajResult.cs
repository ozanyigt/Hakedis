using Domain.Enums;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

public class MetrajResult : Entity<Guid>
{
    public Guid TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? SiteId { get; set; }
    public Guid DrawingId { get; set; }
    public MetrajKalemType KalemType { get; set; }
    public string Unit { get; set; } = null!;
    public decimal Quantity { get; set; }
    public string? FloorName { get; set; }
    public string? SpaceName { get; set; }
    public DateTime CalculatedAt { get; set; }
    public string? Notes { get; set; }

    public virtual Tenant Tenant { get; set; } = null!;
    public virtual Project Project { get; set; } = null!;
    public virtual Site? Site { get; set; }
    public virtual Drawing Drawing { get; set; } = null!;
    public virtual ICollection<ProgressEntry> ProgressEntries { get; set; } = new List<ProgressEntry>();
}
