# Módulo de Vacunas (Catálogo + Aplicación por Mascota)

Este documento describe en detalle cómo funciona el módulo de vacunación de **Happy Pets**, tanto en el backend (`vet-api-Net`) como en su integración con el frontend (`vet-system-WEB`). A diferencia de los documentos de `Document/Infra/*.md` que describen una carpeta completa, este documento describe un **dominio funcional** que atraviesa varias carpetas (`Models`, `DTOs`, `Controller`, `Services`, `Repository`, `Worker`).

---

## 1. Visión General: Dos Subdominios

El módulo se divide conceptualmente en dos partes independientes que comparten la misma tabla de vacunas:

```
┌───────────────────────────────┐        ┌──────────────────────────────────┐
│   1. CATÁLOGO (Vaccine)       │        │  2. APLICACIÓN (PetVaccination)   │
│                                │        │                                    │
│  - Qué vacunas existen        │──────► │  - Qué mascota recibió qué vacuna │
│  - A qué especie aplican      │        │  - Cuándo le toca la próxima dosis │
│  - Su esquema de refuerzos    │        │  - Semáforo de estado por vacuna   │
│  - Sus lotes disponibles      │        │  - Recordatorios y facturación     │
└───────────────────────────────┘        └──────────────────────────────────┘
```

- **Catálogo:** administrado por `VaccineController` — es información maestra ("existe una vacuna Rábica para caninos, cuesta $5, se refuerza cada 12 meses").
- **Aplicación:** administrada por `PetVaccinationController` — es el registro clínico real ("al perro Firulais se le aplicó la vacuna Rábica el 10/03/2026, con el lote L-2024-08, próxima dosis el 10/03/2027").

---

## 2. Modelo de Datos

```
Vaccine (catálogo maestro)
 ├──< VaccineSchemeStage   (esquema de dosis iniciales: 1ra, 2da, 3ra dosis...)
 ├──< VaccineBatch          (lotes físicos comprados, con stock y vencimiento)
 ├──> Producto               (opcional: vínculo con inventario/facturación)
 └──< PetVaccination         (aplicaciones reales a mascotas)
                               ├──> Mascota
                               ├──> VaccineBatch (lote usado)
                               ├──> Consulta (opcional)
                               ├──> Usuario (doctor, opcional)
                               └──> DetallesFactura (opcional, una vez facturada)
```

| Entidad | Campos clave | Notas |
|---|---|---|
| `Vaccine` | `Species`, `MinimumAgeWeeks`, `BoosterFrequencyValue/Unit`, `Price`, `ProductoId?`, `Active` | `BoosterFrequencyUnit` debe ser `"days"`, `"months"` o `"years"`. |
| `VaccineSchemeStage` | `VaccineId`, `StageType` (`"Initial"` o `"Booster"`), `DoseNumber`, `IntervalDaysFromPrevious` | Define el esquema de dosis **iniciales** (ej. cachorro: dosis 1, 2 y 3). Las etapas `"Booster"` existen en el modelo pero **no se usan** en ningún cálculo actual (ver sección 3.2). |
| `VaccineBatch` | `VaccineId`, `BatchNumber`, `ExpirationDate`, `QuantityInStock`, `Active` | `IsExpired` es un campo **calculado** al leer (`ExpirationDate <= hoy`), no se guarda en la base de datos. |
| `PetVaccination` | `MascotaId`, `VaccineId`, `VaccineBatchId?`, `ApplicationDate`, `NextDoseDate?`, `Status`, `PostponementReason?`, `DetalleFacturaId?`, `ReminderSevenSentAt?`, `ReminderThreeSentAt?` | `Status` es un string libre: `"Applied"`, `"Postponed"` (ver sección 3.2 para las transiciones reales que implementa el código). |

---

## 3. Backend: Lógica de Negocio

### 3.1 Catálogo — `VaccineController` → `VaccineService` (`api/Vaccine`, protegido con `[Authorize]`)

| Verbo | Ruta | Función |
|---|---|---|
| GET | `/Vaccine` | Lista paginada, filtra por `species`, `active`, `searchTerm`. |
| GET | `/Vaccine/{id}` | Detalle de una vacuna. |
| POST | `/Vaccine/create` | Crea vacuna. |
| PUT | `/Vaccine/{id}` | Edita vacuna. |
| DELETE | `/Vaccine/{id}` | **Elimina físicamente la fila** (`_context.Vaccines.Remove(...)`). |
| POST/DELETE | `/Vaccine/{id}/scheme-stages...` | Administra el esquema de dosis iniciales. |
| GET/POST | `/Vaccine/{id}/batches` | Lista/crea lotes de esa vacuna. |

