# Visión General del Sistema Happy Pets (API + WEB)

> **Para quién es este documento:** para alguien que nunca tocó este código y necesita entender, en poco tiempo, qué hace el sistema y cómo viaja la información entre el frontend y el backend. No es referencia exhaustiva — para eso están `BACKEND_DOCUMENTATION.md` (vet-api-Net) y `FRONTEND_STRUCTURE.md` (vet-system-WEB), y el resto de `Document/Infra/`.

---

## 1. Qué es Happy Pets

Happy Pets es el sistema de gestión de una clínica veterinaria, dividido en dos proyectos separados que se comunican por HTTP/JSON:

- **`vet-api-Net`** — el backend. API REST en ASP.NET Core (.NET 10) que contiene toda la lógica de negocio y la base de datos.
- **`vet-system-WEB`** — el frontend. Aplicación React (Vite) que consume esa API y la presenta como un panel según el rol del usuario.

No hay lógica de negocio real en el frontend: calcula, valida formularios y pinta pantallas, pero las reglas (¿se puede agendar esta cita?, ¿hay stock?, ¿cuánto es la factura?) viven siempre en el backend.

### Roles del sistema
- **Administrador:** usuarios, métricas financieras, inventario, configuración global.
- **Doctor:** agenda, consultas médicas, recetas, historial clínico, mensajería.
- **Secretaria:** agenda, alta rápida de clientes/mascotas, facturación, mensajería.
- **Portal de Clientes** (`/portal-cliente`): acceso público para que el dueño de la mascota consulte su información, sin rol operativo.

---

## 2. Las dos piezas y cómo se hablan

```
┌────────────────────────┐        HTTP (fetch) / JSON        ┌────────────────────────────┐
│   vet-system-WEB        │ ───────────────────────────────▶ │       vet-api-Net            │
│   (React + Vite)        │                                   │   (ASP.NET Core API)         │
│   http://localhost:5173 │ ◀─────────────────────────────── │   http://localhost:5168       │
└────────────────────────┘      respuesta JSON / archivo      └────────────────────────────┘
                                                                        │
                                                                        ▼
                                                               PostgreSQL / MySQL
```

Puntos clave de esa comunicación:

- **Un único cliente HTTP en el frontend:** todas las llamadas pasan por `request()` en [client.js](../../../src/services/api/client.js) *(vet-system-WEB/src/services/api/client.js)*. Está prohibido usar `fetch` directo en componentes — siempre se agrega una función nueva en `src/services/api/index.js` y se consume desde un hook.
- **Autenticación JWT:** al iniciar sesión, el backend devuelve un token. El frontend lo guarda (sessionStorage + localStorage, vía `storage.js`) y `client.js` lo inyecta como header `Authorization: Bearer ...` en cada request. Si el backend responde `401`, el cliente dispara `logout()` automáticamente y redirige al login.
- **Mismo contrato de datos, dos idiomas:** el backend modela el dominio en **español** (`Mascota`, `Cita`, `Factura`) pero serializa los DTOs en **snake_case** vía `[JsonPropertyName]`, para que el JSON que llega al frontend use la convención estándar de JavaScript. El frontend, además, tolera inconsistencias heredadas del backend (`rol`/`role`, `contenido`/`content`) normalizándolas en la capa de servicios/hooks, no en los componentes.
- **Tiempo real:** mensajería y notificaciones usan SignalR (WebSockets) en vez de polling puro, aunque varias pantallas (citas, facturación) también refrescan por **polling** periódico (`usePolling.js`).
- **Modo sin backend (Dummy Mode):** el frontend puede simular todas las respuestas desde `src/dummy/db.json` sin tocar la red — útil para desarrollar la UI sin levantar la API. Se activa con una clave en `localStorage` (`USE_DUMMY_DATA`).

---

## 3. El flujo dentro de cada lado

### 3.1 Backend: 3 capas estrictas

```
Controller  →  Service  →  Repository  →  AppDbContext (EF Core)  →  Base de datos
```

- **Controller:** recibe el HTTP request, lee rutas desde `Constants/EndpointRoutes.cs` (nunca strings sueltos), delega al Service, traduce el resultado o la excepción a un código HTTP. Nunca toca la base de datos ni contiene reglas de negocio.
- **Service:** toda la lógica de negocio (¿el horario está libre?, ¿hay stock?, ¿cómo se calcula la factura?). Si algo viola una regla, lanza una excepción tipada (`ArgumentException`, `InvalidOperationException`, etc.) que el Controller atrapa.
- **Repository:** el único lugar que conoce `AppDbContext`. Ahí van los `Where`/`OrderBy`/`Skip`/`Take` reales sobre `IQueryable`, para que EF Core los traduzca a SQL en vez de traer toda la tabla a memoria.

### 3.1.1 Cómo se consulta realmente la base: el ORM

