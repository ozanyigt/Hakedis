namespace Application.Services.MetrajCalculation;

public class DrawingLayerItemDto
{
    public string Name { get; set; } = string.Empty;
    public int EntityCount { get; set; }
    public bool HasClosedArea { get; set; }
    public bool HasLines { get; set; }
}

public class DrawingLayersDiscoveryResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public IList<DrawingLayerItemDto> Layers { get; set; } = [];
}
