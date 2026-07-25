using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioGamaEcuador.Models.Sales
{
    public class OrderDetail
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public Guid? CourseId { get; set; }
        public Course? Course { get; set; }

        public Guid? PhysicalProductId { get; set; }
        public PhysicalProduct? PhysicalProduct { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Quantity { get; set; } = 1;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal => UnitPrice * Quantity;
    }
}
