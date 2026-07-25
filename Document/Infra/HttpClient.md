# Carpeta: HttpClient (Clientes de Mensajería Interna)

## Funcionalidad
La carpeta `HttpClient` define y expone clientes HTTP y WebSocket para realizar conexiones y envíos de datos en la red interna o servicios secundarios, relacionados principalmente con el sistema de mensajería del chat y datos de la API WebSocket (`WSMessageAPIData`).

## Cualidades y Características
- **Consumo Asíncrono Eficiente:** Diseñado para realizar peticiones no bloqueantes a servicios internos mediante el uso de `HttpClient` y Serialización JSON.
- **Desacoplamiento mediante Abstracción:** Define un contrato de interfaz (`IWSMessage`) para garantizar que la lógica de envío de mensajes de red sea fácilmente intercambiable y testeable mediante mocks.
- **Integridad en Tiempo Real:** Sirve de puente para que las llamadas que requieren registrar o sincronizar la mensajería utilicen la misma estructura y persistencia que los sockets principales de SignalR.

## Archivos Relevantes
- **[IWSMessage.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/HttpClient/IWSMessage.cs):** Contrato para el servicio cliente que interactúa con la lógica de mensajería WebSocket.
- **[WSMessage.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/HttpClient/WSMessage.cs):** Implementación de la interfaz `IWSMessage` encargada de procesar y despachar los paquetes de datos de la red de chat.
