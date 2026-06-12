# Documentación del Backend Happy Pets

## Descripción General
Este backend es la base de la aplicación Happy Pets, diseñada para gestionar de manera eficiente la administración de citas, clientes, mascotas, reportes, consultas, facturación y productos en una veterinaria. Está construido con .NET 10, ASP.NET Core y C#.

## Funcionalidades Principales
- **Gestión de Citas:** Crear, listar, modificar y eliminar citas para mascotas.
- **Gestión de Clientes:** Registrar, consultar y administrar información de clientes.
- **Gestión de Mascotas:** Registrar, consultar y administrar información de mascotas asociadas a clientes.
- **Gestión de Usuarios:** Registro, autenticación (login), actualización y administración de usuarios.
- **Facturación:** Generación y consulta de facturas asociadas a citas y servicios.
- **Reportes:** Generación de reportes en Excel y PDF sobre actividades, citas, facturación y más.
- **Productos:** Administración de productos veterinarios (altas, bajas, modificaciones).
- **Mensajería:** Envío y recepción de mensajes internos entre usuarios.

## Estructura de Carpetas

**Controllers/**: Controladores de la API REST. Ejemplo:
	- `AuthController.cs`, `CitasController.cs`, `ClientController.cs`, `PetsController.cs`, `FacturasController.cs`, `ReportController.cs`, etc.

**DTOs/**: Objetos de transferencia de datos (Data Transfer Objects) usados para entrada y salida de información en la API. Ejemplo:
	- `CreateCitaDTO.cs`, `ClientRequestDTO.cs`, `ProductsDTO.cs`, etc.

**Models/**: Modelos de datos que representan las entidades de la base de datos. Ejemplo:
	- `Cita.cs`, `Cliente.cs`, `Mascota.cs`, `Factura.cs`, etc.

**Services/**: Lógica de negocio y servicios para cada módulo. Ejemplo:
	- `AuthService.cs`, `CitasRequestService.cs`, `InvoiceService.cs`, etc.

**Interfaces/**: Interfaces que definen los contratos de los servicios, facilitando la inyección de dependencias y la escalabilidad. Ejemplo:
	- `IAuthService.cs`, `ICitasRequestService.cs`, etc.

**Configuration/**: Archivos de configuración y clases para parámetros globales o específicos del sistema.
	- `WorkerSetting.cs`: Configuración de workers o tareas en segundo plano.

**Data/**: Configuración y contexto de la base de datos (Entity Framework Core).
	- `AppDbContext.cs`: Contexto principal de la base de datos.

**Hubs/**: Comunicación en tiempo real usando SignalR.
	- `MessageHub.cs`: Hub para mensajería en tiempo real.

**Middleware/**: Middleware personalizado para la aplicación (autenticación, manejo de errores, etc.).

**Migrations/**: Archivos de migración de la base de datos generados por Entity Framework.
	- Ejemplo: `20260410014251_InitialCreate.cs`, `AppDbContextModelSnapshot.cs`, etc.

**Document/**: Documentación técnica adicional del sistema.
	- Ejemplo: `FacturationSystem.md`, `MessagingSystem.md`, `SecurityAndAuthentication.md`.

**Worker/**: Procesos en segundo plano o tareas programadas.
	- Ejemplo: `DeleteFacturaWorker.cs`.

**wwwroot/**: Archivos estáticos públicos (si aplica).

**scripts/**: Scripts útiles para la administración o despliegue.
	- Ejemplo: `call_docx_debug.ps1`, `create_invoice_test.ps1`.

**Properties/**: Configuración de inicio y perfiles de ejecución del proyecto.
	- `launchSettings.json`: Configuración de perfiles de lanzamiento para desarrollo.

**bin/** y **obj/**: Carpetas generadas automáticamente para la compilación y dependencias del proyecto.

**appsettings.json** y **appsettings.Development.json**: Archivos de configuración de la aplicación (conexión a base de datos, JWT, etc.).

**global.json**: Configuración global de versiones de SDK .NET.

## Principales Dependencias
- **BCrypt.Net-Next:** Hashing seguro de contraseñas.
- **ClosedXML:** Generación y manipulación de archivos Excel.
- **Pomelo.EntityFrameworkCore.MySql:** ORM para MySQL.
- **Microsoft.AspNetCore.Authentication.JwtBearer:** Autenticación basada en JWT.
- **Swashbuckle.AspNetCore:** Documentación Swagger/OpenAPI.
- **QuestPDF:** Generación de documentos PDF.
- **DocumentFormat.OpenXml:** Manipulación avanzada de archivos Office.

## Seguridad y Autenticación
- Autenticación JWT para proteger los endpoints.
- Hashing de contraseñas con BCrypt.
- Políticas de CORS configuradas para permitir el acceso desde el frontend autorizado.

## Flujo General de la Aplicación
1. **Autenticación:** Los usuarios inician sesión y reciben un token JWT.
2. **Operaciones CRUD:** Los controladores gestionan las operaciones sobre citas, clientes, mascotas, productos, facturas y reportes.
3. **Facturación y Reportes:** Se generan facturas y reportes en PDF/Excel bajo demanda.
4. **Mensajería:** Los usuarios pueden enviar y recibir mensajes internos.

## Configuración y Entorno
- Configuración de la base de datos y variables sensibles en `appsettings.json` y variables de entorno.
- Uso de migraciones para la gestión del esquema de la base de datos.

## Recomendaciones
- Mantener actualizadas las dependencias.
- Proteger las claves y cadenas de conexión.
- Consultar la documentación Swagger generada automáticamente para detalles de los endpoints.

---
Para más detalles sobre cada módulo, revisa los archivos en las carpetas `Controller/`, `Services/` y `DTOs/`.
