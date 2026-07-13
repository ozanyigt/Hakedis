using Domain.Enums;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

public class DailySiteReport : Entity<Guid>
{
    public Guid TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid SiteId { get; set; }
    public DateTime ReportDate { get; set; }
    public WeatherCondition Weather { get; set; }
    public decimal? MinTemperatureCelsius { get; set; }
    public decimal? MaxTemperatureCelsius { get; set; }
    public string WorkSummary { get; set; } = null!;
    public string? WorkforceNotes { get; set; }
    public string? EquipmentNotes { get; set; }
    public string? MaterialNotes { get; set; }
    public string? BlockersNotes { get; set; }
    public string? Notes { get; set; }
    public string? RejectionReason { get; set; }
    public Guid CreatedByUserId { get; set; }
    public Guid? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DailySiteReportStatus Status { get; set; } = DailySiteReportStatus.Draft;

    public virtual Tenant Tenant { get; set; } = null!;
    public virtual Project Project { get; set; } = null!;
    public virtual Site Site { get; set; } = null!;
    public virtual User CreatedByUser { get; set; } = null!;
    public virtual User? ApprovedByUser { get; set; }
    public virtual ICollection<DailySiteReportPhoto> Photos { get; set; } = new List<DailySiteReportPhoto>();
    public virtual ICollection<DailySiteReportWorkforceSnapshot> WorkforceSnapshots { get; set; } =
        new List<DailySiteReportWorkforceSnapshot>();
    public virtual ICollection<DailySiteReportMaterialLine> MaterialLines { get; set; } =
        new List<DailySiteReportMaterialLine>();
}
