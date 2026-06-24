using Domain.Enums;
using NArchitecture.Core.Application.Responses;

namespace Application.Features.MetrajResults.Commands.Calculate;

public class CalculateMetrajResponse : IResponse
{
  public Guid DrawingId { get; set; }
  public DrawingStatus Status { get; set; }
  public string? ErrorMessage { get; set; }
  public string? DrawingUnitNote { get; set; }
  public IList<CalculatedMetrajItemDto> Results { get; set; } = [];
}

public class CalculatedMetrajItemDto
{
  public Guid Id { get; set; }
  public MetrajKalemType KalemType { get; set; }
  public MeasurementUnit Unit { get; set; }
  public decimal Quantity { get; set; }
  public string? FloorName { get; set; }
  public string? SpaceName { get; set; }
  public string? Notes { get; set; }
}
