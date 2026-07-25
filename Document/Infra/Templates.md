# Carpeta: Templates (Plantillas HTML)

## Funcionalidad
La carpeta `Templates` contiene los archivos de diseño y plantillas HTML estáticas utilizadas por la aplicación para construir y formatear las comunicaciones del sistema, en especial el cuerpo de los correos electrónicos transaccionales que se envían a los clientes o personal de la veterinaria.

## Cualidades y Características
- **Cuerpo HTML Estructurado:** Proporciona un diseño responsivo y profesional para los correos, incluyendo cabeceras de la veterinaria Happy Pets, logotipos, tablas detalladas y firmas, mejorando la experiencia del usuario.
- **Marcadores Dinámicos (Placeholders):** Las plantillas contienen cadenas de reemplazo específicas (como `{{userName}}`, `{{citaFecha}}`, `{{token}}`). El servicio de correos lee el archivo HTML y reemplaza programáticamente estas marcas con la información en tiempo real del usuario o de la cita.
- **Sincronización con Base de Datos:** Durante la fase de inicialización (`SeedsData.Initialize`), el sistema detecta de forma automática los archivos en esta carpeta, lee su contenido HTML y los almacena en la tabla de la base de datos `EmailTemplates`. Esto permite que, si el administrador lo requiere, el contenido base pueda ser editado o personalizado directamente en caliente desde la base de datos sin redesplegar el código del backend.

## Estructura de Directorios
- **`Templates/Email/`:** Agrupa archivos HTML para diferentes propósitos transaccionales:
  - Notificaciones de confirmación de citas.
  - Correos de restablecimiento y recuperación de contraseñas de cuentas.
  - Envíos de facturas digitales adjuntas.
