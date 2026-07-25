# Carpeta: Controller (Controladores)

## Funcionalidad
La carpeta `Controller` contiene los controladores de la API (Capa HTTP). Su responsabilidad es exponer los puntos de entrada (endpoints) REST de la aplicación veterinaria para ser consumidos por el cliente frontend (React/Vite).

## Cualidades y Características
- **Separación de Responsabilidades (SOC):** Los controladores únicamente gestionan la recepción de la petición HTTP, validación básica del modelo entrante, delegación al servicio correspondiente (`Services`) y retorno del código de estado HTTP adecuado (`Ok`, `Created`, `BadRequest`, `NotFound`, `InternalServerError`).
- **NUNCA acceso a Base de Datos:** Los controladores no interactúan directamente con `AppDbContext` ni contienen lógica de negocio. Toda la orquestación reside en los servicios.
- **Seguridad (Autenticación y Autorización):** Muchos controladores y endpoints están decorados con atributos `[Authorize]` y políticas de roles para proteger la información confidencial de la veterinaria.
- **Gestión Estándar de Rutas y Respuestas:** Consumen la clase `Endpoints` para establecer la propiedad de ruta en los métodos HTTP y `ResponseMessages` para dar formato a las respuestas HTTP.
- **Estructura de Errores Unificada:** Implementan bloques `try-catch` para capturar excepciones de lógica de negocio específicas (ej. `ArgumentException`, `InvalidOperationException`, `KeyNotFoundException`) lanzadas por los servicios y devolver respuestas estructuradas en formato JSON (`{ error = "..." }` o `{ message = "..." }`).

## Archivos Relevantes
- **[AuthController.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Controller/AuthController.cs):** Gestiona el inicio de sesión, registro de nuevos usuarios y tokens JWT de desarrollo.
- **[CitasController.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Controller/CitasController.cs):** Expone endpoints para la administración, aceptación y reprogramación de citas veterinarias.
- **[FacturasController.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Controller/FacturasController.cs):** Provee endpoints para la consulta de facturación y descargas de PDF.
