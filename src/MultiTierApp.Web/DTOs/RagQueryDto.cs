namespace MultiTierApp.Web.DTOs;

public class RagQueryDto
{
    public string Question { get; set; } = string.Empty;
}

public class RagResponseDto
{
    public string Answer { get; set; } = string.Empty;
    public List<ChunkSourceDto> Sources { get; set; } = new();
    public bool Success { get; set; } = true;
    public string? ErrorMessage { get; set; }
}

public class ChunkSourceDto
{
    public string DocumentTitle { get; set; } = string.Empty;
    public string ChunkPreview { get; set; } = string.Empty;
    public double Score { get; set; }
}
