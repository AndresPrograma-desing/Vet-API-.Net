# CLAUDE.md — Guía de Arquitectura y Reglas del Proyecto `vet-api-Net`

> **Propósito:** Este documento es la referencia definitiva para cualquier agente de IA (Claude Code u otro) o desarrollador que modifique este proyecto.
> Antes de crear, modificar o eliminar cualquier archivo, DEBES leer y seguir estas reglas estrictamente.

**Índice:** [1. Visión General](#1-visión-general-del-proyecto) · [2. Estructura de Carpetas](#2-estructura-de-carpetas) · [3. Patrón Repository + Service](#3-patrón-arquitectónico-repository--service) · [4. Convenciones de Código](#4-convenciones-de-código) · [5. Constantes](#5-sistema-de-constantes) · [6. DI](#6-inyección-de-dependencias-extensionsdependencyinjectioncs) · [7. Workers](#7-workers-background-services) · [8. SignalR](#8-signalr-hubs) · [9. Documentos](#9-generación-de-documentos) · [10. appsettings](#10-configuración-de-la-aplicación-appsettingsjson) · [11. Seeding](#11-data-seeding-dataseedsseedsdatacs) · [12. Modelo de Datos](#12-modelo-de-datos-entidades-principales) · [13. Manejo Global de Errores](#13-manejo-global-de-errores) · [14. Reglas Críticas](#14-reglas-críticas-para-cambios) · [15. Comandos](#15-comandos-frecuentes) · [16. Puertos](#16-puertos-y-urls) · [17. Dependencias Externas](#17-resumen-de-dependencias-externas) · [18. Comentarios](#18-comentarios) · [19. Planificación](#19-planificación-de-cambios)

---

## 1. Visión General del Proyecto

**Nombre:** Happy Pets API (vet-api-Net)
**Framework:** ASP.NET Core (.NET 10, SDK `10.0.103` — ver `global.json`) — Minimal hosting (`Program.cs`)
**Base de datos:** PostgreSQL (Supabase) con soporte dual MySQL/PostgreSQL (configurado por `DatabaseSettings:Provider` en `appsettings.json`)
**ORM:** Entity Framework Core (Npgsql + Pomelo para MySQL)
**Autenticación:** JWT Bearer
**Tiempo Real:** SignalR (Hubs)
**Documentos:** QuestPDF (facturas PDF), ClosedXML (Excel), DocumentFormat.OpenXml (DOCX)
**Idioma del código:** Los nombres de modelos, propiedades, tablas y DTOs están en **español**. Los nombres de namespaces, clases de infraestructura y patrones de diseño están en **inglés**.

---

## 2. Estructura de Carpetas

```
vet-api-Net/
├── ApiSettings/            # Clases de opciones (Options Pattern) para appsettings.json
├── Configuration/          # Clases de configuración para Workers
├── Constants/              # Mensajes de respuesta, rutas de endpoints y variables globales
│   ├── EndpointRoutes.cs   # Todas las rutas de los endpoints centralizadas
│   ├── ResponseMessages.cs # Todos los mensajes de respuesta centralizados
│   └── Variables.cs        # Constantes globales simples
├── Controller/             # Controladores de la API (capa HTTP)
├── Data/                   # DbContext y seeds
│   ├── AppDbContext.cs     # Configuración de EF Core y Fluent API
│   └── seeds/SeedsData.cs  # Datos iniciales de la base de datos
├── DTOs/                   # Data Transfer Objects (entrada y salida)
├── Exceptions/             # Excepciones de dominio personalizadas (ej. LoginSecurityException)
├── Extensions/             # Métodos de extensión (DI, mapeos)
│   ├── DependencyInjection.cs   # TODA la configuración de servicios y DI
│   └── CitaMappingExtensions.cs # Extensiones de mapeo Model → DTO
├── HttpClient/             # Clientes HTTP internos (WebSocket message)
├── HttpService/            # Servicios HTTP externos (BCV scraper, Resend email)
├── Hubs/                   # SignalR Hubs (mensajes y notificaciones push)
├── Interface/              # Contratos/Interfaces
│   ├── Repository/         # Interfaces de repositorios
│   ├── Services/           # Interfaces de servicios de negocio
│   └── Utilities/          # Interfaces de utilidades y soporte transversal
├── Middleware/             # Middlewares del pipeline HTTP (ej. GlobalExceptionMiddleware)
├── Migrations/             # Migraciones de EF Core (auto-generadas)
├── Models/                 # Entidades del dominio (mapeo directo a tablas)
├── Repository/             # Implementaciones de repositorios (acceso a datos)
├── Security/               # Utilidades de seguridad transversales (ej. bloqueo de login)
├── Services/               # Implementaciones de servicios (lógica de negocio pura)
├── Templates/              # Plantillas HTML para emails
├── Utilities/              # Implementaciones de utilidades y soporte técnico transversal
├── Worker/                 # Background Services (tareas programadas)
├── Program.cs              # Entry point (configuración del pipeline HTTP)
├── appsettings.json        # Configuración principal
└── wwwroot/                # Archivos estáticos (facturas PDF generadas)
```

> **Subcarpetas por dominio:** `Controller/`, `Interface/Repository/`, `Interface/Services/`, `Repository/` y `Services/` pueden agrupar archivos en subcarpetas por dominio cuando la lógica de un módulo crece (ej. `Clients/` para clientes/mascotas). Ver la nota de namespaces en [4.1](#41-namespaces) — las interfaces SÍ anidan el namespace con el nombre del dominio, las implementaciones NO.

---

## 3. Patrón Arquitectónico: Repository + Service

Este proyecto implementa estrictamente un patrón de **3 capas**:

```
Controller (HTTP) → Service (Lógica) → Repository (Datos)
```

### Reglas de cada capa:

| Capa | Responsabilidad | Lo que NO debe hacer |
|---|---|---|
| **Controller** | Recibir HTTP requests, validar entrada básica, delegar al Service, devolver HTTP responses | NO debe tener lógica de negocio NI acceso directo a `AppDbContext` |
| **Service** | Contener TODA la lógica de negocio, orquestación, validaciones complejas | NO debe devolver `IActionResult`. NO debe construir queries LINQ sobre `AppDbContext` (eso es del Repository). Lanza excepciones para indicar errores |
| **Repository** | Acceso directo a `AppDbContext`, TODAS las queries LINQ/EF Core (`Where`, `OrderBy`, `Skip`/`Take`, `Include`) | NO debe tener lógica de negocio |

> **Error común a evitar:** traer todas las filas con un `GetAllAsync()` y luego filtrar/paginar en el Service con `.Where()`/`.Skip()`/`.Take()` sobre una `List<T>` ya materializada. Esos operadores van en el Repository sobre `IQueryable<T>` para que se traduzcan a SQL. Ver [4.9 Paginación](#49-paginación).

### Flujo de creación de una nueva funcionalidad:

```
1. Model        → Models/{Entidad}.cs
2. DTO           → DTOs/{NombreDTO}.cs
3. Interface Rep → Interface/Repository/I{Entidad}Repository.cs
4. Repository    → Repository/{Entidad}Repository.cs
5. Interface Srv → Interface/Services/I{Entidad}Service.cs
6. Service       → Services/{Entidad}Service.cs
7. Controller    → Controller/{Entidad}Controller.cs
8. Constantes    → Constants/EndpointRoutes.cs + Constants/ResponseMessages.cs
9. DI            → Extensions/DependencyInjection.cs (registrar interfaces ↔ implementaciones)
```

---

## 4. Convenciones de Código

### 4.1 Namespaces

| Carpeta | Namespace |
|---|---|
| Models | `vet_api_Net.Models` |
| DTOs | `DTOs` (global, sin prefijo de proyecto) |
| Controllers | `vet_api_Net.Controllers` o `vet_api_Net.Controller` |
| Services | `vet_api_Net.Services` |
| Repositories | `vet_api_Net.Repositories` |
| Utilities | `vet_api_Net.Utilities` |
| Interfaces de Repositories | `vet_api_Net.Interfaze.Repositories` |
| Interfaces de Services | `vet_api_Net.Interfaze.Services` |
| Interfaces de Utilities | `vet_api_Net.Interfaze.Utilities` |
| Constants | `vet_api_Net.Constants` |
| Endpoint Routes | `vet_api_Net.Routes` |
| Extensions | `vet_api_Net.Extensions` o `Microsoft.Extensions.DependencyInjection` (para DI) |
| Configuration (ApiSettings) | `vet_api_Net.Infrastructure.Configuration` |
| Workers | `vet_api_Net.Workers` o `vet_api_Net.Worker` |
| HttpServices | `vet_api_Net.HttpServices` |
| HttpClient (WSMessage) | `vet_api_Net.Services.WSMessage` |
| Hubs | `vet_api_Net.Hubs` |

> **NOTA:** El namespace de interfaces usa `Interfaze` (con 'z'), NO `Interface`. Respetar este patrón existente.

> **NOTA — dominios agrupados en subcarpetas (ej. `Clients/`):** las interfaces anidan el dominio en el namespace (`vet_api_Net.Interfaze.Repositories.Clients`, `vet_api_Net.Interfaze.Services.Clients`), pero las implementaciones bajo `Repository/{Dominio}/`, `Services/{Dominio}/` y `Controller/{Dominio}/` **mantienen el namespace raíz sin el sufijo del dominio** (`vet_api_Net.Repositories`, `vet_api_Net.Services`, `vet_api_Net.Controllers`). Es una asimetría real del código existente — respétala al añadir archivos nuevos dentro de una subcarpeta de dominio.

### 4.2 Controllers

```csharp
using Microsoft.AspNetCore.Mvc;
using vet_api_Net.Constants;
using vet_api_Net.Routes;
using vet_api_Net.Interfaze.Services;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
public class {Nombre}Controller : ControllerBase
{
    private readonly I{Nombre}Service _{nombreService};

    public {Nombre}Controller(I{Nombre}Service {nombreService})
    {
        _{nombreService} = {nombreService};
    }

    [HttpGet(Endpoints.{NombreEndpoint}.{Accion})]
    public async Task<ActionResult<{ResponseDTO}>> {MetodoAccion}(...)
    {
        try
        {
            var result = await _{nombreService}.{MetodoAsync}(...);
            if (result == null) return NotFound(new { message = ResponseMessages{Dominio}.{NotFound} });
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
```

**Reglas de Controllers:**
- Siempre heredar de `ControllerBase`
- Decorar con `[ApiController]` y `[Route("api/[controller]")]`
- Usar rutas desde `Endpoints.{Clase}.{Propiedad}` — NUNCA strings hardcodeados
- Usar mensajes desde `ResponseMessages{Dominio}.{Mensaje}` — NUNCA strings hardcodeados
- Estructura de error: `new { message = "..." }` para NotFound, `new { error = "..." }` para errores
- El patrón de try/catch debe capturar excepciones específicas antes del `Exception` genérico. `GlobalExceptionMiddleware` (ver [13](#13-manejo-global-de-errores)) es solo la red de seguridad, no reemplaza este try/catch
- Inyectar servicios por constructor (NUNCA repositorios directamente)

### 4.3 Services

```csharp
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;

namespace vet_api_Net.Services;

public class {Nombre}Service : I{Nombre}Service
{
    private readonly I{Nombre}Repository _repository;

    public {Nombre}Service(I{Nombre}Repository repository)
    {
        _repository = repository;
    }

    public async Task<{DTO}?> {MetodoAsync}(...)
    {
        // Lógica de negocio aquí
        // Lanzar excepciones para errores:
        //   - ArgumentNullException para parámetros nulos
        //   - KeyNotFoundException para entidades no encontradas
        //   - InvalidOperationException para violaciones de reglas de negocio
    }
}
```

**Reglas de Services:**
- Implementar la interfaz correspondiente en `Interface/Services/`
- Inyectar SOLO repositorios (interfaces), otros services u `IOptions<T>` por constructor
- NUNCA inyectar `AppDbContext` directamente (excepción: servicios simples sin repositorio dedicado como `PetsService`, `ReportSystemService`)
- NUNCA construir queries LINQ sobre listas materializadas para simular filtrado/paginación de base de datos — eso se pide al Repository (ver [4.9](#49-paginación))
- Para comunicar errores al Controller, lanzar excepciones tipadas (NO devolver null genérico)
- Usar constantes de `ResponseMessages.cs` en los mensajes de las excepciones

### 4.4 Repositories

```csharp
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.Interfaze.Repositories;

namespace vet_api_Net.Repositories;

public class {Nombre}Repository : I{Nombre}Repository
{
    private readonly AppDbContext _context;

    public {Nombre}Repository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<{Entidad}?> GetByIdAsync(int id)
        => await _context.{DbSet}.FindAsync(id);

    public async Task<bool> SaveChangesAsync()
        => await _context.SaveChangesAsync() > 0;
}
```

**Reglas de Repositories:**
- El repositorio es el ÚNICO lugar que accede a `AppDbContext`
- Métodos simples usar expresión de una línea (`=>`)
- Queries complejos con `.Include()` y `.ThenInclude()` van aquí
- Todo `Where`/`OrderBy`/`Skip`/`Take` de filtrado y paginación se construye aquí sobre `IQueryable`, nunca en el Service sobre una lista ya cargada en memoria
- La proyección a DTOs (`.Select(x => new DTO {...})`) puede hacerse aquí para optimizar
- `SaveChangesAsync` devuelve `Task<bool>`

### 4.5 Interfaces

```csharp
// Interface/Repository/I{Nombre}Repository.cs
namespace vet_api_Net.Interfaze.Repositories;

public interface I{Nombre}Repository
{
    Task<{Entidad}?> GetByIdAsync(int id);
    Task<bool> SaveChangesAsync();
}

// Interface/Services/I{Nombre}Service.cs
namespace vet_api_Net.Interfaze.Services;

public interface I{Nombre}Service
{
    Task<{DTO}?> {MetodoAsync}(...);
}
```

### 4.6 Models (Entidades)

```csharp
namespace vet_api_Net.Models;

public partial class {NombreEntidad}
{
    public int Id { get; set; }
    // Propiedades escalares...
    public DateTime Creado { get; set; }
    public DateTime Actualizado { get; set; }

    // Propiedades de navegación (virtual)
    public virtual {EntidadRelacionada} {Nombre} { get; set; } = null!;
    public virtual ICollection<{EntidadHija}> {NombrePlural} { get; set; } = new List<{EntidadHija}>();
}
```

**Reglas de Models:**
- Clase `partial` (para compatibilidad con scaffolding)
- Propiedades de navegación marcadas como `virtual`
- Colecciones inicializadas con `new List<T>()`
- Referencias requeridas inicializadas con `= null!`
- Nombres de propiedades en **español** (ej: `FechaConsulta`, `PesoActual`, `Creado`)

### 4.7 DTOs

```csharp
using System.Text.Json.Serialization;

namespace DTOs;

public record {Nombre}DTO
{
    [JsonPropertyName("nombre_propiedad")]
    public string NombrePropiedad { get; set; } = string.Empty;
}
```

**Reglas de DTOs:**
- Namespace: `DTOs` (sin prefijo)
- Preferir `record` sobre `class` para DTOs nuevos
- Usar `[JsonPropertyName("snake_case")]` para serialización al frontend
- Separar DTOs de entrada (`Create{X}DTO`) de DTOs de respuesta (`{X}RequestDTO`, `{X}ResponseDTO`)
- Valores por defecto: `string.Empty` para strings, `null!` para requeridos, `new()` para listas
- **Idioma de campos nuevos (regla vigente desde 2026-08-29):** todo campo de DTO **nuevo** (propiedad C# y su `JsonPropertyName`) se nombra en **inglés**, manteniendo `snake_case` en el JSON (ej. `MoneyType` → `"money_type"`, no `Moneda`/`"moneda"`). Los campos ya existentes en DTOs actuales (`total_ganancias`, `fecha_cita`, `cliente_nombre`, etc.) **no se renombran retroactivamente** — se mantienen en español para no romper el contrato ya consumido por el frontend. Esta regla reemplaza, solo para campos nuevos, la mención de DTOs en español de la nota de idioma general de este documento.

### 4.8 Utilities

Los servicios que implementen lógica técnica, matemática o soporte transversal que no pertenezca directamente al dominio de negocio (ej. conversiones de monedas, tracking de fallos técnicos de infraestructura, helpers genéricos) deben ubicarse en la carpeta `Utilities/` y sus interfaces bajo `Interface/Utilities/`.

```csharp
using vet_api_Net.Interfaze.Utilities;

namespace vet_api_Net.Utilities;

public class {Nombre}Service : I{Nombre}Service
{
    // Lógica transversal/técnica
}
```

**Reglas de Utilities:**
- El namespace de implementaciones debe ser `vet_api_Net.Utilities`.
- El namespace de interfaces debe ser `vet_api_Net.Interfaze.Utilities`.
- Deben registrarse en la sección dedicada a `//Utilities` en `DependencyInjection.cs`.
- No deben acoplarse directamente a la lógica del flujo de negocio veterinario, sino actuar como soporte reutilizable.

### 4.9 Paginación

Patrón estándar para listados paginados (referencia real en el código: `ClientRepository`/`ClientService` y `CitasRepository`/`CitasRequestService`).

```csharp
// Interface/Repository/I{Entidad}Repository.cs
Task<(List<{Entidad}> Items, int TotalCount)> GetPagedAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null);

// Repository/{Entidad}Repository.cs
public async Task<(List<{Entidad}> Items, int TotalCount)> GetPagedAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
{
    var query = _context.{DbSet}.AsQueryable();

    if (!string.IsNullOrWhiteSpace(searchTerm))
        query = query.Where(x => ...);

    var totalCount = await query.CountAsync();

    var items = await query
        .OrderByDescending(x => x.Id)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return (items, totalCount);
}
```

```csharp
// DTOs/{Dominio}ListResponseDTO.cs
public record {Dominio}ListResponseDTO
{
    [JsonPropertyName("items")]
    public List<{Dominio}DTO> Items { get; set; } = new();

    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }
}
```

**Reglas de Paginación:**
- El `Where` (filtros), el `OrderBy` y el `Skip`/`Take` SIEMPRE van en el Repository sobre `IQueryable` — así se traducen a SQL en lugar de traer toda la tabla a memoria
- El Repository devuelve `(List<T> Items, int TotalCount)`; el Service solo mapea `Items` a DTOs y propaga `TotalCount` tal cual, sin recalcularlo sobre listas en memoria
- El Controller expone `pageNumber` (default `1`) y `pageSize` (default `10`) como `[FromQuery]`, y responde con `{Dominio}ListResponseDTO { Items, TotalCount }`
- Si un endpoint interno (ej. exportación a Excel/PDF) necesita el set completo, pedirlo explícitamente con `pageSize: int.MaxValue` en vez de crear un método paralelo sin paginar

---

## 5. Sistema de Constantes

### 5.1 EndpointRoutes.cs (`Constants/EndpointRoutes.cs`)

Todas las rutas de endpoints se centralizan aquí como constantes `string`:

```csharp
namespace vet_api_Net.Routes;

public static class Endpoints
{
    public static class {NombreControlador}
    {
        public const string {Accion} = "{ruta-relativa}";
    }
}
```

**Regla:** Al crear un nuevo endpoint, SIEMPRE agregar la ruta aquí primero. El Controller la consume con `[HttpGet(Endpoints.{Clase}.{Prop})]`.

### 5.2 ResponseMessages.cs (`Constants/ResponseMessages.cs`)

Todos los mensajes de texto del sistema están centralizados como clases estáticas anidadas:

```csharp
namespace vet_api_Net.Constants;

public static class ResponseMessages{Dominio}
{
    public const string {Nombre} = "Mensaje en español.";
    // Para mensajes dinámicos:
    public static string {Nombre}(int id) => $"Texto con {id}.";
}
```

**Convención de nombrado de clases de mensajes:**
- `ResponseMessages{Dominio}` — Para respuestas HTTP (ej: `ResponseMessagesCitas`)
- `ResponseMessagesFacturaErrors` — Para errores específicos de un dominio
- `ResponseMessages{Dominio}Create` — Clases anidadas para operaciones específicas
- `Status` — Para estados de entidades (`pendiente`, `completada`, etc.)
- `PdfText`, `Exceltext` — Para textos en documentos generados
- `PaymentMethods` — Para valores de métodos de pago
- `TypeConsultas` — Para tipos de consultas

**Regla:** NUNCA hardcodear strings de respuesta en Controllers o Services. SIEMPRE usar constantes de este archivo.

---

## 6. Inyección de Dependencias (`Extensions/DependencyInjection.cs`)

Todo el registro de servicios se centraliza en un ÚNICO método de extensión:

```csharp
namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Configuración de base de datos
        // 2. CORS (dinámico desde BD)
        // 3. JWT Authentication
        // 4. DbContext
        // 5. Controladores y JSON
        // 6. Swagger, SignalR, HealthChecks
        // 7. Repositorios (AddScoped)
        // 8. Servicios (AddScoped)
        // 9. Utilities (AddScoped)
        // 10. Seguridad de datos
        // 11. Workers (AddHostedService)
        // 12. HttpClients externos
        // 13. Options Pattern (Configure<T>)
    }
}
```

**Al agregar un nuevo servicio/repositorio:**
1. Registrar la interfaz del **Repositorio** en la sección `// Repositorios de la Aplicación`
2. Registrar la interfaz del **Servicio** en la sección `// Servicios de la Aplicación`
3. Respetar el orden: `services.AddScoped<IInterface, Implementation>();`

---

## 7. Workers (Background Services)

Los workers se encuentran en `Worker/` y heredan de `BackgroundService`:

```csharp
namespace vet_api_Net.Workers;

public class {Nombre}Worker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<{Nombre}Worker> _logger;
    private readonly IConfiguration _configuration;

    // Inyectar por constructor: IServiceScopeFactory, ILogger, IConfiguration
    // Crear scope para acceder a servicios Scoped:
    // using var scope = _scopeFactory.CreateScope();
    // var service = scope.ServiceProvider.GetRequiredService<IService>();
}
```

**Configuración en `appsettings.json`:**
```json
"WorkerSettings": {
    "{Nombre}Worker": {
        "IntervalValues": 10,
        "IntervalUnits": "minutes",
        "RetentionValues": 30,
        "RetentionUnits": "minutes",
        "Enabled": true
    }
}
```

**Regla:** Los workers NUNCA inyectan servicios Scoped directamente. Siempre usan `IServiceScopeFactory.CreateScope()`.

---

## 8. SignalR Hubs

Los hubs se encuentran en `Hubs/` y se mapean en `Program.cs`:

```csharp
app.MapHub<MessageHub>("/hubs/mensajes");
app.MapHub<NotificactionsPush>("/hubs/notificaciones");
```

- `MessageHub` — Mensajería en tiempo real entre usuarios
- `NotificactionsPush` — Notificaciones push por rol/usuario, agrupa conexiones por `userId` y `rol`

---

## 9. Generación de Documentos

- **PDF:** `Services/GeneratePdfService.cs` usa **QuestPDF** (licencia Community)
- **Excel:** `Services/GenerateReportExcel.cs` usa **ClosedXML**
- **DOCX:** `Services/GenerateDocxService.cs` usa **DocumentFormat.OpenXml**
- Los archivos generados se guardan en `wwwroot/` y se sirven como archivos estáticos

---

## 10. Configuración de la Aplicación (`appsettings.json`)

| Sección | Clase de opciones | Propósito |
|---|---|---|
| `ApiSettings` | `ApiSettingsOptions` | Nombre del sistema, formatos de fecha, monedas |
| `DatabaseSettings` | (directo de IConfiguration) | Selector de proveedor de BD (`PostgreSQL`/`MySQL`) |
| `token-temporal` | `TokenTemporalOptions` | Token de desarrollo temporal |
| `SeedData` | `SeedDataOptions` | Control de datos iniciales |
| `BcvSettings` | (directo de IConfiguration) | Worker de tasa BCV |
| `WorkerSettings:{Worker}` | `WorkerSetting` | Configuración por worker |
| `ResendSettings` | (directo de IConfiguration) | API de envío de emails (Resend) |
| `ConnectionSettings` | (directo de IConfiguration) | CORS, túneles |
| `Jwt` | (directo de IConfiguration) | Credenciales JWT |

---

## 11. Data Seeding (`Data/seeds/SeedsData.cs`)

- Método estático `SeedsData.Initialize(AppDbContext, ApiSettingsOptions, SeedDataOptions)`
- Se ejecuta al inicio en `Program.cs` si `SeedData.Initialize == true`
- Crea usuarios dummy, métodos de pago, categorías, productos, citas de ejemplo
- Lee plantillas HTML de `Templates/Email/` y las inserta en `EmailTemplates`
- Usa `.Any()` para verificar si ya existen datos antes de insertar (idempotente)

---

## 12. Modelo de Datos (Entidades Principales)

```
Usuario ──< Cita >── Mascota ──< Cliente
                │
                ▼
           Consulta ──< ConsultasProducto >── Producto ──< CategoriasProducto
                │
                ▼
            Factura ──< DetallesFactura
```

**Otras entidades:** `MoneyType`, `MetodoPago`, `AlertasInterna`, `Mensaje`, `Vacuna`, `HistorialPrecio`, `MovimientosInventario`, `LogsSistema`, `Reporte`, `ReporConfig`, `FacturaConfig`, `SystemConfig`, `EmailTemplate`, `WSMessageAPIData`

---

## 13. Manejo Global de Errores

`Middleware/GlobalExceptionMiddleware.cs` se registra en `Program.cs` con `app.UseMiddleware<GlobalExceptionMiddleware>()`, antes de `UseAuthentication`/`UseAuthorization`.

- Actúa como red de seguridad para excepciones que se escapan del try/catch de un Controller (bugs, errores de infraestructura) — **no reemplaza** el try/catch descrito en [4.2](#42-controllers), que sigue siendo obligatorio para devolver códigos y mensajes específicos del dominio
- `Exceptions/` contiene excepciones de dominio personalizadas (ej. `LoginSecurityException`) que los Services lanzan y que los Controllers (o el middleware, como último recurso) traducen a respuestas HTTP
- `Security/` contiene utilidades de seguridad transversales no ligadas a un dominio de negocio (ej. `LoginSecurity` para bloqueo de intentos fallidos)

---

## 14. Reglas Críticas para Cambios

### SIEMPRE hacer:
1. Usar constantes de `EndpointRoutes.cs` para rutas en Controllers
2. Usar constantes de `ResponseMessages.cs` para mensajes de error/éxito
3. Registrar nuevos servicios/repositorios en `DependencyInjection.cs`
4. Crear interfaz antes de implementación (Interface → Implementation)
5. Validar nullability con `?` y `!` para evitar warnings CS86xx
6. Usar `DateTime.Now` para timestamps (NO `DateTime.UtcNow` para campos de visualización)
7. Mantener los nombres de propiedades de modelos en español
8. Usar `[JsonPropertyName("snake_case")]` en DTOs para el frontend
9. Aplicar `Where`/`OrderBy`/`Skip`/`Take` en el Repository sobre `IQueryable`, nunca en el Service sobre una lista ya materializada (ver [4.9](#49-paginación))

### NUNCA hacer:
1. NO inyectar `AppDbContext` directamente en Controllers
2. NO hardcodear strings de rutas o mensajes en Controllers/Services
3. NO poner lógica de negocio en Controllers
4. NO crear servicios sin su interfaz correspondiente
5. NO modificar migraciones existentes — crear nuevas con `dotnet ef migrations add`
6. NO olvidar registrar nuevos servicios en `DependencyInjection.cs`
7. NO usar `Console.WriteLine` para logging en producción — usar `ILogger<T>`
8. NO traer una tabla completa a memoria (`GetAllAsync()` + `.Where()` en el Service) cuando lo que se necesita es un listado filtrado o paginado

---

## 15. Comandos Frecuentes

```bash
# Ejecutar el proyecto
dotnet run

# Agregar una migración
dotnet ef migrations add {NombreMigración}

# Aplicar migraciones
dotnet ef database update

# Revertir última migración
dotnet ef migrations remove

# Build sin ejecutar
dotnet build
```

---

## 16. Puertos y URLs

| Servicio | URL | Puerto |
|---|---|---|
| API (desarrollo) | `http://localhost:5168` | 5168 |
| Frontend (Vite) | `http://localhost:5173` | 5173 |
| Swagger | `http://localhost:5168/swagger` | 5168 |
| Hub Mensajes | `ws://localhost:5168/hubs/mensajes` | 5168 |
| Hub Notificaciones | `ws://localhost:5168/hubs/notificaciones` | 5168 |
| Health Check | `http://localhost:5168/health` | 5168 |

---

## 17. Resumen de Dependencias Externas

- **Npgsql.EntityFrameworkCore.PostgreSQL** — PostgreSQL (Supabase)
- **Pomelo.EntityFrameworkCore.MySql** — MySQL (soporte dual)
- **Microsoft.EntityFrameworkCore.Design/Tools** — Migraciones EF Core
- **Microsoft.AspNetCore.Authentication.JwtBearer** — Autenticación JWT
- **QuestPDF** — Generación de PDFs (facturas)
- **ClosedXML** — Generación de reportes Excel
- **DocumentFormat.OpenXml** — Generación de documentos DOCX
- **HtmlAgilityPack** — Parsing HTML (scraper de tasa BCV)
- **BCrypt.Net-Next** — Hashing de contraseñas
- **NCrontab** — Parsing de expresiones cron en Workers
- **Swashbuckle.AspNetCore** — Swagger / OpenAPI
- **supabase-csharp** — Cliente de Supabase (Storage, ej. avatares)
- **SignalR** (incluido en ASP.NET Core) — Comunicación en tiempo real
- **Resend API** — Envío de correos electrónicos (HTTP directo, sin SDK dedicado)
- **BCV Scraper** — Scraping de la tasa del dólar del Banco Central de Venezuela (usa HtmlAgilityPack)

---

## 18. Comentarios

**Regla:** Nunca colocar comentarios en líneas sueltas en medio del código. Colocarlos únicamente:
- Justo después de las directivas `using`, o
- Justo antes de la declaración de `namespace`

En ambos casos con el formato `//Describe:` seguido de una descripción breve del servicio, su función o la lógica de negocio que implementa el archivo.

---

## idioma

**Regla** Simpre colocar la sintaxis en ingles, pero respuestas de errores o otra cosa si van en espanol, como lo hece mayormente todo el API

**Aclaración (vigente desde 2026-08-29):** esta regla aplica hacia adelante, no retroactivamente. Mucho código existente tiene identificadores en español (nombres de método, parámetros, variables) porque así se escribió originalmente — no hay que renombrarlo solo por encontrarlo. Pero todo identificador **nuevo** que se agregue de acá en adelante (parámetros de método, query params, nombres de variable, nombres de método) debe ir en inglés, incluso cuando el código de alrededor ya esté en español. Ejemplo real: el filtro de estado de citas del Dashboard se llama `status` (no `estado`) como parámetro/query param, aunque la propiedad del modelo que compara siga siendo `Cita.Estado` (correcto en español, por la regla de Models/DTOs de la sección 4). Ver también la nota de idioma de DTOs en la sección 4.7.

## Variables de entorno

**Regla** tambien se tienen que modificar el archivo appsettings.Development.json si se meodifica el appsettings.json, ya que en local toma los datos del appsettings.Development.json

## 19. Planificación de Cambios

**Regla:** Cuando se genere un plan de implementación (ej. con `EnterPlanMode`), el archivo del plan se guarda en la raíz del repositorio.
