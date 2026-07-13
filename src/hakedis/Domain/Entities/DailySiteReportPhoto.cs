using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

public class DailySiteReportPhoto : Entity<Guid>
{
    public Guid DailySiteReportId { get; set; }
    public string Url { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long SizeBytes { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; }

    public virtual DailySiteReport DailySiteReport { get; set; } = null!;
}
