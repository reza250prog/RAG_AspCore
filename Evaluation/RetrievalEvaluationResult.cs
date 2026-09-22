namespace SemanticSearch.Evaluation;

public sealed record RetrievalEvaluationResult(
    string Query,
    QueryType Type,
    int? ExpectedArticleId,
    int? ActualTopArticleId,
    float? TopScore,
    int? ExpectedRank,
    float? ExpectedScore,
    bool Top1Hit,
    bool Top3Hit);