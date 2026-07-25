using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BioGamaEcuador.Models.Sales
{
    public class Enrollment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!;

        [Required]
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;
        public IdentityUser User { get; set; } = null!;

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "PendingPayment";

        public Guid? OrderId { get; set; }
        public Order? Order { get; set; }

        [Required]
        [StringLength(20)]
        public string ConfirmationCode { get; set; } = string.Empty;

        public DateTime? AttendedAt { get; set; }

        [StringLength(1000)]
        public string Notes { get; set; } = string.Empty;
    }
}
