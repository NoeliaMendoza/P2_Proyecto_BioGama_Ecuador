using Microsoft.EntityFrameworkCore;
using BioGamaEcuador.Models;
using BioGamaEcuador.Models.Sales;

namespace BioGamaEcuador.Data.Seeders
{
    public static class SalesSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Seed Courses
            if (!await context.Courses.AnyAsync())
            {
                var species = await context.Species.Where(s => s.IsActive).Take(5).ToListAsync();
                var colibriId = species.FirstOrDefault(s => s.CommonName.Contains("Colibrí"))?.Id;
                var orquideaId = species.FirstOrDefault(s => s.CommonName.Contains("Orquídea"))?.Id;
                var ranaId = species.FirstOrDefault(s => s.CommonName.Contains("Rana"))?.Id;
                var osoId = species.FirstOrDefault(s => s.CommonName.Contains("Oso"))?.Id;

                var courses = new[]
                {
                    new Course
                    {
                        Title = "Identificación de Aves del Chocó Andino",
                        Description = "Taller práctico de 2 días: claves de identificación, vocalizaciones, uso de eBird y guías de campo. Incluye salida de campo opcional.",
                        Syllabus = @"## Día 1 (Teoría)
- Familias de aves del Chocó
- Claves morfológicas y vocales
- Uso de guías y apps (Merlin, eBird)

## Día 2 (Práctica)
- Salida a Reserva Mashpi/Intillacta
- Registro de observaciones
- Subida a plataformas ciencia ciudadana",
                        Price = 120.00m,
                        TotalSeats = 20,
                        StartDate = DateTime.UtcNow.AddDays(30),
                        EndDate = DateTime.UtcNow.AddDays(31),
                        StartTime = new TimeSpan(8, 0, 0),
                        EndTime = new TimeSpan(16, 0, 0),
                        Modality = "Hibrido",
                        Venue = "Fundación Jocotoco, Quito + Reserva Mashpi",
                        Instructor = "Dr. Juan Freile",
                        InstructorBio = "Ornitólogo, autor de 'Aves del Ecuador', 20 años experiencia",
                        SpeciesId = colibriId,
                        IsActive = true
                    },
                    new Course
                    {
                        Title = "Monitoreo de Anfibios y Reptiles: Técnicas de Campo",
                        Description = "Curso intensivo 3 días: transectos, trampas de caída, identificación por claves, toma de datos estandarizada, bioseguridad.",
                        Syllabus = @"## Día 1
- Anfibios y reptiles del Ecuador
- Diseño de muestreo: transectos, cuadrantes
- Equipo y seguridad

## Día 2
- Práctica campo: Estación Científica Yasuní
- Trampas de caída, búsqueda visual
- Manejo ético y bioseguridad

## Día 3
- Identificación con claves dicotómicas
- Estandarización datos Darwin Core
- Subida a GBIF/iNaturalist",
                        Price = 180.00m,
                        TotalSeats = 15,
                        StartDate = DateTime.UtcNow.AddDays(60),
                        EndDate = DateTime.UtcNow.AddDays(62),
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(17, 0, 0),
                        Modality = "Presencial",
                        Venue = "Estación Científica Yasuní, Orellana",
                        Instructor = "Dra. Andrea Terán",
                        InstructorBio = "Herpetóloga, investigadora INABIO, especialista anfibios andinos",
                        SpeciesId = ranaId,
                        IsActive = true
                    },
                    new Course
                    {
                        Title = "Fotografía de Biodiversidad para Ciencia Ciudadana",
                        Description = "Taller virtual 4 sesiones: macrofotografía, iluminación, metadatos Darwin Core, subida a iNaturalist/GBIF, ética fotográfica.",
                        Syllabus = @"## Sesión 1 (Sábado)
- Equipamiento: cámara, lentes macro, flashes
- Composición y enfoque en macro

## Sesión 2 (Sábado)
- Iluminación: natural, flash, difusores
- Fotografía de comportamiento

## Sesión 3 (Sábado)
- Metadatos: Darwin Core, georreferenciación
- Organización de archivos

## Sesión 4 (Sábado)
- Subida a iNaturalist, GBIF, eBird
- Ética: no perturbar, permisos, especies sensibles",
                        Price = 60.00m,
                        TotalSeats = 30,
                        StartDate = DateTime.UtcNow.AddDays(15),
                        EndDate = DateTime.UtcNow.AddDays(36), // 4 sábados
                        StartTime = new TimeSpan(10, 0, 0),
                        EndTime = new TimeSpan(13, 0, 0),
                        Modality = "Virtual",
                        Venue = "Zoom (enlace enviado tras confirmación)",
                        Instructor = "MSc. Pablo Jarrín",
                        InstructorBio = "Fotógrafo naturaleza, colaborador iNaturalist, 50k+ observaciones",
                        IsActive = true
                    },
                    new Course
                    {
                        Title = "Introducción a la Botánica de Campo: Familias de Orquídeas",
                        Description = "Curso teórico-práctico: morfología floral, claves de identificación, colecta ética, herborización, bases de datos.",
                        Syllabus = @"## Día 1
- Morfología floral Orquidaceae
- Claves de identificación géneros principales
- Colecta ética y permisos

## Día 2
- Práctica Jardín Botánico Quito
- Herborización y prensado
- Registro en bases de datos",
                        Price = 95.00m,
                        TotalSeats = 18,
                        StartDate = DateTime.UtcNow.AddDays(45),
                        EndDate = DateTime.UtcNow.AddDays(46),
                        StartTime = new TimeSpan(8, 30, 0),
                        EndTime = new TimeSpan(16, 30, 0),
                        Modality = "Presencial",
                        Venue = "Jardín Botánico de Quito",
                        Instructor = "Ing. María González",
                        InstructorBio = "Botánica, curadora herbario QCA, especialista Orchidaceae",
                        SpeciesId = orquideaId,
                        IsActive = true
                    }
                };

                context.Courses.AddRange(courses);
                await context.SaveChangesAsync();
            }

