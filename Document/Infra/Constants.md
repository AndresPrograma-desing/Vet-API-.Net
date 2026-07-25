# Carpeta: Constants

## Funcionalidad
La carpeta `Constants` actúa como un registro único y centralizado para todos los valores constantes y estáticos no mutables de la aplicación. Su objetivo primordial es evitar el uso de cadenas de texto y valores numéricos mágicos (*magic values*) en toda la aplicación.

## Cualidades y Características
- **Centralización Completa:** Agrupa tres tipos de constantes clave: rutas de endpoints, mensajes de respuesta HTTP y variables de entorno/clave globales.
- **Internacionalización y Consistencia:** Todos los mensajes de respuesta del sistema hacia el cliente están centralizados en español, garantizando que el frontend reciba descripciones uniformes independientemente de qué servicio o controlador lance la respuesta.
- **Declaración Fuertemente Tipada de Rutas:** Permite a los controladores usar variables estáticas para las rutas de los métodos HTTP en lugar de strings literales directos.

## Archivos Relevantes
- **[EndpointRoutes.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Constants/EndpointRoutes.cs):** Define la estructura de rutas físicas de la API organizada por clases estáticas según la entidad (ej: `Endpoints.Cita.Create`).
- **[ResponseMessages.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Constants/ResponseMessages.cs):** Contiene la totalidad de los mensajes de error, éxito, advertencia e informativos en español que el API responde al cliente (ej: `ResponseMessagesCitas.NotFound`).
- **[Variables.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Constants/Variables.cs):** Define constantes generales y operativas reutilizables en la aplicación. Destaca la constante `TargetId` (con valor por defecto `1`), la cual se encarga de representar de forma unificada el identificador del registro único en la tabla de configuración de monedas (`MoneyTypes`). Es fundamental en el flujo cambiario del sistema, ya que tanto el servicio de conversión de divisas (`CurrencyService`) como los Workers de sincronización de tasa (`BcvWorker`) y el servicio `MoneyTypeService` la utilizan para validar, leer y actualizar de manera consistente la cotización del dólar (tasa BCV) registrada en la base de datos.

