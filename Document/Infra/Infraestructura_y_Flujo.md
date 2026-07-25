# Guía de Infraestructura, Orden y Flujo de la API

Este documento describe detalladamente la arquitectura técnica del backend de **Happy Pets**, el orden que sigue el código y el flujo de ejecución de los procesos de la aplicación.

---

## 1. Patrón Arquitectónico: Tres Capas Desacopladas

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

---

## 2. Inicialización de la Aplicación (El Orden de Arranque)

Cuando el servidor del backend se enciende (`dotnet run`), sigue un orden secuencial estricto en el punto de entrada:

1. **Lectura de Program.cs:** Arranca el constructor de la aplicación web de ASP.NET Core (`WebApplication.CreateBuilder`).
2. **Inyección de Dependencias (DI):** Se ejecuta el método de extensión `AddInfrastructureServices` (ubicado en [DependencyInjection.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Extensions/DependencyInjection.cs)). En este punto se registran todos los componentes:
   - Se leen las secciones del archivo `appsettings.json` y se vinculan a las clases de [ApiSettings](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/ApiSettings/) y [Configuration](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Configuration/) (Options Pattern).
   - Se configura el `AppDbContext` detectando dinámicamente si se usará **PostgreSQL** o **MySQL**.
   - Se configuran los esquemas de autenticación JWT y políticas CORS.
   - Se registran de forma secuencial las interfaces y clases concretas de **Repositorios**, **Servicios**, **Utilities**, **HttpServices** y **Hubs de SignalR**.
   - Se registran los **Workers** que correrán de forma continua en segundo plano como servicios hospedados.
3. **Construcción del Pipeline HTTP (App Middleware):** Se define el pipeline de middlewares de ASP.NET Core:
   - Se registra el [GlobalExceptionMiddleware](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Middleware/GlobalExceptionMiddleware.cs) al inicio para interceptar cualquier fallo posterior.
   - Se activan los archivos estáticos de la carpeta [wwwroot](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/wwwroot/).
   - Se activan la autenticación y autorización (`UseAuthentication` y `UseAuthorization`).
   - Se mapean los endpoints de los controladores y las rutas de los Hubs de SignalR.
4. **Inicialización y Sembrado de Base de Datos:** Justo antes de correr el pipeline (`app.Run()`), el sistema verifica si está activa la configuración de base de datos para ejecutar migraciones pendientes de forma automática y sembrar la base de datos mediante `SeedsData.Initialize(...)` si no cuenta con información inicial.

---

## 3. El Flujo de una Petición HTTP (Request / Response)

Este es el recorrido detallado que realiza una solicitud HTTP (por ejemplo, el registro de una nueva cita por parte del frontend):

```
[Frontend Client]
       │
       │ (1) POST /api/Citas/crear (JSON Payload con snake_case)
       ▼
[GlobalExceptionMiddleware] (Intercepta y vigila la petición)
       │
       ▼
[Filtros de Seguridad JWT] (Valida el Token en la cabecera Authorization)
       │
       ▼
[CitasController / CrearCita]
       │
       │ (2) Valida ModelState. Mapea el JSON snake_case a CreateCitaDTO.
       │     Invoca a _citasService.CrearCitaAsync(dto)
       ▼
[CitasRequestService]
       │
       │ (3) Ejecuta reglas de negocio:
       │     - ¿Existe la mascota?
       │     - ¿El veterinario está disponible en ese horario?
       │     - Si hay un error, lanza una excepción (ej. `InvalidOperationException`).
       │     - Si todo es correcto, crea una instancia de la entidad `Cita` (Models).
       │     Invoca a _citasRepository.AddAsync(cita) y SaveChangesAsync()
       ▼
[CitasRepository]
       │
       │ (4) Inserta la entidad en el DBSet y ejecuta la llamada física a la BD.
       ▼
[Base de Datos] ◄─── (Guarda los registros)
       │
       │ (5) Retorna confirmación de filas afectadas.
       ▼
[CitasRepository] (Retorna true de SaveChangesAsync)
       │
       ▼
[CitasRequestService]
       │
       │ (6) Convierte la entidad `Cita` guardada en un `CitaResponseDTO`
       │     usando los métodos estáticos de CitaMappingExtensions.
       ▼
[CitasController]
       │
       │ (7) Recibe el DTO mapeado y retorna un código HTTP 201 (Created)
       │     con el DTO en el cuerpo de la respuesta.
       ▼
[GlobalExceptionMiddleware] (Valida que no haya excepciones sueltas)
       │
       │ (8) Response HTTP 201 (JSON con snake_case)
       ▼
[Frontend Client]
```

*Nota: Si en el paso (3) el servicio lanza una excepción, el flujo salta directamente al **GlobalExceptionMiddleware** en el paso (7), el cual intercepta el fallo y genera una respuesta limpia de error (ej: HTTP 400 o HTTP 500) con la estructura `{ error: "Mensaje descriptivo" }` sin llegar a colapsar el hilo de ejecución.*

---

## 4. Flujo de Comunicación en Tiempo Real (SignalR Sockets)

El backend soporta eventos push e interactividad instantánea que se ejecutan en paralelo al flujo HTTP:

```
[Usuario Conecta] ──► [Hub / NotificactionsPush]
                             │
                             ├─► Valida el Token JWT de la conexión WebSocket.
                             ├─► Agrupa la conexión del socket bajo el ID del usuario y su rol.
                             └─► Mantiene el canal abierto y en escucha.

[Evento de Negocio] ──► [Servicios]
                             │
                             ├─► (Ejemplo: Se confirma una cita de urgencia)
                             ├─► El servicio llama a IHubContext<NotificactionsPush>
                             └─► Envía un evento push directo al rol "Veterinario" o al cliente.
```

---

## 5. Flujo de Tareas en Segundo Plano (Workers)

Los Workers operan de forma asíncrona, desacoplados completamente del ciclo HTTP, ejecutando subprocesos automáticos gobernados por cronómetros:

```
[Bucle del Worker] ──► [Espera Intervalo / Cron] (Configurado en appsettings.json)
                             │
                             ▼
                     [Dispara Ejecución]
                             │
                             ├─► Inyecta IServiceScopeFactory para crear un scope local.
                             ├─► Resuelve servicios e interactúa con Repositorios.
                             ├─► (Ej: Limpia archivos de wwwroot, ejecuta scraper BCV, etc.)
                             └─► Retorna al estado de espera y loguea los resultados.
```
