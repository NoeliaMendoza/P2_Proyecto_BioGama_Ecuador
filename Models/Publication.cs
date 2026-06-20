using System.ComponentModel.DataAnnotations;

namespace BioGamaEcuador.Models
{
    public class Publication
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(150)]
        public string Journal { get; set; } = string.Empty;

        [Range(1900, 2100)]
        public int PublicationYear { get; set; }

        [StringLength(300)]
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