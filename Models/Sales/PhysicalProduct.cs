using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioGamaEcuador.Models.Sales
{
    public class PhysicalProduct
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(200, ErrorMessage = "El nombre debe tener máximo {1} caracteres.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, 10000.00, ErrorMessage = "El precio debe estar entre $0.01 y $10,000.00")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, 10000, ErrorMessage = "El stock debe estar entre 0 y 10,000.")]
        public int Stock { get; set; }

        public int ReservedStock { get; set; } = 0;

        public int AvailableStock => Stock - ReservedStock;

        [Range(0, 1000, ErrorMessage = "El stock mínimo debe estar entre 0 y 1,000.")]
        public int MinStock { get; set; } = 5;

        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;

        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public int? SpeciesId { get; set; }
        public Models.Species? Species { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
