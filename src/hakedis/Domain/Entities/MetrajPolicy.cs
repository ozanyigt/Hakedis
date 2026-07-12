using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

/// <summary>
/// Firma metraj politikası (Katman C). Örn. "kırık kiriş sayılmaz".
/// </summary>
public class MetrajPolicy : Entity<Guid>
{
    public Guid TenantId { get; set; }
    public string Code { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public int Version { get; set; } = 1;
    public bool IsActive { get; set; } = true;

    public virtual Tenant Tenant { get; set; } = null!;
}
