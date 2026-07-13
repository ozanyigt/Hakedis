using Domain.Enums;
using NArchitecture.Core.Application.Dtos;
using NArchitecture.Core.Application.Responses;

namespace Application.Features.DailySiteReports;

public class DailySiteReportPhotoDto : IDto
{
    public Guid Id { get; set; }
    public string Url { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long SizeBytes { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; }
}

public class DailySiteReportWorkforceSnapshotDto : IDto
{
    public Guid? SourcePuantajRecordId { get; set; }
    public Guid? WorkerId { get; set; }
    public string WorkerName { get; set; } = null!;
    public string? Trade { get; set; }
    public WorkType WorkType { get; set; }
    public decimal DayCount { get; set; }
    public decimal OvertimeHours { get; set; }
    public PuantajStatus? PuantajStatusAtCapture { get; set; }
    public DateTime? CapturedAt { get; set; }
}

public class DailySiteReportWorkforceDto : IDto
{
    public IReadOnlyList<DailySiteReportWorkforceSnapshotDto> Rows { get; set; } = [];
    public int WorkerCount => Rows.Select(x => x.WorkerId).Distinct().Count();
    public decimal TotalDayCount => Rows.Sum(x => x.DayCount);
    public decimal TotalOvertimeHours => Rows.Sum(x => x.OvertimeHours);
    public int SiteLessCount { get; set; }
    public DateTime? CapturedAt { get; set; }
}

public class DailySiteReportMaterialLineDto : IDto
{
    public Guid Id { get; set; }
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; } = null!;
    public string MaterialName { get; set; } = null!;
    public string Unit { get; set; } = null!;
    public decimal Quantity { get; set; }
    public string? Notes { get; set; }
    public decimal? PostedUnitCost { get; set; }
    public decimal? PostedTotalCost { get; set; }
}

public class DailySiteReportDto : IResponse, IDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = null!;
    public Guid SiteId { get; set; }
    public string SiteName { get; set; } = null!;
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
    public string AuthorName { get; set; } = null!;
    public Guid? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DailySiteReportStatus Status { get; set; }
    public int PhotoCount { get; set; }
    public ICollection<DailySiteReportPhotoDto> Photos { get; set; } = [];
    public ICollection<DailySiteReportWorkforceSnapshotDto> WorkforceSnapshots { get; set; } = [];
    public ICollection<DailySiteReportMaterialLineDto> MaterialLines { get; set; } = [];
    public decimal PostedMaterialCost { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}

public class DailySiteReportListItemDto : IDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = null!;
    public Guid SiteId { get; set; }
    public string SiteName { get; set; } = null!;
    public DateTime ReportDate { get; set; }
    public WeatherCondition Weather { get; set; }
    public string WorkSummary { get; set; } = null!;
    public DailySiteReportStatus Status { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string AuthorName { get; set; } = null!;
    public int PhotoCount { get; set; }
}
