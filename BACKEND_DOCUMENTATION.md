# Documentación del Backend — Happy Pets (vet-api-Net)

Este documento proporciona una guía detallada sobre el diseño, arquitectura, estructura de carpetas, lenguajes y la lógica del negocio del backend de la aplicación **Happy Pets**.

---

## 1. Descripción General
El backend de **Happy Pets** es una API RESTful desarrollada bajo la plataforma de **ASP.NET Core (.NET 10)**. Su objetivo es gestionar los flujos administrativos y operativos de una clínica veterinaria.

### Funcionalidades Clave:
*   **Gestión del Núcleo Veterinario:** Citas, mascotas, clientes, consultas médicas e historias clínicas.
*   **Administración Comercial:** Gestión de inventario de productos, categorías e historial de precios.
*   **Facturación Integral:** Generación automatizada de facturas detalladas en PDF (usando QuestPDF) calculando costos de consultas y productos.
*   **Tiempo Real (SignalR):** Chat interno para el personal de la clínica y notificaciones push segmentadas por usuario y rol.
*   **Tareas Programadas (Workers):** Scraping diario de divisas (tasa del Banco Central de Venezuela), limpieza automática de archivos PDF/Excel temporales y generación de reportes automáticos.
*   **Generación de Reportes:** Exportación de datos de rendimiento a formatos PDF, Excel y Word (OpenXml).

---

## 2. Tecnologías y Lenguajes

El backend está construido con las siguientes tecnologías:

1.  **Lenguaje Principal:** **C#** (utilizando características modernas de .NET 10 y C# 12/13).
2.  **Base de Datos:** Configuración dual que soporta **PostgreSQL (Supabase)** para producción y **MySQL** para desarrollo local mediante la lectura dinámica de `DatabaseSettings:Provider` en el archivo de configuración.
3.  **ORM (Object-Relational Mapping):** **Entity Framework Core**, utilizando `Npgsql` para PostgreSQL y `Pomelo` para MySQL.
4.  **Autenticación y Seguridad:** **JWT (JSON Web Tokens)** Bearer para proteger los endpoints y **BCrypt.Net** para el hashing seguro de contraseñas de usuarios.
5.  **Tiempo Real (WebSockets):** **ASP.NET Core SignalR** para la comunicación bidireccional inmediata.
6.  **Librerías de Documentos:**
    *   **QuestPDF:** Generación fluida de documentos PDF maquetados mediante código C#.
    *   **ClosedXML:** Creación y exportación de reportes tabulares de Excel.
    *   **DocumentFormat.OpenXml:** Manipulación y estructuración de archivos DOCX.
7.  **Servicios Externos:**
    *   **HttpClient** para realizar peticiones de scraping al Banco Central de Venezuela (tasa BCV) y para el envío de correos transaccionales a través de la API de **Resend**.

---

## 3. Convenciones y Reglas del Código

### 3.1 Idioma del Código (Convención Dual)
El código sigue una regla estricta de nombres según el dominio:
*   **Español:** Se utiliza para los nombres de los **modelos de datos**, **propiedades de base de datos**, **tablas** y **DTOs**.
    *   *Ejemplos:* `Mascota`, `Cita`, `PrecioActual`, `CreateCitaDTO`.
*   **Inglés:** Se utiliza para la **infraestructura**, **servicios**, **repositorios**, **controladores**, **namespaces** y **patrones de diseño**.
    *   *Ejemplos:* `CitasRepository`, `IUserService`, `MessagePollerService`, `DependencyInjection`.

### 3.2 Notación de Interfaces
Las interfaces de servicios, repositorios y utilidades están agrupadas en la carpeta `Interface/` bajo el namespace `Interfaze` (escrito con **'z'** de manera intencional en el proyecto):
*   `vet_api_Net.Interfaze.Services`
*   `vet_api_Net.Interfaze.Repositories`
*   `vet_api_Net.Interfaze.Utilities`

### 3.3 Formato de Datos API
Todos los DTOs de salida y entrada están decorados con `[JsonPropertyName("snake_case")]` para garantizar que la comunicación JSON con el frontend de React use la convención estándar de JavaScript (`snake_case`), mientras que en C# se conserva `PascalCase`.

