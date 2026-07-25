using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BioGamaEcuador.Data;
using BioGamaEcuador.Models;
using BioGamaEcuador.Models.Sales;

namespace BioGamaEcuador.Data.Seeders
{
    public static class SalesModuleSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Verificar si ya existen cursos
            if (await context.Courses.AnyAsync())
            {
                return; // Ya sembrado
            }

            // Obtener algunas especies existentes para vincular
            var especies = await context.Species
                .Where(s => s.IsActive)
                .Take(10)
                .ToListAsync();

            var colibriId = especies.FirstOrDefault(s => s.CommonName.Contains("Colibrí"))?.Id;
            var orquideaId = especies.FirstOrDefault(s => s.CommonName.Contains("Orquídea"))?.Id;
            var ranaId = especies.FirstOrDefault(s => s.CommonName.Contains("Rana") || s.CommonName.Contains("rana"))?.Id;
            var osoId = especies.FirstOrDefault(s => s.CommonName.Contains("Oso"))?.Id;

            var cursos = new List<Course>
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
                    StartDate = DateTime.UtcNow.AddDays(60),
                    EndDate = DateTime.UtcNow.AddDays(61),
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(16, 0, 0),
                    Modality = "Hibrido",
                    Venue = "Fundación Jocotoco, Quito + Reserva Mashpi",
                    Instructor = "Dr. Juan Freile",
                    InstructorBio = "Ornitólogo, autor de 'Aves del Ecuador', 20 años experiencia",
                    ImageUrl = "/images/cursos/aves-choco.jpg",
                    SpeciesId = colibriId,
                    IsActive = true,
                    RequiresPriorKnowledge = false,
                    TargetAudience = "Estudiantes, guías, público general",
                    CreatedAt = DateTime.UtcNow
                },
                new Course
                {
                    Title = "Monitoreo de Anfibios y Reptiles: Técnicas de Campo",
                    Description = "Curso intensivo 3 días: transectos, trampas de caída, identificación por claves, toma de datos estandarizada, bioseguridad.",
                    Syllabus = @"## Día 1
- Introducción a herpetofauna ecuatoriana
- Técnicas de muestreo: transectos visuales, auditivos
- Trampas de caída y embudos

## Día 2
- Identificación por claves dicotómicas
- Manipulación segura y bioseguridad
- Práctica en estación científica

## Día 3
- Análisis de datos y reporteo
- Subida a plataformas (iNaturalist, GBIF)
- Evaluación final",
                    Price = 180.00m,
                    TotalSeats = 15,
                    StartDate = DateTime.UtcNow.AddDays(90),
                    EndDate = DateTime.UtcNow.AddDays(92),
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0),
                    Modality = "Presencial",
                    Venue = "Estación Científica Yasuní, Orellana",
                    Instructor = "Dra. Andrea Terán",
                    InstructorBio = "Herpetóloga, investigadora INABIO, especialista anfibios andinos",
                    ImageUrl = "/images/cursos/herpetofauna.jpg",
                    SpeciesId = ranaId,
                    IsActive = true,
                    RequiresPriorKnowledge = true,
                    TargetAudience = "Biólogos, estudiantes avanzados, guardaparques",
                    CreatedAt = DateTime.UtcNow
                },
                new Course
                {
                    Title = "Fotografía de Biodiversidad para Ciencia Ciudadana",
                    Description = "Taller virtual 4 sesiones: macrofotografía, iluminación, metadatos Darwin Core, subida a iNaturalist/GBIF, ética fotográfica.",
                    Syllabus = @"## Sesión 1: Equipo y fundamentos
- Cámaras, lentes macro, iluminación
- Configuración para metadatos

## Sesión 2: Trabajo de campo
- Técnicas macro y aproximación
- Ética: distancia, manipulación, flash

## Sesión 3: Post-procesado y metadatos
- Lightroom/Capture One para ciencia
- Darwin Core, coordenadas, taxonomía

## Sesión 4: Publicación y comunidad
- iNaturalist, GBIF, GBIF-Ecuador
- Validación comunitaria, licencias CC",
                    Price = 60.00m,
                    TotalSeats = 30,
                    StartDate = DateTime.UtcNow.AddDays(30),
                    EndDate = DateTime.UtcNow.AddDays(51), // 4 sábados
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(13, 0, 0),
                    Modality = "Virtual",
                    Venue = "Zoom (enlace enviado tras confirmación)",
                    Instructor = "MSc. Pablo Jarrín",
                    InstructorBio = "Fotógrafo naturaleza, colaborador iNaturalist, 50k+ observaciones",
                    ImageUrl = "/images/cursos/foto-biodiversidad.jpg",
                    IsActive = true,
                    RequiresPriorKnowledge = false,
                    TargetAudience = "Fotógrafos aficionados, naturalistas, estudiantes",
                    CreatedAt = DateTime.UtcNow
                },
                new Course
                {
                    Title = "Introducción a la Botánica de Campo: Familias de Orquídeas",
                    Description = "Curso teórico-práctico: morfología floral, claves de identificación, colecta ética, herborización, bases de datos.",
                    Syllabus = @"## Día 1: Teoría y taller
- Morfología floral Orchidaceae
- Claves de identificación principales géneros
- Ética colecta y permisos

## Día 2: Práctica en jardín botánico
- Identificación in situ
- Técnicas herborizado
- Registro en bases de datos",
                    Price = 95.00m,
                    TotalSeats = 18,
                    StartDate = DateTime.UtcNow.AddDays(120),
                    EndDate = DateTime.UtcNow.AddDays(121),
                    StartTime = new TimeSpan(8, 30, 0),
                    EndTime = new TimeSpan(16, 30, 0),
                    Modality = "Presencial",
                    Venue = "Jardín Botánico de Quito",
                    Instructor = "Ing. María González",
                    InstructorBio = "Botánica, curadora herbario QCA, especialista Orchidaceae",
                    ImageUrl = "/images/cursos/orquideas.jpg",
                    SpeciesId = orquideaId,
                    IsActive = true,
                    RequiresPriorKnowledge = false,
                    TargetAudience = "Estudiantes biología, guías, viveristas",
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.Courses.AddRange(cursos);

            // Productos físicos (posters, guías impresas, kits)
            var productos = new List<PhysicalProduct>
            {
                new PhysicalProduct
                {
                    Name = "Poster A2: Aves Endémicas del Chocó",
                    Description = "Ilustración científica 42x59cm, papel mate 200g, 42 especies con nombres común y científico",
                    Price = 18.00m,
                    Stock = 50,
                    MinStock = 5,
                    SKU = "POS-AVES-CH-001",
                    ImageUrl = "/images/productos/poster-aves-choco.jpg",
                    SpeciesId = colibriId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new PhysicalProduct
                {
                    Name = "Guía de Campo: Orquídeas de los Andes (Impresa)",
                    Description = "120 spp, claves dicotómicas, 200 páginas, encuadernación espiral, resistente al agua",
                    Price = 35.00m,
                    Stock = 30,
                    MinStock = 3,
                    SKU = "GUI-ORQ-AN-001",
                    ImageUrl = "/images/productos/guia-orquideas.jpg",
                    SpeciesId = orquideaId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new PhysicalProduct
                {
                    Name = "Kit Investigador Junior: Lupa 10x + Cuaderno Campo + Guía Bolsillo",
                    Description = "Kit educativo para escuelas y clubes de ciencia: lupa acrílica 10x, cuaderno 100p resistente agua, guía 50 spp bolsillo",
                    Price = 45.00m,
                    Stock = 20,
                    MinStock = 2,
                    SKU = "KIT-JUN-001",
                    ImageUrl = "/images/productos/kit-junior.jpg",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.PhysicalProducts.AddRange(productos);

            await context.SaveChangesAsync();
        }
    }
}