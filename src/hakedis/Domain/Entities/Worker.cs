using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

public class Worker : Entity<Guid>
{
    public Guid TenantId { get; set; }
    public string FullName { get; set; } = null!;
    public string? Trade { get; set; }
    public string? Phone { get; set; }
    public string? IdentityNumber { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual Tenant Tenant { get; set; } = null!;
    public virtual ICollection<PuantajRecord> PuantajRecords { get; set; } = new List<PuantajRecord>();
}
