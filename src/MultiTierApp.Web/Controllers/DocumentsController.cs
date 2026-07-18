using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MultiTierApp.Web.DTOs;
using MultiTierApp.Web.Services;

namespace MultiTierApp.Web.Controllers;

public class DocumentsController : Controller
{
    private readonly IDocumentService _documentService;
    private readonly ICategoryService _categoryService;
    private readonly ILogger<DocumentsController> _logger;

    public DocumentsController(
        IDocumentService documentService,
        ICategoryService categoryService,
        ILogger<DocumentsController> logger)
    {
        _documentService = documentService;
        _categoryService = categoryService;
        _logger = logger;
    }

    // GET: /Documents
    public async Task<IActionResult> Index()
    {
        var documents = await _documentService.GetAllAsync();
        return View(documents);
    }

    // GET: /Documents/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var document = await _documentService.GetByIdAsync(id);
        if (document == null) return NotFound();
        return View(document);
    }

    // GET: /Documents/Create
    public async Task<IActionResult> Create()
    {
        await PopulateCategoriesDropdown();
        return View(new DocumentDto());
    }

    // POST: /Documents/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DocumentDto dto)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesDropdown(dto.CategoryId);
            return View(dto);
        }

        var document = await _documentService.CreateAsync(dto);
        TempData["Success"] = $"Document '{dto.Title}' created with {document.ChunkCount} chunks!";
        return RedirectToAction(nameof(Details), new { id = document.Id });
    }

    // POST: /Documents/Embed/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Embed(int id)
    {
        try
        {
            await _documentService.EmbedDocumentAsync(id);
            TempData["Success"] = "Document embedded successfully! It is now searchable via the AI assistant.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to embed document {Id}", id);
            TempData["Error"] = $"Failed to embed document: {ex.Message}";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    // GET: /Documents/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var document = await _documentService.GetByIdAsync(id);
        if (document == null) return NotFound();
        return View(document);
    }

    // POST: /Documents/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var document = await _documentService.GetByIdAsync(id);
            await _documentService.DeleteAsync(id);
            TempData["Success"] = $"Document '{document?.Title}' deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    private async Task PopulateCategoriesDropdown(int? selectedCategoryId = null)
    {
        var categories = await _categoryService.GetAllAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name", selectedCategoryId);
    }
}
