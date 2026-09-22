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
                minimumScore: 0.50f,
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
                 You are a question-answering assistant.

                 Answer the user's question based ONLY on the context below.

                 Rules:
                 - Use the context to answer the question directly.
                 - Do not simply repeat or rephrase the user's question.
                 - If the context contains the answer, explain it clearly.
                 - If the context does not contain enough information, say:
                   "I don't have enough information to answer this question."
                 - Do not use outside knowledge.

                 Question:
                 {question}

                 Context:
                 {context}

                 Answer:
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