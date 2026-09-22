namespace SemanticSearch.Services;

public sealed class SemanticSearchService(IEmbeddingService embeddingService, InMemoryArticleRepository repository)
{
    public async Task<List<SearchResult>> SearchAsync(string query,
                                                      int topK = 3,
                                                      float minimumScore = 0.70f,
                                                      CancellationToken cancellationToken = default)
    {
        var queryEmbedding =
            await embeddingService.GenerateAsync(
                query,
                cancellationToken);

        return [.. repository
    .GetAll()
    .Select(article => new SearchResult
    {
        Article = article,
        Score = CosineSimilarity(
            queryEmbedding,
            article.Embedding)
    })
    .OrderByDescending(x => x.Score)
    .Take(topK)
    .Select((result, index) =>
    {
        result.Rank = index + 1;
        return result;
    })];
    }

    private static float CosineSimilarity(
        ReadOnlySpan<float> a,
        ReadOnlySpan<float> b)
    {
        if (a.Length != b.Length)
            throw new ArgumentException(
                "Vectors must have the same dimensions.");

        float dot = 0;
        float magnitudeA = 0;
        float magnitudeB = 0;

        for (var i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            magnitudeA += a[i] * a[i];
            magnitudeB += b[i] * b[i];
        }

        if (magnitudeA == 0 || magnitudeB == 0)
            return 0;

        return dot /
            (MathF.Sqrt(magnitudeA) *
             MathF.Sqrt(magnitudeB));
    }
}