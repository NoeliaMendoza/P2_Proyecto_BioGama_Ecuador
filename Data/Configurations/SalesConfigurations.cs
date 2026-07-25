using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BioGamaEcuador.Models.Sales;

namespace BioGamaEcuador.Data.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> b)
        {
            b.ToTable("Courses");
            b.HasKey(c => c.Id);

            b.Property(c => c.Title).HasMaxLength(200).IsRequired();
            b.Property(c => c.Description).HasMaxLength(2000).IsRequired();
            b.Property(c => c.Syllabus).HasMaxLength(5000);
            b.Property(c => c.Price).HasPrecision(18, 2).IsRequired();
            b.Property(c => c.Modality).HasMaxLength(20).IsRequired();
            b.Property(c => c.Venue).HasMaxLength(300).IsRequired();
            b.Property(c => c.Instructor).HasMaxLength(150).IsRequired();
            b.Property(c => c.InstructorBio).HasMaxLength(1000);
            b.Property(c => c.ImageUrl).HasMaxLength(500);
            b.Property(c => c.TargetAudience).HasMaxLength(300);

            b.HasCheckConstraint("CK_Course_TotalSeats_Positive", "\"TotalSeats\" > 0");
            b.HasCheckConstraint("CK_Course_Seats_Consistent", "\"ReservedSeats\" >= 0 AND \"ConfirmedSeats\" >= 0 AND (\"ReservedSeats\" + \"ConfirmedSeats\") <= \"TotalSeats\"");
            b.HasCheckConstraint("CK_Course_Dates_Valid", "\"EndDate\" >= \"StartDate\"");
            b.HasCheckConstraint("CK_Course_Times_Valid", "\"EndTime\" > \"StartTime\"");
            b.HasCheckConstraint("CK_Course_Price_Positive", "\"Price\" > 0");

            b.HasIndex(c => c.IsActive);
            b.HasIndex(c => c.StartDate);
            b.HasIndex(c => new { c.IsActive, c.StartDate });
            b.HasIndex(c => c.SpeciesId);
            b.HasIndex(c => c.Instructor);
            b.HasIndex(c => c.Modality);

            b.HasOne(c => c.Species)
                .WithMany()
                .HasForeignKey(c => c.SpeciesId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Courses_Species");

            b.HasQueryFilter(c => c.DeletedAt == null);
        }
    }

    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> b)
        {
            b.ToTable("Enrollments");
            b.HasKey(e => e.Id);

            b.Property(e => e.Status).HasMaxLength(20).IsRequired();
            b.Property(e => e.ConfirmationCode).HasMaxLength(50).IsRequired();
            b.Property(e => e.Notes).HasMaxLength(1000);

            b.HasIndex(e => new { e.CourseId, e.UserId })
                .HasFilter("\"Status\" != 'Cancelled'")
                .IsUnique();

            b.HasIndex(e => e.UserId);
            b.HasIndex(e => e.Status);
            b.HasIndex(e => e.OrderId);
            b.HasIndex(e => e.ConfirmationCode).IsUnique();

            b.HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(e => e.Order)
                .WithMany()
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }

    public class PhysicalProductConfiguration : IEntityTypeConfiguration<PhysicalProduct>
    {
        public void Configure(EntityTypeBuilder<PhysicalProduct> b)
        {
            b.ToTable("PhysicalProducts");
            b.HasKey(p => p.Id);

            b.Property(p => p.Name).HasMaxLength(200).IsRequired();
            b.Property(p => p.Description).HasMaxLength(2000);
            b.Property(p => p.Price).HasPrecision(18, 2).IsRequired();
            b.Property(p => p.SKU).HasMaxLength(50);
            b.Property(p => p.ImageUrl).HasMaxLength(500);

            b.HasCheckConstraint("CK_PhysicalProduct_Stock_NonNegative", "\"Stock\" >= 0");
            b.HasCheckConstraint("CK_PhysicalProduct_ReservedStock_NonNegative", "\"ReservedStock\" >= 0");
            b.HasCheckConstraint("CK_PhysicalProduct_Price_Positive", "\"Price\" > 0");
            b.HasCheckConstraint("CK_PhysicalProduct_Stock_Consistent", "\"ReservedStock\" <= \"Stock\"");

            b.HasIndex(p => p.IsActive);
            b.HasIndex(p => p.SKU).IsUnique().HasFilter("\"SKU\" IS NOT NULL AND \"SKU\" <> ''");
            b.HasIndex(p => p.SpeciesId);

            b.HasOne(p => p.Species)
                .WithMany()
                .HasForeignKey(p => p.SpeciesId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_PhysicalProducts_Species");

            b.HasQueryFilter(p => p.DeletedAt == null);
        }
    }

    public class SucursalConfiguration : IEntityTypeConfiguration<Sucursal>
    {
        public void Configure(EntityTypeBuilder<Sucursal> b)
        {
            b.ToTable("Sucursales");
            b.HasKey(s => s.Id);

            b.Property(s => s.Name).HasMaxLength(200).IsRequired();
            b.Property(s => s.Address).HasMaxLength(500);
            b.Property(s => s.Phone).HasMaxLength(50);
            b.Property(s => s.City).HasMaxLength(200);

            b.HasQueryFilter(s => s.DeletedAt == null);
        }
    }

    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> b)
        {
            b.ToTable("Orders");
            b.HasKey(o => o.Id);

            b.Property(o => o.Status).HasMaxLength(20).IsRequired();
            b.Property(o => o.Subtotal).HasPrecision(18, 2);
            b.Property(o => o.Tax).HasPrecision(18, 2);
            b.Property(o => o.Total).HasPrecision(18, 2);
            b.Property(o => o.ShippingAddress).HasMaxLength(500);
            b.Property(o => o.Notes).HasMaxLength(1000);

            b.HasIndex(o => o.UserId);
            b.HasIndex(o => o.Status);
            b.HasIndex(o => o.CreatedAt);

            b.HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> b)
        {
            b.ToTable("OrderDetails");
            b.HasKey(d => d.Id);

            b.Property(d => d.UnitPrice).HasPrecision(18, 2).IsRequired();

            b.HasOne(d => d.Order)
                .WithMany(o => o.Details)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(d => d.Course)
                .WithMany()
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(d => d.PhysicalProduct)
                .WithMany()
                .HasForeignKey(d => d.PhysicalProductId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(d => d.OrderId);
            b.HasIndex(d => d.CourseId);
            b.HasIndex(d => d.PhysicalProductId);
        }
    }

    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> b)
        {
            b.ToTable("Payments");
            b.HasKey(p => p.Id);

            b.Property(p => p.Provider).HasMaxLength(20).IsRequired();
            b.Property(p => p.ExternalId).HasMaxLength(100).IsRequired();
            b.Property(p => p.Amount).HasPrecision(18, 2).IsRequired();
            b.Property(p => p.Currency).HasMaxLength(3).IsRequired();
            b.Property(p => p.Status).HasMaxLength(20).IsRequired();
            b.Property(p => p.GatewayResponse).HasMaxLength(4000);

            b.HasIndex(p => new { p.Provider, p.ExternalId }).IsUnique();

            b.HasIndex(p => p.OrderId).IsUnique();
            b.HasIndex(p => p.Status);
            b.HasIndex(p => p.CreatedAt);

            b.HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
    {
        public void Configure(EntityTypeBuilder<InventoryMovement> b)
        {
            b.ToTable("InventoryMovements");
            b.HasKey(m => m.Id);

            b.Property(m => m.TipoMovimiento).HasMaxLength(30).IsRequired();
            b.Property(m => m.Referencia).HasMaxLength(200).IsRequired();
            b.Property(m => m.UsuarioId).HasMaxLength(450).IsRequired();
            b.Property(m => m.Observacion).HasMaxLength(1000);

            b.HasCheckConstraint("CK_InventoryMovement_Cantidad_Positive", "\"Cantidad\" > 0");

            b.HasIndex(m => m.PhysicalProductId);
            b.HasIndex(m => m.CourseId);
            b.HasIndex(m => m.SucursalId);
            b.HasIndex(m => m.TipoMovimiento);
            b.HasIndex(m => m.FechaMovimiento);
            b.HasIndex(m => new { m.PhysicalProductId, m.FechaMovimiento });
            b.HasIndex(m => new { m.CourseId, m.FechaMovimiento });
            b.HasIndex(m => new { m.SucursalId, m.FechaMovimiento });
            b.HasIndex(m => new { m.UsuarioId, m.FechaMovimiento });

            b.HasOne(m => m.PhysicalProduct)
                .WithMany()
                .HasForeignKey(m => m.PhysicalProductId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(m => m.Course)
                .WithMany()
                .HasForeignKey(m => m.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(m => m.Sucursal)
                .WithMany()
                .HasForeignKey(m => m.SucursalId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
