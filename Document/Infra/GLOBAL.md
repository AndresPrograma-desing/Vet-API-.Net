# DOCUMENTACIÓN GLOBAL DE LA INFRAESTRUCTURA Y COMPONENTES (vet-api-Net)

Este documento consolida toda la información de la arquitectura física y lógica del backend de **Happy Pets**, detallando la funcionalidad de cada carpeta y el flujo de ejecución de la API.

---

## ÍNDICE
1. [Guía de Infraestructura, Orden y Flujo de la API](#guía-de-infraestructura-orden-y-flujo-de-la-api)
2. [ApiSettings](#1-apisettings)
3. [Configuration](#2-configuration)
4. [Constants](#3-constants)
5. [Controller](#4-controller)
6. [DTOs](#5-dtos)
7. [Data](#6-data)
8. [Extensions](#7-extensions)
9. [HttpClient](#8-httpclient)
10. [HttpService](#9-httpservice)
11. [Hubs](#10-hubs)
12. [Interface](#11-interface)
13. [Middleware](#12-middleware)
14. [Migrations](#13-migrations)
15. [Models](#14-models)
16. [Repository](#15-repository)
17. [Services](#16-services)
18. [Templates](#17-templates)
19. [Utilities](#18-utilities)
20. [Worker](#19-worker)
21. [wwwroot](#20-wwwroot)

---

# GUÍA DE INFRAESTRUCTURA, ORDEN Y FLUJO DE LA API

## Patrón Arquitectónico: Tres Capas Desacopladas
El backend está estructurado siguiendo un patrón estricto de **3 capas**, donde cada capa tiene responsabilidades bien definidas y se desacopla de las demás mediante la Inyección de Dependencias.

```
                  ┌─────────────────────────┐
                  │   Cliente / Frontend    │
                  └────────────┬────────────┘
                               │ (HTTP Request / WebSockets)
                               ▼
┌─────────────────────────────────────────────────────────────┐
│ Capa 1: Controlador (Controller)                            │
│  - Define las rutas usando Constants/EndpointRoutes.cs      │
│  - Valida el payload de entrada a través de DTOs.           │
│  - Retorna respuestas HTTP estructuradas.                   │
└──────────────────────────────┬──────────────────────────────┘
                               │ (Llamada al Service)
                               ▼
┌─────────────────────────────────────────────────────────────┐
│ Capa 2: Servicio (Service)                                  │
│  - Contiene la lógica y reglas de negocio del dominio.      │
│  - Lanza excepciones tipadas si se infringen las reglas.    │
│  - Consume utilidades y servicios HTTP externos.            │
└──────────────────────────────┬──────────────────────────────┘
                               │ (Llamada al Repository)
                               ▼
┌─────────────────────────────────────────────────────────────┐
│ Capa 3: Repositorio (Repository)                            │
│  - Única capa con acceso directo a AppDbContext.            │
│  - Realiza queries optimizadas con LINQ / Entity Framework. │
└──────────────────────────────┬──────────────────────────────┘
                               │ (SQL Query)
                               ▼
                  ┌─────────────────────────┐
                  │      Base de Datos      │
                  └─────────────────────────┘
```

### Reglas Críticas de Aislamiento:
1. **Controladores:** Tienen prohibido acceder al `AppDbContext` o implementar reglas de negocio. Se comunican exclusivamente con las interfaces de la capa de Servicios.
2. **Servicios:** Tienen prohibido comunicarse directamente con la base de datos (con excepciones mínimas de administración). Inyectan las interfaces de Repositorios. Retornan errores a través de excepciones del sistema.
3. **Repositorios:** Tienen prohibido implementar reglas de negocio. Su único propósito es resolver consultas eficientes y guardar los datos.

## Inicialización de la Aplicación (El Orden de Arranque)
Cuando el servidor del backend se enciende (`dotnet run`), sigue un orden secuencial estricto en el punto de entrada:
1. **Lectura de Program.cs:** Arranca el constructor de la aplicación web de ASP.NET Core (`WebApplication.CreateBuilder`).
2. **Inyección de Dependencias (DI):** Se ejecuta el método de extensión `AddInfrastructureServices` (ubicado en `Extensions/DependencyInjection.cs`). En este punto se registran todos los componentes:
   - Se leen las secciones del archivo `appsettings.json` y se vinculan a las clases de `ApiSettings` y `Configuration` (Options Pattern).
   - Se configura el `AppDbContext` detectando dinámicamente si se usará **PostgreSQL** o **MySQL**.
   - Se configuran los esquemas de autenticación JWT y políticas CORS.
   - Se registran de forma secuencial las interfaces y clases concretas de **Repositorios**, **Servicios**, **Utilities**, **HttpServices** y **Hubs de SignalR**.
   - Se registran los **Workers** que correrán de forma continua en segundo plano como servicios hospedados.
3. **Construcción del Pipeline HTTP (App Middleware):** Se define el pipeline de middlewares de ASP.NET Core:
   - Se registra el `GlobalExceptionMiddleware` al inicio para interceptar cualquier fallo posterior.
   - Se activan los archivos estáticos de la carpeta `wwwroot`.
   - Se activan la autenticación y autorización (`UseAuthentication` y `UseAuthorization`).
   - Se mapean los endpoints de los controladores y las rutas de los Hubs de SignalR.
4. **Inicialización y Sembrado de Base de Datos:** Justo antes de correr el pipeline (`app.Run()`), el sistema verifica si está activa la configuración de base de datos para ejecutar migraciones pendientes de forma automática y sembrar la base de datos mediante `SeedsData.Initialize(...)` si no cuenta con información inicial.

## El Flujo de una Petición HTTP (Request / Response)
Este es el recorrido detallado que realiza una solicitud HTTP:
1. **POST /api/Citas/crear:** El frontend envía un payload JSON en snake_case.
2. **GlobalExceptionMiddleware / Filtros JWT:** Intercepta la petición y valida el Token JWT.
3. **CitasController / CrearCita:** Recibe la petición, valida ModelState, mapea a `CreateCitaDTO` y llama a `_citasService.CrearCitaAsync(dto)`.
4. **CitasRequestService:** Ejecuta las reglas de negocio (ej. disponibilidad de veterinario, existencia de mascota). Si algo falla, lanza excepciones (ej. `InvalidOperationException`). Si es válido, crea la entidad `Cita` (Models) e invoca a `_citasRepository.AddAsync(cita)` y `SaveChangesAsync()`.
5. **CitasRepository:** Inserta el registro en el DBSet y ejecuta la transacción física en la Base de Datos.
6. **CitasRequestService (Mapeo a DTO):** Al recibir confirmación, convierte la entidad `Cita` en un `CitaResponseDTO` usando los métodos estáticos de `CitaMappingExtensions`.
7. **CitasController (Response):** Retorna un código HTTP 201 (Created) con el DTO resultante en formato JSON.

---

# DETALLE DE COMPONENTES POR CARPETA

## 1. ApiSettings

### Funcionalidad
La carpeta `ApiSettings` es responsable de definir e implementar el mapeo tipado de las configuraciones de la aplicación declaradas en el archivo `appsettings.json` utilizando el patrón **Options** (`Options Pattern`) de ASP.NET Core.

### Cualidades y Características
- **Tipado Fuerte (Strongly Typed):** Evita el uso de cadenas de texto mágicas (*string keys*) en otras capas del sistema para acceder a configuraciones globales (ej. nombre del sistema, formatos de fecha, tipo de moneda principal).
- **Inyección de Dependencias:** Se configuran en el contenedor DI (`DependencyInjection.cs`) mediante `services.Configure<TOptions>(...)` y se consumen utilizando `IOptions<TOptions>` o `IOptionsSnapshot<TOptions>`.

### Archivos Relevantes
- **[ApiSettingsOptions.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/ApiSettings/ApiSettingsOptions.cs):** Representa las configuraciones básicas de la API veterinaria como monedas admitidas (`MoneyTypes`), datos de la empresa, formatos de hora y fecha.

---

## 2. Configuration

### Funcionalidad
Agrupa las clases que definen los modelos de configuración específicos del sistema, en particular para parametrizar el comportamiento dinámico de los servicios en segundo plano (`Workers`).

### Cualidades y Características
- **Modularidad:** Separa la configuración operativa y de infraestructura de la configuración general de la API.
- **Flexibilidad:** Facilita la configuración en tiempo de ejecución de intervalos de tiempo y unidades de tiempo (minutos, horas, días) para la ejecución periódica de procesos.

### Archivos Relevantes
- **[WorkerSetting.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Configuration/WorkerSetting.cs):** Parámetros de ejecución periódica (intervalos, unidades, habilitado) y retención.
- **[FacturasDeletingSetting.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Configuration/FacturasDeletingSetting.cs):** Configura la eliminación de facturas generadas.
- **[ReportGenratingSetting.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Configuration/ReportGenratingSetting.cs):** Configuración asociada a la generación periódica de reportes.

---

## 3. Constants

### Funcionalidad
Actúa como un registro único y centralizado para todos los valores constantes y estáticos no mutables de la aplicación para evitar cadenas mágicas (*magic values*).

### Cualidades y Características
- **Centralización Completa:** Agrupa rutas de endpoints, mensajes de respuesta HTTP y variables operativas globales.
- **Internacionalización y Consistencia:** Todos los mensajes de respuesta del sistema hacia el cliente están centralizados en español.
- **La Constante `TargetId`:** Definida en `Variables.cs` con el valor `1`, sirve para identificar unívocamente la fila única en la tabla `MoneyTypes` de la base de datos que maneja la conversión de moneda y tasa BCV. Es fundamental para la integridad del flujo cambiario en los servicios y workers.

### Archivos Relevantes
- **[EndpointRoutes.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Constants/EndpointRoutes.cs):** Estructura de rutas de la API organizada por clases estáticas según la entidad.
- **[ResponseMessages.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Constants/ResponseMessages.cs):** Mensajes de error, éxito, advertencia e informativos en español.
- **[Variables.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Constants/Variables.cs):** Define constantes generales reutilizables y la constante `TargetId` para control de divisas.

---

## 4. Controller

### Funcionalidad
Contiene los controladores de la API (Capa HTTP). Su responsabilidad es exponer los puntos de entrada (endpoints) REST de la aplicación veterinaria para ser consumidos por el cliente frontend (React/Vite).

### Cualidades y Características
- **Separación de Responsabilidades:** No interactúan con la base de datos ni contienen lógica de negocio. Delegan todo a la capa de servicios.
- **Seguridad (Autenticación y Autorización):** Muchos controladores están decorados con atributos `[Authorize]` y políticas de roles.
- **Estructura de Errores Unificada:** Capturan excepciones de negocio específicas de los servicios y retornan un formato JSON estándar (`{ error = "..." }` o `{ message = "..." }`).

### Archivos Relevantes
- **[AuthController.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Controller/AuthController.cs):** Gestión de autenticación, inicios de sesión y tokens JWT.
- **[CitasController.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Controller/CitasController.cs):** Puntos de acceso para el flujo de reserva y atención de citas.

---

## 5. DTOs (Data Transfer Objects)

### Funcionalidad
Contiene los objetos planos para transferir información entre el frontend y el backend, desacoplando y protegiendo las entidades del dominio de la base de datos.

### Cualidades y Características
- **Uso de `record` para Inmutabilidad:** Los DTOs se definen preferentemente como `record` para inmutabilidad y comparación por valor.
- **Serialización snake_case:** Propiedades decoradas con `[JsonPropertyName("snake_case")]` para comunicarse de forma estándar con el frontend de JavaScript/React, mientras mantienen la convención `PascalCase` en el código C#.

### Archivos Relevantes
- **[CreateCitaDTO.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/DTOs/CreateCitaDTO.cs):** Modelo para registrar citas.
- **[FacturaResponseDTO.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/DTOs/FacturaResponseDTO.cs):** Formato de envío de facturación al frontend.

---

## 6. Data

### Funcionalidad
Se encarga de la configuración del ORM (Entity Framework Core) y de la inicialización de los datos requeridos por la aplicación a través de la base de datos.

### Cualidades y Características
- **Soporte Dual:** Puede operar dinámicamente tanto con PostgreSQL (Supabase) como con MySQL (Pomelo) según la configuración del archivo `appsettings.json`.
- **Fluent API:** Define relaciones complejas, claves foráneas y tipos de datos explícitos en `OnModelCreating`.
- **Inicialización Idempotente (Data Seeding):** La clase `SeedsData` lee plantillas HTML y si la base de datos está vacía, inserta de forma segura usuarios por defecto, métodos de pago, configuraciones base del sistema y plantillas de correo electrónico.

### Archivos Relevantes
- **[AppDbContext.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Data/AppDbContext.cs):** Contexto principal del ORM.
- **[SeedsData.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Data/seeds/SeedsData.cs):** Lógica del sembrado de datos en la inicialización.

---

## 7. Extensions

### Funcionalidad
Almacena métodos de extensión estáticos del sistema. Su propósito es agregar funcionalidades adicionales a clases existentes y centralizar configuraciones del contenedor DI.

### Cualidades y Características
- **Modularidad del Pipeline:** `DependencyInjection` agrupa el registro de dependencias (Bases de datos, JWT, CORS, Repositorios, Servicios, Workers), manteniendo limpio el archivo `Program.cs`.
- **Mapeos Manuales de Alto Rendimiento:** Métodos de extensión fuertemente tipados para convertir entidades del dominio a DTOs (ej: `CitaMappingExtensions.cs`), evitando el uso de reflexión en tiempo de ejecución.

### Archivos Relevantes
- **[DependencyInjection.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Extensions/DependencyInjection.cs):** Métodos de extensión para `IServiceCollection`.
- **[CitaMappingExtensions.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Extensions/CitaMappingExtensions.cs):** Extensiones para mapeo manual de Citas.

---

## 8. HttpClient

### Funcionalidad
Define clientes HTTP y WebSocket para realizar conexiones y envíos de datos en la red interna o servicios secundarios de chat/mensajería.

### Cualidades y Características
- **Abstracción mediante interfaces:** Utiliza la interfaz `IWSMessage` para encapsular la lógica de envío, permitiendo modularidad y realización de pruebas.
- **Asincronía persistente:** Diseñado para el manejo no bloqueante de tráfico en tiempo real.

### Archivos Relevantes
- **[IWSMessage.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/HttpClient/IWSMessage.cs):** Contrato del cliente WebSocket.
- **[WSMessage.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/HttpClient/WSMessage.cs):** Procesador de paquetes de mensajería interna.

---

## 9. HttpService

### Funcionalidad
Encapsula los servicios responsables de la comunicación saliente con servidores externos de terceros.

### Cualidades y Características
- **Web Scraping Financiero:** Extrae diariamente en tiempo real la tasa del dólar del portal oficial del Banco Central de Venezuela (`BcvScraper`).
- **Envío de Correos:** Integra la plataforma **Resend** para mandar correos con formato rico en HTML empleando las plantillas HTML del sistema.

### Archivos Relevantes
- **[BcvScraper.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/HttpService/BcvScraper.cs):** Extrae la tasa cambiaria del portal web del BCV.
- **[ResendEmailService.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/HttpService/ResendEmailService.cs):** Despacha correos electrónicos transaccionales.

---

## 10. Hubs

### Funcionalidad
Contiene los centros de comunicación en tiempo real basados en **ASP.NET Core SignalR** para comunicación WebSocket bidireccional.

### Cualidades y Características
- **Actualizaciones en Tiempo Real de Baja Latencia:** Mantiene la interactividad del chat y notificaciones push sin realizar consultas repetitivas HTTP.
- **Segmentación por Usuario y Rol:** `NotificactionsPush` asocia los sockets activos de un usuario mediante su `userId` y `rol` para despachar alertas específicas.

### Archivos Relevantes
- **[MessageHub.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Hubs/MessageHub.cs):** Gestiona la comunicación por chat interno entre el personal.
- **[NotificactionsPush.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Hubs/NotificactionsPush.cs):** Administra el envío dinámico de alertas y notificaciones del sistema.

---

## 11. Interface

### Funcionalidad
Contiene las interfaces y contratos de la aplicación. Separa las declaraciones y firmas de métodos de su implementación física.

### Cualidades y Características
- **Namespace Especial `Interfaze`:** Utiliza el namespace personalizado con la letra **"z"** (`vet_api_Net.Interfaze.Repositories`, `vet_api_Net.Interfaze.Services`, `vet_api_Net.Interfaze.Utilities`).
- **Inversión de Dependencias (SOLID):** Permite cambiar las implementaciones físicas de las clases (por ejemplo, cambiar el motor de persistencia o el proveedor de correos) sin modificar la lógica cliente que las consume.

### Estructura de Directorios
- **`Interface/Repository/`:** Contratos para el acceso a datos.
- **`Interface/Services/`:** Contratos de lógica de negocio.
- **`Interface/Utilities/`:** Contratos de utilidades técnicas.

---

## 12. Middleware

### Funcionalidad
Contiene interceptores que se incorporan en el pipeline HTTP para interceptar, auditar o transformar solicitudes y respuestas.

### Cualidades y Características
- **Captura Global y Unificada de Excepciones:** Contiene un interceptor que atrapa cualquier error inesperado en cualquier capa, formateando la salida a JSON y devolviendo códigos de estado uniformes (ej: 500) de forma segura.

### Archivos Relevantes
- **[GlobalExceptionMiddleware.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Middleware/GlobalExceptionMiddleware.cs):** Capturador y formateador centralizado de errores.

---

## 13. Migrations

### Funcionalidad
Contiene el historial de cambios incrementales de la estructura de base de datos controlados y generados de manera automatizada por Entity Framework Core.

### Cualidades y Características
- **Portabilidad del Esquema:** Permite regenerar o actualizar de forma consistente las tablas y campos de la base de datos mediante comandos de consola.
- **Model Snapshot:** El snapshot (`AppDbContextModelSnapshot.cs`) representa el estado actual de base de datos modelado en C#.

### Archivos Relevantes
- **[AppDbContextModelSnapshot.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Migrations/AppDbContextModelSnapshot.cs):** Instantánea del esquema físico del dominio actual.

---

## 14. Models

### Funcionalidad
Representa la capa de datos del dominio de la aplicación, definiendo las clases que mapean a las tablas físicas de la base de datos.

### Cualidades y Características
- **Nombres en Español:** Clases, propiedades y relaciones nombradas estrictamente en **idioma español** (ej: `Cliente`, `Mascota`, `PrecioActual`, `Creado`).
- **Navegación Virtual:** Propiedades de relaciones marcadas como `virtual` para dar soporte a la carga diferida (*lazy loading*).
- **Evitar Nullability Warnings:** Uso del operador null-forgiving (`= null!`) para propiedades requeridas inicializadas por el ORM.

### Archivos Relevantes
- **[Cliente.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Models/Cliente.cs):** Dueños de las mascotas.
- **[Cita.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Models/Cita.cs):** Agenda de citas de la veterinaria.
- **[Factura.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Models/Factura.cs):** Registro comercial y montos de facturación.

---

## 15. Repository

### Funcionalidad
Implementa el patrón Repository de acceso a datos, realizando las llamadas físicas, consultas en LINQ/EF Core y persistencia en la base de datos.

### Cualidades y Características
- **Aislamiento Exclusivo:** Es la **única** capa con permitido inyectar y usar `AppDbContext`.
- **Sintaxis de Expresión:** Métodos sencillos implementados en una línea (`=>`).
- **Carga Ansiosa:** Uso de `.Include()` y `.ThenInclude()` para resolver consultas relacionales optimizadas en una sola llamada SQL.

### Archivos Relevantes
- **[CitasRepository.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Repository/CitasRepository.cs):** Búsquedas y filtros sobre la agenda de citas.
- **[UserRepository.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Repository/UserRepository.cs):** Consultas de cuentas de usuarios y credenciales.

---

## 16. Services

### Funcionalidad
Capa de lógica y reglas de negocio del backend. Orquesta procesos y validaciones, comunicando controladores con repositorios.

### Cualidades y Características
- **Reglas de Negocio Centralizadas:** Valida restricciones (citas, stock, historiales) y coordina flujos de trabajo de múltiples dominios.
- **Errores Basados en Excepciones:** Lanza excepciones tipadas (ej: `ArgumentException`, `InvalidOperationException`) utilizando los mensajes unificados de `ResponseMessages.cs`.
- **Desacoplado:** Inyecta interfaces de repositorios, no contextos directos de bases de datos.

### Archivos Relevantes
- **[AuthService.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Services/AuthService.cs):** Lógica de hashing de claves y generación de tokens JWT.
- **[InvoiceService.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Services/InvoiceService.cs):** Emisión y cálculo de facturas, control de stock y tasas.

---

## 17. Templates

### Funcionalidad
Contiene los archivos de diseño y plantillas HTML estáticas utilizadas para correos transaccionales.

### Cualidades y Características
- **Marcadores Dinámicos:** Placeholders (`{{userName}}`, `{{citaFecha}}`) que el servicio de correo reemplaza programáticamente.
- **Lectura Automática en Seeding:** Se leen en caliente durante la inicialización y se sincronizan con la tabla `EmailTemplates` de la base de datos.

### Estructura de Directorios
- **`Templates/Email/`:** Plantillas para confirmaciones de citas, recuperación de contraseñas y envíos de facturas.

---

## 18. Utilities

### Funcionalidad
Implementa servicios de soporte transversal que no pertenecen directamente al dominio veterinario (Excel, conversor cambiario, avatares, analizadores de texto).

### Cualidades y Características
- **Reutilización:** Consumidos por múltiples módulos del negocio.
- **Aislamiento de Complejidades Técnicas:** Evita acoplar la lógica de negocio con detalles de manipulación de formatos binarios de Excel/Word o lógicas de reintentos ante fallas.

### Archivos Relevantes
- **[CurrencyService.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Utilities/CurrencyService.cs):** Conversiones aritméticas de monedas (VES / USD) basadas en la tasa del día.
- **[ExcelGenerator.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Utilities/ExcelGenerator.cs):** Generador estético de reportes de Excel usando **ClosedXML**.

---

## 19. Worker

### Funcionalidad
Contiene los procesos en segundo plano y servicios hospedados (`BackgroundService`) que se ejecutan asíncronamente y de manera continua.

### Cualidades y Características
- **Resolución de Scopes Localizados:** Inyectan `IServiceScopeFactory` para crear scopes locales temporales y poder consumir servicios Scoped (DbContext y Repositorios) de forma segura.
- **Configuración Dinámica:** Su estado e intervalos de tiempo se parametrizan en el archivo de configuración `appsettings.json`.

### Archivos Relevantes
- **[BcvWorker.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Worker/BcvWorker.cs):** Scraping diario de la tasa cambiaria del BCV.
- **[DeleteFacturaWorker.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Worker/DeleteFacturaWorker.cs):** Limpieza automática de facturas PDF viejas en disco.
- **[DeleteReportWorker.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Worker/DeleteReportWorker.cs):** Limpieza de reportes de Excel y Word expirados.

---

## 20. wwwroot

### Funcionalidad
Directorio raíz para servir archivos estáticos de forma pública en el servidor. Almacena de forma temporal facturas y reportes generados.

### Cualidades y Características
- **Acceso Público Directo:** Los archivos guardados aquí pueden descargarse mediante URLs tradicionales.
- **Depuración Periodica:** Es monitoreada por los workers de depuración para evitar la acumulación excesiva de documentos antiguos generados por QuestPDF o ClosedXML.
