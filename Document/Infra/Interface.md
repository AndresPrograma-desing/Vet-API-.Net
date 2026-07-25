# Carpeta: Interface (Contratos e Interfaces)

## Funcionalidad
La carpeta `Interface` contiene la definición de todas las interfaces (contratos) de la aplicación, clasificadas en repositorios de datos, servicios de negocio y herramientas técnicas. Su propósito es definir los métodos que cada componente del sistema debe implementar obligatoriamente.

## Cualidades y Características
- **Inversión de Dependencias (SOLID):** Cumple con el principio de que los módulos de alto nivel no deben depender de módulos de bajo nivel, sino de abstracciones. Los controladores dependen de las interfaces de servicios, y los servicios de las interfaces de repositorios.
- **Namespace Especial `Interfaze`:** Por convención propia del diseño de este proyecto, el namespace físico de este directorio está escrito con **"z"** (`vet_api_Net.Interfaze.*`), por ejemplo:
  - `vet_api_Net.Interfaze.Services`
  - `vet_api_Net.Interfaze.Repositories`
  - `vet_api_Net.Interfaze.Utilities`
- **Facilidad para Pruebas Unitarias (Mocking):** El uso de interfaces permite simular el comportamiento de la base de datos o de APIs externas (como el envío de correos o generación de PDF) en pruebas automatizadas utilizando frameworks de mocking (ej. Moq, NSubstitute).
- **Flexibilidad del Sistema:** Permite sustituir la implementación de un servicio técnico o repositorio sin tener que modificar la lógica de las clases que lo consumen.

## Subcarpetas e Interfaces Relevantes
- **`Interface/Repository/`:** Contratos para el acceso a base de datos (ej: `ICitasRepository.cs`, `IUserRepository.cs`).
- **`Interface/Services/`:** Contratos de lógica de negocio pura (ej: `ICitasRequestService.cs`, `IAuthService.cs`).
- **`Interface/Utilities/`:** Contratos de soporte técnico transversal (ej: `ICurrencyService.cs`, `IGeneratePdfService.cs`).
