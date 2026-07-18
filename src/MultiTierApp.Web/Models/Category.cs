namespace MultiTierApp.Web.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconClass { get; set; } = "bi-folder"; // Bootstrap icon class
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property — one category has many documents
    public ICollection<Document> Documents { get; set; } = new List<Document>();
}
