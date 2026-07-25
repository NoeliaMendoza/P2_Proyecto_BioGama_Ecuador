using BioGamaEcuador.Services.Ollama;

namespace BioGamaEcuador.Services;

public class AIService : IAIService
{
    private readonly IOllamaService _ollama;

    public AIService(IOllamaService ollama)
    {
        _ollama = ollama;
    }

    public async Task<AIResult> GenerateAsync(string instruction, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _ollama.GenerateResponseAsync(instruction, "Eres un biologo experto en biodiversidad ecuatoriana. Responde en espanol, conciso.");
            return new AIResult
            {
                Success = result.success,
                Response = result.response,
                ErrorMessage = result.error
            };
        }
        catch (Exception ex)
        {
            return new AIResult
            {
                Success = false,
                ErrorMessage = $"Error al generar respuesta: {ex.Message}"
            };
        }
    }
}
