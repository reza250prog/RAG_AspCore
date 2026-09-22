namespace SemanticSearch.Evaluation;

public static class RetrievalTestCases
{
    public static IReadOnlyList<RetrievalTestCase> All =>
    [
        // Direct
        new("What is Redis?", 1, QueryType.Direct),
        new("What is React Query?", 10, QueryType.Direct),
        new("What is Clean Architecture?", 6, QueryType.Direct),
        new("What is Domain-Driven Design?", 5, QueryType.Direct),
        new("What is distributed tracing?", 9, QueryType.Direct),

        // Paraphrased
        new(
            "How can I avoid repeatedly querying the database for the same data?",
            1,
            QueryType.Paraphrased),

        new(
            "How can a React application avoid fetching the same server data repeatedly?",
            10,
            QueryType.Paraphrased),

        new(
            "How can I keep business rules independent from infrastructure?",
            6,
            QueryType.Paraphrased),

        new(
            "How can I model complex business rules around the domain?",
            5,
            QueryType.Paraphrased),

        new(
            "How can I follow a request across multiple services?",
            9,
            QueryType.Paraphrased),

        // Conversational
        new(
            "My API keeps hitting the database for the same information. What can I do?",
            1,
            QueryType.Conversational),

        new(
            "My React app keeps requesting server data. Is there something that can manage that state?",
            10,
            QueryType.Conversational),

        new(
            "I don't want my business logic to depend on databases and frameworks.",
            6,
            QueryType.Conversational),

        new(
            "I have a complicated business domain and need a better way to model it.",
            5,
            QueryType.Conversational),

        new(
            "A request goes through several services and I need to know where it becomes slow.",
            9,
            QueryType.Conversational),

        // Technical
        new(
            "How can I implement a distributed in-memory cache for frequently accessed data?",
            1,
            QueryType.Technical),

        new(
            "How can I handle client-side caching and background refetching of server state in React?",
            10,
            QueryType.Technical),

        new(
            "How should dependencies flow between domain, application, infrastructure, and presentation layers?",
            6,
            QueryType.Technical),

        new(
            "How can Aggregate Roots and Value Objects be used to model a business domain?",
            5,
            QueryType.Technical),

        new(
            "How can distributed tracing identify latency across microservices?",
            9,
            QueryType.Technical),

        // Out of domain
        new(
            "How do I configure PostgreSQL backups?",
            null,
            QueryType.OutOfDomain),

        new(
            "How do I build a mobile application with Flutter?",
            null,
            QueryType.OutOfDomain),

        new(
            "What is the best way to cook pasta?",
            null,
            QueryType.OutOfDomain),

        new(
            "How can I configure an Nginx reverse proxy?",
            null,
            QueryType.OutOfDomain),

        new(
            "How do I deploy a Python machine learning model?",
            null,
            QueryType.OutOfDomain)
    ];
}