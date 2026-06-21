using System.ComponentModel.DataAnnotations;

namespace BioGamaEcuador.Models
{
    public class Location
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del lugar es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre del lugar debe tener máximo {1} caracteres.")]
        public string PlaceName { get; set; } = string.Empty;

        public int Altitude { get; set; }

        [Range(-90, 90, ErrorMessage = "La latitud debe estar entre {1} y {2}.")]
        public decimal Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "La longitud debe estar entre {1} y {2}.")]
        public decimal Longitude { get; set; }

        public int NaturalReserveId { get; set; }
        public NaturalReserve? NaturalReserve { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
