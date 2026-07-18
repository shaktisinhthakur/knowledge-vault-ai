using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MultiTierApp.Web.Services;

/// <summary>
/// Calls Groq API (OpenAI-compatible) for LLM chat completions.
/// Your Groq API key goes in appsettings.json → GroqApi:ApiKey
/// </summary>
public class LlmService : ILlmService
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly ILogger<LlmService> _logger;

    public LlmService(HttpClient httpClient, IConfiguration configuration, ILogger<LlmService> logger)
    {
        _httpClient = httpClient;
        _model = configuration["GroqApi:Model"] ?? "llama-3.3-70b-versatile";
        _logger = logger;

        var apiKey = configuration["GroqApi:ApiKey"];
        if (!string.IsNullOrEmpty(apiKey) && apiKey != "YOUR_GROQ_API_KEY_HERE")
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }
    }

    public async Task<string> GenerateResponseAsync(string userQuery, string context)
    {
        var systemPrompt = @"You are a helpful AI assistant that answers questions based on the provided context documents. 
Your answers should be:
- Accurate and grounded in the provided context
- Clear and well-structured
- If the context doesn't contain relevant information, say so honestly
- Reference which document(s) your answer comes from when possible

CONTEXT DOCUMENTS:
" + context;

        var requestBody = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userQuery }
            },
            temperature = 0.3,
            max_tokens = 1024
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync("/openai/v1/chat/completions", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Groq API error: {Status} — {Body}", response.StatusCode, errorBody);
                return $"Sorry, the AI service returned an error (HTTP {(int)response.StatusCode}). Please check your API key configuration.";
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            var answer = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return answer ?? "No response generated.";
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to connect to Groq API.");
            return "Could not connect to the Groq AI service. Please verify your API key in appsettings.json and check your internet connection.";
        }
    }
}
