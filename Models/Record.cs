using System.ComponentModel.DataAnnotations;

namespace BioGamaEcuador.Models
{
    public class Record
    {
        public int Id { get; set; }

        public int SpeciesId { get; set; }
        public Species? Species { get; set; }

        public int LocationId { get; set; }
        public Location? Location { get; set; }

        public int ResearcherId { get; set; }
        public Researcher? Researcher { get; set; }

        [Required]
        public DateTime DiscoveryDate { get; set; }

        [Range(1, 99999)]
        public int ObservedQuantity { get; set; }

        [StringLength(500)]
        public string Observations { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}