**⚠️ Punto importante:** `DELETE /Vaccine/{id}` es un **hard delete**, no una desactivación. El campo `Active` solo se usa como filtro de catálogo (para no ofrecer vacunas "descontinuadas" al registrar una dosis) — para retirar una vacuna sin romper el historial de aplicaciones existentes, se debe usar `PUT` con `active: false` en vez de `DELETE`.

**Validaciones al crear/editar (`ValidateVaccineInput`):** `Name`/`Species` obligatorios, `MinimumAgeWeeks >= 0`, `BoosterFrequencyValue > 0`, `BoosterFrequencyUnit` ∈ {`days`, `months`, `years`}.

**Validación al crear un lote (`AddBatchAsync`):** `ExpirationDate` debe ser **estrictamente futura** respecto a hoy. No se valida `QuantityInStock` al crear (puede quedar en 0). Todo lote nuevo se crea con `Active = true` sin importar lo enviado en el DTO.

### 3.2 Aplicación — `PetVaccinationController` → `PetVaccinationService` (`api/PetVaccination`, protegido con `[Authorize]`)

#### a) Semáforo de estado — `GET /mascota/{id}/status`

Para cada vacuna activa cuya `Species` coincide con la especie de la mascota, se busca la **última dosis aplicada** (`Status == "Applied"`, más reciente por `ApplicationDate`) y se calcula:

```
¿Nunca se aplicó esa vacuna a esta mascota?
        │
        ├── SÍ ──────────────────────────────► 🔴 ROJO (Vencida/pendiente)
        │
        NO
        │
        ▼
¿La última aplicación no tiene NextDoseDate (esquema terminado)?
        │
        ├── SÍ ──────────────────────────────► 🟢 VERDE (al día)
        │
        NO
        │
        ▼
diasRestantes = NextDoseDate - hoy
        │
        ├── diasRestantes < 0 ───────────────► 🔴 ROJO (vencida)
        ├── 0 <= diasRestantes <= 30 ────────► 🟡 AMARILLO (próxima, ventana de 30 días)
        └── diasRestantes > 30 ──────────────► 🟢 VERDE (al día)
```

El umbral de 30 días es la constante `VaccinationVariables.AlertWindowDays`.

#### b) Registrar una dosis — `POST /register`

1. Valida que existan la `Mascota`, la `Vaccine` y el `VaccineBatch`, y que el lote pertenezca a esa vacuna.
2. Valida el lote: `Active == true`, `ExpirationDate > fecha_de_aplicación`, `QuantityInStock > 0` — cualquier falla lanza un error 400 específico (`BatchInactive`, `BatchExpired`, `BatchNoStock`).
3. Calcula la **próxima dosis** (`NextDoseDate`):
   - Si el frontend envía `next_dose_date_override`, se usa tal cual.
   - Si no, se resuelve automáticamente (`ResolveNextDoseDateAsync`):
     ```
     dosisAplicadas = cantidad de PetVaccination "Applied" previas (para esa mascota+vacuna)
     siguienteNumeroDosis = dosisAplicadas + 1

     ¿Existe un VaccineSchemeStage con StageType="Initial" y DoseNumber=siguienteNumeroDosis?
             │
             ├── SÍ ── NextDoseDate = fecha_aplicación + IntervalDaysFromPrevious (del stage)
             │
             └── NO ── NextDoseDate = fecha_aplicación + BoosterFrequencyValue/Unit (de la vacuna)
     ```
     Es decir: mientras la mascota esté completando el esquema de cachorro/inicial (dosis 1, 2, 3...), se usa el intervalo definido en `VaccineSchemeStage`. Una vez agotado el esquema inicial, cae automáticamente en la frecuencia de refuerzo general de la vacuna. Las etapas `"Booster"` del modelo **no se consultan nunca** en este cálculo.
4. Crea el `PetVaccination` con `Status = "Applied"`.
5. **Descuenta 1 unidad de `QuantityInStock` del lote usado.**
6. Guarda todo en una sola transacción.

#### c) Posponer — `PATCH /{id}/postpone`

- Requiere un `reason` no vacío.
- Si el registro ya está `Postponed`, lanza error (`AlreadyPostponed`) — **no se puede posponer dos veces**.
- Efecto: solo cambia `Status = "Postponed"` y guarda el motivo. La fecha de aplicación, dosis, lote y `NextDoseDate` **no se modifican**. No existe ningún endpoint para revertir una postergación a `"Applied"` — es una transición de un solo sentido en el código actual.

#### d) Enviar a factura — `POST /{id}/send-to-invoice`

