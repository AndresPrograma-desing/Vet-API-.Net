# Carpeta: Middleware (Interceptores del Pipeline HTTP)

## Funcionalidad
La carpeta `Middleware` aloja componentes que se ensamblan en el pipeline de la aplicación ASP.NET Core para inspeccionar, transformar o interceptar solicitudes HTTP y respuestas. Su funcionalidad clave es el manejo global y centralizado de excepciones.

## Cualidades y Características
- **Captura Global de Excepciones:** Contiene un middleware que envuelve la ejecución de toda la petición HTTP. Si ocurre una excepción no controlada en cualquier capa (ej: error de conexión a la base de datos o error de lógica inesperado), el middleware la captura inmediatamente.
- **Respuestas de Error Consistentes:** Transforma excepciones genéricas en respuestas HTTP estructuradas en formato JSON (`{ error = "..." }`) con los códigos de estado apropiados (habitualmente HTTP 500).
- **Seguridad en Producción:** Oculta los detalles técnicos del error (como el stack trace de C#) al cliente final para evitar brechas de seguridad, mientras los registra de manera segura en el backend.
- **Logging Centralizado:** Utiliza `ILogger` para dejar constancia de los fallos, facilitando el análisis a los desarrolladores sin necesidad de depurar paso a paso.

## Archivos Relevantes
- **[GlobalExceptionMiddleware.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Middleware/GlobalExceptionMiddleware.cs):** Middleware encargado de atrapar cualquier excepción lanzada durante el procesamiento de solicitudes HTTP y retornar un JSON de error uniforme.
