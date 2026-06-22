namespace Application.Services.MetrajCalculation;

public class MetrajCalculationRequest
{
  public required string FilePath { get; set; }
  public required string FileExtension { get; set; }
  public IReadOnlyList<MetrajKalemRule> Rules { get; set; } = [];
  public string? FloorName { get; set; }
  public string? SpaceName { get; set; }
}
