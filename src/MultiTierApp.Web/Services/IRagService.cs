using MultiTierApp.Web.DTOs;

namespace MultiTierApp.Web.Services;

/// <summary>
/// Orchestrates the full RAG pipeline: embed query → search vectors → build context → call LLM.
/// </summary>
public interface IRagService
{
    Task<RagResponseDto> AskAsync(string question);
}
