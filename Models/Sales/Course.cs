using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioGamaEcuador.Models.Sales
{
    public class Course
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(200, ErrorMessage = "El título debe tener máximo {1} caracteres.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(2000, ErrorMessage = "La descripción debe tener máximo {1} caracteres.")]
        public string Description { get; set; } = string.Empty;

        [StringLength(10000)]
        public string Syllabus { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, 10000.00, ErrorMessage = "El precio debe estar entre $0.01 y $10,000.00")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Los cupos totales son obligatorios.")]
        [Range(1, 500, ErrorMessage = "Los cupos totales deben estar entre 1 y 500.")]
        public int TotalSeats { get; set; }

        public int ReservedSeats { get; set; } = 0;
        public int ConfirmedSeats { get; set; } = 0;

        public int AvailableSeats => TotalSeats - ReservedSeats - ConfirmedSeats;

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "El horario de inicio es obligatorio.")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "El horario de fin es obligatorio.")]
        public TimeSpan EndTime { get; set; }

        [Required(ErrorMessage = "La modalidad es obligatoria.")]
        [StringLength(20)]
        public string Modality { get; set; } = string.Empty;

        [Required(ErrorMessage = "El lugar es obligatorio.")]
        [StringLength(300)]
        public string Venue { get; set; } = string.Empty;

        [Required(ErrorMessage = "El instructor es obligatorio.")]
        [StringLength(150)]
        public string Instructor { get; set; } = string.Empty;

        [StringLength(2000)]
        public string InstructorBio { get; set; } = string.Empty;

        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public bool RequiresPriorKnowledge { get; set; } = false;

        [StringLength(300)]
        public string TargetAudience { get; set; } = string.Empty;

        public int? SpeciesId { get; set; }
        public Models.Species? Species { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
