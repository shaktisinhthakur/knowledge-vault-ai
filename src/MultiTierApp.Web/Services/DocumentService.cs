using MultiTierApp.Web.DTOs;
using MultiTierApp.Web.Models;
using MultiTierApp.Web.Repositories;

namespace MultiTierApp.Web.Services;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorStoreService _vectorStoreService;
    private readonly ILogger<DocumentService> _logger;

    public DocumentService(
        IDocumentRepository documentRepository,
        ICategoryRepository categoryRepository,
        IEmbeddingService embeddingService,
        IVectorStoreService vectorStoreService,
        ILogger<DocumentService> logger)
    {
        _documentRepository = documentRepository;
        _categoryRepository = categoryRepository;
        _embeddingService = embeddingService;
        _vectorStoreService = vectorStoreService;
        _logger = logger;
    }

    public async Task<IEnumerable<DocumentDto>> GetAllAsync()
    {
        var documents = await _documentRepository.GetAllWithCategoryAsync();
        return documents.Select(MapToDto);
    }

    public async Task<DocumentDto?> GetByIdAsync(int id)
    {
        var document = await _documentRepository.GetByIdWithChunksAsync(id);
        if (document == null) return null;
        return MapToDto(document);
    }

    public async Task<DocumentDto> CreateAsync(DocumentDto dto)
    {
        var document = new Document
        {
            Title = dto.Title,
            Content = dto.Content,
            Source = dto.Source,
            CategoryId = dto.CategoryId,
            IsEmbedded = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Chunk the content
        var chunks = ChunkText(dto.Content);
        foreach (var (text, index) in chunks.Select((t, i) => (t, i)))
        {
            document.Chunks.Add(new DocumentChunk
            {
                Content = text,
                ChunkIndex = index,
                VectorId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            });
        }

        await _documentRepository.AddAsync(document);

        dto.Id = document.Id;
        dto.ChunkCount = document.Chunks.Count;
        dto.CreatedAt = document.CreatedAt;
        return dto;
    }

    public async Task DeleteAsync(int id)
    {
        var document = await _documentRepository.GetByIdWithChunksAsync(id);
        if (document == null)
            throw new KeyNotFoundException($"Document with ID {id} not found.");

        // Remove vectors from Qdrant
        if (document.IsEmbedded)
        {
            try
            {
                var vectorIds = document.Chunks.Select(c => c.VectorId).ToList();
                await _vectorStoreService.DeleteVectorsAsync(vectorIds);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete vectors from Qdrant for document {Id}. Continuing with DB deletion.", id);
            }
        }

        await _documentRepository.DeleteAsync(document);
    }

    public async Task<DashboardDto> GetDashboardDataAsync()
    {
        var totalCategories = await _categoryRepository.CountAsync();
        var totalDocuments = await _documentRepository.CountAsync();
        var totalChunks = await _documentRepository.GetChunkCountAsync();
        var embeddedDocuments = await _documentRepository.GetEmbeddedDocumentCountAsync();
        var recentDocs = await _documentRepository.GetRecentAsync(5);

        return new DashboardDto
        {
            TotalCategories = totalCategories,
            TotalDocuments = totalDocuments,
            TotalChunks = totalChunks,
            EmbeddedDocuments = embeddedDocuments,
            RecentDocuments = recentDocs.Select(MapToDto).ToList()
        };
    }

    public async Task EmbedDocumentAsync(int documentId)
    {
        var document = await _documentRepository.GetByIdWithChunksAsync(documentId);
        if (document == null)
            throw new KeyNotFoundException($"Document with ID {documentId} not found.");

        if (document.Chunks.Count == 0)
        {
            _logger.LogWarning("Document {Id} has no chunks to embed.", documentId);
            return;
        }

        _logger.LogInformation("Embedding {Count} chunks for document '{Title}'...", document.Chunks.Count, document.Title);

        // Generate embeddings for each chunk and store in Qdrant
        var chunkDataList = new List<(Guid vectorId, float[] embedding, string content, int documentId, string title)>();

        foreach (var chunk in document.Chunks)
        {
            try
            {
                var embedding = await _embeddingService.GetEmbeddingAsync(chunk.Content);
                chunkDataList.Add((chunk.VectorId, embedding, chunk.Content, document.Id, document.Title));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to embed chunk {ChunkIndex} of document {DocId}", chunk.ChunkIndex, documentId);
                throw;
            }
        }

        await _vectorStoreService.UpsertChunksAsync(chunkDataList);

        document.IsEmbedded = true;
        document.UpdatedAt = DateTime.UtcNow;
        await _documentRepository.UpdateAsync(document);

        _logger.LogInformation("Successfully embedded document '{Title}'.", document.Title);
    }

    /// <summary>
    /// Splits text into chunks of approximately 300 words each with some overlap.
    /// </summary>
    private static List<string> ChunkText(string text, int chunkSize = 300, int overlap = 50)
    {
        var chunks = new List<string>();
        if (string.IsNullOrWhiteSpace(text)) return chunks;

        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (words.Length <= chunkSize)
        {
            chunks.Add(text.Trim());
            return chunks;
        }

        var start = 0;
        while (start < words.Length)
        {
            var end = Math.Min(start + chunkSize, words.Length);
            var chunk = string.Join(' ', words[start..end]);
            chunks.Add(chunk.Trim());
            start += chunkSize - overlap;
        }

        return chunks;
    }

    private static DocumentDto MapToDto(Document doc)
    {
        return new DocumentDto
        {
            Id = doc.Id,
            Title = doc.Title,
            Content = doc.Content,
            Source = doc.Source,
            CategoryId = doc.CategoryId,
            CategoryName = doc.Category?.Name,
            IsEmbedded = doc.IsEmbedded,
            ChunkCount = doc.Chunks.Count,
            CreatedAt = doc.CreatedAt,
            UpdatedAt = doc.UpdatedAt
        };
    }
}