El Repository nunca escribe SQL a mano. Usa **Entity Framework Core**, que es un **ORM** (*Object-Relational Mapper*): una capa que traduce código C# (clases, propiedades, métodos LINQ) a sentencias SQL, y traduce las filas que vuelven de la base de datos en objetos C# otra vez. La idea es no escribir `SELECT ... FROM ... WHERE ...` como texto suelto, sino construir la consulta encadenando métodos sobre una colección (`IQueryable<T>`), y que EF Core decida el SQL exacto según el motor de base configurado (PostgreSQL vía Npgsql, o MySQL vía Pomelo — ver `DatabaseSettings:Provider`).

Ejemplo real, el mismo que arma la lista de citas (`CitaRepository.GetPagedWithRelationsAsync`):

```csharp
// vet-api-Net/Repository/CitaRepository.cs
var query = _context.Citas
    .Include(c => c.Mascota).ThenInclude(m => m.Cliente)
    .Include(c => c.Mascota).ThenInclude(m => m.Especie)
    .Include(c => c.Doctor)
    .Include(c => c.Secretaria)
    .Include(c => c.MetodoPago)
    .AsQueryable();

if (doctorId.HasValue)
    query = query.Where(c => c.DoctorId == doctorId.Value);

if (!allDates)
{
    var targetDate = (date ?? DateTime.Today).Date;
    query = query.Where(c => c.FechaCita.Date == targetDate);
}

if (!string.IsNullOrWhiteSpace(status))
    query = query.Where(c => c.Estado != null && c.Estado.ToLower() == status.ToLower());

return await query
    .OrderByDescending(c => c.FechaCita)
    .ThenByDescending(c => c.HoraCita)
    .ToPagedResultAsync(pageNumber, pageSize); // aplica Skip/Take internamente
```

Lo que pasa paso a paso:

1. **`_context.Citas`** es un `DbSet<Cita>` dentro de `AppDbContext` — EF Core lo mapea a la tabla `Citas` de la base de datos. Ni esta línea ni las siguientes tocan la base todavía: solo están **armando** la consulta en memoria (por eso el tipo es `IQueryable<Cita>`, no una lista real).
2. **`.Include()` / `.ThenInclude()`** le dicen a EF Core que, además de la cita, traiga de una sola vez sus relaciones (mascota, dueño, especie, doctor, secretaria, método de pago) — en SQL esto se traduce en varios `JOIN` o consultas relacionadas, evitando que el código tenga que pedir cada relación por separado (lo que generaría decenas de queries sueltas, el problema clásico de "N+1 queries").
3. **`.Where(...)` encadenado condicionalmente:** cada `if` agrega un filtro solo si aplica. Como la consulta todavía no se ejecutó, EF Core combina todos los `Where` en una sola cláusula `WHERE ... AND ...` del SQL final — no se ejecutan filtros por separado.
4. **`.OrderByDescending(...).ThenByDescending(...)`** se traduce a `ORDER BY fecha_cita DESC, hora_cita DESC`.
5. **`ToPagedResultAsync(pageNumber, pageSize)`** (helper propio del proyecto) aplica `.Skip((pageNumber - 1) * pageSize).Take(pageSize)` — es decir, `OFFSET ... LIMIT ...` en SQL — y además calcula el total de filas con `COUNT(*)` para poder armar la paginación. Recién en este punto, al llamar `.ToListAsync()` (dentro del helper) o algo equivalente, EF Core **ejecuta** la consulta contra la base.

El SQL que efectivamente se dispara contra PostgreSQL es, conceptualmente, algo así (simplificado):

```sql
SELECT c.*, m.*, cl.*, e.*, d.*, s.*, mp.*
FROM "Citas" c
LEFT JOIN "Mascotas" m ON m."Id" = c."MascotaId"
LEFT JOIN "Clientes" cl ON cl."Id" = m."ClienteId"
LEFT JOIN "Especies" e ON e."Id" = m."EspecieId"
LEFT JOIN "Usuarios" d ON d."Id" = c."DoctorId"
LEFT JOIN "Usuarios" s ON s."Id" = c."SecretariaId"
LEFT JOIN "MetodosPago" mp ON mp."Id" = c."MetodoPagoId"
WHERE c."DoctorId" = @doctorId
  AND c."Estado" ILIKE @status
ORDER BY c."FechaCita" DESC, c."HoraCita" DESC
OFFSET @skip LIMIT @pageSize;
```

**Por qué esto va en el Repository y no en el Service:** mientras la consulta sea un `IQueryable` (no se haya "materializado" con `.ToListAsync()`, `.FirstOrDefaultAsync()`, etc.), todo el filtrado, orden y paginación se resuelve en la base de datos. Si ese mismo filtrado se hiciera en el Service después de un `GetAllAsync()` que ya trajo todas las filas a memoria (una `List<Cita>`), EF Core ya no tiene nada que traducir a SQL: se estaría descargando toda la tabla para filtrarla en C#, mucho más lento y memoria-intensivo a medida que crece la tabla. Esta es la razón detrás de la regla del proyecto: *"Where/OrderBy/Skip/Take siempre en el Repository sobre IQueryable, nunca en el Service sobre una lista ya materializada"* (ver `CLAUDE.md` sección 4.9).

