using Domain.Enums;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

public class ProjectMetrajLayerMapping : Entity<Guid>
{
    public Guid TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public MetrajKalemType KalemType { get; set; }
    public string LayerNames { get; set; } = string.Empty;

    public virtual Tenant Tenant { get; set; } = null!;
    public virtual Project Project { get; set; } = null!;
}
