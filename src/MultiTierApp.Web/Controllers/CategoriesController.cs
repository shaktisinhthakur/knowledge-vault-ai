using Microsoft.AspNetCore.Mvc;
using MultiTierApp.Web.DTOs;
using MultiTierApp.Web.Services;

namespace MultiTierApp.Web.Controllers;

public class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // GET: /Categories
    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllAsync();
        return View(categories);
    }

    // GET: /Categories/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null) return NotFound();
        return View(category);
    }

    // GET: /Categories/Create
    public IActionResult Create()
    {
        return View(new CategoryDto());
    }

    // POST: /Categories/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        await _categoryService.CreateAsync(dto);
        TempData["Success"] = $"Category '{dto.Name}' created successfully!";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Categories/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null) return NotFound();
        return View(category);
    }

    // POST: /Categories/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryDto dto)
    {
        if (id != dto.Id) return BadRequest();
        if (!ModelState.IsValid) return View(dto);

        try
        {
            await _categoryService.UpdateAsync(dto);
            TempData["Success"] = $"Category '{dto.Name}' updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // GET: /Categories/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null) return NotFound();
        return View(category);
    }

    // POST: /Categories/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var category = await _categoryService.GetByIdAsync(id);
            await _categoryService.DeleteAsync(id);
            TempData["Success"] = $"Category '{category?.Name}' deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