Las migraciones (`Migrations/`) son la otra mitad del ORM: en vez de escribir `CREATE TABLE`/`ALTER TABLE` a mano, EF Core genera ese SQL automáticamente a partir de los cambios en las clases de `Models/`, cuando se corre `dotnet ef migrations add` + `dotnet ef database update`.

### 3.1.2 El mismo ejemplo, sin ORM (SQL directo con ADO.NET)

Para que quede claro qué trabajo hace el ORM por detrás, así se vería la misma consulta (citas filtradas por doctor/estado, paginadas, con sus relaciones) si se escribiera **sin EF Core**, usando ADO.NET puro (`NpgsqlCommand`) — tal como se hacía antes de que existieran los ORMs:

```csharp
// Equivalente SIN ORM — nadie en este proyecto escribe código así, es solo referencia
using var connection = new NpgsqlConnection(connectionString);
await connection.OpenAsync();

var sql = @"
    SELECT c.""Id"", c.""FechaCita"", c.""HoraCita"", c.""Estado"",
           m.""Id"" AS mascota_id, m.""Nombre"" AS mascota_nombre,
           cl.""Nombre"" AS cliente_nombre, cl.""Apellido"" AS cliente_apellido,
           d.""Nombre"" AS doctor_nombre
    FROM ""Citas"" c
    LEFT JOIN ""Mascotas"" m ON m.""Id"" = c.""MascotaId""
    LEFT JOIN ""Clientes"" cl ON cl.""Id"" = m.""ClienteId""
    LEFT JOIN ""Usuarios"" d ON d.""Id"" = c.""DoctorId""
    WHERE (@doctorId IS NULL OR c.""DoctorId"" = @doctorId)
      AND (@status IS NULL OR LOWER(c.""Estado"") = LOWER(@status))
    ORDER BY c.""FechaCita"" DESC, c.""HoraCita"" DESC
    OFFSET @skip LIMIT @pageSize;
";

using var command = new NpgsqlCommand(sql, connection);
command.Parameters.AddWithValue("doctorId", doctorId.HasValue ? (object)doctorId.Value : DBNull.Value);
command.Parameters.AddWithValue("status", status != null ? (object)status : DBNull.Value);
command.Parameters.AddWithValue("skip", (pageNumber - 1) * pageSize);
command.Parameters.AddWithValue("pageSize", pageSize);

var citas = new List<Cita>();
using var reader = await command.ExecuteReaderAsync();
while (await reader.ReadAsync())
{
    // Hay que mapear cada columna a mano, campo por campo
    citas.Add(new Cita
    {
        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        FechaCita = reader.GetDateTime(reader.GetOrdinal("FechaCita")),
        Estado = reader.IsDBNull(reader.GetOrdinal("Estado")) ? null : reader.GetString(reader.GetOrdinal("Estado")),
        Mascota = new Mascota
        {
            Id = reader.GetInt32(reader.GetOrdinal("mascota_id")),
            Nombre = reader.GetString(reader.GetOrdinal("mascota_nombre")),
            Cliente = new Cliente
            {
                Nombre = reader.GetString(reader.GetOrdinal("cliente_nombre")),
                Apellido = reader.GetString(reader.GetOrdinal("cliente_apellido")),
            }
        },
        Doctor = new Usuario { Nombre = reader.GetString(reader.GetOrdinal("doctor_nombre")) }
    });
}
// Y aparte, otra query completa solo para el COUNT(*) del total...
```

**Diferencias claves frente a la versión con EF Core (`CitaRepository.GetPagedWithRelationsAsync`):**

| | Sin ORM (ADO.NET) | Con ORM (EF Core, lo que usa el proyecto) |
|---|---|---|
| El SQL | Se escribe a mano, como texto (`string sql = "SELECT..."`) | Lo genera EF Core a partir de `.Where()`, `.Include()`, `.OrderBy()` en C# |
| Mapeo fila → objeto | Manual, columna por columna (`reader.GetInt32(...)`) | Automático: EF Core devuelve directamente `List<Cita>` con sus relaciones ya pobladas |
| Relaciones (JOIN) | Hay que escribir los `JOIN` y mapear cada tabla a mano | `.Include(c => c.Mascota).ThenInclude(m => m.Cliente)` — EF Core arma el `JOIN` solo |
| Cambiar de motor de BD (Postgres ↔ MySQL) | Hay que revisar sintaxis SQL específica de cada motor | Automático: EF Core adapta el SQL generado según el proveedor configurado (`Npgsql` vs `Pomelo`) |
| Riesgo de SQL injection | Alto si se concatenan strings en vez de usar parámetros (acá se evitó con `Parameters.AddWithValue`, pero hay que acordarse siempre) | Bajo por diseño: LINQ nunca concatena texto, siempre parametriza internamente |
| Cantidad de código | Mucho más código repetitivo por cada entidad nueva | Una vez definido el `Model` y la relación en `AppDbContext`, el resto es una línea de LINQ |

En otras palabras: el ORM no es "magia", es una capa que automatiza exactamente el trabajo mostrado arriba (escribir el SQL, enviarlo, leer el resultado, mapearlo a objetos) para no repetirlo a mano en cada Repository.

