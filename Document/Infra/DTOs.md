# Carpeta: DTOs (Data Transfer Objects)

## Funcionalidad
La carpeta `DTOs` contiene los objetos de transferencia de datos planos de la API. Se encargan de definir el formato de entrada (payloads de peticiones) y salida (respuestas JSON) entre el frontend y el backend, evitando la exposición directa de las entidades físicas de la base de datos (Models).

## Cualidades y Características
- **Uso de `record` para Inmutabilidad:** Los DTOs modernos del sistema se definen usando la palabra clave `record` en lugar de `class`, proporcionando inmutabilidad por defecto y comparación por valor.
- **Formateo de Serialización:** Cada propiedad está decorada con el atributo `[JsonPropertyName("snake_case")]`. Esto asegura que la API serialice sus campos en formato `snake_case` para el frontend en JavaScript/React, mientras mantiene la convención `PascalCase` en el código de C#.
- **Prevención de Ataques de Over-posting:** Al requerir DTOs de entrada específicos (ej: `CreateCitaDTO`), se impide que clientes maliciosos actualicen columnas protegidas o de auditoría.
- **Protección de Datos Sensibles:** Permite omitir campos internos de la base de datos, contraseñas hasheadas o relaciones cíclicas complejas al construir la respuesta.
- **Valores por Defecto:** Se inicializan los strings con `string.Empty` y las listas con constructores vacíos (`new()`), reduciendo la posibilidad de excepciones de referencia nula (`NullReferenceException`).

## Archivos Relevantes
- **[CreateCitaDTO.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/DTOs/CreateCitaDTO.cs):** Contiene los datos requeridos para registrar una cita.
- **[UserLoginRequest.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/DTOs/UserLoginRequest.cs):** Payload simple de credenciales para iniciar sesión.
- **[FacturaResponseDTO.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/DTOs/FacturaResponseDTO.cs):** Define la estructura de respuesta de las facturas enviadas al cliente.
