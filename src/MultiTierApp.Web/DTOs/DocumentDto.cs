using System.ComponentModel.DataAnnotations;

namespace MultiTierApp.Web.DTOs;

public class DocumentDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Content is required.")]
    public string Content { get; set; } = string.Empty;

    public string Source { get; set; } = "manual";

    [Display(Name = "Category")]
    [Required(ErrorMessage = "Please select a category.")]
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }
    public bool IsEmbedded { get; set; }
    public int ChunkCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
