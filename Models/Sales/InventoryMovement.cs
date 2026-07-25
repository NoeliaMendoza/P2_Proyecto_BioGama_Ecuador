using System.ComponentModel.DataAnnotations;

namespace BioGamaEcuador.Models.Sales
{
    public class InventoryMovement
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? PhysicalProductId { get; set; }
        public PhysicalProduct? PhysicalProduct { get; set; }

        public Guid? CourseId { get; set; }
        public Course? Course { get; set; }

        public Guid? SucursalId { get; set; }
        public Sucursal? Sucursal { get; set; }

        [Required]
        [StringLength(30)]
        public string TipoMovimiento { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        public int StockAnterior { get; set; }
        public int StockPosterior { get; set; }

        [Required]
        [StringLength(200)]
        public string Referencia { get; set; } = string.Empty;

        public DateTime FechaMovimiento { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(450)]
        public string UsuarioId { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Observacion { get; set; } = string.Empty;
    }
}
