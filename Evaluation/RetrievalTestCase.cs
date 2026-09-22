namespace SemanticSearch.Evaluation;

public enum QueryType
{
    Direct,
    Paraphrased,
    Conversational,
    Technical,
    OutOfDomain
}

public sealed record RetrievalTestCase(
    string Query,
    int? ExpectedArticleId,
    QueryType Type);