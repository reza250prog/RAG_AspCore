using SemanticSearch.Services;

namespace SemanticSearch.Evaluation;

public sealed class RetrievalEvaluator(SemanticSearchService searchService)
{
    public async Task<List<RetrievalEvaluationResult>> EvaluateAsync(
        CancellationToken cancellationToken = default)
    {
        var results = new List<RetrievalEvaluationResult>();

        foreach (var testCase in RetrievalTestCases.All)
        {
            var searchResults = await searchService.SearchAsync(
                testCase.Query,
                topK: 10,
                minimumScore: 0,
                cancellationToken);

            var topResult = searchResults.FirstOrDefault();

            // Out-of-domain query
            if (testCase.ExpectedArticleId is null)
            {
                results.Add(
                    new RetrievalEvaluationResult(
                        Query: testCase.Query,
                        Type: testCase.Type,
                        ExpectedArticleId: null,
                        ActualTopArticleId: topResult?.Article.Id,
                        TopScore: topResult?.Score,
                        ExpectedRank: null,
                        ExpectedScore: null,
                        Top1Hit: false,
                        Top3Hit: false));

                continue;
            }

            // In-domain query
            var expectedIndex = searchResults.FindIndex(
                x => x.Article.Id == testCase.ExpectedArticleId);

            var top1Hit =
                topResult is not null &&
                topResult.Article.Id == testCase.ExpectedArticleId;

            var top3Hit =
                searchResults
                    .Take(3)
                    .Any(x => x.Article.Id == testCase.ExpectedArticleId);

            results.Add(
                new RetrievalEvaluationResult(
                    Query: testCase.Query,
                    Type: testCase.Type,
                    ExpectedArticleId: testCase.ExpectedArticleId,
                    ActualTopArticleId: topResult?.Article.Id,
                    TopScore: topResult?.Score,
                    ExpectedRank: expectedIndex >= 0
                        ? expectedIndex + 1
                        : null,
                    ExpectedScore: expectedIndex >= 0
                        ? searchResults[expectedIndex].Score
                        : null,
                    Top1Hit: top1Hit,
                    Top3Hit: top3Hit));
        }

        return results;
    }

    public RetrievalEvaluationSummary Summarize(IReadOnlyList<RetrievalEvaluationResult> results)
    {
        if (results.Count == 0)
            return new RetrievalEvaluationSummary(
                0, 0, 0, 0, 0);

        var top1Hits = results.Count(x => x.Top1Hit);
        var top3Hits = results.Count(x => x.Top3Hit);

        return new RetrievalEvaluationSummary(
            TotalQueries: results.Count,
            Top1Hits: top1Hits,
            Top3Hits: top3Hits,
            Top1Accuracy: (double)top1Hits / results.Count,
            Top3Accuracy: (double)top3Hits / results.Count);
    }
}
