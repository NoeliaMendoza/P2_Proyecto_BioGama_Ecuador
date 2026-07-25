namespace BioGamaEcuador.Services;

public class AIResult
{
    public bool Success { get; set; }
    public string Response { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}

public interface IAIService
{
    Task<AIResult> GenerateAsync(string instruction, CancellationToken cancellationToken = default);
}
