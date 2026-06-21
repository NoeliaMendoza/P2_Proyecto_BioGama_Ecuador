using System.ComponentModel.DataAnnotations;

namespace BioGamaEcuador.Models
{
    public class Species
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CommonName { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string ScientificName { get; set; } = string.Empty;

        public int? ConservationStatusId { get; set; }
        public ConservationStatus? ConservationStatus { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [StringLength(300)]
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