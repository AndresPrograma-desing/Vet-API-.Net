# Carpeta: HttpService (Servicios HTTP Externos)

## Funcionalidad
La carpeta `HttpService` agrupa los servicios y scrapers responsables de la comunicación saliente con servidores de terceros y APIs externas. Su tarea es obtener datos externos (tasas de cambio financiero) y despachar servicios transaccionales (envío de correos electrónicos).

## Cualidades y Características
- **Aislamiento de Servicios de Terceros:** Encapsula las dependencias externas (como el API de Resend para correos o peticiones de web scraping al BCV). De este modo, cambios en las APIs externas no afectan la lógica de negocio principal.
- **Web Scraping Financiero:** Extrae en tiempo real la tasa del dólar del Banco Central de Venezuela a través de un parseador HTTP (`BcvScraper`), asegurando que la veterinaria maneje precios convertidos correctamente según la cotización oficial del día.
- **Envío de Correos Transaccionales:** Integra la plataforma **Resend** para mandar correos con formato rico en HTML empleando las plantillas de la aplicación.
- **Estructura Asíncrona Robusta:** Los métodos devuelven tareas asíncronas (`Task`), gestionando tiempos de espera (*timeouts*) y manejo de errores de conexión de red de forma independiente.

## Archivos Relevantes
- **[BcvScraper.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/HttpService/BcvScraper.cs):** Realiza la lectura y extracción de la tasa cambiaria del portal web oficial del BCV.
- **[ResendEmailService.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/HttpService/ResendEmailService.cs):** Envía correos electrónicos transaccionales usando la API de Resend.
- **[NowGrod.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/HttpService/NowGrod.cs):** Utilidades adicionales de integración de servicios y logs del sistema.
