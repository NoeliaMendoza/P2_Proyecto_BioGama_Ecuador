using System.ComponentModel.DataAnnotations;
using BioGamaEcuador.Models.Sales;

namespace BioGamaEcuador.Models.Admin;

public sealed class CourseFormViewModel
{
    public Guid Id { get; set; }
    [Required, StringLength(200)] public string Title { get; set; } = string.Empty;
    [Required, StringLength(2000)] public string Description { get; set; } = string.Empty;
    [StringLength(5000)] public string Syllabus { get; set; } = string.Empty;
    [Range(0.01, 10000)] public decimal Price { get; set; }
    [Range(1, 500)] public int TotalSeats { get; set; }
    [Required] public DateTime StartDate { get; set; } = DateTime.Today;
    [Required] public DateTime EndDate { get; set; } = DateTime.Today;
    [Required] public TimeSpan StartTime { get; set; }
    [Required] public TimeSpan EndTime { get; set; }
    [Required] public string Modality { get; set; } = "Presencial";
    [Required, StringLength(300)] public string Venue { get; set; } = string.Empty;
    [Required, StringLength(150)] public string Instructor { get; set; } = string.Empty;
    [StringLength(1000)] public string InstructorBio { get; set; } = string.Empty;
    [StringLength(500)] public string ImageUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool RequiresPriorKnowledge { get; set; }
    [StringLength(300)] public string TargetAudience { get; set; } = string.Empty;
    public int? SpeciesId { get; set; }
}

public sealed class ProductFormViewModel
{
    public Guid Id { get; set; }
    [Required, StringLength(200)] public string Name { get; set; } = string.Empty;
    [StringLength(2000)] public string Description { get; set; } = string.Empty;
    [Range(0.01, 10000)] public decimal Price { get; set; }
    [Range(0, 10000)] public int Stock { get; set; }
    [Range(0, 1000)] public int MinStock { get; set; } = 5;
    [Required, StringLength(50)] public string SKU { get; set; } = string.Empty;
    [StringLength(500)] public string ImageUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int? SpeciesId { get; set; }
}

public class InventoryTransactionViewModel
{
    [Required] public Guid ProductId { get; set; }
    [Range(1, 10000)] public int Quantity { get; set; }
    [Required, StringLength(200)] public string Reference { get; set; } = string.Empty;
    [StringLength(1000)] public string Notes { get; set; } = string.Empty;
}

public sealed class InventoryAdjustmentViewModel : InventoryTransactionViewModel
{
    [Range(0, 10000)] public int NewStock { get; set; }
}

public sealed class UserListViewModel
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
}

public sealed class UserEditViewModel
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<RoleCheckItem> Roles { get; set; } = new();
}

public sealed class RoleCheckItem
{
    public string RoleName { get; set; } = string.Empty;
    public bool IsAssigned { get; set; }
}

public sealed class AdminDashboardViewModel
{
    public int ActiveCourses { get; set; }
    public int LowStockProducts { get; set; }
    public int MonthlyEnrollments { get; set; }
    public IReadOnlyList<PhysicalProduct> LowStockItems { get; set; } = Array.Empty<PhysicalProduct>();
}
