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
        Description = "Using Redis as an in-memory distributed cache to improve application performance, reduce database load, and provide fast access to frequently requested data."
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
        Description = "Structuring applications so that business logic remains independent from frameworks, databases, and external infrastructure, making the system easier to test, maintain, and evolve."
    },
    new()
    {
        Id = 7,
        Title = "Distributed Transactions",
        Description = "Handling data consistency across multiple microservices without relying on traditional distributed transactions by using patterns such as Saga and Transactional Outbox."
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
        Description = "Monitoring and troubleshooting distributed applications using logs, metrics, traces, and tools such as OpenTelemetry, Jaeger, Prometheus, and Grafana."
    },
    new()
    {
        Id = 10,
        Title = "API Optimization with React Query",
        Description = "Managing server state in React applications with features such as client-side caching, background refetching, request deduplication, synchronization, and automatic handling of loading and error states."
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
    //var results =
    //    await searchService.SearchAsync(
    //        q,
    //        topK: 3,
    //        minimumScore: 0.70f,
    //        cancellationToken: cancellationToken);

    var results =
    await searchService.SearchAsync(
        q,
        topK: 10,
        minimumScore: 0.0f,
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

app.Run();