### 3.2 Frontend: hook → servicio API → componente

```
Componente (JSX)  →  Hook (src/hooks/*.js)  →  src/services/api/index.js  →  client.js  →  (red)
```

- El **componente** solo pinta UI y delega el fetch de datos a un **hook**.
- El **hook** (ej. `useGetCitas`) maneja estado de carga/error y llama a una función de **servicio** (ej. `getCitas()`).
- La función de **servicio**, en `src/services/api/index.js`, sabe el endpoint exacto y los query params, pero delega el fetch real a `request()` de `client.js`.
- `client.js` es la única pieza que sabe de tokens, prefijos de ruta (`/api/...`), manejo de 401 y parseo de la respuesta (JSON, blob para PDFs, etc.).

---

## 4. Ejemplo end-to-end: listar las citas de hoy en el panel

Para que el flujo no quede abstracto, se sigue un caso real del código: **la secretaria abre su agenda y la pantalla pide la lista de citas**.

### Paso 1 — El componente dispara el hook
El hook `useGetCitas` (usado, entre otros, por las pantallas de citas) se encarga de pedir y normalizar los datos:

```js
// vet-system-WEB/src/hooks/GetCitas.js
const res = await api.getCitas({
  allDates: true,
  pageNumber,
  pageSize
});
```

### Paso 2 — El servicio API arma el endpoint
`getCitas()` traduce esos parámetros a query string sobre la ruta `/Citas/requests`:

```js
// vet-system-WEB/src/services/api/index.js
export async function getCitas(params = {}) {
  const query = new URLSearchParams();
  if (params.status) query.append('status', params.status);
  if (params.date) query.append('date', params.date);
  if (params.allDates !== undefined) query.append('allDates', params.allDates);
  if (params.pageNumber) query.append('pageNumber', params.pageNumber);
  if (params.pageSize) query.append('pageSize', params.pageSize);

  const queryString = query.toString();
  const url = queryString ? `/Citas/requests?${queryString}` : '/Citas/requests';
  return request(url, { method: 'GET' });
}
```

### Paso 3 — `client.js` hace el request real
`request()` agrega el prefijo `/api`, inyecta el token JWT guardado en `storage.js`, y hace el `fetch`:

```js
// vet-system-WEB/src/services/api/client.js
const token = getStorageValue(TOKEN_KEY);
if (token) {
  opts.headers['Authorization'] = `Bearer ${token}`;
}
...
res = await fetch(url, opts);
```

Esa petición llega al backend como `GET /api/Citas/requests?allDates=true&pageNumber=1&pageSize=10`.

### Paso 4 — El Controller recibe la petición
`CitasController` (protegido con `[Authorize]`) lee el rol y el id del usuario desde el propio token JWT, y delega al Service:

```csharp
// vet-api-Net/Controller/CitasController.cs
[HttpGet(Endpoints.Citas.GetAllRequests)]
public async Task<ActionResult<CitasListResponseDTO>> GetAllCitasRequests(
    [FromQuery] string? status = null,
    [FromQuery] DateTime? date = null,
    [FromQuery] bool allDates = true,
    ...)
{
    var role = User.FindFirst(ClaimTypes.Role)?.Value;
    int? currentUserId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var parsedId) ? parsedId : null;

    var (items, totalCount) = await _citasRequestService.GetAllCitasRequestsAsync(
        status, date, allDates, role, currentUserId, doctorId, secretariaId, pageNumber, pageSize);

    return Ok(new CitasListResponseDTO { Items = items, TotalCount = totalCount });
}
```

### Paso 5 — El Service aplica las reglas de negocio
`CitasRequestService` no solo pide los datos: primero resuelve qué puede ver ese usuario según su rol (`IUserScopeResolver`), y de paso marca automáticamente como "no asistida" cualquier cita vencida antes de devolver la lista:

```csharp
// vet-api-Net/Services/CitasRequestService.cs
var (doctorId, secretariaId) = _userScopeResolver.ResolveDoctorSecretariaScope(
    currentUserRole, currentUserId, requestedDoctorId, requestedSecretariaId);

var expirableAppointments = await _citasRepository.GetActiveForExpirationCheckAsync(doctorId, secretariaId);
foreach (var a in expirableAppointments)
{
    if (appointmentDateTime < now)
    {
        a.Estado = Status.NotAssisted;
        _citasRepository.Update(a);
        anyExpired = true;
    }
}
if (anyExpired) await _citasRepository.SaveChangesAsync();

var (appointments, totalCount) = await _citasRepository.GetPagedWithRelationsAsync(
    doctorId, secretariaId, date, allDates, status, pageNumber, pageSize);
```

Esto es lógica de negocio real: una secretaria o doctor nunca ve "pendiente" una cita cuya hora ya pasó, sin que nadie la haya marcado a mano.

### Paso 6 — El Repository consulta la base
`GetPagedWithRelationsAsync` es quien realmente construye el `Where`/`Skip`/`Take` sobre `AppDbContext` y lo traduce a SQL — el Service nunca toca EF Core directamente.