- Si `DetalleFacturaId` ya tiene valor → error (`AlreadySentToInvoice`). Este campo es justamente el guardián contra doble facturación.
- Requiere que la vacuna tenga `ProductoId` vinculado (si no, error `VaccineNotLinkedToProduct`) y que el registro tenga `ConsultaId` (si no, error `NoConsultaLinked`).
- **No crea una factura nueva**: busca una `Factura` ya existente asociada a esa `ConsultaId`. Si no existe, lanza `FacturaNotFoundForConsulta` (la vacuna solo puede facturarse si la consulta ya fue facturada previamente).
- Crea un `DetallesFactura` (cantidad 1, precio = `Vaccine.Price`), suma el monto al `Subtotal`/`Total` de la `Factura`, y guarda el `DetalleFacturaId` en el `PetVaccination`.

#### e) Pendientes de la semana — `GET /pending-this-week`

Filtra `Status == "Applied" && NextDoseDate != null && NextDoseDate >= hoy && NextDoseDate <= hoy + 7 días` (ventana de **8 días incluyendo ambos extremos**, no es una semana calendario lunes-domingo). Admite `searchTerm` sobre nombre de mascota o de vacuna.

### 3.3 Carnet de Vacunación — `PdfController` (`GET /Pdf/mascota/{id}/carnet`)

Genera el PDF descargable con el historial completo de dosis de una mascota (vía `IVaccinationCertificatePdfUtilities`).

Los tres controladores del módulo (`VaccineController`, `PetVaccinationController`, `PdfController`) tienen `[Authorize]` a nivel de clase, igual que el resto de los controladores de negocio del sistema (`CitasController`, `ConsultasController`, etc.) — cualquier endpoint del módulo exige un usuario autenticado. *(Corregido: `VaccineController` y `PetVaccinationController` no tenían `[Authorize]` originalmente; se alinearon con el resto del sistema.)*

### 3.4 Worker de Recordatorios — `Worker/VaccinationReminderWorker.cs`

Se ejecuta en bucle según `WorkerSettings:VaccinationReminderWorker` de `appsettings.json` (por defecto cada 1 hora), pudiendo sobreescribirse en caliente desde una tabla `worker_configs` en base de datos.

```
En cada ejecución:
  hoy = fecha actual
  objetivo7 = hoy + 7 días
  objetivo3 = hoy + 3 días

  Busca PetVaccination donde Status="Applied" y:
    (NextDoseDate == objetivo7  Y  ReminderSevenSentAt es nulo)
      O
    (NextDoseDate == objetivo3  Y  ReminderThreeSentAt es nulo)

  Por cada resultado:
    - Envía email al cliente (si tiene correo) usando la plantilla "VaccinePets"
    - Envía notificación push interna al usuario "Admin" (para seguimiento telefónico)
    - Marca ReminderSevenSentAt o ReminderThreeSentAt = ahora (evita reenviar)
```

**⚠️ Limitación conocida:** la comparación es de **fecha exacta** (`NextDoseDate == objetivo`), no un rango. Si el worker está deshabilitado o caído justo el día en que una dosis queda a exactamente 7 o 3 días, ese recordatorio puntual se pierde para siempre (no hay una consulta de "recuperación" con rango de fechas).

---

## 4. Frontend (`vet-system-WEB`): Integración

### 4.1 Capa de API centralizada

Todas las llamadas HTTP viven en `src/services/api/index.js` (catálogo, lotes, aplicación) y `src/services/api/pdf/index.js` (`getVaccinationCarnetPdf`, descarga como `blob`). Ningún componente llama `fetch` directamente — regla de `GEMINI.md`.

### 4.2 Catálogo — `src/components/Vacccines/`

- **`Vaccines.jsx`** — enrutador según `location.state.view`: `'list'` (solo lectura), `'manage'` (CRUD), `'pending'` y `'batches'` (ver más abajo).
- **`VaccineList.jsx`** — listado de solo lectura con filtros (especie/estado/búsqueda).
- **`VaccineCreate.jsx`** — pantalla de gestión: tabla con acciones **Editar** y **Eliminar** por fila, y un `DrawPanel` compartido para alta/edición (mismo formulario, título y botón cambian dinámicamente según si `editingVaccine` está seteado).

### 4.3 Lotes — `src/components/PetVaccination/VaccineBatches/`

Página independiente ("Lotes de Vacunas", accesible por su propio ítem de menú): un selector de vacuna y, según la vacuna elegida, la tabla de lotes (`BatchesTable.jsx`) más un botón para registrar un nuevo lote (`BatchForm.jsx` dentro de un `DrawPanel`). Se separó del catálogo para poder gestionar el stock de lotes sin depender de abrir cada vacuna individualmente.

