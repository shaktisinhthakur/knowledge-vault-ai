namespace MultiTierApp.Web.DTOs;

public class DashboardDto
{
    public int TotalCategories { get; set; }
    public int TotalDocuments { get; set; }
    public int TotalChunks { get; set; }
    public int EmbeddedDocuments { get; set; }
    public List<DocumentDto> RecentDocuments { get; set; } = new();
}
