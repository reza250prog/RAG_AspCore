using SemanticSearch.Models;

namespace SemanticSearch.Services;

public sealed class SearchResult
{
    public int Rank { get; set; }
    public Article Article { get; init; } = default!;
    public float Score { get; init; }
}
