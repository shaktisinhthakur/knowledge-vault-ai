using MultiTierApp.Web.DTOs;

namespace MultiTierApp.Web.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<CategoryDto> CreateAsync(CategoryDto dto);
    Task UpdateAsync(CategoryDto dto);
    Task DeleteAsync(int id);
}
