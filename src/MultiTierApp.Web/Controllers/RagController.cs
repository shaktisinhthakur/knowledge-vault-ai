using Microsoft.AspNetCore.Mvc;
using MultiTierApp.Web.DTOs;
using MultiTierApp.Web.Services;

namespace MultiTierApp.Web.Controllers;

public class RagController : Controller
{
    private readonly IRagService _ragService;

    public RagController(IRagService ragService)
    {
        _ragService = ragService;
    }

    // GET: /Rag — renders the chat page
    public IActionResult Index()
    {
        return View();
    }

    // POST: /Rag/Ask — AJAX endpoint for chat
    [HttpPost]
    public async Task<IActionResult> Ask([FromBody] RagQueryDto query)
    {
        if (query == null || string.IsNullOrWhiteSpace(query.Question))
        {
            return Json(new RagResponseDto
            {
                Success = false,
                ErrorMessage = "Please enter a question."
            });
        }

        var response = await _ragService.AskAsync(query.Question);
        return Json(response);
    }
}
