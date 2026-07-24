using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using BioGamaEcuador.Settings;

namespace BioGamaEcuador.Services.Ollama
{
    public class OllamaService : IOllamaService
    {
        private readonly HttpClient _httpClient;
        private readonly OllamaSettings _settings;

        public OllamaService(HttpClient httpClient, IOptions<OllamaSettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;
            
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri(_settings.BaseUrl.TrimEnd('/') + "/");
            }
            _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
        }

        public async Task<OllamaResponseDto> GenerateResponseAsync(string prompt, string? systemPrompt = null)
        {
            var requestBody = new OllamaRequestDto
            {
                model = _settings.Model,
                prompt = prompt,
                system = systemPrompt ?? "Eres un biólogo experto en biodiversidad ecuatoriana. Responde en español, conciso.",
                stream = false
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/generate", requestBody);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new OllamaResponseDto
                    {
                        success = false,
                        error = $"Ollama respondió con código {(int)response.StatusCode}: {responseContent}"
                    };
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<OllamaResponseDto>(responseContent, options);

                if (result == null)
                {
                    return new OllamaResponseDto
                    {
                        success = false,
                        error = "No se pudo deserializar la respuesta obtenida de Ollama."
                    };
                }

                result.success = true;
                return result;
            }
            catch (Exception ex)
            {
                return new OllamaResponseDto
                {
                    success = false,
                    error = $"Error al comunicarse con Ollama ({_settings.BaseUrl}): {ex.Message}. Asegúrese de que Ollama esté ejecutándose."
                };
            }
        }
    }
}
