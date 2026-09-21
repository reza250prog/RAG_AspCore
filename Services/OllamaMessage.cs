namespace SemanticSearch.Services;

public sealed class OllamaMessage
{
    public string? Role { get; init; }

    public string? Content { get; init; }
}