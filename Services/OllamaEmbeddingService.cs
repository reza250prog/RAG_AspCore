namespace SemanticSearch.Services;

public sealed class OllamaEmbeddingService(HttpClient httpClient) : IEmbeddingService
{
    public async Task<float[]> GenerateAsync(string text, CancellationToken cancellationToken = default)
    {
        var request = new
        {
            model = "nomic-embed-text",
            input = text
        };

        var response = await httpClient.PostAsJsonAsync(
            "/api/embed",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content
                .ReadFromJsonAsync<OllamaEmbeddingResponse>(
                    cancellationToken);

        return result?.Embeddings.FirstOrDefault()
               ?? throw new InvalidOperationException(
                   "Ollama returned no embedding.");
    }
}