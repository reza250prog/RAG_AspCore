namespace SemanticSearch.Evaluation;

public sealed record RetrievalEvaluationSummary(
    int TotalQueries,
    int Top1Hits,
    int Top3Hits,
    double Top1Accuracy,
    double Top3Accuracy);