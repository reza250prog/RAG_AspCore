namespace SemanticSearch.Services;

public sealed class OllamaEmbeddingResponse
{
    public List<float[]> Embeddings { get; init; } = [];
}