---

## 4. Estructura de Carpetas

A continuación se detalla la estructura física del proyecto y el propósito de cada directorio:

```
vet-api-Net/
├── ApiSettings/            # Mapeo de configuraciones tipadas (Options Pattern) para appsettings.json.
├── Configuration/          # Clases de configuración para los Workers y tareas recurrentes.
├── Constants/              # Constantes globales del sistema. Centraliza las rutas de endpoints (EndpointRoutes.cs) y los mensajes de respuesta (ResponseMessages.cs).
├── Controller/             # Controladores de la API (Capa HTTP). Reciben peticiones, validan datos básicos y devuelven respuestas.
├── DTOs/                   # Data Transfer Objects. Objetos planos para transferir información entre cliente y servidor.
├── Data/                   # DbContext (AppDbContext.cs), mapeos de Fluent API y semillas de base de datos (seeds/SeedsData.cs).
├── Extensions/             # Métodos de extensión. Destaca DependencyInjection.cs que centraliza la inyección de dependencias de la aplicación.
├── HttpClient/             # Clientes de llamadas websocket e internas (mensajería).
├── HttpService/            # Servicios HTTP externos (Scraping de tasa BCV y Resend Email).
├── Hubs/                   # Hubs de SignalR para chat en tiempo real y notificaciones push.
├── Interface/              # Contratos e Interfaces organizados en las subcarpetas Repository, Services y Utilities.
├── Middleware/             # Middlewares de ASP.NET Core, como el gestor global de excepciones (GlobalExceptionMiddleware).
├── Migrations/             # Migraciones automáticas de EF Core para PostgreSQL/MySQL.
├── Models/                 # Entidades del dominio mapeadas directamente a las tablas de la base de datos.
├── Properties/             # Configuraciones de lanzamiento para desarrollo (launchSettings.json).
├── Repository/             # Implementación de la capa de datos (consultas LINQ y operaciones en base de datos).
├── Services/               # Implementación de la lógica de negocio pura (cálculos, validaciones complejas, flujos de trabajo).
├── Templates/              # Plantillas HTML estructuradas para correos electrónicos (citas, recuperación de claves).
├── Utilities/              # Servicios de soporte técnico y herramientas transversales (conversión de divisas, generador de Excel).
├── Worker/                 # Workers en segundo plano (BackgroundServices) que corren de manera asíncrona.
├── wwwroot/                # Servidor de archivos estáticos donde se guardan temporalmente PDFs y reportes generados.
├── Program.cs              # Punto de entrada de la aplicación. Configura el pipeline HTTP, middlewares y ejecuta el servidor.
└── appsettings.json        # Configuración general (credenciales, cadenas de conexión, JWT, etc.).
```

---

## 5. Patrón Arquitectónico y Flujo de Datos

El backend implementa de forma estricta un **patrón de 3 capas** desacoplado mediante inyección de dependencias:

```
[Cliente / Frontend] 
       │ (HTTP Request)
       ▼
 ┌───────────┐
 │Controller │ ──> Valida datos de entrada y define rutas (usando Constants).
 └───────────┘
       │ (Llamada al Servicio)
       ▼
 ┌───────────┐
 │  Service  │ ──> Contiene la lógica de negocio. Lanza excepciones específicas para errores.
 └───────────┘
       │ (Llamada al Repositorio)
       ▼
 ┌───────────┐
 │Repository │ ──> Accede a AppDbContext. Realiza queries LINQ / EF Core.
 └───────────┘
       │ (SQL Query)
       ▼
[Base de Datos]
```

