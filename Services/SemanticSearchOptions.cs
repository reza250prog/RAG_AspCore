namespace SemanticSearch.Services;

public sealed class SemanticSearchOptions
{
    public int TopK { get; init; } = 3;
    public float MinimumScore { get; init; } = 0.70f;
}