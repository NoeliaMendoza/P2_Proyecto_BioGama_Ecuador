using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioGamaEcuador.Models.Sales
{
    public class Payment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string Provider { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ExternalId { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3)]
        public string Currency { get; set; } = "USD";

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ConfirmedAt { get; set; }

        [StringLength(4000)]
        public string GatewayResponse { get; set; } = string.Empty;

        public int VerificationAttempts { get; set; } = 0;
    }
}
