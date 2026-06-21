using System.ComponentModel.DataAnnotations;

namespace BioGamaEcuador.Models
{
    public class Species
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre común es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre común debe tener máximo {1} caracteres.")]
        public string CommonName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre científico es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre científico debe tener máximo {1} caracteres.")]
        public string ScientificName { get; set; } = string.Empty;

        public int? ConservationStatusId { get; set; }
        public ConservationStatus? ConservationStatus { get; set; }

        [StringLength(500, ErrorMessage = "La descripción debe tener máximo {1} caracteres.")]
        public string Description { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "La URL de la imagen debe tener máximo {1} caracteres.")]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsEndemic { get; set; } = false;

        public int FamilyId { get; set; }
        public Family? Family { get; set; }

        public ICollection<Publication> Publications { get; set; } = new List<Publication>();

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
