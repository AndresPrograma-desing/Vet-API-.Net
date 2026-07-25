# Carpeta: Migrations (Control de Versiones de Base de Datos)

## Funcionalidad
La carpeta `Migrations` es autogenerada y gestionada por Entity Framework Core. Contiene el historial de cambios incrementales aplicados al esquema de la base de datos (tablas, columnas, índices, relaciones) a lo largo del ciclo de vida del desarrollo.

## Cualidades y Características
- **Control de Versiones del Esquema:** Cada migración es un archivo C# que define dos métodos principales:
  - `Up(MigrationBuilder)`: Aplica los cambios en la base de datos.
  - `Down(MigrationBuilder)`: Revierte los cambios introducidos por esa migración específica en caso de ser necesario.
- **Portabilidad del Entorno:** Garantiza que cualquier desarrollador o servidor en producción pueda recrear y actualizar la base de datos local o remota al ejecutar comandos como `dotnet ef database update`.
- **Instantánea del Modelo (`Model Snapshot`):** Contiene el archivo de snapshot (`AppDbContextModelSnapshot.cs`), el cual representa el esquema completo de la base de datos en su versión actual. EF Core lo utiliza para comparar los modelos del dominio C# con el snapshot y generar la siguiente migración diferencial.
- **Soporte Multi-Proveedor:** Aunque las migraciones se autogeneran, el sistema está diseñado para mapear los tipos de datos a sintaxis PostgreSQL o MySQL en función del proveedor configurado.

## Archivos Relevantes
- **`{Timestamp}_{NombreMigracion}.cs`:** Archivos que implementan los cambios del esquema de la base de datos.
- **[AppDbContextModelSnapshot.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Migrations/AppDbContextModelSnapshot.cs):** Representa el esquema de base de datos actual en C#.
