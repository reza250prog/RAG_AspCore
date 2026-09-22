using SemanticSearch.Evaluation;
using SemanticSearch.Models;
using SemanticSearch.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

const string ollamaUrl = "http://localhost:11434";

builder.Services.AddHttpClient<IEmbeddingService, OllamaEmbeddingService>(
    client =>
    {
        client.BaseAddress = new Uri(ollamaUrl);
    });

builder.Services.AddHttpClient<IChatService, OllamaChatService>(
    client =>
    {
        client.BaseAddress = new Uri(ollamaUrl);
    });

builder.Services.AddSingleton<InMemoryArticleRepository>();
builder.Services.AddScoped<SemanticSearchService>();
builder.Services.AddScoped<ArticleEmbeddingInitializer>();
builder.Services.AddScoped<RagService>();
builder.Services.AddScoped<RetrievalEvaluator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var articles = new List<Article>
{
    new()
    {
        Id = 1,
        Title = "Redis Caching",
        Description = "Using Redis as an in-memory distributed cache to improve application\r\nperformance, reduce database load, and provide fast access to frequently\r\nrequested data. Caching is especially useful for data that is requested\r\nrepeatedly and does not change frequently, allowing applications to avoid\r\nunnecessary database queries."
    },
    new()
    {
        Id = 2,
        Title = "Microservices Architecture",
        Description = "Designing distributed systems as a collection of small, independently deployable services, where each service owns a specific business capability and can be developed, scaled, and deployed independently."
    },
    new()
    {
        Id = 3,
        Title = "JWT Authentication",
        Description = "Implementing stateless authentication and authorization in ASP.NET Core APIs using JSON Web Tokens, including token validation, claims, roles, and access control."
    },
    new()
    {
        Id = 4,
        Title = "Kubernetes Scalability",
        Description = "Scaling applications horizontally with Kubernetes by running multiple pod replicas, distributing workloads across nodes, and automatically adjusting the number of application instances based on demand."
    },
    new()
    {
        Id = 5,
        Title = "Domain-Driven Design (DDD)",
        Description = "Modeling complex business domains using concepts such as Aggregate Roots, Entities, Value Objects, Domain Events, and bounded contexts to keep business rules close to the domain model."
    },
    new()
    {
        Id = 6,
        Title = "Clean Architecture",
        Description = "Clean Architecture is an application architecture pattern that organizes software into separate layers such as domain, application, infrastructure, and presentation. Business rules remain independent from frameworks, databases, UI, and external systems, with dependencies pointing inward toward the core business logic. This separation improves testability, maintainability, and the ability to change infrastructure without changing business rules."
    },
    new()
    {
        Id = 7,
        Title = "Distributed Transactions",
        Description = "Distributed transactions coordinate a business operation that spans multiple independent databases or services. They help maintain consistency when a single operation must update data across distributed systems. Common approaches include the Saga pattern, compensating actions, and reliable messaging instead of relying on a single ACID transaction across services."
    },
    new()
    {
        Id = 8,
        Title = "Event-Driven Architecture",
        Description = "Building reactive and loosely coupled distributed systems where services communicate through events and message brokers such as RabbitMQ or Apache Kafka."
    },
    new()
    {
        Id = 9,
        Title = "Observability and Distributed Tracing",
        Description = "Observability helps understand the internal behavior of distributed applications by collecting logs, metrics, and traces. Distributed tracing follows a request as it travels across multiple services, making it easier to identify latency, failures, and bottlenecks in microservice architectures."
    },
    new()
    {
        Id = 10,
        Title = "API Optimization with React Query",
        Description = "React Query is a library for managing server state in React applications.\r\nIt provides client-side caching, background refetching,\r\nrequest deduplication, synchronization, and automatic handling\r\nof loading and error states."
    }
};

using (var scope = app.Services.CreateScope())
{
    var initializer =
        scope.ServiceProvider
            .GetRequiredService<ArticleEmbeddingInitializer>();

    await initializer.InitializeAsync(articles);
}

app.MapPost("/embedding", async (
    string text,
    IEmbeddingService embeddingService,
    CancellationToken cancellationToken) =>
{
    var embedding =
        await embeddingService.GenerateAsync(
            text,
            cancellationToken);

    return Results.Ok(new
    {
        Text = text,
        Dimensions = embedding.Length,
        Embedding = embedding
    });
});

app.MapGet("/search", async (
    string q,
    SemanticSearchService searchService,
    CancellationToken cancellationToken) =>
{
    var results =
    await searchService.SearchAsync(
        q,
        topK: 10,
        minimumScore: 0.50f,
        cancellationToken: cancellationToken);

    return Results.Ok(
        results.Select(x => new
        {
            x.Article.Id,
            x.Article.Title,
            x.Article.Description,
            x.Score
        }));
});

app.MapGet("/ask", async (
    string q,
    RagService ragService,
    CancellationToken cancellationToken) =>
{
    var result = await ragService.AskAsync(
        q,
        cancellationToken);

    return Results.Ok(result);
});

app.MapGet(
    "/evaluation/retrieval",
    async (
        RetrievalEvaluator evaluator,
        CancellationToken cancellationToken) =>
    {
        var results = await evaluator.EvaluateAsync(cancellationToken);

        var summary = evaluator.Summarize(results);

        return Results.Ok(new
        {
            Summary = summary,
            Results = results
        });
    });

app.Run();