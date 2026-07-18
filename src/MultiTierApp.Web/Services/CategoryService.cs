using MultiTierApp.Web.DTOs;
using MultiTierApp.Web.Models;
using MultiTierApp.Web.Repositories;

namespace MultiTierApp.Web.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllWithDocumentCountAsync();
        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            IconClass = c.IconClass,
            DocumentCount = c.Documents.Count,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        });
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdWithDocumentsAsync(id);
        if (category == null) return null;

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IconClass = category.IconClass,
            DocumentCount = category.Documents.Count,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    public async Task<CategoryDto> CreateAsync(CategoryDto dto)
    {
        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            IconClass = dto.IconClass,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _categoryRepository.AddAsync(category);

        dto.Id = category.Id;
        dto.CreatedAt = category.CreatedAt;
        dto.UpdatedAt = category.UpdatedAt;
        return dto;
    }

    public async Task UpdateAsync(CategoryDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(dto.Id);
        if (category == null)
            throw new KeyNotFoundException($"Category with ID {dto.Id} not found.");

        category.Name = dto.Name;
        category.Description = dto.Description;
        category.IconClass = dto.IconClass;
        category.UpdatedAt = DateTime.UtcNow;

        await _categoryRepository.UpdateAsync(category);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            throw new KeyNotFoundException($"Category with ID {id} not found.");

        await _categoryRepository.DeleteAsync(category);
    }
}
