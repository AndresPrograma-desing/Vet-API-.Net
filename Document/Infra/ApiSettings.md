# Carpeta: ApiSettings

## Funcionalidad
La carpeta `ApiSettings` es responsable de definir e implementar el mapeo tipado de las configuraciones de la aplicación declaradas en el archivo `appsettings.json`. Utiliza el patrón **Options** (`Options Pattern`) provisto por ASP.NET Core para enlazar las secciones del archivo de configuración JSON con objetos fuertemente tipados en C#.

## Cualidades y Características
- **Tipado Fuerte (Strongly Typed):** Evita el uso de cadenas de texto mágicas (*string keys*) en otras capas del sistema para acceder a configuraciones globales (ej. nombre del sistema, formatos de fecha, tipo de moneda principal).
- **Inyección de Dependencias:** Las clases definidas en esta carpeta se configuran en el contenedor DI (`DependencyInjection.cs`) mediante `services.Configure<TOptions>(...)` y se consumen en servicios y controladores utilizando `IOptions<TOptions>` o `IOptionsSnapshot<TOptions>`.
- **Mantenimiento y Cohesión:** Al centralizar el mapeo de configuración, se facilita la adición de nuevas variables globales o cambios en la estructura de `appsettings.json`.

## Archivos Relevantes
- **[ApiSettingsOptions.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/ApiSettings/ApiSettingsOptions.cs):** Representa las configuraciones básicas de la API veterinaria como monedas admitidas (`MoneyTypes`), datos de la empresa, formatos de hora y fecha.
