using Domain.Enums;

namespace Application.Services.MetrajCalculation;

public class MetrajKalemRule
{
  public MetrajKalemType KalemType { get; set; }
  public string Unit { get; set; } = "m²";
  public string[] LayerPatterns { get; set; } = [];
  public string[] EntityTypes { get; set; } = [];
  public MetrajCalculationMethod Method { get; set; }
  public decimal? DefaultHeight { get; set; }
  public decimal? DefaultThickness { get; set; }
  public bool DeductOpenings { get; set; }
  public int SideCount { get; set; } = 1;
  public string Description { get; set; } = string.Empty;
}
