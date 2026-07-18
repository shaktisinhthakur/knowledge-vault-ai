using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace MultiTierApp.Web.Services;

/// <summary>
/// Stores and retrieves vector embeddings from Qdrant vector database.
/// </summary>
public class VectorStoreService : IVectorStoreService
{
    private readonly QdrantClient _qdrantClient;
    private readonly string _collectionName;
    private readonly ILogger<VectorStoreService> _logger;

    public VectorStoreService(QdrantClient qdrantClient, IConfiguration configuration, ILogger<VectorStoreService> logger)
    {
        _qdrantClient = qdrantClient;
        _collectionName = configuration["Qdrant:CollectionName"] ?? "document_chunks";
        _logger = logger;
    }

    public async Task EnsureCollectionExistsAsync(int vectorSize = 384)
    {
        try
        {
            var collections = await _qdrantClient.ListCollectionsAsync();
            if (collections.Any(c => c == _collectionName))
            {
                _logger.LogInformation("Qdrant collection '{Collection}' already exists.", _collectionName);
                return;
            }

            await _qdrantClient.CreateCollectionAsync(_collectionName, new VectorParams
            {
                Size = (ulong)vectorSize,
                Distance = Distance.Cosine
            });

            _logger.LogInformation("Created Qdrant collection '{Collection}' with vector size {Size}.", _collectionName, vectorSize);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not connect to Qdrant. Vector search features will be unavailable until Qdrant is running.");
        }
    }

    public async Task UpsertChunksAsync(IEnumerable<(Guid vectorId, float[] embedding, string content, int documentId, string title)> chunks)
    {
        var points = chunks.Select(chunk => new PointStruct
        {
            Id = new PointId { Uuid = chunk.vectorId.ToString() },
            Vectors = chunk.embedding,
            Payload =
            {
                ["content"] = chunk.content,
                ["document_id"] = chunk.documentId.ToString(),
                ["document_title"] = chunk.title
            }
        }).ToList();

        await _qdrantClient.UpsertAsync(_collectionName, points);
        _logger.LogInformation("Upserted {Count} vectors to Qdrant collection '{Collection}'.", points.Count, _collectionName);
    }

    public async Task<IEnumerable<(Guid vectorId, double score)>> SearchSimilarAsync(float[] queryVector, int topK = 5)
    {
        var searchResult = await _qdrantClient.SearchAsync(
            collectionName: _collectionName,
            vector: queryVector,
            limit: (ulong)topK
        );

        return searchResult.Select(r => (
            vectorId: Guid.Parse(r.Id.Uuid),
            score: (double)r.Score
        ));
    }

    public async Task DeleteVectorsAsync(IEnumerable<Guid> vectorIds)
    {
        var ids = vectorIds.Select(id => new PointId { Uuid = id.ToString() }).ToList();
        if (ids.Count == 0) return;

        await _qdrantClient.DeleteAsync(_collectionName, ids);
        _logger.LogInformation("Deleted {Count} vectors from Qdrant collection '{Collection}'.", ids.Count, _collectionName);
    }
}