            // Seed Physical Products
            if (!await context.PhysicalProducts.AnyAsync())
            {
                var species = await context.Species.Where(s => s.IsActive).Take(3).ToListAsync();
                var colibriId = species.FirstOrDefault(s => s.CommonName.Contains("Colibrí"))?.Id;
                var orquideaId = species.FirstOrDefault(s => s.CommonName.Contains("Orquídea"))?.Id;

                var products = new[]
                {
                    new PhysicalProduct
                    {
                        Name = "Poster A2: Aves endémicas del Chocó",
                        Description = "Ilustración científica 42x59cm, papel mate 200g, 42 especies con nombre común y científico",
                        Price = 18.00m,
                        Stock = 50,
                        ReservedStock = 0,
                        MinStock = 5,
                        SKU = "POS-AVES-CH-001",
                        ImageUrl = "/images/products/poster-aves-choco.jpg",
                        SpeciesId = colibriId,
                        IsActive = true
                    },
                    new PhysicalProduct
                    {
                        Name = "Guía de campo: Orquídeas de los Andes (impresa)",
                        Description = "120 spp, claves dicotómicas, 200 páginas, encuadernación espiral, papel resistente al agua",
                        Price = 35.00m,
                        Stock = 30,
                        ReservedStock = 0,
                        MinStock = 3,
                        SKU = "GUI-ORQ-AN-001",
                        ImageUrl = "/images/products/guia-orquideas.jpg",
                        SpeciesId = orquideaId,
                        IsActive = true
                    },
                    new PhysicalProduct
                    {
                        Name = "Kit investigador junior: Lupa 10x + Cuaderno campo + Guía bolsillo",
                        Description = "Kit educativo para escuelas y clubes de ciencia: lupa 10x, cuaderno 80 pág papel reciclado, guía 50 spp bolsillo",
                        Price = 45.00m,
                        Stock = 20,
                        ReservedStock = 0,
                        MinStock = 2,
                        SKU = "KIT-JUN-001",
                        ImageUrl = "/images/products/kit-junior.jpg",
                        IsActive = true
                    }
                };

                context.PhysicalProducts.AddRange(products);
                await context.SaveChangesAsync();
            }
        }
    }
}