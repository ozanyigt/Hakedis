using Domain.Enums;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

public class Site : Entity<Guid>
{
    public Guid TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = null!;
    public string? Code { get; set; }
    public string? Location { get; set; }
    public SiteStatus Status { get; set; } = SiteStatus.Active;
    public string? Description { get; set; }

    public virtual Tenant Tenant { get; set; } = null!;
    public virtual Project Project { get; set; } = null!;
    public virtual ICollection<Drawing> Drawings { get; set; } = new List<Drawing>();
    public virtual ICollection<MetrajResult> MetrajResults { get; set; } = new List<MetrajResult>();
    public virtual ICollection<PuantajRecord> PuantajRecords { get; set; } = new List<PuantajRecord>();
}
