namespace Application.Services.MetrajCalculation;

public class MetrajCalculationResultDto
{
  public bool Success { get; set; }
  public string? ErrorMessage { get; set; }
  public string? DrawingUnitNote { get; set; }
  public IReadOnlyList<MetrajCalculationItemDto> Items { get; set; } = [];
  public IReadOnlyList<MetrajCalculationLayerDto> Layers { get; set; } = [];
}

public class MetrajCalculationLayerDto
{
  public string Name { get; set; } = string.Empty;
  public int EntityCount { get; set; }
  public double ClosedArea { get; set; }
  public double LineLength { get; set; }
}
