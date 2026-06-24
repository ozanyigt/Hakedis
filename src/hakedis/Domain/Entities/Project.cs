using Domain.Enums;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

public class Project : Entity<Guid>
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = null!;
    public string? Code { get; set; }
    public string? Location { get; set; }
    public string? ClientName { get; set; }
    public decimal ContractAmount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;
    public string? Description { get; set; }

    public virtual Tenant Tenant { get; set; } = null!;
    public virtual ICollection<Site> Sites { get; set; } = new List<Site>();
    public virtual ICollection<Drawing> Drawings { get; set; } = new List<Drawing>();
    public virtual ICollection<MetrajResult> MetrajResults { get; set; } = new List<MetrajResult>();
    public virtual ICollection<PuantajRecord> PuantajRecords { get; set; } = new List<PuantajRecord>();
    public virtual ICollection<ContractItem> ContractItems { get; set; } = new List<ContractItem>();
    public virtual ICollection<ProjectMetrajLayerMapping> MetrajLayerMappings { get; set; } =
        new List<ProjectMetrajLayerMapping>();
    public virtual ICollection<HakedisPeriod> HakedisPeriods { get; set; } = new List<HakedisPeriod>();
}
