namespace SemanticSearch.Services;

public interface IChatService
{
    Task<string> GenerateAsync(
        string prompt,
        CancellationToken cancellationToken = default);
}