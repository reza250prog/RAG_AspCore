namespace SemanticSearch.Services;

public sealed class RagResponse
{
    public string Question { get; init; } = string.Empty;

    public string Answer { get; init; } = string.Empty;

    public List<RagSource> Sources { get; init; } = [];
}
