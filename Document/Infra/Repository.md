# Carpeta: Repository (Capa de Acceso a Datos)

## Funcionalidad
La carpeta `Repository` implementa el patrón **Repository** de acceso a datos. Es la responsable exclusiva de interactuar con el contexto de la base de datos (`AppDbContext`), realizar consultas complejas en lenguaje LINQ, e indicar la persistencia (guardado) de cambios.

## Cualidades y Características
- **Aislamiento del Origen de Datos:** Es la **única** capa con autorización de inyectar y usar `AppDbContext`. Ningún controlador o servicio de negocio tiene permitido interactuar directamente con el contexto de EF Core.
- **Consultas Optimizadas:** Aloja las consultas estructuradas que requieren la inclusión de relaciones complejas mediante cargas ansiosas (`.Include()` y `.ThenInclude()`), como por ejemplo cargar una Cita con su Mascota y su Cliente asociado en una sola consulta de base de datos.
- **Sintaxis Concisa (Expression-bodied members):** Los métodos simples de consulta o persistencia se definen utilizando expresiones de una línea (`=>`), haciendo el código limpio y legible.
- **Operaciones Asíncronas Integrales:** Todos los métodos de consulta y persistencia devuelven objetos `Task` y utilizan variantes asíncronas de EF Core (ej: `ToListAsync`, `FirstOrDefaultAsync`, `FindAsync`) para maximizar la escalabilidad del servidor.
- **Retorno de Persistencia:** Los repositorios implementan un método `SaveChangesAsync()` que retorna un booleano (`Task<bool>`), informando al servicio si la transacción en la base de datos fue exitosa o no.

## Archivos Relevantes
- **[CitasRepository.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Repository/CitasRepository.cs):** Gestiona las consultas de citas y sus estados asociados.
- **[UserRepository.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Repository/UserRepository.cs):** Implementa búsquedas de usuarios por credenciales, correo electrónico y roles.
- **[FacturaRepository.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Repository/FacturaRepository.cs):** Consulta y procesa facturas con sus detalles y métodos de pago.
