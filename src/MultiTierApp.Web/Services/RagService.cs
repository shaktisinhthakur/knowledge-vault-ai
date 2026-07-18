using System.Text;
using MultiTierApp.Web.DTOs;
using MultiTierApp.Web.Repositories;

namespace MultiTierApp.Web.Services;

/// <summary>
/// Orchestrates the full RAG pipeline:
/// 1. Embed the user's query via Ollama
/// 2. Search Qdrant for top-K relevant chunks
/// 3. Retrieve chunk metadata from SQL Server
/// 4. Build a context prompt and call Groq LLM
/// </summary>
public class RagService : IRagService
{
    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorStoreService _vectorStoreService;
    private readonly ILlmService _llmService;
    private readonly IDocumentRepository _documentRepository;
    private readonly ILogger<RagService> _logger;

    public RagService(
        IEmbeddingService embeddingService,
        IVectorStoreService vectorStoreService,
        ILlmService llmService,
        IDocumentRepository documentRepository,
        ILogger<RagService> logger)
    {
        _embeddingService = embeddingService;
        _vectorStoreService = vectorStoreService;
        _llmService = llmService;
        _documentRepository = documentRepository;
        _logger = logger;
    }

    public async Task<RagResponseDto> AskAsync(string question)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                return new RagResponseDto
                {
                    Success = false,
                    ErrorMessage = "Please enter a question."
                };
            }

            _logger.LogInformation("RAG query: '{Question}'", question);

            // Step 1: Embed the user's query
            float[] queryEmbedding;
            try
            {
                queryEmbedding = await _embeddingService.GetEmbeddingAsync(question);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate embedding for query.");
                return new RagResponseDto
                {
                    Success = false,
                    ErrorMessage = "Could not generate embeddings. Please ensure Ollama is running with the all-minilm model."
                };
            }

            // Step 2: Search Qdrant for similar chunks
            var searchResults = await _vectorStoreService.SearchSimilarAsync(queryEmbedding, topK: 5);
            var resultsList = searchResults.ToList();

            if (resultsList.Count == 0)
            {
                return new RagResponseDto
                {
                    Success = true,
                    Answer = "I don't have any embedded documents to search through yet. Please add and embed some documents first, then try asking again.",
                    Sources = new List<ChunkSourceDto>()
                };
            }

            // Step 3: Retrieve chunk content from SQL Server
            var vectorIds = resultsList.Select(r => r.vectorId).ToList();
            var chunks = (await _documentRepository.GetChunksByVectorIdsAsync(vectorIds)).ToList();

            // Step 4: Build context string
            var contextBuilder = new StringBuilder();
            var sources = new List<ChunkSourceDto>();

            foreach (var result in resultsList)
            {
                var chunk = chunks.FirstOrDefault(c => c.VectorId == result.vectorId);
                if (chunk == null) continue;

                contextBuilder.AppendLine($"--- Document: {chunk.Document.Title} (Relevance: {result.score:F2}) ---");
                contextBuilder.AppendLine(chunk.Content);
                contextBuilder.AppendLine();

                sources.Add(new ChunkSourceDto
                {
                    DocumentTitle = chunk.Document.Title,
                    ChunkPreview = chunk.Content.Length > 150
                        ? chunk.Content[..150] + "..."
                        : chunk.Content,
                    Score = result.score
                });
            }

            // Step 5: Call Groq LLM with context
            var answer = await _llmService.GenerateResponseAsync(question, contextBuilder.ToString());

            _logger.LogInformation("RAG response generated with {SourceCount} source chunks.", sources.Count);

            return new RagResponseDto
            {
                Success = true,
                Answer = answer,
                Sources = sources
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RAG pipeline error.");
            return new RagResponseDto
            {
                Success = false,
                ErrorMessage = $"An error occurred while processing your question: {ex.Message}"
            };
        }
    }
}
