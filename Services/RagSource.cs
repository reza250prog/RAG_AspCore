namespace SemanticSearch.Services;

public sealed class RagSource
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public float Score { get; init; }
}