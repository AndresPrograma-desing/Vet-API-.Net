# Carpeta: Services (Capa de Lógica de Negocio)

## Funcionalidad
La carpeta `Services` representa el motor de lógica y reglas de negocio del sistema. Es el intermediario crítico entre los controladores de la API y los repositorios de datos. Aquí se orquesta la toma de decisiones, validaciones de procesos veterinarios, cálculos financieros de facturación e integraciones del sistema.

## Cualidades y Características
- **Lógica de Negocio Centralizada:** Todo proceso que requiera validaciones lógicas complejas (ej: confirmar disponibilidad de horario para una cita, validar stock de productos veterinarios antes de dispensar, calcular impuestos o conversión de divisas) reside estrictamente en esta capa.
- **Desacoplamiento de Base de Datos:** Los servicios **no** interactúan directamente con `AppDbContext` (con excepciones muy puntuales y justificadas). En su lugar, consumen las abstracciones de los repositorios (`Interface/Repository`) que se inyectan en su constructor.
- **Manejo de Errores Orientado a Excepciones:** Para notificar violaciones a las reglas operativas, el servicio lanza excepciones del sistema fuertemente tipadas (ej: `KeyNotFoundException` para datos no encontrados, `InvalidOperationException` para faltas de stock o cruces de horarios). Los mensajes de estas excepciones se toman obligatoriamente de la clase estática `ResponseMessages` para evitar textos aleatorios.
- **Orquestación de Múltiples Servicios:** Permite coordinar llamadas cruzadas entre diferentes dominios. Por ejemplo, al registrar una consulta médica, el servicio interactúa con el inventario de productos, con el historial de precios y con el sistema de mensajería SignalR para notificar al cliente.

## Archivos Relevantes
- **[AuthService.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Services/AuthService.cs):** Implementa el hashing de claves, verificación de credenciales y generación del token JWT.
- **[CitasRequestService.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Services/CitasRequestService.cs):** Lógica para gestionar las solicitudes, confirmaciones, cancelaciones y recordatorios de citas.
- **[InvoiceService.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Services/InvoiceService.cs):** Realiza la lógica de emisión de facturas, cálculo de tasas en bolívares/dólares e inventariado.
- **[GeneratePdfService.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Services/GeneratePdfService.cs):** Utiliza QuestPDF para maquetar visualmente la factura digital.
