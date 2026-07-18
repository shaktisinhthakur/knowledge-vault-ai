using System.Text;
using System.Text.Json;

namespace MultiTierApp.Web.Services;

/// <summary>
/// Calls Ollama's local API to generate vector embeddings using the all-minilm model.
/// </summary>
public class EmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly ILogger<EmbeddingService> _logger;
    private int? _cachedDimensions;

    public EmbeddingService(HttpClient httpClient, IConfiguration configuration, ILogger<EmbeddingService> logger)
    {
        _httpClient = httpClient;
        _model = configuration["Ollama:EmbeddingModel"] ?? "all-minilm";
        _logger = logger;
    }

    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        var requestBody = new
        {
            model = _model,
            prompt = text
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync("/api/embeddings", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            var embeddingArray = doc.RootElement.GetProperty("embedding");
            var embedding = new float[embeddingArray.GetArrayLength()];
            var index = 0;
            foreach (var element in embeddingArray.EnumerateArray())
            {
                embedding[index++] = element.GetSingle();
            }

            return embedding;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to connect to Ollama. Make sure Ollama is running at the configured URL.");
            throw new InvalidOperationException(
                "Could not connect to Ollama embedding service. Please ensure Ollama is running " +
                "and the all-minilm model is pulled. Run: docker-compose up -d && docker exec ollama ollama pull all-minilm", ex);
        }
    }

    public async Task<int> GetDimensionsAsync()
    {
        if (_cachedDimensions.HasValue)
            return _cachedDimensions.Value;

        var testEmbedding = await GetEmbeddingAsync("test");
        _cachedDimensions = testEmbedding.Length;
        return _cachedDimensions.Value;
    }
}
