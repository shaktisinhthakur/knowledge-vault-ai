using Microsoft.AspNetCore.Mvc;
using MultiTierApp.Web.Services;

namespace MultiTierApp.Web.Controllers;

public class HomeController : Controller
{
    private readonly IDocumentService _documentService;

    public HomeController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    public async Task<IActionResult> Index()
    {
        var dashboard = await _documentService.GetDashboardDataAsync();
        return View(dashboard);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
