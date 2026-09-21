namespace SemanticSearch.Models;

public sealed class Article
{
    public int Id { get; init; }

    public string Title { get; init; } = default!;

    public string Description { get; init; } = default!;

    public float[] Embedding { get; set; } = [];
}