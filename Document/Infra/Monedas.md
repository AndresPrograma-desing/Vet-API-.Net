# Conversión de Monedas y Manejo de Tipo de Moneda (VES / USD)

Este documento describe cómo funciona **hoy** la lógica de conversión de moneda en **Happy Pets** (`vet-api-Net`). A diferencia de los documentos de `Document/Infra/*.md` que describen una carpeta completa, este describe un **dominio funcional** que atraviesa varias carpetas (`Models`, `DTOs`, `Interface`, `Utilities`, `Services`, `Worker`, `Controller`).

---

## 1. Idea Central: USD es la Moneda Base

Todo precio que se **guarda** en la base de datos (`Producto.Precio`, `Producto.PrecioVenta`, `Consulta.ConsultaPrice`, `Vaccine.Price`) está expresado en **dólares (USD)**, sin importar en qué moneda esté configurado el sistema para mostrarlo al usuario.

La moneda que ve el usuario final (Bs o USD) es una **preferencia de visualización global del sistema** (una sola fila en la tabla `MoneyTypes`), no un dato por producto ni por factura. La conversión USD → moneda activa se hace **al momento de leer/servir** los datos, nunca al guardarlos — el mismo patrón que ya vimos en `ProductService.GetProductsAsync` ([Services/ProductServices.cs:26-45](../../Services/ProductServices.cs#L26-L45)):

```
Base de datos (siempre USD) ──► Service lee el precio ──► ConvertPrice(precio, moneda_activa) ──► DTO que ve el frontend
```

---

## 2. Modelo de Datos: `MoneyType`

Una única tabla, pensada para tener **una sola fila activa**, identificada por la constante `Variables.TargetId = 1` ([Constants/Variables.cs:13](../../Constants/Variables.cs#L13)) y por `appsettings.json:BcvSettings:TargetId` (también `1` por defecto).

| Campo | Tipo | Significado |
|---|---|---|
| `Id` | `int` | Debe coincidir con `TargetId` (1). Si no coincide, `MoneyTypeService` lanza excepción — ver sección 6. |
| `MoneyName` | `string` | La moneda **activa para mostrar** al usuario: `"USD"` o `"VES"`. Es el switch que el admin cambia desde el frontend. |
| `BcvDollar` | `decimal` | La tasa del dólar (cuántos Bs equivalen a 1 USD). La actualiza el `BcvWorker` automáticamente, o el endpoint manual. |
| `DollarPersistence` | `string` | Siempre se fuerza a `"USD"` cada vez que se actualiza la tasa (ver `BcvWorker.cs:78,97` y `MoneyTypeService.UpdateBcvDollarPriceAsync:119`). No es un valor configurable por el usuario. |
| `Fecha` | `DateTime` | Fecha/hora de la última actualización de `BcvDollar`. |

Semillas iniciales (`Data/seeds/SeedsData.cs`): `MoneyName = apiSettings.USD` (o `"USD"`), `BcvDollar = 500.50m`, `DollarPersistence = "USD"`.

`ApiSettingsOptions` ([ApiSettings/ApiSettingsOptions.cs:16-17](../../ApiSettings/ApiSettingsOptions.cs#L16-L17)) define las dos cadenas que se comparan contra `MoneyName`: `USD = "USD"`, `VES = "VES"` (`appsettings.json:ApiSettings`). Estas constantes son las que usa todo el sistema para comparar `money.TypeMoney == _apiSettings.VES`, evitando strings mágicos repetidos.

---

## 3. Los Dos Métodos de Conversión — `ICurrencyUtilities`

Implementado en [Utilities/CurrencyUtilities.cs](../../Utilities/CurrencyUtilities.cs). Son **dos operaciones inversas**, y la causa más común de bugs en este sistema es confundir cuál usar:

### `ConvertPrice(precioEnUsd, money)` / `ConvertPriceAsync(precioEnUsd)`

**USD → moneda activa.** Recibe un precio que se sabe que está en USD (porque así se guarda siempre en la base de datos) y lo pasa a la moneda que el sistema tiene configurada para mostrar:

```csharp
if (money.TypeMoney == VES) return precioEnUsd * money.TasaBcv;  // USD -> Bs
return precioEnUsd;                                                // ya es USD, no toca nada
```

Se usa para **leer/mostrar** datos: listar productos, dashboard, PDFs de factura.

### `ConvertToUsdAsync(precioActual)`

**Moneda activa → USD.** Hace la operación inversa: recibe un precio que se asume expresado en la moneda **actualmente activa** del sistema y lo normaliza de vuelta a USD:

```csharp
if (money.TypeMoney == VES && money.TasaBcv > 0) return precioActual / money.TasaBcv;  // Bs -> USD
return precioActual;                                                                     // ya es USD
```

Se usa para **normalizar entradas del usuario antes de guardar**: si el admin edita el precio de un producto mientras el sistema muestra Bs, el número que llega del formulario está en Bs y hay que dividirlo por la tasa antes de persistirlo en `Producto.Precio` (que siempre debe quedar en USD). Ver `ProductServices.cs:120,126` (`UpdateProductAsync`).

**⚠️ Punto de confusión real (ya causó un bug en el dashboard, ver sección 6):** el nombre `ConvertToUsdAsync` sugiere "convierte a USD" en abstracto, pero en realidad asume que el valor de entrada **ya está en la moneda activa**, no en USD. Llamarlo sobre un valor que **ya es USD** (como los totales crudos de `Factura`) hace que lo divida por la tasa cuando el sistema está en Bs, arruinando el número. La regla mental correcta es: *`ConvertPrice`/`ConvertPriceAsync` para mostrar, `ConvertToUsdAsync` solo para normalizar lo que el usuario acaba de escribir en un formulario.*

### `GetActiveMoneyTypeAsync()`

Obtiene la fila activa de `MoneyType` (vía `MoneyTypeService.GetMoneyTypeAsync`) y lanza excepción si no existe o está corrupta (`TypeMoney == null && TasaBcv == 0`). Se llama una sola vez y se reutiliza en bucles (`ConvertPrice` síncrono) para no hacer una consulta a base de datos por cada fila — patrón usado en `ProductServices.cs:29` y en el fix reciente de `DashboardService.cs:43-53`.

Existe además un bloque de código muerto (`GetMoney()`, comentado) al final de `CurrencyUtilities.cs:76-88` — lectura directa por `IProductRepository.GetMoneyTypeByIdAsync`, ya no se usa; el camino real es siempre a través de `IMoneyTypeService`.

---

## 4. `MoneyTypeService` — Dueño de la Fila de Configuración

[Services/MoneyTypeService.cs](../../Services/MoneyTypeService.cs). Responsable exclusivo de leer/escribir la fila `MoneyTypes` (con `Id == TargetId`).

| Método | Uso |
|---|---|
| `GetMoneyTypeAsync()` | Lee la fila activa, valida `Id == TargetId`, devuelve `MoneyTypesDTO` con `TypeMoney = MoneyName.ToUpper()`. Es lo que consume `CurrencyUtilities` en cada conversión. |
| `UpdateMoneyTypeAsync(dto)` | Cambia únicamente `MoneyName` — es el switch Bs/USD que expone `PUT /Money/update` para el admin. **No toca `BcvDollar`.** |
| `GetTasaDollarBcvAsync()` | Busca la fila por `DollarPersistence == "USD"` (en la práctica, la misma fila `TargetId`) y devuelve la tasa formateada como string. Si `BcvDollar == 0`, devuelve un DTO de error controlado (`BcvFallen`) en vez de lanzar excepción. |
| `UpdateBcvDollarPriceAsync(price)` | Crea o actualiza la fila `TargetId` con una tasa nueva, fuerza `DollarPersistence = "USD"` y `Fecha = DateTime.Now`. La llama tanto el `BcvWorker` como el endpoint manual. |

**⚠️ Inconsistencia de namespace:** la clase `MoneyTypeService` está declarada bajo `namespace vet_api_Net.Interfaze.Services;` ([Services/MoneyTypeService.cs:8](../../Services/MoneyTypeService.cs#L8)) en vez de `vet_api_Net.Services`, violando la convención de namespaces del proyecto (`CLAUDE.md §4.1`: las implementaciones de Services van en `vet_api_Net.Services`, `Interfaze.Services` es solo para las interfaces). Funciona porque C# no obliga a que el namespace coincida con la carpeta física, pero es inconsistente con el resto del código y puede confundir a quien busque la clase por namespace.

---

## 5. Quién Actualiza la Tasa: `BcvWorker` + `BcvScraper`

[Worker/BcvWorker.cs](../../Worker/BcvWorker.cs) es un `BackgroundService` que corre en bucle infinito:

```
En cada ciclo:
  1. Lee BcvSettings de appsettings.json (WorkerEnabled, TargetId, CronExpression)
  2. Si WorkerEnabled = false → duerme 5 minutos y reintenta (no hace scraping)
  3. Llama a IBcvScraper.ObtenerPrecioBcvAsync() (scraping del portal bcv.org.ve, HttpService/BcvScraper.cs)
  4. Si el precio es válido (> 0):
       - Actualiza (o crea) la fila MoneyTypes[TargetId]: BcvDollar, DollarPersistence="USD", Fecha=ahora
       - Envía una notificación push al admin (éxito)
  5. Si el precio es inválido/nulo:
       - Envía notificación push al admin (fallo del scraper) — NO toca la base de datos
  6. Calcula el próximo retardo según CronExpression (appsettings: "0 9,17 * * *" por defecto —
     ojo: el appsettings.json actual del repo tiene "* 9/17 * * *", una sintaxis distinta)
  7. Duerme hasta la próxima ejecución programada
```

Importante: el `BcvWorker` **solo escribe `BcvDollar`**, nunca toca `MoneyName` — es decir, actualizar la tasa nunca cambia si el sistema muestra Bs o USD; eso solo lo cambia `PUT /Money/update` (acción manual del admin).

---

## 6. Dónde se Aplica la Conversión Hoy (por módulo)

| Módulo | Dirección | Detalle |
|---|---|---|
| `ProductService.GetProductsAsync` | USD → activa | Convierte `Precio`/`PrecioVenta` al listar productos ([ProductServices.cs:39-40](../../Services/ProductServices.cs#L39-L40)). |
| `ProductService.UpdateProductAsync` | activa → USD | Normaliza el precio editado por el admin antes de guardar (`ConvertToUsdAsync`, líneas 120 y 126). **`CreateProductAsync` NO hace esta normalización** — guarda `productDto.Precio`/`PrecioVenta` tal cual vengan del formulario, sin convertir. Es una inconsistencia real entre crear y editar: si el sistema está en Bs, un producto creado quedaría con un precio en Bs guardado como si fuera USD. |
| `InvoiceService.MapearItemsAsync` (usado por `AllFacturasAsync`, `ChangeStatusFacturaAsync`) | USD → activa | Recalcula los ítems de una factura para mostrarlos, convirtiendo `DetallesFactura.PrecioUnitario` y `Consulta.ConsultaPrice` en el momento de leer — **ignora por completo `Factura.Total` guardado** y reconstruye el total desde cero con la tasa actual. |
| `ConsultasService.ProcessInternalBillingAsync` | (ninguna) | Camino **real** de creación de `Factura`/`DetallesFactura` al facturar una consulta. Guarda `Total`/`PrecioUnitario` tomados directo de `Producto.PrecioVenta`/`Vaccine.Price`, **sin ninguna conversión** — quedan correctamente en USD. |
| `InvoiceService.GenerateInvoiceForCitaAsync` / `SaveOrUpdateFacturaPersistenceAsync` | USD → activa (y así se **persiste**) | Camino alternativo de creación de facturas (flujo de citas). A diferencia del anterior, sí convierte los precios a la moneda activa **antes de guardar** `Factura.Total`/`Subtotal`. **Esto es inconsistente** con `ProcessInternalBillingAsync`: si ambos caminos están en uso, unas facturas quedan en USD y otras en la moneda que estaba activa al momento de crearlas, sin ningún campo que registre cuál — ver el hallazgo de la sección 7. |
| `DashboardService.GetDashboardStatsAsync` | USD → activa (o cruda) | `SUM(Factura.Total)` del repositorio viene en USD. Si `useUsd=false` (default), convierte a la moneda activa con `ConvertPrice`; si `useUsd=true`, devuelve el crudo en USD sin tocar. **Corregido recientemente** — antes hacía la conversión al revés (ver sección 7). |

---

## 7. Puntos de Atención para Futuros Cambios

1. **Bug corregido — Dashboard usaba `ConvertToUsdAsync` al revés.** `DashboardService.cs` llamaba `ConvertToUsdAsync` sobre un total que **ya era USD** cuando `useUsd=true` (lo dividía de más si el sistema estaba en Bs), y no convertía nada cuando `useUsd=false` (dejaba el crudo en USD mal etiquetado como Bs). Se corrigió invirtiendo la lógica para usar `ConvertPrice` (USD → activa) cuando `useUsd=false`, y no tocar el valor cuando `useUsd=true`.

2. **Inconsistencia sin resolver — dos caminos de creación de `Factura` con distinta moneda de persistencia.** `ProcessInternalBillingAsync` (facturación de consultas) guarda `Factura.Total` en USD crudo; `GenerateInvoiceForCitaAsync`/`SaveOrUpdateFacturaPersistenceAsync` (flujo de citas) guarda el total ya convertido a la moneda activa en el momento de crear la factura. **`Factura` no tiene ninguna columna que registre en qué moneda quedó su `Total`.** Si el segundo camino está en uso real, cualquier suma agregada sobre `Factura.Total` (como hace el Dashboard) puede mezclar filas en USD con filas en Bs sin forma de distinguirlas. Pendiente de decidir: (a) alinear el segundo camino para que también guarde en USD crudo, o (b) agregar una columna `Moneda` a `Factura` y hacer que las agregaciones normalicen por fila en vez de sumar `Total` directo.

3. **`ProductService.CreateProductAsync` no normaliza el precio a USD**, a diferencia de `UpdateProductAsync`. Si se crea un producto mientras el sistema está en Bs, el precio ingresado (en Bs) queda guardado tal cual, como si fuera USD.

4. **Nombre engañoso de `ConvertToUsdAsync`.** No es un conversor genérico "a USD" — asume que la entrada está en la moneda **activa**, no en USD. Cualquier código nuevo que necesite pasar un valor de USD a la moneda activa debe usar `ConvertPrice`/`ConvertPriceAsync`, nunca `ConvertToUsdAsync`.

5. **Documentación desactualizada en otros archivos:** `Document/Infra/GLOBAL.md` y `Document/Infra/Utilities.md` referencian un archivo `Utilities/CurrencyService.cs` que ya no existe — la implementación actual es `Utilities/CurrencyUtilities.cs` (interfaz `ICurrencyUtilities`). Actualizar esas referencias si se retoca esta zona.

6. **Discrepancia menor en `BcvWorker`:** el comentario/fallback interno de `CalcularProximoRetardo` usa `"0 9,17 * * *"`, pero el `CronExpression` real en `appsettings.json:BcvSettings` es `"* 9/17 * * *"` — sintaxis de cron distinta (el fallback solo se usa si la expresión configurada falla al parsear).

7. **`GetTasaDollarBcvAsync` vs `GetMoneyTypeAsync` leen criterios distintos** (`DollarPersistence == "USD"` vs `Id == TargetId`) que hoy apuntan a la misma fila porque `DollarPersistence` siempre se fuerza a `"USD"` en cada escritura — pero son dos queries independientes que podrían divergir si esa invariante se rompe en el futuro (ej. una migración de datos que no respete la regla).
