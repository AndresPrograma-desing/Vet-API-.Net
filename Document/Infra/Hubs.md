# Carpeta: Hubs (Comunicación en Tiempo Real - SignalR)

## Funcionalidad
La carpeta `Hubs` contiene los centros de comunicación en tiempo real basados en **ASP.NET Core SignalR**. Estos hubs actúan como servidores de WebSockets que permiten establecer flujos bidireccionales inmediatos de datos entre el servidor API y los clientes conectados en el navegador.

## Cualidades y Características
- **WebSockets de Baja Latencia:** Proporciona actualización en tiempo real sin requerir que el cliente realice solicitudes periódicas (*polling*), reduciendo la carga del servidor.
- **Mensajería Interna Directa:** El `MessageHub` permite chatear en tiempo real a los administradores, veterinarios y personal técnico, registrando las conversaciones de manera asíncrona.
- **Notificaciones Segmentadas:** El hub `NotificactionsPush` gestiona dinámicamente las conexiones entrantes asociándolas con el ID del usuario (`userId`) y su rol. Esto permite enviar alertas críticas directas a un usuario específico (ej: "Tu cita ha sido reprogramada") o a un rol completo (ej: "Nueva cita pendiente de aprobación para los doctores").
- **Mapeo Dinámico de Conexiones:** Mantiene un diccionario en memoria o base de datos que asocia las múltiples conexiones abiertas de un mismo usuario con su perfil, garantizando la entrega del mensaje en cualquier pestaña o dispositivo del usuario.

## Archivos Relevantes
- **[MessageHub.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Hubs/MessageHub.cs):** Administra el chat interactivo en tiempo real entre los empleados de la clínica veterinaria.
- **[NotificactionsPush.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Hubs/NotificactionsPush.cs):** Gestiona la conexión de usuarios para recibir notificaciones push en tiempo real según eventos del sistema.
