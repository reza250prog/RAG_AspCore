using SemanticSearch.Models;

namespace SemanticSearch.Services;

public sealed class ArticleEmbeddingInitializer(InMemoryArticleRepository repository, IEmbeddingService embeddingService)
{
    public async Task InitializeAsync(IEnumerable<Article> articles, CancellationToken cancellationToken = default)
    {
        foreach (var article in articles)
        {
            var text =
                $"{article.Title}. {article.Description}";

            article.Embedding =
                await embeddingService.GenerateAsync(
                    text,
                    cancellationToken);

            repository.Add(article);
        }
    }
}