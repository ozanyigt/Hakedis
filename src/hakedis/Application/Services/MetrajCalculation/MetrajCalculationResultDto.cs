namespace Application.Services.MetrajCalculation;

public class MetrajCalculationResultDto
{
  public bool Success { get; set; }
  public string? ErrorMessage { get; set; }
  public string? DrawingUnitNote { get; set; }
  public IReadOnlyList<MetrajCalculationItemDto> Items { get; set; } = [];
}
