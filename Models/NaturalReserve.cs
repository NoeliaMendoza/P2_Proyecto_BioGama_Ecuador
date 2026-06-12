using System.ComponentModel.DataAnnotations;

namespace BioGamaEcuador.Models
{
    public class NaturalReserve
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Region { get; set; } = string.Empty;

        [Range(0.01, 9999999)]
        public decimal SurfaceHectares { get; set; }

        public int YearCreated { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}