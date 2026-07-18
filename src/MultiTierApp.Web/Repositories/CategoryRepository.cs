using Microsoft.EntityFrameworkCore;
using MultiTierApp.Web.Data;
using MultiTierApp.Web.Models;

namespace MultiTierApp.Web.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetByIdWithDocumentsAsync(int id)
    {
        return await _dbSet
            .Include(c => c.Documents)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Category>> GetAllWithDocumentCountAsync()
    {
        return await _dbSet
            .Include(c => c.Documents)
            .OrderByDescending(c => c.UpdatedAt)
            .ToListAsync();
    }
}
