namespace MultiTierApp.Web.Models;

public class Document
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty; // e.g., "uploaded", "manual"
    public bool IsEmbedded { get; set; } = false; // Whether chunks have been embedded in vector DB
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Foreign key to Category
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    // Navigation property — one document has many chunks
    public ICollection<DocumentChunk> Chunks { get; set; } = new List<DocumentChunk>();
}
