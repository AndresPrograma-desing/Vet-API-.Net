# Backend Happy Pets 

Este es el repositorio del backend de Happy Pets, una aplicación diseñada para optimizar el flujo administrativo de citas, mascotas, reportes, consultas, citas. El backend está construido utilizando .Net Core, ASP.NET con el lenguaje C#.
        << 10/04/24 >>

  ## Repositorios

  - Los repositorios del Backend y Frontend se encuentran en el repositorio de github: [
    <https://github.com/AndresPrograma-desing>
  ]

## Características Principales
- Gestión de citas: Permite a los usuarios programar, modificar y cancelar citas para sus mascotas.
- Gestión de mascotas: Permite a los usuarios agregar, editar y eliminar información sobre sus mascotas
- Reportes: Genera reportes detallados sobre las citas, mascotas y consultas.
- Consultas: Permite a los usuarios realizar consultas sobre sus mascotas y citas.
- Gestión de usuarios: Permite a los usuarios registrarse, iniciar sesión y administrar su perfil.
- Productos: Permite a los usuarios agregar, editar y eliminar productos relacionados con el cuidado de sus mascotas.


## Tecnologías Utilizadas
- .Net Core
- ASP.NET
- C#

## Instalación
1. Clona el repositorio: `git clone <URL_DEL_REPOSITORIO>`
2. Navega al directorio del proyecto: `cd backend-happy-pets`
3. Restaura las dependencias: `dotnet restore`
4. Ejecuta la aplicación: `dotnet run`

## Posibles fallas al ejecutar
- Asegúrate de tener instalado .Net Core SDK en tu máquina.
- Verifica que las variables de entorno estén configuradas correctamente, especialmente la cadena de conexión a la base de datos.
- Si encuentras errores relacionados con dependencias, intenta limpiar el proyecto y restaurar las dependencias nuevamente.
- Si Windows Defender o tu antivirus bloquea la ejecución, asegúrate de permitir el acceso a la aplicación o ejecutar VSCode o entorno de desarrollo como administrador.

## Base de Datos
El proyecto utiliza una base de datos MySQL para almacenar la información de las citas, mascotas, reportes, consultas y usuarios. Asegúrate de configurar la cadena de conexión en el archivo `appsettings.json` para que apunte a tu instancia de MySQL o tu gestor de base de datos preferido.

- Puedes generar la base de datos utilizando Entity Framework Core con el comando `dotnet ef database update` después de configurar la cadena de conexión correctamente.

## Mas características
- Seguridad: Implementación de autenticación y autorización para proteger los datos de los usuarios.
- API RESTful: El backend expone una API RESTful para facilitar la integración con el frontend y otros servicios.
- Escalabilidad: El diseño del backend permite una fácil escalabilidad para manejar un mayor número de usuarios y datos a medida que la aplicación crece.
  
  ## Sistema de mensajería y notificaciones
  EL Sistema tiene caracterizticas avazadas como un entorno de mensajeria en tiempo real interno para mejorar la comunicación entre el personal de la clínica veterinaria, permitiendo una atención más rápida y eficiente. Además, se implementa un sistema de notificaciones para mantener a los usuarios informados sobre sus citas, recordatorios y cualquier cambio en su programación.
  El sisteme de mensajeria usa SignalR para permitir la comunicación en tiempo real entre el personal de la clínica veterinaria, mejorando la eficiencia y la experiencia del usuario. Las notificaciones se implementan utilizando un sistema de colas para garantizar que los usuarios reciban información oportuna sobre sus citas y cualquier cambio en su programación.

  EL Sistema de mensajeria usa una tabla en la base de datos para almacenar los mensajes y las conversaciones entre el personal de la clínica veterinaria. Cada mensaje se asocia con una conversación específica, lo que permite un seguimiento eficiente de las comunicaciones (Para ver mas detalles sobre la implementación del sistema de mensajería, consulta el archivo `MessagingSystem.md` en este repositorio).

  ## Sistema de generación de facturas
  El sistema de generación de facturas en Happy Pets permite a los usuarios generar facturas detalladas para los servicios prestados a sus mascotas. Este sistema se integra con la gestión de citas y productos para calcular automáticamente los costos asociados a cada servicio y producto utilizado durante la atención veterinaria. Las facturas generadas incluyen información sobre los servicios prestados, los productos utilizados, los costos individuales y el total a pagar, proporcionando una experiencia transparente y eficiente para los usuarios (Para ver más detalles sobre la implementación del sistema de generación de facturas, consulta el archivo `FacturationSystem.md` en este repositorio).

  ## Seguridad y autenticación
    El backend de Happy Pets implementa un sistema de seguridad robusto utilizando JWT (JSON Web Tokens) para la autenticación y autorización de usuarios. Esto garantiza que solo los usuarios autorizados puedan acceder a ciertas funcionalidades y datos dentro de la aplicación. Además, se implementan medidas de seguridad adicionales, como el cifrado de contraseñas y la validación de entradas para proteger contra ataques comunes como la inyección SQL y el cross-site scripting (XSS) (Para ver más detalles sobre la implementación del sistema de seguridad y autenticación, consulta el archivo `SecurityAndAuthentication.md` en este repositorio).

  ## Arquitectura y diseño
  El backend de Happy Pets sigue una arquitectura basada en capas, separando las responsabilidades en diferentes capas como la capa de presentación, la capa de negocio y la capa de datos. Esto facilita el mantenimiento y la escalabilidad del código, permitiendo una mejor organización y modularidad. Además, se utilizan patrones de diseño como el repositorio y el servicio para abstraer la lógica de acceso a datos y la lógica de negocio, respectivamente (Para ver más detalles sobre la arquitectura y diseño del backend, consulta el archivo `ArchitectureAndDesign.md` en este repositorio).

  ## Estructura del proyecto
  El proyecto está organizado en varias carpetas principales:
- `Controllers`: Contiene los controladores que manejan las solicitudes HTTP y coordinan la lógica de negocio.
- `Models`: Contiene las clases que representan los datos y las entidades del sistema.
- `Services`: Contiene las clases que implementan la lógica de negocio y las operaciones relacionadas con las entidades.
- `Data`: Contiene las clases relacionadas con el acceso a datos, como los contextos de base de datos y los repositorios.
- `DTOs`: Contiene las clases de transferencia de datos utilizadas para enviar y recibir información entre el cliente y el servidor.
- `Migrations`: Contiene las migraciones de la base de datos generadas por Entity
    Framework Core.
- `appsettings.json`: Archivo de configuración que contiene la cadena de conexión a la base de datos y otras configuraciones del sistema.
   para mas detalles sobre la estructura del proyecto, consulta el archivo `ProjectStructure.md` en este repositorio).