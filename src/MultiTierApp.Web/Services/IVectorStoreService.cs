namespace MultiTierApp.Web.Services;

/// <summary>
/// Manages vector storage and similarity search using Qdrant.
/// </summary>
public interface IVectorStoreService
{
    Task EnsureCollectionExistsAsync(int vectorSize = 384);
    Task UpsertChunksAsync(IEnumerable<(Guid vectorId, float[] embedding, string content, int documentId, string title)> chunks);
    Task<IEnumerable<(Guid vectorId, double score)>> SearchSimilarAsync(float[] queryVector, int topK = 5);
    Task DeleteVectorsAsync(IEnumerable<Guid> vectorIds);
}
