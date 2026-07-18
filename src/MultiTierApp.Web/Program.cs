using Microsoft.EntityFrameworkCore;
using MultiTierApp.Web.Data;
using MultiTierApp.Web.Repositories;
using MultiTierApp.Web.Services;
using Qdrant.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database Configuration (SQL Server Code-First)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Generic & Custom Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();

// Register Domain & Core Business Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();

// Register RAG Infrastructure Services
builder.Services.AddScoped<IVectorStoreService, VectorStoreService>();
builder.Services.AddScoped<IRagService, RagService>();

// Register Qdrant Client (gRPC)
builder.Services.AddSingleton(sp =>
{
    var host = builder.Configuration["Qdrant:Host"] ?? "localhost";
    var port = int.Parse(builder.Configuration["Qdrant:Port"] ?? "6334");
    return new QdrantClient(host, port);
});

// Configure and Register Embedding Service HttpClient
builder.Services.AddHttpClient<IEmbeddingService, EmbeddingService>(client =>
{
    var baseUrl = builder.Configuration["Ollama:BaseUrl"] ?? "http://localhost:11434";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Configure and Register Groq LLM Service HttpClient
builder.Services.AddHttpClient<ILlmService, LlmService>(client =>
{
    var baseUrl = builder.Configuration["GroqApi:BaseUrl"] ?? "https://api.groq.com";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(60);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Database Migration & Seeding + Vector Store Initialization on Startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        logger.LogInformation("Applying pending SQL Server migrations...");
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
        logger.LogInformation("SQL Server database migrated successfully.");

        logger.LogInformation("Initializing Qdrant Vector Collection...");
        var vectorStoreService = services.GetRequiredService<IVectorStoreService>();
        // Initialize Qdrant collection (all-minilm produces 384 dimensional vectors)
        await vectorStoreService.EnsureCollectionExistsAsync(384);
        logger.LogInformation("Vector DB initialization check complete.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database migration or vector DB initialization.");
    }
}

app.Run();
