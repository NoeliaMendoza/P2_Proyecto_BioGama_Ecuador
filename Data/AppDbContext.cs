using Microsoft.EntityFrameworkCore;
using BioGamaEcuador.Models;

namespace BioGamaEcuador.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Family> Families { get; set; }
        public DbSet<Species> Species { get; set; }
        public DbSet<Researcher> Researchers { get; set; }
        public DbSet<NaturalReserve> NaturalReserves { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Record> Records { get; set; }
    }
}