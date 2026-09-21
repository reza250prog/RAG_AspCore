namespace SemanticSearch.Services;

public sealed class RagService(SemanticSearchService searchService,
                               IChatService chatService)
{
    public async Task<RagResponse> AskAsync(
        string question,
        CancellationToken cancellationToken = default)
    {
        var results =
            await searchService.SearchAsync(
                question,
                topK: 3,
                minimumScore: 0.70f,
                cancellationToken: cancellationToken);

        if (results.Count == 0)
        {
            return new RagResponse
            {
                Question = question,
                Answer =
                    "I don't have enough information to answer this question."
            };
        }

        var context = string.Join(
            "\n\n",
            results.Select(x =>
                $"""
                Title: {x.Article.Title}
                Description: {x.Article.Description}
                """));

        var prompt =
            $"""
            You are a helpful assistant.

            Answer the user's question using ONLY the provided context.

            If the answer cannot be found in the context,
            say that you don't have enough information.

            Do not use outside knowledge.

            User question:
            {question}

            Context:
            {context}
            """;

        var answer =
            await chatService.GenerateAsync(
                prompt,
                cancellationToken);

        return new RagResponse
        {
            Question = question,
            Answer = answer,
            Sources = [.. results
                .Select(x => new RagSource
                {
                    Id = x.Article.Id,
                    Title = x.Article.Title,
                    Score = x.Score
                })]
        };
    }
}