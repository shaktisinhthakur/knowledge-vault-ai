using MultiTierApp.Web.Models;

namespace MultiTierApp.Web.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByIdWithDocumentsAsync(int id);
    Task<IEnumerable<Category>> GetAllWithDocumentCountAsync();
}
