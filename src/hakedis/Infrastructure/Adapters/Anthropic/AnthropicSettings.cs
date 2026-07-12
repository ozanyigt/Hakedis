namespace Infrastructure.Adapters.Anthropic;

public class AnthropicSettings
{
    public const string SectionName = "AnthropicSettings";

    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "claude-sonnet-4-20250514";
    public string ApiUrl { get; set; } = "https://api.anthropic.com/v1/messages";
    public string ApiVersion { get; set; } = "2023-06-01";
    public int MaxTokens { get; set; } = 4096;
    public bool Enabled { get; set; } = true;
}
