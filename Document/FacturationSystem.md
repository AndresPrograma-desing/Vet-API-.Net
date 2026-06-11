## Backend Happy Pets — Sistema de facturación 

Este documento describe, en lenguaje claro, cómo el backend de Happy Pets genera facturas en formato `.docx`, cómo guarda y expone la URL del archivo en la base de datos, cómo y cuándo se eliminan esos archivos y qué debe hacer el frontend para mostrarlos o descargarlos.

**Resumen en una frase:** cuando pides generar una factura desde el frontend, el backend crea un `.docx`, lo guarda en `wwwroot/facturas`, persiste una URL en la tabla `facturas` y te devuelve esa URL para que la muestres o descargues; un proceso en segundo plano borra archivos viejos y marca la fila en la DB como "la factura se ha eliminado".

---

### 1) ¿Cómo se genera una factura?

- Inicio: el frontend hace una llamada al endpoint de generación (por ejemplo `POST /api/Facturas/cita/{citaId}/docx`).
- Preparación: el servicio `InvoiceService` prepara un DTO con los datos de la cita/consulta, cliente, mascota, ítems, totales y una `NumeroFactura` temporal si aún no existe.
- Nombre de archivo: el servicio que crea el `.docx` (`GenerateDocxService`) construye un nombre seguro y legible como `Factura_<Cliente>_<Mascota>_yyyyMMdd_HHmmss.docx` para evitar colisiones.
- Generación: `GenerateDocxService` crea el documento OpenXML y lo guarda en la carpeta `wwwroot/facturas` del servidor.
- Respuesta: el controlador devuelve al frontend el `file` (nombre) y la `url` pública `https://<host>/facturas/<filename>.docx` para que el frontend la use inmediatamente.

Archivos clave (código): [Controller/FacturasController.cs](Controller/FacturasController.cs), [Services/InvoiceService.cs](Services/InvoiceService.cs), [Services/GenerateDocxService.cs](Services/GenerateDocxService.cs).

---

### 2) ¿Cómo y cuándo se guarda la URL en la base de datos?

- Inmediatamente después de crear el `.docx`, el controlador compone la URL pública usando el host de la petición (`Request.Scheme` + `Request.Host`) y el nombre del archivo.
- Si ya existe una fila de `factura` para esa `consulta` (campo `ConsultaId`), el código actualiza esa fila: `factura.UrlDocx = <url>` y `factura.NumeroFactura = <numero>`; si no existe, crea una nueva fila con esos datos y los totales.
- Si la persistencia en la DB falla por alguna razón, el backend no hace fallar la generación: devuelve la URL en la respuesta y registra una advertencia. Aun así, es recomendable que el frontend guarde el `url` devuelto o valide mediante el endpoint GET.

Tabla y campos usados (ejemplo): `facturas.NumeroFactura`, `facturas.UrlDocx`, `facturas.ConsultaId`, `facturas.ClienteId`, `facturas.MascotaId`, `facturas.FechaEmision`, `facturas.Total`.

---

### 3) ¿Cómo obtiene el frontend la URL para mostrar/descargar la factura?

- Paso 1: `POST /api/Facturas/cita/{citaId}/docx` → respuesta `{ file: "Factura_...docx", url: "https://.../facturas/Factura_...docx" }`.
- Paso 2: el frontend muestra un botón o enlace directo con esa `url` (o abre en nueva pestaña) y/o llama `GET /api/Facturas/files/download/{fileName}` para forzar descarga.

Opción 2: consultar el estado
- `GET /api/Facturas/cita/{citaId}` devuelve un DTO de factura; el controlador intenta localizar en disco un `.docx` cuyo nombre contenga `NumeroFactura` y, si lo encuentra, rellena `dto.UrlDocx` con la URL pública.

Ejemplo de uso (frontend):
```
// Simplificado (fetch):
const resp = await fetch(`/api/Facturas/cita/${citaId}/docx`, { method: 'POST' });
const data = await resp.json();
window.open(data.url, '_blank'); // o crear enlace para descarga
```

Endpoint de descarga directo: `GET /api/Facturas/files/download/{fileName}` — devuelve el `PhysicalFile` con `content-type` correcto.

---

### 4) ¿Cómo y por qué la DB pasa a decir "la factura se ha eliminado"?

- Hay un worker en segundo plano llamado `DeleteFacturaWorker` que escanea `wwwroot/facturas` periódicamente.
- Lógica del worker:
	- Busca archivos `.docx` más antiguos que un umbral configurable (en desarrollo se incrementó a 30 minutos para evitar borrados prematuros).
	- Para cada archivo que borra, busca filas en `facturas` cuya `UrlDocx` coincida con ese nombre (por comparación o escape) y actualiza `factura.UrlDocx = "la factura se ha eliminado"`.
	- El worker también intenta notificar al personal (p. ej. secretaria) vía mensajes, pero si no encuentra usuarios objetivo solo registra el evento.

Efecto visible: cuando el worker borra un `.docx`, si el frontend pide `GET /api/Facturas/cita/{id}` encontrará en `UrlDocx` el texto literal "la factura se ha eliminado" en lugar de una URL válida; eso indica que el archivo ya no está disponible en disco.

### Flujo detallado paso a paso

1) Generación (POST /api/Facturas/cita/{citaId}/docx)

