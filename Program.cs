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
        Description =
            "Using Redis to improve application performance and reduce database load."
    },

    new()
    {
        Id = 2,
        Title = "Microservices Architecture",
        Description =
            "Building distributed systems using independently deployable services."
    },

    new()
    {
        Id = 3,
        Title = "JWT Authentication",
        Description =
            "Implementing authentication and authorization in ASP.NET Core APIs."
    },

    new()
    {
        Id = 4,
        Title = "Kubernetes Scaling",
        Description =
            "Scaling applications horizontally using multiple pods and replicas."
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
            topK: 3,
            minimumScore: 0.70f,
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
    var answer = await ragService.AskAsync(
        q,
        cancellationToken);

    return Results.Ok(new
    {
        Question = q,
        Answer = answer
    });
});

app.Run();