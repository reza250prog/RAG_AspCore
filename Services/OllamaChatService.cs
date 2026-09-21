using System.Text.Json;

namespace SemanticSearch.Services;

public sealed class OllamaChatService(HttpClient httpClient) : IChatService
{
    public async Task<string> GenerateAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            model = "gemma3:1b",
            messages = new[]
                {
                 new
                 {
                     role = "user",
                     content = prompt
                 }
                 },
            stream = false
        };

        var response = await httpClient.PostAsJsonAsync(
            "/api/chat",
            request,
            cancellationToken);

        var body = await response.Content.ReadAsStringAsync(
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Ollama returned {(int)response.StatusCode}: {body}");
        }

        var result =
            JsonSerializer.Deserialize<OllamaChatResponse>(
                body,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        return result?.Message?.Content
            ?? throw new InvalidOperationException(
                "Ollama returned no response.");
    }
}