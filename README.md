# BioGama Ecuador

Plataforma web para el registro centralizado de biodiversidad del Ecuador, con módulo de ventas, pagos integrados y clasificación con IA local.

## Tecnologías

- **ASP.NET Core MVC (.NET 10)** — Framework principal
- **Entity Framework Core + Npgsql** — ORM con PostgreSQL 16
- **PostgreSQL 16 Alpine** — Base de datos relacional
- **Docker Swarm** — Despliegue con 2 réplicas web + servicios independientes
- **Ollama (gemma2:2b)** — Clasificación de especies con IA local
- **PayPal / PayPhone** — Pasarelas de pago en entorno Sandbox
- **MailKit + MimeKit** — Envío de correos transaccionales vía Gmail SMTP
- **Google Authenticator** — Autenticación multifactor (TOTP)

## Módulos

### CRUD de Biodiversidad
- **Families** — Clasificación taxonómica superior
- **Species** — Catálogo maestro de especies (~1,000,000 registros)
- **Researchers** — Investigadores de campo
- **NaturalReserves** — Áreas protegidas del Ecuador
- **Locations** — Puntos geográficos dentro de reservas
- **Records** — Bitácora de hallazgos de campo

### Ventas y Pagos
- Catálogo de productos físicos con control de stock
- Cursos y capacitaciones con gestión de cupos
- Carrito de compras con reserva temporal de stock
- Checkout con selección de pasarela (PayPal o PayPhone)
- Webhook de PayPhone para confirmación asincrónica
- Confirmación de orden con actualización de inventario
- Correo electrónico de confirmación al comprador

### Infraestructura
- Despliegue en Docker Swarm con 2 réplicas web
- Worker de correo (MailWorker) en contenedor separado
- Servicio Ollama para clasificación con IA local
- Sesiones distribuidas con Data Protection Keys compartidas
- Health checks para orquestación
- Secretos externos (db_password, email_password, aspnetcore_keys)

### Seguridad
- Autenticación con ASP.NET Core Identity
- Roles: Administrador, Investigador, UserPublico
- Autenticación multifactor con Google Authenticator
- Inicio de sesión con Google OAuth2
- Validación de teléfono ecuatoriano (regex)
- Eliminación lógica (soft delete) en todas las tablas
- Auditoría con ILogger (operaciones CRUD, login, MFA, roles)

## Requisitos previos

- .NET 10 SDK
- Docker Desktop (para despliegue Swarm)
- PostgreSQL 16 (local o en contenedor)
- Ollama (opcional, para clasificación con IA)

## Configuración y ejecución local

```bash
# 1. Clonar
git clone https://github.com/NoeliaMendoza/P2_Proyecto_BioGama_Ecuador.git
cd P2_Proyecto_BioGama_Ecuador

# 2. Configurar credenciales
cp appsettings.example.json appsettings.json
# Editar appsettings.json con conexión a PostgreSQL

# 3. Crear base de datos
psql -U postgres -c "CREATE DATABASE biogama_ecuador;"
psql -U postgres -c "CREATE USER biogama_user WITH PASSWORD 'tu_clave';"
psql -U postgres -c "GRANT ALL PRIVILEGES ON DATABASE biogama_ecuador TO biogama_user;"

# 4. Restaurar backup (opcional)
# tar -xzf database/backup.sql.tar.gz -C database/
# psql -U biogama_user -d biogama_ecuador < database/backup.sql

# 5. Aplicar migraciones y seeders
dotnet ef database update
# Los seeders se ejecutan automáticamente al iniciar

# 6. Ejecutar
dotnet run
```

## Despliegue con Docker Swarm

```bash
# 1. Construir imagen
docker build -t biogama-web:latest .

# 2. Crear secretos (una sola vez)
echo "tu_password_db" | docker secret create db_password -
echo "tu_password_email" | docker secret create email_password -
echo "clave_proteccion_datos" | docker secret create aspnetcore_keys -

# 3. Desplegar stack
docker stack deploy -c docker-stack.yml biogama
```

### Servicios del stack

| Servicio | Réplicas | Puerto | Descripción |
|---|---|---|---|
| web | 2 | 8080 | Aplicación web ASP.NET Core |
| db | 1 | — | PostgreSQL 16 Alpine |
| ollama | 1 | 11434 | IA local con gemma2:2b |
| mail | 1 | — | MailWorker para correos en cola |

## Usuarios de prueba

| Correo | Contraseña | Rol |
|---|---|---|
| admin@biogama.ec | Admin123* | Administrador |
| investigador@biogama.ec | Invest123* | Investigador |
| usuario@biogama.ec | Usuario123* | UserPublico |

## Servicios implementados

Todos registrados via DI en `Program.cs`:

- `IInventoryService` — Movimientos de inventario (reserva, confirmación, liberación, ajuste, entrada, salida, transferencia, devolución)
- `IPaymentGateway` — Abstracción común para PayPal y PayPhone
- `IPaymentService` — Fachada de pagos con resolución dinámica de gateway
- `IEmailService` — 9 tipos de correos transaccionales (confirmación, recuperación, MFA, pedidos, inscripciones, alertas de stock)
- `IAuditService` — Auditoría con ILogger (operaciones, login, MFA, roles)
- `IAccountService` — Gestión de cuentas de usuario
- `IAIService` — Clasificación de especies con Ollama/gemma2:2b

## Estructura del proyecto

```
BioGamaEcuador/
├── Controllers/          # Controladores MVC
│   ├── OrdersController  # Carrito, checkout, PayPal SDK
│   └── PaymentController # Webhook PayPhone, pago manual
├── Data/
│   ├── AppDbContext.cs    # DbContext
│   └── Seeders/           # IdentitySeeder, SalesModuleSeeder, SalesSeeder
├── Migrations/            # Migraciones EF Core
├── Models/
│   ├── Sales/             # Order, Payment, PhysicalProduct, Enrollment, etc.
│   ├── Audit/             # AuditLog
│   └── PendingEmail.cs    # Cola de correos
├── Services/
│   ├── Payments/          # PayPalService, PayPhoneApiLinkService, Gateways
│   ├── Ollama/            # IOllamaService, OllamaService
│   ├── EmailService.cs    # MailKit + MimeKit
│   ├── AuditService.cs    # ILogger-based
│   └── ...                # IAIService, IAccountService, etc.
├── Settings/              # PayPalSettings, PayPhoneSettings, EmailSettings, OllamaSettings
├── Workers/
│   └── MailWorker.cs      # BackgroundService de correos
├── Views/                 # Razor Views
├── database/
│   ├── backup.sql.tar.gz  # Backup comprimido (~25 MB)
│   ├── seed-extra.sql     # 50 especies adicionales
│   └── seed-extra-50.sql  # 50 especies más (local)
├── docker-stack.yml       # Stack de Docker Swarm
├── Dockerfile             # Imagen .NET 10
├── Program.cs             # Punto de entrada
└── appsettings.example.json  # Plantilla de configuración
```

## Licencia

Proyecto académico — P2 Proyecto BioGama Ecuador
