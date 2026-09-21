using SemanticSearch.Models;

namespace SemanticSearch.Services;

public sealed class InMemoryArticleRepository
{
    private readonly List<Article> _articles = [];

    public IReadOnlyList<Article> GetAll()
        => _articles;

    public void Add(Article article)
        => _articles.Add(article);
}
