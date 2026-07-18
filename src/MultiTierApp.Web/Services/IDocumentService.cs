using MultiTierApp.Web.DTOs;

namespace MultiTierApp.Web.Services;

public interface IDocumentService
{
    Task<IEnumerable<DocumentDto>> GetAllAsync();
    Task<DocumentDto?> GetByIdAsync(int id);
    Task<DocumentDto> CreateAsync(DocumentDto dto);
    Task DeleteAsync(int id);
    Task<DashboardDto> GetDashboardDataAsync();
    Task EmbedDocumentAsync(int documentId);
}
