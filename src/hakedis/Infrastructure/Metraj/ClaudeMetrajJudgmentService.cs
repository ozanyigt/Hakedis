using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Services.MetrajJudgment;
using Domain.Enums;
using Infrastructure.Adapters.Anthropic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Metraj;

public class ClaudeMetrajJudgmentService : IMetrajJudgmentService
{
    private static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

    private readonly HttpClient _httpClient;
    private readonly AnthropicSettings _settings;
    private readonly ILogger<ClaudeMetrajJudgmentService> _logger;

    public ClaudeMetrajJudgmentService(
        HttpClient httpClient,
        IOptions<AnthropicSettings> settings,
        ILogger<ClaudeMetrajJudgmentService> logger
    )
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<MetrajJudgmentResult> JudgeAsync(
        MetrajJudgmentRequest request,
        CancellationToken cancellationToken = default
    )
    {
        if (!_settings.Enabled || string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            return BuildFallback(
                request,
                usedAi: false,
                "Anthropic API anahtarı tanımlı değil. Tüm kalemler insan incelemesine bırakıldı."
            );
        }

        try
        {
            string systemPrompt =
                """
                Sen Türkiye'de inşaat hakediş metraj hüküm asistanısın.
                Deterministik CAD motoru brüt miktarları zaten hesapladı. Sen ASLA yeni m²/m uydurma.
                Görevin: firma politikasına ve CAD özetine bakarak her kalem için decision vermek:
                - count: say (suggestedQuantity = grossQuantity)
                - ignore: sayma (suggestedQuantity = 0) — örn. kırık/kesik kiriş, ihmal limiti
                - needs_review: belirsiz, insan onayı şart
                Yalnızca geçerli JSON döndür. Markdown kullanma.
                Şema:
                {"items":[{"metrajResultId":"guid","decision":"count|ignore|needs_review","suggestedQuantity":number,"reason":"string","policyRef":"K-xx|null","confidence":0.0}]}
                """;

            var userPayload = new
            {
                drawingId = request.DrawingId,
                drawingUnitNote = request.DrawingUnitNote,
                policies = request.Policies,
                layers = request.Layers,
                items = request.Items.Select(item => new
                {
                    metrajResultId = item.MetrajResultId,
                    kalemType = item.KalemType.ToString(),
                    unit = item.Unit.ToString(),
                    grossQuantity = item.GrossQuantity,
                    floorName = item.FloorName,
                    spaceName = item.SpaceName,
                    notes = item.Notes,
                    layerHint = item.LayerHint
                })
            };

            var body = new
            {
                model = _settings.Model,
                max_tokens = _settings.MaxTokens,
                system = systemPrompt,
                messages = new[]
                {
                    new { role = "user", content = JsonSerializer.Serialize(userPayload, JsonOptions) }
                }
            };

            using HttpRequestMessage httpRequest = new(HttpMethod.Post, _settings.ApiUrl);
            httpRequest.Headers.Add("x-api-key", _settings.ApiKey);
            httpRequest.Headers.Add("anthropic-version", _settings.ApiVersion);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpRequest.Content = new StringContent(JsonSerializer.Serialize(body, JsonOptions), Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            string responseText = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Anthropic API hata: {Status} {Body}", (int)response.StatusCode, responseText);
                return BuildFallback(request, usedAi: false, $"Claude API hatası ({(int)response.StatusCode}). Kalemler incelemeye alındı.");
            }

            using JsonDocument document = JsonDocument.Parse(responseText);
            string? textContent = document
                .RootElement.GetProperty("content")
                .EnumerateArray()
                .FirstOrDefault(element => element.TryGetProperty("type", out JsonElement type) && type.GetString() == "text")
                .GetProperty("text")
                .GetString();

            if (string.IsNullOrWhiteSpace(textContent))
                return BuildFallback(request, usedAi: false, "Claude boş yanıt döndü.");

            string json = ExtractJson(textContent);
            ClaudeJudgmentResponse? parsed = JsonSerializer.Deserialize<ClaudeJudgmentResponse>(json, JsonOptions);
            if (parsed?.Items is null || parsed.Items.Count == 0)
                return BuildFallback(request, usedAi: true, "Claude yanıtı parse edilemedi.");

            Dictionary<Guid, MetrajJudgmentItemRequest> byId = request.Items.ToDictionary(item => item.MetrajResultId);
            List<MetrajJudgmentItemResult> results = [];

            foreach (ClaudeJudgmentItem item in parsed.Items)
            {
                if (!Guid.TryParse(item.MetrajResultId, out Guid id) || !byId.TryGetValue(id, out MetrajJudgmentItemRequest? source))
                    continue;

                MetrajJudgmentDecision decision = ParseDecision(item.Decision);
                decimal suggested = decision switch
                {
                    MetrajJudgmentDecision.Ignore => 0,
                    MetrajJudgmentDecision.Count => item.SuggestedQuantity ?? source.GrossQuantity,
                    _ => item.SuggestedQuantity ?? source.GrossQuantity
                };

                // AI asla brütten büyük sayı üretemez
                if (suggested > source.GrossQuantity)
                    suggested = source.GrossQuantity;

                results.Add(
                    new MetrajJudgmentItemResult
                    {
                        MetrajResultId = id,
                        Decision = decision,
                        SuggestedQuantity = suggested,
                        Reason = string.IsNullOrWhiteSpace(item.Reason) ? "Claude hükmü." : item.Reason.Trim(),
                        PolicyRef = item.PolicyRef,
                        Confidence = item.Confidence
                    }
                );
            }

            foreach (MetrajJudgmentItemRequest missing in request.Items.Where(item => results.All(r => r.MetrajResultId != item.MetrajResultId)))
            {
                results.Add(
                    new MetrajJudgmentItemResult
                    {
                        MetrajResultId = missing.MetrajResultId,
                        Decision = MetrajJudgmentDecision.NeedsReview,
                        SuggestedQuantity = missing.GrossQuantity,
                        Reason = "Claude bu kalem için hüküm vermedi.",
                        Confidence = 0
                    }
                );
            }

            return new MetrajJudgmentResult
            {
                Success = true,
                UsedAi = true,
                Items = results
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Claude metraj hükmü başarısız");
            return BuildFallback(request, usedAi: false, $"Claude çağrısı başarısız: {ex.Message}");
        }
    }

    private static MetrajJudgmentResult BuildFallback(MetrajJudgmentRequest request, bool usedAi, string message)
    {
        return new MetrajJudgmentResult
        {
            Success = true,
            UsedAi = usedAi,
            ErrorMessage = message,
            Items = request
                .Items.Select(item => new MetrajJudgmentItemResult
                {
                    MetrajResultId = item.MetrajResultId,
                    Decision = MetrajJudgmentDecision.NeedsReview,
                    SuggestedQuantity = item.GrossQuantity,
                    Reason = message,
                    Confidence = 0
                })
                .ToList()
        };
    }

    private static MetrajJudgmentDecision ParseDecision(string? value) =>
        (value ?? string.Empty).Trim().ToLowerInvariant() switch
        {
            "count" => MetrajJudgmentDecision.Count,
            "ignore" => MetrajJudgmentDecision.Ignore,
            "needs_review" or "needsreview" or "review" => MetrajJudgmentDecision.NeedsReview,
            _ => MetrajJudgmentDecision.NeedsReview
        };

    private static string ExtractJson(string text)
    {
        string trimmed = text.Trim();
        if (trimmed.StartsWith("```", StringComparison.Ordinal))
        {
            int firstNewline = trimmed.IndexOf('\n');
            int lastFence = trimmed.LastIndexOf("```", StringComparison.Ordinal);
            if (firstNewline >= 0 && lastFence > firstNewline)
                trimmed = trimmed[(firstNewline + 1)..lastFence].Trim();
        }

        int start = trimmed.IndexOf('{');
        int end = trimmed.LastIndexOf('}');
        if (start >= 0 && end > start)
            return trimmed[start..(end + 1)];

        return trimmed;
    }

    private sealed class ClaudeJudgmentResponse
    {
        public List<ClaudeJudgmentItem> Items { get; set; } = [];
    }

    private sealed class ClaudeJudgmentItem
    {
        public string MetrajResultId { get; set; } = string.Empty;
        public string Decision { get; set; } = "needs_review";
        public decimal? SuggestedQuantity { get; set; }
        public string? Reason { get; set; }
        public string? PolicyRef { get; set; }
        public decimal? Confidence { get; set; }
    }
}
