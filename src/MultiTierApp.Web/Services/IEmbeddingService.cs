namespace MultiTierApp.Web.Services;

/// <summary>
/// Generates vector embeddings for text using Ollama's local API.
/// </summary>
public interface IEmbeddingService
{
    Task<float[]> GetEmbeddingAsync(string text);
    Task<int> GetDimensionsAsync();
}
