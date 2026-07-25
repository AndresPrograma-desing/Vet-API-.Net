# Carpeta: Data (Acceso y Semillero de Datos)

## Funcionalidad
La carpeta `Data` se encarga de la configuración del motor de persistencia (Entity Framework Core) y de la inicialización de los datos requeridos por la aplicación. Contiene el contexto principal de base de datos (`DbContext`) y la lógica para el sembrado inicial de registros de prueba y configuración (`Seeds`).

## Cualidades y Características
- **Mapeo Relacional de Datos (ORM):** Configura la forma en que los modelos del dominio en español se mapean y relacionan en la base de datos PostgreSQL/MySQL mediante Fluent API en `OnModelCreating`.
- **Soporte Dual (Base de Datos Flexible):** Trabaja dinámicamente según el proveedor indicado en el archivo de configuración. Puede conectarse y operar con PostgreSQL (Supabase) en producción o MySQL (Pomelo) para desarrollo local.
- **Auditoría e Integridad:** Define restricciones de claves foráneas, borrados en cascada personalizados y tipos de datos precisos para campos numéricos y de fecha.
- **Inicialización Idempotente (Data Seeding):** La clase de semillas (`SeedsData`) inicializa la base de datos de manera segura al validar la preexistencia de datos mediante `.Any()`. Esto evita la duplicación de datos al reiniciar la aplicación y crea automáticamente usuarios por defecto, métodos de pago, configuraciones de sistema y plantillas de correo electrónico a partir de archivos HTML.

## Archivos Relevantes
- **[AppDbContext.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Data/AppDbContext.cs):** Contexto principal del ORM. Administra los conjuntos de entidades (`DbSet`) y sus relaciones.
- **[SeedsData.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Data/seeds/SeedsData.cs):** Carga los datos base del sistema (ej. roles, cuentas administrador por defecto, monedas y plantillas de correo HTML) si la configuración de sembrado está activa.