### Paso 7 — La respuesta vuelve y se normaliza en el frontend
El JSON llega de vuelta al hook, que normaliza los nombres heterogéneos (`Mascotum.Cliente`, `doctor.nombre`, etc.) antes de que el componente los use:

```js
// vet-system-WEB/src/hooks/GetCitas.js
const normalized = list.map((cita) => {
  const cliente = cita?.Mascotum?.Cliente || {};
  const doctor = cita?.doctor || {};
  return {
    ...cita,
    paciente_nombre: [cliente?.nombre, cliente?.apellido].filter(Boolean).join(' ').trim() || '—',
    doctor_nombre: [doctor?.nombre, doctor?.apellido].filter(Boolean).join(' ').trim() || '—',
    mascota_nombre: cita?.Mascotum?.nombre || cita?.mascota?.nombre || '—',
  };
});
setCitas(normalized);
```

Recién ahí el componente de la agenda recibe `citas` ya limpio y lo renderiza.

### Resumen visual del ciclo completo

```
[Secretaria abre la agenda]
        │
        ▼
Componente React ── useGetCitas() ──▶ api.getCitas() ──▶ client.js request()
        │                                                        │
        │                                             GET /api/Citas/requests
        │                                                        ▼
        │                                              CitasController (JWT, rol)
        │                                                        │
        │                                              CitasRequestService
        │                                       (resuelve scope por rol, vence citas vencidas)
        │                                                        │
        │                                              CitasRepository (EF Core → SQL)
        │                                                        │
        │                                                    PostgreSQL
        │                                                        │
        │                                          JSON de vuelta (snake_case)
        ▼                                                        │
Hook normaliza nombres heterogéneos  ◀─────────────────────────────
        │
        ▼
Tabla de citas en pantalla
```

---

## 4.1 Cómo arma la pantalla el frontend: colores, textos y componentes

Más allá de llamar a la API, cada pantalla se construye combinando tres fuentes separadas — nunca escritas "a mano" dentro del JSX:

### Colores y estilos → variables CSS + módulos CSS
Todos los colores, sombras, radios y gradientes del sistema están centralizados como **variables CSS (`--variable`)** en un único archivo:

```css
/* vet-system-WEB/src/styles/variables.css */
:root {
  --button-blue: linear-gradient(135deg, #0284c7, #0369a1);
  --color-primary: rgb(23, 23, 255);
  --color-danger: #ea1f1f;
  --radius-md: 0.5rem;
  --shadow-md: 0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1);
}
```

Ningún componente escribe un color literal (`#0284c7`) directamente en su JSX. En vez de eso, cada componente tiene su propio archivo **`index.module.css`** (CSS Module, con clases que se compilan a nombres únicos para que no choquen entre componentes) que **consume** esas variables:

```css
/* ejemplo de un .module.css cualquiera */
.button {
  background: var(--button-blue);
  border-radius: var(--radius-md);
  box-shadow: var(--shadow-md);
}
```

Y el componente solo importa esas clases, sin CSS inline:

```jsx
import styles from './index.module.css';
<button className={styles.button}>Guardar</button>
```

Esto es una regla dura del proyecto (`CLAUDE.md`): "no se permite CSS inline ni hardcodeado en JSX". Si una pantalla necesita un color o sombra nueva, primero se define como variable en `variables.css`, no como valor suelto en un componente.

### Textos → archivos de constantes
Ningún label, placeholder o mensaje se escribe como string literal dentro del JSX. Se centralizan en `src/constants/` (`TEXT.js`, `constansts.js`):

```js
// vet-system-WEB/src/constants/TEXT.js
export const TEXT_INVOICES_GESTION = {
  gestioPagos: "Gestion de pagos",
  paidtTotal: "Total pagado",
  refreshButton: "Refrescar",
  searchPlaceholder: "Buscar...",
};
```

Y el componente lo consume importando el objeto, no escribiendo el texto directamente:

```jsx
import { TEXT_INVOICES_GESTION } from '../../constants/TEXT';
<h2>{TEXT_INVOICES_GESTION.gestioPagos}</h2>
```

Esto permite cambiar la redacción de toda la app desde un solo lugar, sin buscar el texto componente por componente.

### Componentes → librería interna `anteriority-ui` primero, locales después
Antes de construir un botón, input, modal, etc. desde cero, la regla del proyecto es revisar primero el catálogo de **`anteriority-ui`** — una librería de diseño interna (paquete npm propio, documentado en `node_modules/anteriority-ui/AnteriorityUI.md`) que ya tiene ~50 componentes listos (botones, inputs, loaders, selects, avatares...). Ejemplos reales de uso:

```jsx
// vet-system-WEB/src/pages/DoctorSystem/CreateCustomerAndPets/CustomerAndPets.jsx
import { Input, ProfileAvatar } from 'anteriority-ui';

// vet-system-WEB/src/pages/AdminSystem/.../CurrencySelector.jsx
import { Loading, PlaceholderButton, Checks } from 'anteriority-ui';
```