### 4.4 Aplicación por mascota — `src/components/PetVaccination/Index.jsx`

Este panel **se incrusta dentro de `HistorialMedico`** (pestaña "Vacunas" de la ficha de un paciente), reemplazando la vista anterior que solo mostraba una tabla de solo lectura. Muestra:

1. **Tarjetas de semáforo** (`status`, vía `GET /mascota/{id}/status`) — una por vacuna aplicable a la especie, coloreada Rojo/Amarillo/Verde según la sección 3.2.a.
2. **Tabla de historial** (`GET /mascota/{id}/history`) con acciones por fila: **Posponer** (abre `AlertModal` con campo de texto para el motivo) y **Enviar a Factura** (confirmación simple). Ambas reutilizan el mismo componente `AlertModal` ya existente en el proyecto (con `showInput` para el motivo), sin crear un modal nuevo.
3. **"Registrar Dosis"** — `DrawPanel` con `RegisterVaccinationForm.jsx`: selector de vacuna → selector de lote **dependiente** (se recargan los lotes vigentes de la vacuna elegida) → fecha de aplicación, peso, dosis, observaciones.
4. **"Descargar Carnet"** — llama a `getVaccinationCarnetPdf` y dispara la descarga del PDF en el navegador (mismo patrón que la credencial de usuario en `GenericPanel.jsx`).

### 4.5 Pendientes de la semana — `src/components/PetVaccination/PendingVaccinations.jsx`

Página con tabla paginada (`GET /pending-this-week`) mostrando mascota, cliente, teléfono (con copiado rápido vía `CopyText`) y próxima dosis — pensada para que el personal haga seguimiento telefónico.

### 4.6 Rutas y menús

No se crearon rutas nuevas en `App.jsx`: todo cuelga de la ruta ya existente `/AdminSystem/vaccines`, diferenciando la vista por `location.state.view` (`list`, `manage`, `pending`, `batches`). El menú lateral (`src/components/GenericPanel/Routes.jsx`) expone estas cuatro vistas dentro del submenú "Vacunas" para los tres roles operativos (admin, secretaria, doctor).

---

## 5. Flujo Completo de Ejemplo: Registrar una Dosis desde el Historial Clínico

```
[Doctor en HistorialMedico, mascota seleccionada]
        │
        │ Click "Registrar Dosis"
        ▼
[RegisterVaccinationForm] ── selecciona Vacuna ──► GET /Vaccine?active=true
        │                                                │
        │ selecciona Lote ──► GET /Vaccine/{id}/batches (filtra lotes no vencidos)
        │
        │ completa fecha/peso/dosis/observaciones
        │ click "Registrar"
        ▼
POST /PetVaccination/register  { mascota_id, vaccine_id, vaccine_batch_id, ... }
        │
        ▼
[PetVaccinationService.RegisterApplicationAsync]
        │
        ├─ valida mascota/vacuna/lote y reglas del lote (activo, no vencido, con stock)
        ├─ calcula NextDoseDate (esquema inicial o frecuencia de refuerzo)
        ├─ crea PetVaccination (Status="Applied")
        └─ descuenta 1 unidad de stock del lote
        │
        ▼
[Frontend] refresca estado (semáforo) e historial de la mascota
```

---

## 6. Puntos de Atención para Futuros Cambios

1. **`DELETE /Vaccine/{id}` es destructivo.** Si una vacuna ya tiene `PetVaccination` asociadas, eliminarla puede violar integridad referencial o borrar en cascada según la configuración de FK — para "retirar" una vacuna del catálogo sin riesgo, usar `PUT` con `active: false`.
2. ~~`VaccineController` y `PetVaccinationController` no requerían autenticación~~ — **ya corregido**, ambos tienen `[Authorize]` a nivel de clase.
3. **El estado `"Postponed"` no tiene retorno** en el backend actual — una vez pospuesta, una dosis no puede volver a `"Applied"` mediante la API existente.
4. **`SendToInvoiceAsync` no crea facturas**, solo agrega un ítem a una factura de consulta ya existente. Si se quiere facturar una vacuna aplicada fuera de una consulta con factura, el endpoint actual no lo permite.
5. **Los recordatorios por email/push son de fecha exacta** (7 y 3 días antes), sin ventana de recuperación — si el worker no corre exactamente ese día, ese aviso puntual no se reintenta.
6. **Las etapas de tipo `"Booster"` en `VaccineSchemeStage` no afectan ningún cálculo hoy** — solo las etapas `"Initial"` se usan para determinar la próxima dosis durante el esquema inicial; después de eso siempre se usa la frecuencia de refuerzo general de la vacuna.
