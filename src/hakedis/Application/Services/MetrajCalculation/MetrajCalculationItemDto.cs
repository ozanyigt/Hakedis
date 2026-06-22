using Domain.Enums;

namespace Application.Services.MetrajCalculation;

public class MetrajCalculationItemDto
{
  public MetrajKalemType KalemType { get; set; }
  public string Unit { get; set; } = null!;
  public decimal Quantity { get; set; }
  public string? FloorName { get; set; }
  public string? SpaceName { get; set; }
  public string? Notes { get; set; }
}
