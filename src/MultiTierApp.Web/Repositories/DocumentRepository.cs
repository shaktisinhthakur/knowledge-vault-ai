using Microsoft.EntityFrameworkCore;
using MultiTierApp.Web.Data;
using MultiTierApp.Web.Models;

namespace MultiTierApp.Web.Repositories;

public class DocumentRepository : Repository<Document>, IDocumentRepository
{
    public DocumentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Document?> GetByIdWithChunksAsync(int id)
    {
        return await _dbSet
            .Include(d => d.Chunks.OrderBy(c => c.ChunkIndex))
            .Include(d => d.Category)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<Document>> GetAllWithCategoryAsync()
    {
        return await _dbSet
            .Include(d => d.Category)
            .Include(d => d.Chunks)
            .OrderByDescending(d => d.UpdatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetRecentAsync(int count)
    {
        return await _dbSet
            .Include(d => d.Category)
            .OrderByDescending(d => d.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Document?> GetByIdWithCategoryAsync(int id)
    {
        return await _dbSet
            .Include(d => d.Category)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<DocumentChunk>> GetChunksByVectorIdsAsync(IEnumerable<Guid> vectorIds)
    {
        var idList = vectorIds.ToList();
        return await _context.DocumentChunks
            .Include(c => c.Document)
            .Where(c => idList.Contains(c.VectorId))
            .ToListAsync();
    }

    public async Task<int> GetChunkCountAsync()
    {
        return await _context.DocumentChunks.CountAsync();
    }

    public async Task<int> GetEmbeddedDocumentCountAsync()
    {
        return await _dbSet.CountAsync(d => d.IsEmbedded);
    }
}
