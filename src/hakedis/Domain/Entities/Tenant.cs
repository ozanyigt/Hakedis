using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

public class Tenant : Entity<Guid>
{
    public string Name { get; set; } = null!;
    public string? TaxNumber { get; set; }
    public string? TaxOffice { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