Solo si `anteriority-ui` no tiene un componente equivalente, se cae al set local en `src/components/screens/components/` (componentes base históricos del proyecto, hoy mayormente reemplazados por la librería), o se construye uno nuevo ahí siguiendo las mismas convenciones (carpeta propia + `index.jsx` + `index.module.css`).

### En resumen, una pantalla cualquiera combina:
```
JSX del componente
 ├─ Texto      → import desde src/constants/TEXT.js (o constansts.js)
 ├─ Estilo     → import styles from './index.module.css' (que a su vez usa var(--algo) de variables.css)
 ├─ Componente → import { Boton, Input, ... } from 'anteriority-ui' (o local si no existe en la librería)
 └─ Datos      → hook propio (src/hooks/*.js) que llama a src/services/api/index.js
```

## 4.2 Anatomía de un componente real

Para entender cómo se escribe un componente en este frontend, sirve mirar uno simple y completo: `Filter` (el panel de filtros reutilizable que aparece en varias pantallas de listados).

```jsx
// vet-system-WEB/src/components/screens/components/Filter/Index.jsx
import React, { useState } from 'react';
import { Filter as FilterIcon, Trash2, Search } from 'lucide-react';
import DrawPanel from '../DrawPanel/index';
import Button from '../Button/index';
import styles from './Index.module.css';
import ModalTooltip from '../../../ModalTooltip/ModalTooltip';

const Filter = ({
    children,
    onClear,
    onApply,
    searchTerm = '',
    width = '350px',
    title = 'Filtros',
    activeFiltersCount = 0
}) => {
    const [isOpen, setIsOpen] = useState(false);

    const handleApply = () => {
        setIsOpen(false);
        if (onApply) onApply();
    };

    return (
        <div className={styles.filterWrapper}>
            <Button onClick={() => setIsOpen(true)} variant={Button.VARIANTS.PRIMARY}>
                <FilterIcon size={18} /> Filtrar
            </Button>
            {activeFiltersCount > 0 && (
                <span className={styles.filterBadge}>{activeFiltersCount}</span>
            )}

            <DrawPanel isOpen={isOpen} onClose={() => setIsOpen(false)} title={title} width={width}>
                <div className={styles.fields}>{children}</div>
                <Button onClick={handleApply} variant={Button.VARIANTS.PRIMARY}>Aplicar</Button>
            </DrawPanel>
        </div>
    );
};

export default Filter;
```

A continuación, el desglose **línea por línea** de ese mismo bloque de código — qué hace cada una, no solo qué es:

```
Línea 1  import React, { useState } from 'react';
```
Importa la librería de React y, de ella, específicamente el hook `useState`. Sin esta línea, `useState` no existiría dentro del archivo — cada hook o utilidad de React se trae explícitamente, nunca está "disponible globalmente".

```
Línea 2  import { Filter as FilterIcon, Trash2, Search } from 'lucide-react';
```
Trae tres íconos de la librería `lucide-react`. El ícono `Filter` se renombra a `FilterIcon` en el `import` (`as FilterIcon`) porque el nombre `Filter` ya lo usa el propio componente que se está definiendo más abajo — si no se renombrara, chocarían los dos identificadores.

```
Línea 3  import DrawPanel from '../DrawPanel/index';
```
Importa otro componente ya existente en el mismo catálogo (`screens/components/DrawPanel`), que es el panel lateral deslizable que `Filter` usa por dentro para mostrar los campos de filtro. `Filter` no dibuja ese panel desde cero: lo reutiliza.

```
Línea 4  import Button from '../Button/index';
```
Mismo principio: el botón genérico del catálogo, reutilizado en vez de escribir un `<button>` suelto con estilos propios.

```
Línea 5  import styles from './Index.module.css';
```
Importa el CSS Module propio de este componente. `styles` queda como un objeto JavaScript donde cada clase definida en `Index.module.css` aparece como una propiedad (`styles.filterWrapper`, `styles.filterBadge`, etc.) — ver sección 4.1 para cómo se construyen esas clases a partir de las variables globales.

```
Línea 6  import ModalTooltip from '../../../ModalTooltip/ModalTooltip';
```
Otro componente compartido, pero ubicado *fuera* de la carpeta `screens/components` (de ahí las tres subidas de carpeta `../../../`) — el tooltip que muestra "Filtros aplicados" sobre el contador de filtros activos.

```
Línea 8  const Filter = ({
Línea 9      children,
Línea 10     onClear,
Línea 11     onApply,
Línea 12     searchTerm = '',
Línea 13     width = '350px',
Línea 14     title = 'Filtros',
Línea 15     activeFiltersCount = 0
Línea 16 }) => {
```
Esta es la firma del componente. `Filter` es una función de JavaScript (una *función flecha*) que recibe un único parámetro — un objeto — y ese objeto se **desestructura** directamente ahí mismo, extrayendo siete propiedades puntuales de él. Esas siete propiedades son las **props**: lo que quien use `<Filter ... />` le pasa desde afuera, exactamente igual que los argumentos de cualquier función. Cuatro de ellas (`searchTerm`, `width`, `title`, `activeFiltersCount`) tienen **valores por defecto** escritos con `=` — si quien usa el componente no pasa esa prop, JavaScript usa automáticamente ese valor en su lugar, así el componente nunca recibe `undefined` sin darse cuenta. `children` es una prop especial de React que no hace falta pasar explícitamente: contiene lo que se escriba *entre* las etiquetas de apertura y cierre del componente (por ejemplo el `<input ... />` de un formulario de filtros). `onClear` y `onApply` no tienen valor por defecto porque son **funciones callback**: el padre decide qué hacer en cada caso, y si no las pasa, el componente simplemente no las ejecuta (ver las líneas 20 y 23 más abajo).

