# Carpeta: Extensions (Métodos de Extensión)

## Funcionalidad
La carpeta `Extensions` almacena métodos de extensión estáticos del sistema. Su propósito es agregar funcionalidades adicionales a clases existentes y centralizar configuraciones complejas fuera del punto de entrada principal (`Program.cs`), promoviendo una arquitectura más limpia.

## Cualidades y Características
- **Centralización de Inyección de Dependencias (DI):** En lugar de llenar `Program.cs` con llamadas de registro de dependencias, la clase `DependencyInjection` actúa como un orquestador único para registrar bases de datos (PostgreSQL/MySQL), políticas de seguridad (CORS, JWT Bearer), Swagger, SignalR, Repositorios, Servicios, Utilities y Workers.
- **Mapeos Rápidos y Desacoplados:** Evita el uso de librerías de mapeo dinámico pesadas basadas en reflexión (ej. AutoMapper) al proveer extensiones de mapeo manual fuertemente tipado. Esto mejora notablemente el rendimiento y facilita la trazabilidad en tiempo de compilación.
- **Limpieza del Pipeline:** Al delegar la configuración a esta capa, la inicialización del servidor en `Program.cs` es extremadamente legible y fácil de mantener.

## Archivos Relevantes
- **[DependencyInjection.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Extensions/DependencyInjection.cs):** Extiende `IServiceCollection` para agrupar todo el registro de dependencias del framework y de la aplicación veterinaria.
- **[CitaMappingExtensions.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Extensions/CitaMappingExtensions.cs):** Contiene extensiones para convertir la entidad `Cita` del dominio C# a sus respectivos DTOs de salida para la interfaz API.
