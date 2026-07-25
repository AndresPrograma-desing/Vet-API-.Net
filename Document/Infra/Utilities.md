# Carpeta: Utilities (Utilidades y Soporte Técnico Transversal)

## Funcionalidad
La carpeta `Utilities` agrupa implementaciones de servicios técnicos de soporte transversal que no pertenecen directamente al dominio funcional del negocio veterinario. Su responsabilidad incluye la generación de documentos de cálculo (Excel), conversión matemática de monedas nacionales y extranjeras, análisis de textos clínicos y utilidades gráficas como la gestión de avatares.

## Cualidades y Características
- **Soporte Reutilizable (Transversalidad):** Sus componentes son reutilizados por múltiples módulos del sistema (ej: el generador de Excel es consumido por los servicios de reporte de inventarios y de citas; el conversor de divisas es consumido tanto por la lógica de facturas como por la de reportes financieros).
- **Separación de Namespaces:** Las implementaciones usan el namespace `vet_api_Net.Utilities` y sus firmas se declaran en `vet_api_Net.Interfaze.Utilities`. Se registran en la sección correspondiente de `DependencyInjection.cs`.
- **Aislamiento de Complejidades Técnicas:** Evita acoplar la lógica de negocio con detalles de bajo nivel, como la manipulación de celdas y filas en ClosedXML (`ExcelGenerator`) o la lógica de tracking y reintentos ante fallos de red (`FailureTracker`).

## Archivos Relevantes
- **[CurrencyService.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Utilities/CurrencyService.cs):** Realiza operaciones aritméticas de conversión monetaria entre Bolívares (VES) y Dólares (USD) utilizando la tasa más reciente registrada en la base de datos.
- **[ExcelGenerator.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Utilities/ExcelGenerator.cs):** Encapsula el uso de la librería **ClosedXML** para dar formato estético, bordes, colores y fórmulas a los reportes tabulares de Excel descargables.
- **[HistoriaClinicaAnalizador.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Utilities/HistoriaClinicaAnalizador.cs):** Analiza y procesa términos y datos clínicos dentro del historial de las mascotas.
- **[FailureTracker.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Utilities/FailureTracker.cs):** Registra y realiza seguimiento a incidencias técnicas y caídas de servicios de infraestructura.
- **[AvatarUrlRequestUtilities.cs](file:///c:/Users/Usuario/ProjectoUniversidad/vet-api-Net-due/vet-api-Net/Utilities/AvatarUrlRequestUtilities.cs):** Provee funciones auxiliares para validar y resolver URLs de imágenes de perfil de los usuarios.
