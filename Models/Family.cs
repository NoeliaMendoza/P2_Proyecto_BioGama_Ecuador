using System.ComponentModel.DataAnnotations;

namespace BioGamaEcuador.Models
{
    public class Family
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la familia es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre debe tener máximo {1} caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El reino es obligatorio.")]
        [StringLength(50, ErrorMessage = "El reino debe tener máximo {1} caracteres.")]
        public string Kingdom { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
