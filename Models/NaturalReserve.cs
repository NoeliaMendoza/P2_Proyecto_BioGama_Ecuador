using System.ComponentModel.DataAnnotations;

namespace BioGamaEcuador.Models
{
    public class NaturalReserve
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la reserva es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre debe tener máximo {1} caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La región es obligatoria.")]
        [StringLength(50, ErrorMessage = "La región debe tener máximo {1} caracteres.")]
        public string Region { get; set; } = string.Empty;

        [Range(0.01, 9999999, ErrorMessage = "La superficie debe estar entre {1} y {2} hectáreas.")]
        public decimal SurfaceHectares { get; set; }

        public int YearCreated { get; set; }

        [StringLength(500, ErrorMessage = "La descripción debe tener máximo {1} caracteres.")]
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
