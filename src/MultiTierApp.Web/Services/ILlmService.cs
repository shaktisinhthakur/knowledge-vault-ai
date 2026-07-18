namespace MultiTierApp.Web.Services;

/// <summary>
/// Generates LLM responses using Groq API.
/// </summary>
public interface ILlmService
{
    Task<string> GenerateResponseAsync(string userQuery, string context);
}
