using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioGamaEcuador.Models.Sales
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;
        public IdentityUser User { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Tax { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [StringLength(500)]
        public string ShippingAddress { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Notes { get; set; } = string.Empty;

        public Payment? Payment { get; set; }
        public ICollection<OrderDetail> Details { get; set; } = new List<OrderDetail>();
    }
}
