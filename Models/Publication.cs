using System.ComponentModel.DataAnnotations;

namespace BioGamaEcuador.Models
{
    public class Publication
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(200, ErrorMessage = "El título debe tener máximo {1} caracteres.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(150, ErrorMessage = "El nombre de la revista debe tener máximo {1} caracteres.")]
        public string Journal { get; set; } = string.Empty;

        [Range(1900, 2100, ErrorMessage = "El año debe estar entre {1} y {2}.")]
        public int PublicationYear { get; set; }

        [StringLength(300, ErrorMessage = "La URL debe tener máximo {1} caracteres.")]
        public string Url { get; set; } = string.Empty;

        public int ResearcherId { get; set; }
        public Researcher? Researcher { get; set; }

        public ICollection<Species> RelatedSpecies { get; set; } = new List<Species>();

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
