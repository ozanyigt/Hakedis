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
    public MeasurementUnit Unit { get; set; } = MeasurementUnit.M2;

    /// <summary>Hakedişte kullanılan nihai miktar (onay sonrası kilitlenir).</summary>
    public decimal Quantity { get; set; }

    /// <summary>Deterministik motorun ürettiği brüt miktar (Katman B).</summary>
    public decimal GrossQuantity { get; set; }

    /// <summary>AI / politikanın önerdiği miktar (Ignore ise 0 olabilir).</summary>
    public decimal? SuggestedQuantity { get; set; }

    public MetrajApprovalStatus ApprovalStatus { get; set; } = MetrajApprovalStatus.Pending;
    public MetrajJudgmentDecision? JudgmentDecision { get; set; }
    public string? JudgmentReason { get; set; }
    public string? PolicyRef { get; set; }
    public decimal? AiConfidence { get; set; }
    public bool IsLocked { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public DateTime? ReviewedAt { get; set; }

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
