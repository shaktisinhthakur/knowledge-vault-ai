namespace MultiTierApp.Web.Models;

public class DocumentChunk
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int ChunkIndex { get; set; } // Order within the document
    public Guid VectorId { get; set; } = Guid.NewGuid(); // ID used in the vector database (Qdrant)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign key to Document
    public int DocumentId { get; set; }
    public Document Document { get; set; } = null!;
}
