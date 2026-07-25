# Carpeta: Configuration

## Funcionalidad
La carpeta `Configuration` agrupa las clases que definen los modelos de configuración específicos del sistema, en particular para parametrizar el comportamiento dinámico y las variables operativas de los servicios en segundo plano (`Workers`).

## Cualidades y Características
- **Modularidad:** Separa la configuración operativa y de infraestructura de la configuración general de la API, permitiendo parametrizar componentes individuales como la limpieza de archivos, la expiración de datos o la periodicidad de los reportes.
- **Acoplamiento Débil:** Se conecta directamente con el patrón Options de ASP.NET Core, permitiendo mapear secciones específicas de `appsettings.json` (por ejemplo, bajo la sección de configuraciones de workers) a clases específicas del dominio.
- **Flexibilidad:** Facilita la configuración en tiempo de ejecución de intervalos de tiempo y unidades de tiempo (minutos, horas, días) para la ejecución periódica de procesos.

## Archivos Relevantes
- **[WorkerSetting.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Configuration/WorkerSetting.cs):** Define los parámetros de ejecución periódica (intervalos, unidades, habilitado) y tiempos de retención para los Workers.
- **[FacturasDeletingSetting.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Configuration/FacturasDeletingSetting.cs):** Configura las reglas de retención y eliminación automática de las facturas generadas.
- **[ReportGenratingSetting.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Configuration/ReportGenratingSetting.cs):** Configuración específica asociada a la periodicidad y ejecución automática de reportes del sistema.
