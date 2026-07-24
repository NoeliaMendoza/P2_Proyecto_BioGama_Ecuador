using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioGamaEcuador.Data;
using BioGamaEcuador.Services.Ollama;

namespace BioGamaEcuador.Controllers.Api
{
    public class IaQueryRequest
    {
        public string? Prompt { get; set; }
        public int? SpeciesId { get; set; }
        public string? TipoConsulta { get; set; } // general / ficha / conservacion
    }

    public class IaQueryResponse
    {
        public bool Success { get; set; }
        public string Modelo { get; set; } = "gemma2:2b";
        public string Respuesta { get; set; } = string.Empty;
        public string? Error { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    [ApiController]
    [Route("api/ia")]
    public class IaApiController : ControllerBase
    {
        private readonly IOllamaService _ollamaService;
        private readonly AppDbContext _context;

        public IaApiController(IOllamaService ollamaService, AppDbContext context)
        {
            _ollamaService = ollamaService;
            _context = context;
        }

        /// <summary>
        /// Endpoint API REST propio para consultar el modelo local Ollama (tinyllama)
        /// Ruta: POST /api/ia/generar
        /// </summary>
        [HttpPost("generar")]
        public async Task<IActionResult> Generar([FromBody] IaQueryRequest request)
        {
            if (request == null)
            {
                return BadRequest(new IaQueryResponse
                {
                    Success = false,
                    Error = "El cuerpo de la petición no puede estar vacío."
                });
            }

            string finalPrompt = request.Prompt ?? string.Empty;

            // Si se proporciona un id de especie, enriquecemos la consulta con el contexto del sistema
            if (request.SpeciesId.HasValue && request.SpeciesId.Value > 0)
            {
                var species = await _context.Species
                    .Include(s => s.ConservationStatus)
                    .Include(s => s.Family)
                    .FirstOrDefaultAsync(s => s.Id == request.SpeciesId.Value);

                if (species != null)
                {
                    string estadoCons = species.ConservationStatus?.Name ?? "No evaluado";
                    string endemica = species.IsEndemic ? "Sí, es endémica de Ecuador" : "No endémica";

                    if (request.TipoConsulta == "ficha")
                    {
                        finalPrompt = $"Genera una Ficha Ecológica completa en español para " +
                                      $"{species.CommonName}, nombre científico {species.ScientificName}, " +
                                      $"estado UICN {estadoCons}, endemismo {endemica}.";
                    }
                    else if (request.TipoConsulta == "conservacion")
                    {
                        finalPrompt = $"Elabora un Plan de Conservación Estratégico para la especie " +
                                      $"amenazada {species.CommonName}, estado UICN {estadoCons}.";
                    }
                    else
                    {
                        finalPrompt = $"Basándote en la especie {species.CommonName}, responde. {request.Prompt}";
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(finalPrompt))
            {
                return BadRequest(new IaQueryResponse
                {
                    Success = false,
                    Error = "Debe ingresar un prompt o seleccionar una especie válida."
                });
            }

            var result = await _ollamaService.GenerateResponseAsync(finalPrompt);

            if (!result.success)
            {
                return StatusCode(500, new IaQueryResponse { Success = false, Error = result.error });
            }

            return Ok(new IaQueryResponse { Success = true, Modelo = result.model, Respuesta = result.response });
        }
    }
}
