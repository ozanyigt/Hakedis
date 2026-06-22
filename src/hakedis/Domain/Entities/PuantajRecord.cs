using Domain.Enums;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

public class PuantajRecord : Entity<Guid>
{
    public Guid TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? SiteId { get; set; }
    public Guid? WorkerId { get; set; }
    public DateTime WorkDate { get; set; }
    public string WorkType { get; set; } = null!;
    public decimal DayCount { get; set; }
    public decimal OvertimeHours { get; set; }
    public PuantajStatus Status { get; set; } = PuantajStatus.Draft;
    public Guid? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? Notes { get; set; }

    public virtual Tenant Tenant { get; set; } = null!;
    public virtual Project Project { get; set; } = null!;
    public virtual Site? Site { get; set; }
    public virtual Worker? Worker { get; set; }
    public virtual User? ApprovedByUser { get; set; }
}
