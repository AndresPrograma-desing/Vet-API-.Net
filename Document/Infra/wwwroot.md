# Carpeta: wwwroot (Recursos Estáticos Públicos)

## Funcionalidad
La carpeta `wwwroot` actúa como el directorio raíz físico para almacenar y servir archivos estáticos públicos en la aplicación ASP.NET Core. En este proyecto veterinario, su función clave es almacenar temporalmente los documentos físicos de facturas y reportes (PDF, Excel, Word) generados dinámicamente por la API para que puedan ser descargados por los usuarios desde la interfaz web.

## Cualidades y Características
- **Servidor de Archivos Estáticos:** Habilitado mediante `app.UseStaticFiles()` en el pipeline de `Program.cs`. Permite al frontend acceder directamente a un archivo (ej. `http://localhost:5168/facturas/factura_123.pdf`) mediante llamadas HTTP estándar.
- **Estructuración Organizada:** Los documentos generados se dividen en subcarpetas específicas según su naturaleza (ej. una subcarpeta para facturas PDF, otra para reportes de Excel o Word, y otra para recursos visuales estáticos).
- **Mapeo de Rutas Físicas:** Los servicios de documentos usan `IWebHostEnvironment` para resolver la ruta absoluta en disco de esta carpeta (usando `WebRootPath`) al momento de escribir y guardar los archivos binarios.
- **Depuración Automatizada:** Dado que la generación de archivos consume almacenamiento en disco de forma acumulativa, este directorio es monitoreado y depurado periódicamente por los Workers del sistema (`DeleteFacturaWorker` y `DeleteReportWorker`), que eliminan los archivos antiguos una vez superan el tiempo de vida parametrizado en la configuración.
