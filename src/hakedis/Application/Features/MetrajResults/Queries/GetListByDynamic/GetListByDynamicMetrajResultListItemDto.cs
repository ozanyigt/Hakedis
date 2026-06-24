using NArchitecture.Core.Application.Dtos;
using Domain.Enums;
namespace Application.Features.MetrajResults.Queries.GetListByDynamic;

public class GetListByDynamicMetrajResultListItemDto : IDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? SiteId { get; set; }
    public Guid DrawingId { get; set; }
    public MetrajKalemType KalemType { get; set; }
    public MeasurementUnit Unit { get; set; }
    public decimal Quantity { get; set; }
    public string? FloorName { get; set; }
    public string? SpaceName { get; set; }
    public DateTime CalculatedAt { get; set; }
    public string? Notes { get; set; }
}
