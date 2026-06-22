using NArchitecture.Core.Application.Dtos;
using Domain.Enums;

namespace Application.Features.Drawings.Queries.GetList;

public class GetListDrawingListItemDto : IDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? SiteId { get; set; }
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public string FileExtension { get; set; }
    public long FileSizeBytes { get; set; }
    public DrawingStatus Status { get; set; }
    public string? ParseErrorMessage { get; set; }
    public DateTime? ParsedAt { get; set; }
}