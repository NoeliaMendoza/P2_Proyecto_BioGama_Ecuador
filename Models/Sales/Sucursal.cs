using System.ComponentModel.DataAnnotations;

namespace BioGamaEcuador.Models.Sales
{
    public class Sucursal
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [StringLength(50)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(200)]
        public string City { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