### Reglas de Diseño por Capa:
1.  **Controladores:** NUNCA acceden a `AppDbContext` de forma directa ni contienen lógica de negocio. Devuelven códigos HTTP (`Ok`, `BadRequest`, `NotFound`, `InternalServerError`) basándose en el resultado o en las excepciones lanzadas por los servicios. Consumen constantes físicas de `ResponseMessages` y `EndpointRoutes` para evitar cadenas mágicas (*hardcoded strings*).
2.  **Servicios:** Orquestan la lógica. Si ocurre una violación a las reglas del negocio (por ejemplo, registrar una cita en un horario ocupado o producto sin stock), lanzan excepciones del sistema (`InvalidOperationException`, `ArgumentException`, `KeyNotFoundException`) que el middleware global captura y transforma en respuestas de error limpias para el cliente.
3.  **Repositorios:** Es la única capa que interactúa con la base de datos a través del `AppDbContext`. Se limitan a realizar consultas optimizadas (usando `.Include()` y `.ThenInclude()` cuando es necesario) y a persistir los datos.

---

## 6. Lógica de Módulos Clave

### 6.1 Sistema de Citas y Consultas
El flujo de atención veterinaria sigue el ciclo:
`Creación de Cita` ➔ `Cita Aceptada/Completada` ➔ `Generación de Consulta Médica` ➔ `Facturación`.
Durante la consulta, el veterinario asocia los productos e insumos utilizados de la tabla `Productos`, lo que actualiza automáticamente el inventario (`MovimientosInventario`) y calcula los montos correspondientes.

### 6.2 Facturación y Conversión de Monedas
El sistema cuenta con una lógica avanzada de facturación:
*   Maneja tipos de moneda dual (Bolívares / Dólares) a través del servicio de conversión `CurrencyService`.
*   El valor del dólar se actualiza automáticamente mediante el `BcvWorker`, que ejecuta un scraper (`BcvScraper`) contra la web oficial del Banco Central de Venezuela a intervalos programados.
*   Una vez guardada la factura, el `GeneratePdfService` genera un comprobante PDF con la librería **QuestPDF**, almacenándolo en la carpeta `wwwroot/` para que pueda ser descargado de inmediato por el cliente.

### 6.3 Mensajería y Notificaciones en Tiempo Real
Mediante los hubs de SignalR:
*   `MessageHub`: Canal directo WebSocket para la comunicación entre doctores, administradores y personal de la veterinaria.
*   `NotificactionsPush`: Permite enviar alertas en tiempo real al usuario correspondiente cuando se le asigna una cita, se actualiza una historia clínica o hay eventos del sistema. Para asegurar el envío, asocia las conexiones de SignalR con los identificadores de usuario (`userId`) y sus roles.

### 6.4 Tareas en Segundo Plano (Workers)
Se ejecutan servicios hospedados (`BackgroundService`) para mantener limpio y actualizado el backend:
*   `BcvWorker`: Actualiza el tipo de cambio oficial diariamente.
*   `DeleteFacturaWorker` & `DeleteReportWorker`: Eliminan de `wwwroot/` los PDFs y reportes de Excel antiguos para evitar el consumo innecesario de almacenamiento.
*   `AutoGenerateReportWorker`: Genera reportes automatizados de rendimiento según las configuraciones del sistema.
*   `ClearNotificationsWorker`: Limpia notificaciones obsoletas del sistema.

---

## 7. Configuración e Inicio de la Aplicación

### Requisitos Previos:
*   .NET 10.0 SDK instalado.
*   Base de datos PostgreSQL o MySQL activa.

### Configuración inicial (`appsettings.json`):
Debe configurarse la sección `DatabaseSettings` indicando el proveedor:
```json
"DatabaseSettings": {
  "Provider": "PostgreSQL" // o "MySQL"
}
```
Y configurar correspondientemente las cadenas de conexión (`DefaultConnection` o `SupabaseConnection`), las claves del token JWT y las credenciales de la API de correos Resend.

### Comandos de Ejecución y Consola:
1.  **Restaurar paquetes NuGet:**
    ```bash
    dotnet restore
    ```
2.  **Compilar el backend:**
    ```bash
    dotnet build
    ```
3.  **Aplicar migraciones pendientes:**
    ```bash
    dotnet ef database update
    ```
4.  **Correr la aplicación:**
    ```bash
    dotnet run
    ```
    El servidor iniciará en el puerto configurado por defecto (`http://localhost:5168`). Se puede acceder al panel interactivo de Swagger en `http://localhost:5168/swagger`.
