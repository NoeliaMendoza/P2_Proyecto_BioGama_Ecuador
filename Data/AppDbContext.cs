using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BioGamaEcuador.Models;

namespace BioGamaEcuador.Data;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Family> Families { get; set; }
    public DbSet<Species> Species { get; set; }
    public DbSet<Researcher> Researchers { get; set; }
    public DbSet<NaturalReserve> NaturalReserves { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Record> Records { get; set; }
    public DbSet<ConservationStatus> ConservationStatuses { get; set; }
    public DbSet<Publication> Publications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relación muchos a muchos entre Publication y Species,
        // materializada en la tabla intermedia PublicationSpecies.
        modelBuilder.Entity<Publication>()
            .HasMany(p => p.RelatedSpecies)
            .WithMany(s => s.Publications)
            .UsingEntity(j => j.ToTable("PublicationSpecies"));

        // Relación opcional Species -> ConservationStatus. Es opcional para
        // no romper los 500.000 registros ya cargados en producción al migrar;
        // los registros sin clasificar quedan en NULL hasta reclasificarse.
        modelBuilder.Entity<Species>()
            .HasOne(s => s.ConservationStatus)
            .WithMany(c => c.SpeciesList)
            .HasForeignKey(s => s.ConservationStatusId)
            .OnDelete(DeleteBehavior.SetNull);

        // Catálogo fijo de estados de conservación según la Lista Roja de la
        // UICN (Unión Internacional para la Conservación de la Naturaleza),
        // el organismo internacional que define estas categorías de riesgo
        // de extinción para especies silvestres. Se siembra desde el propio
        // DbContext porque son datos de referencia, no datos de prueba ni
        // de negocio. Significado de cada código:
        //   EX - Extinta: no quedan individuos vivos conocidos.
        //   CR - En peligro crítico: riesgo extremadamente alto de extinción.
        //   EN - En peligro: riesgo muy alto de extinción en estado silvestre.
        //   VU - Vulnerable: riesgo alto de extinción en estado silvestre.
        //   NT - Casi amenazada: podría calificar como amenazada pronto.
        //   LC - Preocupación menor: no calificada como amenazada por ahora.
        //   DD - Datos insuficientes: no hay información suficiente para evaluar el riesgo.
        modelBuilder.Entity<ConservationStatus>().HasData(
            new ConservationStatus { Id = 1, Code = "EX", Name = "Extinta", Description = "No quedan individuos vivos conocidos.", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConservationStatus { Id = 2, Code = "CR", Name = "En peligro crítico", Description = "Riesgo extremadamente alto de extinción en estado silvestre.", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConservationStatus { Id = 3, Code = "EN", Name = "En peligro", Description = "Riesgo muy alto de extinción en estado silvestre.", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConservationStatus { Id = 4, Code = "VU", Name = "Vulnerable", Description = "Riesgo alto de extinción en estado silvestre.", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConservationStatus { Id = 5, Code = "NT", Name = "Casi amenazada", Description = "Podría calificar como amenazada en un futuro cercano.", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConservationStatus { Id = 6, Code = "LC", Name = "Preocupación menor", Description = "No calificada como amenazada actualmente.", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConservationStatus { Id = 7, Code = "DD", Name = "Datos insuficientes", Description = "No hay información suficiente para evaluar el riesgo.", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}