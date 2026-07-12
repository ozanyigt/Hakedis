using Domain.Enums;

namespace Application.Services.MetrajJudgment;

public class MetrajJudgmentItemRequest
{
    public Guid MetrajResultId { get; set; }
    public MetrajKalemType KalemType { get; set; }
    public MeasurementUnit Unit { get; set; }
    public decimal GrossQuantity { get; set; }
    public string? FloorName { get; set; }
    public string? SpaceName { get; set; }
    public string? Notes { get; set; }
    public string? LayerHint { get; set; }
}

public class MetrajLayerSummaryDto
{
    public string Name { get; set; } = string.Empty;
    public int EntityCount { get; set; }
    public double ClosedArea { get; set; }
    public double LineLength { get; set; }
}

public class MetrajPolicySnippetDto
{
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}

public class MetrajJudgmentRequest
{
    public Guid DrawingId { get; set; }
    public string? DrawingUnitNote { get; set; }
    public IReadOnlyList<MetrajJudgmentItemRequest> Items { get; set; } = [];
    public IReadOnlyList<MetrajLayerSummaryDto> Layers { get; set; } = [];
    public IReadOnlyList<MetrajPolicySnippetDto> Policies { get; set; } = [];
}

public class MetrajJudgmentItemResult
{
    public Guid MetrajResultId { get; set; }
    public MetrajJudgmentDecision Decision { get; set; } = MetrajJudgmentDecision.NeedsReview;
    public decimal? SuggestedQuantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? PolicyRef { get; set; }
    public decimal? Confidence { get; set; }
}

public class MetrajJudgmentResult
{
    public bool Success { get; set; }
    public bool UsedAi { get; set; }
    public string? ErrorMessage { get; set; }
    public IReadOnlyList<MetrajJudgmentItemResult> Items { get; set; } = [];
}

public interface IMetrajJudgmentService
{
    Task<MetrajJudgmentResult> JudgeAsync(MetrajJudgmentRequest request, CancellationToken cancellationToken = default);
}