- Entrada: el frontend invoca `POST /api/Facturas/cita/{citaId}/docx`.
- Controlador: `FacturasController.GenerateDocxForCita(int citaId)`.
	- Llama a `_invoiceService.GenerateInvoiceForCitaAsync(citaId)` y obtiene un `FacturationDTO` (`dto`) con campos clave: `ConsultaId`, `ClienteId`, `ClienteNombre`, `MascotaId`, `MascotaNombre`, `Items` (lista de `FacturationItemDTO` con `Codigo`, `Nombre`, `Descripcion`, `Cantidad`, `PrecioUnitario`, `Total`), `Subtotal`, `Descuento`, `Total`, `FechaEmision`, `Notas`, `NumeroFactura` (puede ser temporal).
	- Calcula `consultaId = dto.ConsultaId ?? citaId`.
	- Consulta la DB: `facturaExistente = _db.Facturas.FirstOrDefaultAsync(f => f.ConsultaId == consultaId)`.
	- Decide `numeroParaUsar`: si existe `facturaExistente.NumeroFactura` no vacío, se reusa; si no, se genera uno nuevo del tipo `F-yyyyMMddHHmmss-{consultaId}` y se asigna a `dto.NumeroFactura`.
	- Llama a `_docxService.GenerateInvoiceDocx(dto, webRoot)` que devuelve `fileName` y guarda el `.docx` en `wwwroot/facturas/{fileName}`.
	- Compone la URL pública: `url = $"{Request.Scheme}://{Request.Host}/facturas/{Uri.EscapeDataString(fileName)}"`.
	- Persiste en la DB (dentro de un `try`):
		- Si `facturaExistente != null`: actualizar `facturaExistente.UrlDocx = url` y `facturaExistente.NumeroFactura = numeroParaUsar`; `_db.Facturas.Update(...)` + `await _db.SaveChangesAsync()`.
		- Si no existe: crea un nuevo `Factura` con `NumeroFactura`, `ClienteId`, `MascotaId`, `ConsultaId = consultaId`, `SecretariaId` (seleccionada según la cita/doctor/usuario por defecto), `FechaEmision = DateTime.UtcNow`, `Subtotal`, `Descuento`, `Total`, `MetodoPago`, `EstadoPago = "pendiente"`, `Notas`, `Creado`, `Actualizado`, `UrlDocx = url`; luego `Add` + `SaveChanges`.
	- Respuesta al frontend: `Ok(new { file = fileName, url })`.

2) Consulta/recuperación (GET /api/Facturas/cita/{citaId})

- Entrada: el frontend llama `GET /api/Facturas/cita/{citaId}` para obtener el DTO de la factura.
- Controlador: `FacturasController.GetInvoiceForCita(int citaId)`.
	- Llama a `_invoiceService.GenerateInvoiceForCitaAsync(citaId)` para obtener `dto` (que normalmente incluye `NumeroFactura` si existe en la DB).
	- Si `dto.NumeroFactura` no es null y existe la carpeta `wwwroot/facturas`, busca un archivo con patrón `*{dto.NumeroFactura}*.docx` usando `Directory.GetFiles(...)`.
	- Si encuentra coincidencia, asigna `dto.UrlDocx = $"{Request.Scheme}://{Request.Host}/facturas/{Uri.EscapeDataString(Path.GetFileName(match))}"` y devuelve el DTO.
	- Si no encuentra archivo pero la DB contiene `UrlDocx` con otro valor (por ejemplo el texto marcado por el worker), ese valor llegará en el DTO porque `InvoiceService` rellena el DTO a partir de la fila `facturas` cuando existe.

3) Worker de limpieza (DeleteFacturaWorker)

- Tipo: servicio hospedado en segundo plano (`IHostedService`) que ejecuta periódicamente una pasada.
- Ruta de trabajo: `var dir = Path.Combine(webRoot, "facturas")`.
- Umbral: calcula la antigüedad de cada `.docx` con `DateTime.UtcNow - File.GetCreationTimeUtc(file)`, y compara con `_threshold` (configurable; en desarrollo se ajustó a 30 minutos).
- Borrado: elimina el archivo (`File.Delete(path)`) si supera el umbral.
- Sincronización con la DB: tras borrar, busca filas en `facturas` cuya `UrlDocx` contenga el nombre del archivo (o su forma escapada) y actualiza cada fila con `factura.UrlDocx = "la factura se ha eliminado"`; finalmente `await _db.SaveChangesAsync()`.
- Notificaciones: intenta enviar mensajes a usuarios/secretarias avisando del borrado; si no encuentra destinatarios registra advertencias.

4) Persistencia y visibilidad en la DB

- Campos implicados: `facturas.NumeroFactura`, `facturas.UrlDocx`, `facturas.ConsultaId`, `facturas.ClienteId`, `facturas.MascotaId`, `facturas.FechaEmision`, `facturas.Subtotal`, `facturas.Total`, `facturas.MetodoPago`, `facturas.EstadoPago`, `facturas.Notas`, `facturas.Creado`, `facturas.Actualizado`.
- Casos típicos:
	- Generación exitosa y persistencia ok → la fila `facturas` contiene `NumeroFactura` y `UrlDocx` con la URL pública al archivo en `wwwroot/facturas`.
	- Archivo borrado por el worker → la fila `facturas.UrlDocx` queda con el literal `"la factura se ha eliminado"` (por diseño del worker) y el frontend verá ese texto si consulta la factura.

5) Relación `NumeroFactura` ↔ nombre de archivo

- `NumeroFactura` se usa como criterio para buscar el fichero físico: el `GenerateDocxService` concatena cliente+mascota+timestamp para `fileName`, y el controlador busca archivos cuyo nombre contenga `NumeroFactura` para rellenar `UrlDocx` en las consultas.

Archivo relevante: [Controller/FacturasController.cs](Controller/FacturasController.cs), [Services/InvoiceService.cs](Services/InvoiceService.cs), [Services/GenerateDocxService.cs](Services/GenerateDocxService.cs), [Worker/DeleteFacturaWorker.cs](Worker/DeleteFacturaWorker.cs).

Fin.