```
Línea 18     const [isOpen, setIsOpen] = useState(false);
```
Declara el **estado local** del componente: una variable `isOpen` que arranca en `false`, y una función `setIsOpen` que es la única forma permitida de cambiarla. `useState(false)` le dice a React "recordá este valor entre renders, empezando en `false`". Cada vez que algo llama a `setIsOpen(...)`, React vuelve a ejecutar el componente entero con el nuevo valor y actualiza lo que se ve en pantalla — sin esto, `isOpen` sería una variable común que se reiniciaría a `false` en cada render y nunca podría "recordar" que el panel está abierto.

```
Línea 20     const handleApply = () => {
Línea 21         setIsOpen(false);
Línea 22         if (onApply) onApply();
Línea 23     };
```
Define una función interna `handleApply`, que se ejecuta cuando el usuario toca "Aplicar" dentro del panel. Hace dos cosas en orden: primero cierra el panel (`setIsOpen(false)`, que dispara un nuevo render con `isOpen` en `false`), y después, *solo si el padre pasó una función `onApply`* (`if (onApply)`), la ejecuta. Esa comprobación evita un error si alguien usa `<Filter>` sin pasar `onApply` — llamar a algo que es `undefined` rompería la aplicación.

```
Línea 26     return (
Línea 27         <div className={styles.filterWrapper}>
```
Arranca el `return`: a partir de aquí el componente describe *qué se ve en pantalla* (JSX), no más lógica. El `<div>` raíz toma su clase CSS desde `styles.filterWrapper` — la clase definida en `Index.module.css` bajo el nombre `filterWrapper`, ya compilada a un nombre único por el CSS Module.

```
Línea 28             <Button onClick={() => setIsOpen(true)} variant={Button.VARIANTS.PRIMARY}>
Línea 29                 <FilterIcon size={18} /> Filtrar
Línea 30             </Button>
```
Renderiza el botón "Filtrar" reutilizando el componente `Button` importado en la línea 4. `onClick` recibe una función flecha que llama a `setIsOpen(true)` — es decir, al tocar este botón se abre el panel. `variant={Button.VARIANTS.PRIMARY}` le pasa una prop al propio `Button` para pedirle su estilo "primario" (una constante que el componente `Button` expone). Adentro va el ícono `FilterIcon` (importado en la línea 2, con tamaño `18`) seguido del texto "Filtrar".

```
Línea 31             {activeFiltersCount > 0 && (
Línea 32                 <span className={styles.filterBadge}>{activeFiltersCount}</span>
Línea 33             )}
```
Esto es **renderizado condicional**: en JSX, `condición && <algo>` significa "mostrá `<algo>` solo si la condición es verdadera; si es falsa, no mostrés nada". Acá, el badge con el número de filtros activos solo aparece si `activeFiltersCount` (la prop recibida en la línea 15) es mayor a cero.

```
Línea 35             <DrawPanel isOpen={isOpen} onClose={() => setIsOpen(false)} title={title} width={width}>
```
Renderiza el panel lateral (`DrawPanel`, importado en la línea 3), pasándole cuatro props: `isOpen` (el estado local de la línea 18, para que el panel sepa si debe mostrarse), `onClose` (una función que cierra el panel poniendo `isOpen` en `false` — así `DrawPanel` puede cerrarse solo, por ejemplo si el usuario toca afuera), y `title`/`width`, que son directamente las props que le llegaron a `Filter` desde su propio padre (líneas 14 y 13).

```
Línea 36                 <div className={styles.fields}>{children}</div>
```
Acá se usa la prop `children` (línea 9): todo lo que quien use `<Filter>` haya puesto entre sus etiquetas de apertura y cierre se inserta exactamente en este punto, envuelto en un `<div>` con la clase `styles.fields`. Así `Filter` puede mostrar cualquier formulario de filtros sin tener que conocer sus campos de antemano.

```
Línea 37                 <Button onClick={handleApply} variant={Button.VARIANTS.PRIMARY}>Aplicar</Button>
Línea 38             </DrawPanel>
Línea 39         </div>
Línea 40     );
Línea 41 };
```
El botón "Aplicar" del panel usa como `onClick` la función `handleApply` definida en la línea 20 (no una función nueva en línea, porque esta sí necesita ejecutar dos pasos). Después se cierran ordenadamente todas las etiquetas abiertas: `</DrawPanel>`, el `</div>` raíz de la línea 27, el paréntesis del `return`, y la llave que cierra el cuerpo de la función `Filter`.

