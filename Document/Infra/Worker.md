# Carpeta: Worker (Procesos en Segundo Plano)

## Funcionalidad
La carpeta `Worker` contiene las tareas programadas y servicios hospedados (`Hosted Services`) que se ejecutan asíncronamente en segundo plano. Sus tareas principales incluyen el scraping periódico de la cotización cambiaria, la depuración automática de archivos temporales de reportes/facturas para liberar espacio, y la creación automatizada de reportes.

## Cualidades y Características
- **Ejecución Asíncrona en Segundo Plano:** Heredan de la clase abstracta `BackgroundService` de ASP.NET Core y se ejecutan en bucles persistentes no bloqueantes durante el ciclo de vida del servidor.
- **Resolución Segura de Ciclo de Vida (Scopes):** Al ser servicios Singleton, los workers no pueden inyectar directamente servicios con ciclo de vida Scoped (como repositorios o el `DbContext`). Para solucionar esto, inyectan `IServiceScopeFactory` y crean scopes locales mediante `using var scope = _scopeFactory.CreateScope()` para resolver servicios internos de manera aislada y limpia.
- **Configuración Parametrizada:** Su comportamiento se parametriza en el archivo `appsettings.json` (dentro de `WorkerSettings`), lo que permite activar/desactivar procesos individuales o cambiar intervalos de tiempo (ej: ejecutar cada 10 minutos) sin cambiar el código C#.
- **Robustez y Tolerancia a Fallos:** Implementan bloques try-catch internos para evitar que un fallo temporal (por ejemplo, pérdida de conexión a internet durante el scraping de la tasa BCV) detenga por completo el proceso de fondo de la API.

## Archivos Relevantes
- **[BcvWorker.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Worker/BcvWorker.cs):** Worker diario que ejecuta el scraper del Banco Central de Venezuela y actualiza la tasa cambiaria de bolívares en la base de datos.
- **[DeleteFacturaWorker.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Worker/DeleteFacturaWorker.cs):** Analiza y elimina de manera segura los archivos PDF de facturas antiguas almacenados en el servidor para evitar saturación de almacenamiento.
- **[DeleteReportWorker.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Worker/DeleteReportWorker.cs):** Remueve archivos Excel y reportes DOCX expirados de la carpeta pública del servidor.
- **[AutoGeneratingReportWorker.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Worker/AutoGeneratingReportWorker.cs):** Genera de forma automatizada informes de rendimiento financiero y de salud de la clínica veterinaria.
- **[ClearNotificationsWorker.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Worker/ClearNotificationsWorker.cs):** Depura notificaciones obsoletas del sistema almacenadas en base de datos.
