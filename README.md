# BioGama Ecuador

Plataforma web para el registro centralizado de biodiversidad del Ecuador, desarrollada con ASP.NET Core MVC, Entity Framework Core y PostgreSQL.

## Descripción

BioGama Ecuador permite registrar especies de flora, fauna y hongos presentes en el territorio nacional, vinculando cada hallazgo con una ubicación geográfica dentro de una reserva natural y con el investigador responsable del registro.

## Módulos del sistema

- **Families** – Clasificación taxonómica superior de los organismos
- **Species** – Catálogo maestro de especies registradas
- **Researchers** – Personal científico que realiza trabajo de campo
- **NaturalReserves** – Áreas protegidas del Sistema Nacional de Áreas Protegidas del Ecuador
- **Locations** – Puntos geográficos específicos dentro de cada reserva
- **Records** – Bitácora histórica de hallazgos de campo

## Tecnologías utilizadas

- ASP.NET Core MVC (.NET 10)
- Entity Framework Core
- PostgreSQL
- Npgsql.EntityFrameworkCore.PostgreSQL

## Requisitos previos

- .NET 10 SDK instalado
- PostgreSQL instalado y en ejecución
- Visual Studio Code o cualquier editor compatible

## Configuración y ejecución

1. Clonar el repositorio
   ```bash
   git clone https://github.com/tu_usuario/BioGamaEcuador.git
   cd BioGamaEcuador
   ```

2. Copiar el archivo de configuración de ejemplo
   ```bash
   cp appsettings.example.json appsettings.json
   ```

3. Editar `appsettings.json` con las credenciales reales de PostgreSQL

4. Crear la base de datos y el usuario en PostgreSQL
   ```sql
   CREATE DATABASE biogama_ecuador;
   CREATE USER biogama_user WITH PASSWORD 'tu_clave';
   GRANT ALL PRIVILEGES ON DATABASE biogama_ecuador TO biogama_user;
   GRANT ALL ON SCHEMA public TO biogama_user;
   ```

5. Aplicar las migraciones
   ```bash
   dotnet ef database update
   ```

6. Ejecutar la aplicación
   ```bash
   dotnet run
   ```

7. Abrir en el navegador
   ```
   https://localhost:5001
   ```

## Estructura del proyecto

```
BioGamaEcuador/
├── Controllers/        # Controladores MVC
├── Data/               # DbContext y configuración de datos
├── Migrations/         # Migraciones de Entity Framework Core
├── Models/             # Entidades del negocio
├── Services/           # Lógica de apoyo
├── Views/              # Interfaces Razor
├── appsettings.example.json
├── Program.cs
└── README.md
```

## Diagrama de base de datos

![Diagrama ER](docs/diagrama-er.png)