```
Línea 43 export default Filter;
```
Exporta el componente para que otros archivos puedan importarlo con `import Filter from '.../Filter'`. `export default` es la forma estándar de exportar cuando el archivo define un único componente principal.

### Lo que queda claro al leerlo así
- **Props** = parámetros de entrada, con o sin valor por defecto, incluida la prop especial `children`.
- **`useState`** = memoria local que persiste entre renders y que, al cambiar, vuelve a dibujar el componente.
- **Funciones internas** (`handleApply`) = la lógica se nombra y se saca del JSX, no se mezcla con el `return`.
- **JSX en el `return`** = pura descripción visual: combina props, estado y otros componentes, con renderizado condicional (`&&`) donde haga falta — nunca cálculos complejos sueltos ahí mismo.

### `useEffect` — el hook que falta en este ejemplo
`Filter` no usa `useEffect` porque no necesita sincronizarse con nada externo a React (ni timers, ni fetches, ni suscripciones). Para verlo en acción hay que mirar otro componente real del proyecto, `CountdownBar`:
```jsx
// vet-system-WEB/src/components/screens/components/CountdownBar/utils.js
useEffect(() => {
  if (!active) {
    setState({ progress: 100, remainingSeconds: durationRef.current / 1000 });
    return;
  }
  const timer = setInterval(() => { /* recalcula progreso cada 50ms */ }, 50);
  return () => clearInterval(timer); // limpieza: se ejecuta si `active` cambia o el componente se desmonta
}, [active]); // ← lista de dependencias: el efecto se re-ejecuta solo si "active" cambia
```
Mientras `useState` guarda un valor, `useEffect` ejecuta código *cuando ese valor cambia*: la función que recibe como primer argumento corre después de cada render en el que alguna dependencia cambió, y lo que esa función retorna (acá, `() => clearInterval(timer)`) es la **limpieza**, que React ejecuta antes de volver a correr el efecto o al desmontar el componente — por eso el `setInterval` nunca queda corriendo de más. El segundo argumento (`[active]`) es la **lista de dependencias**: React solo vuelve a correr el efecto si alguno de esos valores cambió desde el último render. Un array vacío (`[]`) significa "correr solo una vez, al montar el componente" — el patrón típico para un fetch inicial de datos, como en `useGetCitas` (sección 4): `useEffect(() => { if (autoFetch) fetchCitas(); }, [fetchCitas, autoFetch])`.

### Constantes dentro de un componente
No son `const` de JavaScript cualquiera: suelen ser valores derivados, recalculados en cada render a partir de las props o el estado — por ejemplo en `CurrencySelector.jsx`:
```jsx
const isGlobalLoading = currencyLoading || bcvLoading;
const currencies = [
  { code: 'USD', icon: DollarSign, label: getText('moneySettings.dollar') },
  { code: 'VES', icon: Wallet, label: getText('moneySettings.bolivares') }
];
```
Estas no se centralizan en `constants/` porque dependen de datos en tiempo de ejecución (el idioma vía `getText`, el estado de carga) — lo que sí va en `constants/` son los **textos fijos** (ver sección 4.1), no estos derivados.

---

## 5. Otros flujos importantes (resumen)

- **Facturación:** una cita completada genera una consulta médica, que descuenta stock de productos usados y calcula el total (con conversión Bolívares/Dólares vía la tasa BCV que un Worker actualiza a diario). La factura se genera como PDF con QuestPDF y se guarda en `wwwroot/` para descarga inmediata. Detalle en `Document/FacturationSystem.md`.
- **Mensajería y notificaciones:** en tiempo real vía SignalR (`MessageHub`, `NotificactionsPush`), agrupando conexiones por `userId` y rol. Detalle en `Document/MessagingSystem.md`.
- **Tolerancia a fallos:** el frontend chequea la salud de la API cada 10s (`useHealthCheck`) y redirige a una pantalla de espera (`/espera`) si detecta que el backend o internet están caídos, restaurando la vista anterior al reconectar.
- **Workers en segundo plano:** tareas programadas del lado del backend (actualizar tasa BCV, limpiar PDFs/reportes viejos, generar reportes automáticos) corren independientes de cualquier request HTTP.

---

## 6. Dónde seguir leyendo

| Quiero saber... | Dónde mirar |
|---|---|
| Todos los endpoints y módulos del backend | `vet-api-Net/BACKEND_DOCUMENTATION.md` y `Document/Infra/Controller.md` |
| Arquitectura detallada por capas del backend | `Document/Infra/Infraestructura_y_Flujo.md` |
| Facturación a fondo | `Document/FacturationSystem.md` |
| Mensajería/notificaciones a fondo | `Document/MessagingSystem.md` |
| Seguridad y autenticación | `Document/SecurityAndAuthentication.md` |
| Estructura completa del frontend | `vet-system-WEB/FRONTEND_STRUCTURE.md` |
| Reglas de desarrollo del frontend (componentes, estilos, API layer) | `vet-system-WEB/CLAUDE.md` |
| Reglas de arquitectura del backend (capas, convenciones, DI) | `vet-api-Net/CLAUDE.md` |
