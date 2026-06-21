using System.ComponentModel.DataAnnotations;

namespace BioGamaEcuador.Models
{
    public class Researcher
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre debe tener máximo {1} caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La institución es obligatoria.")]
        [StringLength(100, ErrorMessage = "La institución debe tener máximo {1} caracteres.")]
        public string Institution { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [StringLength(100, ErrorMessage = "El correo electrónico debe tener máximo {1} caracteres.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La especialidad es obligatoria.")]
        [StringLength(100, ErrorMessage = "La especialidad debe tener máximo {1} caracteres.")]
        public string Specialty { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
