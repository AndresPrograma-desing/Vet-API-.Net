# Carpeta: Models (Entidades de Dominio)

## Funcionalidad
La carpeta `Models` representa el núcleo del dominio de la aplicación. Contiene las clases C# (entidades) que se mapean de manera directa a las tablas físicas de la base de datos PostgreSQL o MySQL a través de Entity Framework Core.

## Cualidades y Características
- **Nombres en Español:** Por convención estricta del proyecto, todas las entidades del dominio, sus propiedades, columnas y nombres de tablas asociadas están redactados en **idioma español** (ej: `Cliente`, `Mascota`, `Cita`, `Factura`, `Creado`, `Actualizado`).
- **Clases Parciales (`partial`):** Se definen como `partial class` para facilitar la extensibilidad y mantener la compatibilidad con herramientas de ingeniería inversa o scaffolding.
- **Propiedades de Navegación Virtuales (`virtual`):** Las propiedades de navegación que representan relaciones con otras tablas (uno a muchos, muchos a muchos) se marcan con la palabra clave `virtual` para dar soporte a la carga diferida (*lazy loading*) y la creación de proxies de EF Core.
- **Inicialización Segura de Colecciones:** Las colecciones hijas (ej: relaciones de uno a muchos) se inicializan inmediatamente en el constructor o declaración de propiedad con `new List<T>()` para evitar excepciones de tipo `NullReferenceException` al iterarlas.
- **Evitar Warnings de Nullability:** Las referencias requeridas que no se asignan por constructor se inicializan con `= null!` para notificar al compilador de C# que estos campos serán provistos por el ORM y no serán nulos en tiempo de ejecución.

## Archivos y Relaciones Relevantes
- **[Usuario.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Models/Usuario.cs):** Representa a los usuarios del sistema (Administrador, Veterinario, Recepcionista).
- **[Cliente.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Models/Cliente.cs):** Información personal de los dueños de mascotas.
- **[Mascota.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Models/Mascota.cs):** Datos médicos y de perfil de los animales de la clínica.
- **[Cita.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Models/Cita.cs):** Registros de agendas médicas asociadas a mascotas y clientes.
- **[Factura.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Models/Factura.cs):** Registro comercial y montos de facturación.
