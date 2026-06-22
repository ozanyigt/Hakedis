using Domain.Enums;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

public class MetrajRuleTemplate : Entity<Guid>
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = null!;
    public MetrajKalemType KalemType { get; set; }
    public string LayerPatterns { get; set; } = null!;
    public string EntityTypes { get; set; } = null!;
    public string Unit { get; set; } = null!;
    public decimal? DefaultThickness { get; set; }
    public decimal? DefaultHeight { get; set; }
    public bool DeductOpenings { get; set; }
    public string? OpeningLayerPatterns { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual Tenant Tenant { get; set; } = null!;
}
