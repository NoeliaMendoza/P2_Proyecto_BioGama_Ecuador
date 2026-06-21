using System.ComponentModel.DataAnnotations;

namespace BioGamaEcuador.Models
{
    public class ConservationStatus
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El código es obligatorio.")]
        [StringLength(10, ErrorMessage = "El código debe tener máximo {1} caracteres.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre debe tener máximo {1} caracteres.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "La descripción debe tener máximo {1} caracteres.")]
        public string Description { get; set; } = string.Empty;

        public ICollection<Species> SpeciesList { get; set; } = new List<Species>();

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
