using Domain.Enums;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

public class Drawing : Entity<Guid>
{
    public Guid TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? SiteId { get; set; }
    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public string FileExtension { get; set; } = null!;
    public long FileSizeBytes { get; set; }
    public DrawingStatus Status { get; set; } = DrawingStatus.Uploaded;
    public string? ParseErrorMessage { get; set; }
    public DateTime? ParsedAt { get; set; }

    public virtual Tenant Tenant { get; set; } = null!;
    public virtual Project Project { get; set; } = null!;
    public virtual Site? Site { get; set; }
    public virtual ICollection<MetrajResult> MetrajResults { get; set; } = new List<MetrajResult>();
}
