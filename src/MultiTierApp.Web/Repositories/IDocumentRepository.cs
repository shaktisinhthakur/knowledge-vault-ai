using MultiTierApp.Web.Models;

namespace MultiTierApp.Web.Repositories;

public interface IDocumentRepository : IRepository<Document>
{
    Task<Document?> GetByIdWithChunksAsync(int id);
    Task<IEnumerable<Document>> GetAllWithCategoryAsync();
    Task<IEnumerable<Document>> GetRecentAsync(int count);
    Task<Document?> GetByIdWithCategoryAsync(int id);
    Task<IEnumerable<DocumentChunk>> GetChunksByVectorIdsAsync(IEnumerable<Guid> vectorIds);
    Task<int> GetChunkCountAsync();
    Task<int> GetEmbeddedDocumentCountAsync();
}
