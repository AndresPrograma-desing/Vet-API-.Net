using System;

/*
 describe:
 - Este archivo es para configurar los mensajes de respuesta del sistema,
   Organiza los strings de las respuestas en clases para su facil orden y escalabilidad
*/

namespace vet_api_Net.Constants
{
    public static class ResponseErrors
    {
        public const string InternalServerError = "Ocurrio un error interno del servidor.";
        public const string NotFound = "Recurso no encontrado.";
        public const string BadRequest = "Solicitud incorrecta.";
        public const string Unauthorized = "No autorizado.";
        public const string Forbidden = "Prohibido.";
    }
    public static class Roles
    {
        public const string Admin = "admin";
        public const string Secretaria = "secretaria";
        public const string Doctor = "doctor";
    }
    public static class ResponseMessagesDtos
    {
        public static class Cliente
        {
            public const string NombreRequired = "El nombre es requerido.";
            public const string NombreMaxLength = "El nombre no puede exceder los 100 caracteres.";
            public const string ApellidoRequired = "El apellido es requerido.";
            public const string ApellidoMaxLength = "El apellido no puede exceder los 100 caracteres.";
            public const string EmailRequired = "El correo electrónico es requerido.";
            public const string InvalidEmailFormat = "El correo electrónico no tiene un formato válido.";
            public const string TelefonoMaxLength = "El teléfono no puede exceder los 20 caracteres.";
            public const string DireccionMaxLength = "La dirección no puede exceder los 300 caracteres.";
            public const string IdentificacionMaxLength = "La identificación no puede exceder los 20 caracteres.";
            public const string NotaMaxLength = "La nota no puede exceder los 500 caracteres.";
        }
        public static class Mascota
        {
            public const string NombreRequired = "El nombre de la mascota es requerido.";
            public const string NombreMaxLength = "El nombre no puede exceder los 100 caracteres.";
            public const string EspecieRequired = "La especie es requerida.";
            public const string EspecieMaxLength = "La especie no puede exceder los 50 caracteres.";
            public const string RazaMaxLength = "La raza no puede exceder los 50 caracteres.";
            public const string ColorMaxLength = "El color no puede exceder los 50 caracteres.";
            public const string SexoRequired = "El sexo es requerido.";
            public const string SexoMaxLength = "El sexo no puede exceder los 20 caracteres.";
            public const string FechaNacimientoMaxLength = "La fecha de nacimiento no puede exceder los 20 caracteres.";
            public const string PesoNoNegativo = "El peso no puede ser negativo.";
            public const string IdentificacionMaxLength = "La identificación de la mascota no puede exceder los 50 caracteres.";
            public const string AlergiasMaxLength = "Las alergias no pueden exceder los 500 caracteres.";
            public const string CondicionesMedicasMaxLength = "Las condiciones médicas no pueden exceder los 500 caracteres.";
        }
        public static class Producto
        {
            public const string CodigoRequired = "El código del producto es requerido.";
            public const string CodigoMaxLength = "El código no puede exceder los 50 caracteres.";
            public const string NombreMaxLength = "El nombre no puede exceder los 200 caracteres.";
            public const string DescripcionMaxLength = "La descripción no puede exceder los 500 caracteres.";
            public const string CategoriaIdInvalid = "La categoria_id debe ser un identificador válido.";
            public const string TipoRequired = "El tipo de producto es requerido.";
            public const string TipoMaxLength = "El tipo no puede exceder los 50 caracteres.";
            public const string StockNoNegativo = "El stock no puede ser negativo.";
            public const string StockMinimoNoNegativo = "El stock mínimo no puede ser negativo.";
            public const string UnidadMedidaMaxLength = "La unidad de medida no puede exceder los 50 caracteres.";
            public const string ProveedorMaxLength = "El proveedor no puede exceder los 200 caracteres.";
        }
        public static class Consulta
        {
            public const string CitaIdInvalid = "El cita_id debe ser un identificador válido.";
            public const string MascotaIdInvalid = "El mascota_id debe ser un identificador válido.";
            public const string DoctorIdInvalid = "El doctor_id debe ser un identificador válido.";
            public const string PesoActualNoNegativo = "El peso actual no puede ser negativo.";
            public const string SintomasRequired = "Los síntomas son requeridos.";
            public const string SintomasMaxLength = "Los síntomas no pueden exceder los 1000 caracteres.";
            public const string DiagnosticoMaxLength = "El diagnóstico no puede exceder los 1000 caracteres.";
            public const string TratamientoMaxLength = "El tratamiento no puede exceder los 1000 caracteres.";
            public const string RecetaMaxLength = "La receta no puede exceder los 1000 caracteres.";
            public const string ObservacionesMaxLength = "Las observaciones no pueden exceder los 1000 caracteres.";
            public const string ConsultaPriceNoNegativo = "El precio de la consulta no puede ser negativo.";
        }
        public static class ConsultaProducto
        {
            public const string ProductoIdInvalid = "El producto_id debe ser un identificador válido.";
            public const string CantidadPositiva = "La cantidad debe ser mayor a cero.";
            public const string PrecioUnitarioNoNegativo = "El precio unitario no puede ser negativo.";
            public const string DosisMaxLength = "La dosis no puede exceder los 100 caracteres.";
            public const string ViaAdministracionMaxLength = "La vía de administración no puede exceder los 100 caracteres.";
            public const string FrecuenciaMaxLength = "La frecuencia no puede exceder los 100 caracteres.";
            public const string DuracionMaxLength = "La duración no puede exceder los 100 caracteres.";
            public const string InstruccionesMaxLength = "Las instrucciones no pueden exceder los 500 caracteres.";
            public const string AplicadoPorInvalid = "El aplicado_por debe ser un identificador válido.";
        }
        public static class Usuario
        {
            public const string InvalidEmailFormat = "El correo electrónico no tiene un formato válido.";
            public const string RolRequired = "El rol es requerido.";
            public const string RolMaxLength = "El rol no puede exceder los 20 caracteres.";
            public const string TelefonoMaxLength = "El teléfono no puede exceder los 20 caracteres.";
            public const string NombreMaxLength = "El nombre no puede exceder los 100 caracteres.";
            public const string ApellidoMaxLength = "El apellido no puede exceder los 100 caracteres.";
        }
        public static class Cita
        {
            public const string SecretariaIdInvalid = "secretaria_id inválido";
            public const string RequiredMotivo = "El motivo de la cita es requerido.";
            public const string MotivoMaxLength = "El motivo no puede exceder los 500 caracteres.";
            public const string TipoCitaMaxLength = "El tipo de cita no puede exceder los 50 caracteres.";
            public const string EstadoMaxLength = "El estado no puede exceder los 50 caracteres.";
            public const string NotasMaxLength = "Las notas no pueden exceder los 1000 caracteres.";
            public const string MetodoPagoMaxLength = "El método de pago no puede exceder los 50 caracteres.";
            public const string EstadoRequired = "El estado es requerido.";
        }
        public static class Factura
        {
            public const string NumeroFacturaRequired = "El número de factura es requerido.";
            public const string NumeroFacturaMaxLength = "El número de factura no puede exceder los 50 caracteres.";
            public const string ClienteIdInvalid = "El cliente_id debe ser un identificador válido.";
            public const string MascotaIdInvalid = "El mascota_id debe ser un identificador válido.";
            public const string DoctorIdInvalid = "El doctor_id debe ser un identificador válido.";
            public const string ProductoIdInvalid = "El producto_id debe ser un identificador válido.";
            public const string ItemsRequired = "La factura debe tener al menos un ítem.";
            public const string MontoNoNegativo = "El monto no puede ser negativo.";
            public const string CantidadPositiva = "La cantidad debe ser mayor a cero.";
            public const string MetodoPagoRequired = "El método de pago es requerido.";
            public const string MetodoPagoMaxLength = "El método de pago no puede exceder los 50 caracteres.";
            public const string EstadoPagoMaxLength = "El estado de pago no puede exceder los 50 caracteres.";
            public const string NotasMaxLength = "Las notas no pueden exceder los 1000 caracteres.";
            public const string CodigoRequired = "El código del producto es requerido.";
            public const string CodigoMaxLength = "El código no puede exceder los 50 caracteres.";
            public const string ItemNombreRequired = "El nombre del producto es requerido.";
            public const string ItemNombreMaxLength = "El nombre no puede exceder los 200 caracteres.";
            public const string ItemDescripcionMaxLength = "La descripción no puede exceder los 500 caracteres.";
        }
    }
    public static class ResponseMessagesLogin
    {
        public const string IsDisabled = "Tu cuenta ha sido deshabilitada, por favor contacta al administrador.";
        public const string RequiredEmail = "El correo electrónico es requerido.";
        public const string RequiredPassword = "La contraseña es requerida.";
        public const string RequiredOldPassword = "La contraseña actual es requerida.";
        public const string IsMenorThan6 = "La contraseña debe tener al menos 6 caracteres.";
        public const string NoWhiteSpace = "La contraseña no puede contener espacios en blanco.";
        public const string IncorrectOldPassword = "Ingrese la contraseña actual correcta.";
        public const string IsEqualToCurrentPassword = "La nueva contraseña no puede ser igual a la actual.";
        public const string EmailAndRolRequired = "Email, Contrasena y Rol son requerido";
        public const string GenericError = "Ocurrió un error interno, por favor intenta nuevamente.";
        public const string ErrorSavingPassword = "Ocurrió un error interno al guardar la contraseña.";
        public const string ErrorCredential = "Credenciales incorrectas";
        public const string UserNotExisting = "El usuario con ese correo electrónico no existe.";
        public const string NoRolForUser = "Rol seleccionado no valido para ese usuario";
        public const string TryAgainIntents = "Demasiados intentos fallidos, intenta en: ";


    }
    public static class ResponseMessagesPasswordRecovery
    {
        public const string RequiredEmail = "El correo electrónico es requerido.";
        public const string ErrorRequestingCode = "No se encontró ningún usuario con ese correo electrónico o no se pudo enviar el correo.";
        public const string CodeSentSuccess = "Código de recuperación enviado con éxito.";
        public const string RequiredCode = "El código de verificación es requerido.";
        public const string InvalidCode = "Código de verificación inválido, expirado o error al enviar el correo.";
        public const string CodeVerifiedSuccess = "Código verificado con éxito. Contraseña enviada a su correo.";
        public const string DefaultSystemName = "Happy Pets";
        public static string RecoveryCodeSubject(string systemName) => $"Código de Recuperación de Clave - {systemName}";
        public static string ResendPasswordSubject(string systemName) => $"Restablecer Contraseña - {systemName}";
        public const string DefaultResetPasswordHtml = "Su código de recuperación es: {{token_codigo}}";
        public const string DefaultResendPasswordHtml = "Su clave de acceso es: {{nueva_clave}}";
        public const string RequirePasswordChangeCode = "REQUIRE_PASSWORD_CHANGE";
        public const string PushNotificationChangePasswordTitle = "Cambio de Contraseña Sugerido";
        public const string PushNotificationChangePasswordMessage = "Iniciaste sesión con una contraseña temporal. Por tu seguridad, te sugerimos cambiarla lo antes posible desde tu perfil.";
    }
    public static class ResponseMessagesCitas
    {
        public const string CitaCreada = "Cita creada exitosamente.";
        public const string CitaActualizada = "Cita actualizada exitosamente.";
        public const string CitaEliminada = "Cita eliminada exitosamente.";
        public const string CitaNotFound = "Cita no encontrada.";
        public const string ErrorInterno = "Ocurrió un error interno al crear la cita.";
        public const string ErrorRequestingCitas = "Error al obtener citas:";
        public const string ErrorRequestDetails = "Error al obtener detalles de la cita:";
        public const string ErrorDeletingCita = "Error al eliminar la cita:";
        public const string InvalidMascotaId = "mascota_id inválido";
        public const string InvalidDoctorId = "doctor_id inválido";
        public const string RequiredHoraCita = "hora_cita es requerida";
        public const string MascotaNotFound = "Mascota no encontrada";
        public const string ExistingCitaConflict = "Ya existe una cita para ese doctor en la misma fecha y hora";
        public const string InvalidHoraCita = "Formato de 'hora_cita' inválido. Use 'HH:mm' o 'HH:mm:ss'.";
        public const string StatuNoValid = "Estado no válido. Valores permitidos:";
        public const string ErrorCreatingCita = "No se pudo crear la cita; revisa datos y conflictos de horario:";
        public const string ErrorModifyingCita = "No se puede modificar una cita que ya está en curso.";
        public const string ErrorProcessingCita = "Error al procesar la cita en el servidor.";
        public const string StatusValid = "Estado no válido. Valores permitidos: completada, en_curso, cancelada, no_asistida";
        public const string CancelledCita = "CITA CANCELADA.";
        public const string NotCitasToday = "No hay citas programadas para el día de hoy.";
    }
    public static class TypeConsultas
    {
        public const string Consulta = "consulta";
        public const string Cirugia = "cirugia";
        public const string Emergencia = "emergencia";
        public const string Vacunacion = "vacunacion";
        public const string Desparasitacion = "desparasitacion";
        public const string Seguimiento = "seguimiento";

    }
    public static class ResponseMessagesClient
    {
        public const string ClientNotFound = "No se pudo encontrar el cliente solicitado.";
        public const string ErrorGettingClient = "Ocurrió un error interno al obtener el cliente.";
        public const string ErrorGettingClients = "Ocurrió un error interno al obtener los clientes.";
        public const string ExistingEmail = "El correo electrónico ya está en uso.";
        public const string RequireIdentificacion = "La identificación es requerida para crear un cliente.";
        public const string RequireIdentificacionMascota = "La identificación de la mascota ya existe en el sistema, por favor ingrese una identificación única.";
        public const string DeleteClientError = "Hubo un error al eliminar el cliente.";
        public const string ClientDeletedSuccess = "Cliente eliminado exitosamente.";
        public const string NotClientDeleted = "El Cliente no existe o no encontrado.";
        public const string InvalidBirthDate = "La fecha de nacimiento no puede ser posterior a la fecha actual ni superior al año actual.";
        public const string ClientUpdatedSuccess = "Cliente actualizado exitosamente.";
        public const string ClientUpdatedError = "Error al actualizar el cliente, por favor verifique los datos ingresados.";
        public const string DataRequired = "Debe proporcionar la cedula o correo electronico registrado en el sistema";
        public const string ErrorValidateData = "Error al validar los datos";
    }
    public static class ResponseMessagesUsers
    {
        public const string CannotDeleteAdmin = "No se puede eliminar a un administrador.";
        public const string UserNotFound = "Usuario no encontrado.";
        public const string DoctorNotFound = "Doctor no encontrado.";
        public const string AdminNotFound = "Administrador no encontrado.";
        public const string SecretarialNotFound = "Secretaria no encontrada.";
        public const string EmailNotConfigured = "El usuario no tiene un email configurado o el email en uso no funciona.";
        public const string ErrorGettingUser = "Ocurrió un error interno al obtener el usuario.";
        public const string ErrorGettingUsers = "Ocurrió un error interno al obtener los usuarios.";
        public const string ExistingUsername = "El nombre de usuario ya está en uso.";
        public const string EmailRequired = "El correo electrónico es requerido.";
        public const string NameRequired = "El nombre y apellido son requeridos.";
        public const string ExistingEmail = "El correo electrónico ya está en uso.";
        public const string PasswordResetTicketCreated = "Ticket de reseteo de contraseña creado exitosamente.";
        public const string PasswordResetTicketNotFoundOrExpired = "El ticket de reseteo no es válido, ya fue utilizado o ha expirado.";
        public const string PasswordResetTicketAccepted = "El ticket de reseteo de contraseña ha sido confirmado correctamente. Puede cerrar esta ventana.";
        public const string PasswordAssignedSuccess = "La nueva contraseña ha sido asignada correctamente.";
        public const string PasswordResetAlreadyPending = "Ya existe una solicitud pendiente para este usuario. Espere a que expire (15 minutos).";
        public const string PasswordResetSubject = "Solicitud de Reseteo de Contraseña";
        public const string EmailAndNewPasswordRequired = "El email y la nueva contraseña son requeridos.";
        public const string InvalidToken = "Token inválido o expirado.";
        public static string DeletingUser(int id) => $"Usuario con ID {id} eliminado exitosamente";
        public static string NotUpdateNameUser = "No se pudo cambiar el nombre del usuario";
        public static string UpdateNameUser = "Nombre del usuario actualizado exitosamente";
        public const string CannotDeleteUserWithDependencies = "No se puede eliminar el usuario porque tiene registros o historial asociados en el sistema. Considere deshabilitar la cuenta en su lugar.";
        public const string RolesError = "Error al optener los roles";
        public static class UsersVariable
        {
            public const string UserStatusActivo = "enabled";
            public const string UserStatusInactivo = "disabled";

        }
    }

    public static class PaymentMethods
    {
        public const string Cash = "Efectivo";
        public const string Card = "Tarjeta";
        public const string Transfer = "Transferencia";
        public const string MobilePayment = "Pago móvil";
        public const string Contain1 = "efectivo";
        public const string Contain2 = "tarjeta";
        public const string Contain3 = "transfer";
        public const string Contain4 = "movil";
        public const string Contain5 = "pago";
        public const string Contain6 = "móvil";
        public const string Others = "Otros";


    }
    public static class ResponseMessagesFacturaErrors
    {
        public const string ConsultaNotFound = "Consulta no encontrada para recibos.";
        public const string CitaNotFound = "Cita no encontrada para recibos.";
        public const string MascotaNotFound = "Mascota no encontrada para recibos.";
        public const string OnlyCitaAllowed = "La Recibo solo puede generarse para citas completadas.";
        public const string ErrorProcessingInvoice = "Error al procesar la Recibos internamente. Consulta creada pero no se pudo generar la factura.";
        public const string InvoiceGenerationError = "Error al guardar la información de la Recibo generada. Sin embargo, el PDF se ha creado correctamente.";

    }
    public static class PdfText
    {
        public const string Clinte = "Cliente";
        public const string IdCliente = "IdCliente:";
        public const string Mascota = "Mascota";
        public const string Doctor = "Doctor";
        public const string Fecha = "Fecha:";
        public const string Vencimiento = "Vencimiento:";
        public const string Hora = "Hora:";
        public const string DMY = "dd/MM/yyyy";
        public const string Servicios = "Servicios";
        public const string Total = "Total";
        public const string Factura = "Recibo";
        public const string Currency = "BS";
        public const string HappyPets = "Happy Pets";
        public const string FooterText = "Factura generada por el sistema de gestión de Happy Pets 2026";
        public const string ThankYou = "Gracias por confiar en Happy Pets";
        public const string SpecialVet = "Veterinaria Especializada";
        public const string Code = "Código";
        public const string Description = "Descripción";
        public const string Price = "P. Unit.";
        public const string Quantity = "Cant";
        public const string Subtotal = "Subtotal";
        public const string NotServices = "Sin servicios registrados.";
        public const string IVA = "IVA";
        public const string NumberPrecio = "0.00";
        public const string ConsultaPrice = "Consulta";

    }
    public static class Exceltext
    {
        public const string Clientes = "Clientes";
        public const string Mascotas = "Mascotas";
        public const string Productos = "Productos";
        public const string Facturas = "Recibo";
        public const string Usuarios = "Usuarios";
        public const string InvalidReport = "Reporte inválido o sin datos.";
        public const string DeserializationError = "No se pudo deserializar el contenido del reporte.";
    }
    public static class Status
    {
        public const string Pending = "pendiente";
        public const string Completed = "completada";
        public const string Programed = "programada";
        public const string Cancelled = "cancelada";
        public const string InCurso = "en_curso";
        public const string InCursoII = "en_curso";
        public const string NotAssisted = "no_asistida";
        public const string Active = "activo";
        public const string Inactive = "inactivo";
        public const string InvoicePending = "factura_pendiente";
        public const string Busy = "ocupado";
        public const string NotAvailable = "no_disponible";
        public const string Available = "disponible";
        public const string Blocked = "bloqueado";

    }
    public static class ResponseMessagesFactura
    {
        public const string FacturaCreada = "Recibo creado exitosamente.";
        public const string FacturaNoEncontrada = "Recibo no encontrado.";
        public const string ErrorInterno = "Ocurrió un error interno al procesar la solicitud de recibo.";
        public const string FacturaActualizada = "Recibo actualizado exitosamente.";
        public const string EmptyProducts = "No se pueden generar Recibo sin productos asociados.";
        public const string NA = "N/A";
        public const int EmptyProductId = 0;
        public const string EmptyProductName = "Sin productos asociados";
        public const string EmptyProductDescription = "No se encontraron productos vinculados a esta factura.";
        public const decimal EmptyProductPrice = 0.00m;
        public const int EmptyProductQuantity = 0;
        public const int EmptyProductTotal = 0;
        public const string Notes = "No hay notas disponibles";
        public const string NotFactura = "No hay Recibo registradas en el sistema.";
        public const string DeleteReceiptsNotificactionTittle = "Eliminacion de Recibo";
        public const string DeleteReceiptsNotificactionDesc = "Se ha eliminado el Recibo de ${cliente_name}, para generarlo nuevamente ir a centro de recibos";

    }
    public static class MessagePoller
    {
        public const string Query = "SELECT 1 FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name = 'mensajes' LIMIT 1";
    }
    public static class ResponseMessagesMessaging
    {
        public const string MessageNotFound = "Mensaje no encontrado.";
    }
    public static class ResponseMessagesProduct
    {
        public const string NoProductName = "Sin Producto.";
        public const string ProductNotFound = "Producto no encontrado.";
        public const string NoProductPrice = "Sin Precio.";
        public const string ProductCantPositive = "La cantidad debe ser un número positivo.";
        public const string NotSufficientStock = "Stock insuficiente.";
        public const string ErrorDecreasingStock = "Error al disminuir el stock.";
        public const string DecreaseStockSuccess = "Stock disminuido exitosamente.";
        public const string ErrorUpdatingProduct = "Error al actualizar el producto.";
        public const string ProductCantNegative = "El precio no puede ser negativo.";
        public const string UpdateProductSuccess = "Producto actualizado exitosamente.";
        public const string UpdateProductNoChanges = "No se aplicaron cambios.";
        public const string ProductCantEqualOrLessThan = "El precio de venta no puede ser menor o igual al precio de compra.";

        public static class ResponseMessagesProductCreate
        {
            public const string CategoryNotFound = "Categoría no encontrada.";
            public const string ProducNameOrCodeExists = "El código o el nombre del producto ya existe.";
            public const string NameAndCodeRequired = "El nombre y el código del producto son requeridos.";
            public const string PriceCannotBeNegative = "El precio y el precio de venta no pueden ser negativos.";
        }
    }
    public static class ResponseMessagesReport
    {
        public const string ReportTittle = "Reporte del sistema completo";
        public const string ResportCategory = "FullSystem";
        public const string Filtre = "{}";
        public const string ReportgeneratedTittle = "Se ha generado un reporte del sistema";
        public const string ReportgeneratedDescription = "El reporte se ha generado correctamente. puedes descargarlo en Ajustes| Accesos Y Seguridad | Repor.";

    }
    public static class ResponseMessagesAuthController
    {
        public const string Unauthorized = "Email, Contraseña y Rol son requeridos.";
        public const string BadRequestUpdateCredentials = "Email, Contraseña Actual y Nueva Contraseña son requeridos.";
        public const string InvalidCredentials = "Credenciales inválidas.";
        public const string UpdatePasswordSuccess = "Contraseña actualizada exitosamente.";
        public const string UserAlreadyExists = "El usuario ya existe.";
        public const string PasswordMismatch = "La contraseña no coincide.";
        public const string InvalidToken = "Token inválido.";
    }
    public static class ResponseMessagesClientPetController
    {
        public const string InvalidPayload = "Payload inválido";
        public const string InternalErrorCP = "Error interno al crear cliente y mascota";
    }
    public static class ResponseMessagesFacturaController
    {
        public const string ConsultaCitaNotFound = "Consulta no encontrada";
        public const string FacturaNotFound = "Factura no encontrada";
        public const string FileNotFound = "Archivo no encontrado";
        public const string RequireNameFile = "Nombre del archivo requerido";
        public const string StatusPagoIsRequired = "estado_pago es requerido";

    }
    public static class ResponseMessagesConsultas
    {
        public const string ErrorCreated = "Error al crear la consulta";
        public const string TemperaturaInvalid = "La temperatura no puede superar los 100 grados";
        public const string NotFound = "Consulta no encontrada.";
    }
    public static class ResponseMessagesGroqServices
    {
        public const string ModelNotConfigured = "Modelo no configurado";
        public const string ErrorFormat = "Error en formato";
        public const string RequestFailed = "Petición fallida";
        public const string ServiceError = "Error en servicio";
        public const string TimeoutError = "Tiempo de espera agotado";
        public const string ServiceUnavailable = "El servicio de Nowgrod.ia no está disponible.";

        public static class GroqPromts
        {
            public const string YouAre = "Eres un asistente veterinario experto e inteligente. Responde las preguntas de manera clara, concisa y profesional en español. Se más profesional con preguntas relacionadas a animales o cosas relacionadas con veterinarios.";
            public const string GroqRule = "\nReglas de comportamiento que DEBES seguir estrictamente:";
            public const string GroqBaseKnowledge = "\nBase de conocimiento de la veterinaria que debes usar para responder consultas generales:";
            public const string GroqResponseHistorial = "Eres un asistente veterinario experto. Debes analizar la información de la mascota y devolver obligatoriamente un objeto JSON con los campos 'resumen_ia' (un resumen clínico fluido y profesional que evalúe también la evolución respecto al historial anterior si se provee, soy grop), 'alertas_riesgo_ia' (alertas de riesgos de salud importantes separadas por ' | ') y 'sugerencias_ia' (sugerencias y recomendaciones veterinarias preventivas separadas por ' | '). No agregues texto adicional fuera del JSON.";
        }
    }
    public static class ResponseMessagesGroq
    {
        public const string ResponseError = "Error en la respuesta.";
        public const string MessageRequired = "El mensaje es requerido.";
        public const string InvalidQuestion = "Pregunta inválida.";
    }
    public static class ResponseMessagesIaConocimiento
    {
        public const string ErrorRetrieving = "Error al obtener conocimiento";
        public const string ErrorCreating = "Error al crear conocimiento";
        public const string NotFound = "Conocimiento no encontrado";
        public const string InvalidData = "Datos inválidos";
        public const string CategoryRequired = "La categoría es requerida.";
        public const string CategoryNotEmpty = "La categoria no puede estar vacia.";
        public const string ConfigSaved = "Configuración de IA guardada exitosamente.";
        public const string ConfigNotFound = "Configuración no encontrada para la categoría especificada.";
    }
    public static class ResponseMessagesHistoriaClinica
    {
        public const string ErrorCreated = "Error al crear historia clínica";
        public const string NotFound = "Historia clínica no encontrada";
        public const string InvalidData = "Datos inválidos";
        public const string ErrorUpdating = "Error al actualizar";
        public const string HistoriaClinicaNotFound = "Historia clínica no encontrada.";
        public const string MascotaNotFound = "Mascota no encontrada.";
    }
    public static class ResponseMessagesHealthController
    {
        public const string Message = "Activo my bro";

    }
    public static class ResponseMessagesPetsController
    {
        public const string MascotaNotFound = "Mascota no encontrada";
    }
    public static class ResponseMessagesUserPetsController
    {
        public const string MascotaUserNotFound = "Error al obtener las mascotas del usuario";
    }
    public static class ResponseMessagesUsersController
    {
        public const string UserIdNotFound = "No se encontró el usuario con ID";
        public const string DoctorNotFound = "Doctor no encontrado.";
        public const string AdminNotFound = "Administrador no encontrado.";
        public const string SecretarialNotFound = "Secretaria no encontrada.";
        public const string ErrorGettingUser = "Ocurrió un error interno al obtener el usuario.";
        public const string ErrorGettingUsers = "Ocurrió un error interno al obtener los usuarios.";
        public const string ExistingUsername = "El nombre de usuario ya está en uso.";
    }
    public static class ResponseMessagesUserPermissions
    {
        public const string UserNotFound = "No se encontró el usuario indicado.";
        public const string PermissionsRequired = "Debe indicar la lista de permisos.";
        public const string ErrorGettingPermissions = "Ocurrió un error interno al obtener los permisos del usuario.";
        public const string ErrorUpdatingPermissions = "Ocurrió un error interno al actualizar los permisos del usuario.";
    }
    public static class ResponseMessagesReportController
    {
        public const string Enabled = "activada";
        public const string Disabled = "desactivada";
        public const string AutoDeleteStatus = "La eliminación de reportes automática ha sido ";
        public const string AutoGenerateStatus = "La generación de reportes automática ha sido ";
        public static string RetentionDaysUpdated(int days) => $"Los reportes ahora se eliminarán tras {days} días.";
        public const string MinNumber = "La cantidad de días debe ser mayor a 0.";
    }
    public static class ResponseMessagesWorkerConfig
    {
        public const string WorkerConfigNotFound = "No se encontró configuración para el worker indicado.";
        public const string WorkerConfigUpdated = "Configuración del worker actualizada correctamente.";
        public const string InvalidIntervalValue = "El intervalo debe ser mayor a 0.";
    }
    public static class ResponseMessagesMoneyTypes
    {
        public const string MoneyDataNotFound = "Datos de tipo de moneda no encontrados.";
        public const string MoneyNotFound = "No hay datos de tipo de moneda registrados en el sistema.";
        public const string ErrorUpdatingMoneyType = "Ocurrió un error interno al actualizar el tipo de moneda.";
        public const string GetTasaBcvError = "No se pudo obtener la tasa del BCV. Intente nuevamente más tarde. \n Fuente: BCV Scraping.";
        public const string MoneyType = "USD";
        public const string fuente = "Banco Central de Venezuela (Real-time)";
        public const string ScrapingError = "Error al obtener datos del BCV. Intente nuevamente más tarde.";
        public const string InvalidId = "Error al obtener el tipo de moneda";
        public const string ErrorUpdate = "Error al actualizar el tipo de moneda";
        public const string BcvFallen = "BCV Caido";
        public const string UpdateTasaBcv = "Tasa BCV actualizada";
        public const string BcvRequestSuccess = "Se ha obtenido exitosamente la tasa del BCV.";
        public const string InvalidPrice = "El precio del dólar debe ser mayor a 0.";
        public const string APIBcvFail = "No se pudo obtener el precio del dolar en Bcv";
        public const string APIBcvFailDatails = "Servidor del BCV caido, la tasa en Bs. no se actualizo, puedes actualizarlo manualmente en Ajustes | Configuracion de Moneda.";

    }
    public static class ResponseMessageNotificactionPush
    {
        public const string Error = "Error";
        public const string Worker = "Worker";
        public const string System = "Sistema";
        public const string AllUsers = "ALL";
        public const string Admin = "Admin";
        public const string SE = "Secretaria";
        public const string DC = "Doctor";

    }
    public static class ResponseMessagesNotification
    {
        public const string NoNotifications = "No se encontraron notificaciones de alerta interna.";
        public const string ErrorGettingNotifications = "Ocurrió un error interno al obtener las notificaciones de alerta interna.";
        public const string NotDestination = "Sin destino";
        public const string ErrorCreatingNotification = "Ocurrió un error interno al crear la notificación de alerta interna.";
        public const string DuplicateNotification = "Ya existe una notificación idéntica enviada hace menos de 5 minutos.";
        public const string TitleRequired = "El título es obligatorio para crear una notificación.";
        public const string NotificationNotFound = "Notificación no encontrada.";
        public const string ErrorUpdatingNotification = "Ocurrió un error interno al actualizar la notificación de alerta interna.";
        public const string NotificationMarkedAsRead = "Notificación marcada como leída.";
    }

    public static class ResponseMessagesWSMessageAPI
    {
        public const string CitaNotFound = "La cita especificada no existe.";
        public const string ConsultaNotFound = "No se encontró ninguna consulta médica relacionada a los parámetros de la cita.";
        public const string FacturaNotFound = "La factura correspondiente a esta consulta aún no ha sido generada en el sistema.";
        public const string ClientErrorNumberPhone = "El cliente no posee un número telefónico o datos de contacto válidos.";
        public const string MessageTempleteDefault = "Hemos creado la factura correspondiente de la consulta de tu mascota";
        public const string PetsDefault = "Tu mascota";

        public const string MessageDefault = "Tu factura correspondiente a la consulta de {0} ya está lista";
        public const string FallbackMessageBase = "Tu factura correspondiente a la consulta de {0} ya está lista";
        public static string FormatMessage(string? template, string petName)
        {
            string activeTemplate = string.IsNullOrWhiteSpace(template) ? MessageDefault : template;

            try
            {
                return string.Format(activeTemplate, petName);
            }
            catch (System.FormatException)
            {
                return string.Format(FallbackMessageBase, petName);
            }
        }
    }
    public static class ResponseMessagesWSMessageController
    {
        public const string Success = "El envío del recibo a través de WhatsApp fue exitoso.";
        public const string PartialFailure = "El envio a WhatsApp fallo, intente mas tarde";
        public const string CriticalFailure = "Hubo un fallo crítico en el procesamiento y despacho del recibo.";
        public const string SessionInitSuccess = "Solicitud de inicialización de sesión enviada con éxito:";
        public const string SessionInitFailure = "No se pudo completar la inicialización de la sesión con el servidor de mensajería.";
        public const string SessionInitCriticalFailure = "Hubo un error crítico al procesar la solicitud de inicialización.";
        public const string SessionStatusNotFound = "No se encontró información o la instancia de WhatsApp no está inicializada.";
        public const string SessionStatusCriticalFailure = "Hubo un error crítico al recuperar el estado del canal.";
    }
    public static class ResponseMessagesNotificationPush
    {
        public const string PushTicket = "El usuario ${user_name} marco el ticket como visto";
        public const string Success = "Notificación push enviada correctamente.";
        public const string Failure = "No se pudo completar la inicialización de la sesión con el servidor de mensajería.";
        public const string CriticalFailure = "Hubo un error crítico al procesar la solicitud de inicialización.";
        public const string SessionNotFound = "No se encontró información o la instancia de WhatsApp no está inicializada.";
        public const string SessionStatusCriticalFailure = "Hubo un error crítico al recuperar el estado del canal.";
    }

    public static class ResponseMessagesEmailsController
    {
        public const string DispatchSuccess = "Correo enviado exitosamente al cliente {0}";
        public const string DispatchFailure = "No se pudo enviar el correo al cliente {0}";
        public const string DispatchCriticalFailure = "Fallo crítico al intentar despachar la factura por correo.";
        public const string MissingToParameter = "El parámetro 'to' es requerido.";
        public const string TestEmailSuccess = "Correo de prueba enviado con éxito a {0}.";
        public const string TestEmailFailure = "Error al intentar enviar el correo de prueba. Revisa los logs de la consola.";
        public const string TestEmailSubject = "Correo de Prueba - HappyPets";
        public const string TestEmailBody = "<h1>¡Hola!</h1><p>Esta es una prueba de envío exitosa de HappyPets utilizando la API de <strong>Resend</strong>.</p>";

        public const string ClientNoEmail = "El cliente no tiene un correo electrónico registrado para el envío de factura.";
        public const string PdfNotFound = "No se encontró el archivo PDF para la factura {0}.";
        public const string TemplateNotFound = "<h1>Plantilla no encontrada</h1>";
        public const string TemplateNotFoundById = "No se encontró la plantilla con el ID especificado.";
        public const string UpdateSuccess = "Plantilla de correo actualizada exitosamente.";
        public const string UpdateFailure = "Ocurrió un error al intentar actualizar la plantilla.";
        public const string InvalidPayload = "Datos de actualización inválidos.";

        public const string SendEmailError = "Se ha producido un error al intentar enviar el correo, por favor intente de nuevo.";
        public const string ProviderError = "ERROR. Hubo un error al intentar enviar el correo, por favor intente de nuevo.";
        public const string ConnectionFailed = "No se pudo establecer conexión con el proveedor de correos (Resend). Verifique su conexión de red.";
        public const string UnexpectedError = "Ocurrió un error inesperado al enviar el correo.";

        public const string ProviderErrorInvalidKey = "ERROR: La clave de la API de Resend no es válida o ha expirado. Por favor, revise la configuración en appsettings.json o en el panel del administrador.";
        public const string ProviderErrorInvalidTo = "ERROR: El correo de destino no es válido o tiene un dominio incorrecto (ej. un correo como 'admin@.dev' no existe). Por favor, verifique el correo del usuario.";
        public const string ProviderErrorSandboxRestricted = "ERROR: El servicio de correos (Resend) está en modo Sandbox de desarrollo. Solo se permite enviar correos a la dirección registrada en la cuenta de Resend.";
        public static string ProviderErrorValidation(string message) => $"ERROR de validación de correo: {message}";
        public static string ProviderErrorGeneric(string message) => $"ERROR: {message}";
    }

    public static class EmailTypes
    {
        public const int Recibos = 25;
        public const int ResetPassword = 15;
        public const int Bienvenido = 10;

        public static string GetEmailTypeString(int typeCode)
        {
            return typeCode switch
            {
                Recibos => "Recibos",
                ResetPassword => "ResetPassword",
                Bienvenido => "Bienvenido",
                _ => "Unknown"
            };
        }
    }

    public static class ResponseMessagesDashboard
    {
        public const string ErrorRetrievingStats = "Error al obtener las estadísticas del Dashboard.";
        public const string NoDefinido = "No Definido";
        public const string UnknownMonth = "Desconocido";
        public const string Cancelado = "cancelado";
        public static string InvalidGroupBy(string groupBy) => $"Agrupación '{groupBy}' inválida. Las opciones válidas son: por día, por semana, por mes o por año.";

        public static class Months
        {
            public const string Ene = "Ene";
            public const string Feb = "Feb";
            public const string Mar = "Mar";
            public const string Abr = "Abr";
            public const string May = "May";
            public const string Jun = "Jun";
            public const string Jul = "Jul";
            public const string Ago = "Ago";
            public const string Sep = "Sep";
            public const string Oct = "Oct";
            public const string Nov = "Nov";
            public const string Dic = "Dic";
        }

        public static class GroupBy
        {
            public const string Day = "day";
            public const string Week = "week";
            public const string Month = "month";
            public const string Year = "year";
        }
    }
    public static class ResponseMessagesUtilties
    {
        public const string ErroConverMoney = "Hubo un error al hacer la conversion de monedas";
    }
    public static class ResponseMessagesProfileController
    {
        public const string ProfileUpdated = "Perfil actualizado correctamente.";
        public const string InvalidUser = "Usuario no autenticado o sesión inválida.";
        public const string ErrorUpdatingProfile = "Ocurrió un error interno al actualizar el perfil.";
    }
    public static class ResponseMessagesSupabase
    {
        public const string EmptyFile = "El archivo de imagen es requerido.";
        public const string InvalidFileType = "Formato de imagen no soportado. Use JPG, PNG, WEBP o GIF.";
        public const string FileTooLarge = "La imagen supera el tamaño máximo permitido de 5MB.";
        public const string ErrorUploadingAvatar = "Ocurrió un error al subir la imagen a Supabase Storage.";
        public const string AvatarsBucket = "avatars";
        public static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png", "image/webp", "image/gif" };
        public const long MaxFileSizeBytes = 5 * 1024 * 1024;
        public const string SaveAvatarError = "No se pudo guardar el avatar: no existe el usuario con id {0}";
        // public const string
    }
    public static class ResponseMessagesCalendar
    {
        public const string ErrorProcessingCalendar = "Error al obtener el calendario:";
        public const string ErrorCheckingAvailability = "Error al verificar la disponibilidad del doctor:";
        public const string InvalidTimeFormat = "El formato de hora no es válido. Use HH:mm (ej: 10:00).";
        public const string DoctorNotFound = "Doctor no encontrado.";
        public const string RequiredId = "El id del doctor es requerido.";
        public const string RequiredDate = "La fecha es requerida.";
        public const string DoctorNotWorking = "El doctor no trabaja en la fecha especificada.";
        public const string DoctorIsBusy = "El doctor está ocupado en la fecha y hora especificada.";
        public const string DoctorAvailable = "El doctor está disponible en la fecha y hora especificadas.";
        public const string DoctorIsAvailable = "El doctor está disponible";
        public const string DoctorIsNotAvailable = "El doctor no está disponible";
    }
    public static class ResponseMessagesLogs
    {
        public const string LogRegistered = "Log registrado con éxito.";
        public const string ErrorLoadingLogs = "Error al obtener los logs del sistema.";
    }
    public static class ResponseMessagesVaccination
    {
        public const string VaccineNotFound = "Vacuna no encontrada.";
        public const string VaccineNameRequired = "El nombre de la vacuna es requerido.";
        public const string VaccineSpeciesRequired = "La especie de la vacuna es requerida.";
        public const string InvalidMinimumAge = "La edad mínima de inicio no puede ser negativa.";
        public const string InvalidBoosterFrequency = "La frecuencia de refuerzo debe ser mayor a cero.";
        public const string InvalidBoosterFrequencyUnit = "La unidad de frecuencia de refuerzo debe ser days, months o years.";
        public const string SchemeStageNotFound = "Etapa de esquema no encontrada.";
        public const string DoseNumberMustBePositive = "El número de dosis de la etapa debe ser mayor a cero.";
        public const string IntervalDaysMustBeNonNegative = "El intervalo de días entre dosis no puede ser negativo.";
        public const string InvalidStageType = "El tipo de etapa debe ser Initial o Booster.";
        public const string BatchNotFound = "Lote no encontrado.";
        public const string BatchExpirationMustBeFuture = "La fecha de vencimiento del lote debe ser posterior a hoy.";
        public const string BatchExpired = "El lote seleccionado está vencido y no puede aplicarse.";
        public const string BatchInactive = "El lote seleccionado está inactivo.";
        public const string BatchNoStock = "El lote seleccionado no tiene dosis disponibles en stock.";
        public const string MascotaNotFound = "Mascota no encontrada.";
        public const string PetVaccinationNotFound = "Registro de vacunación no encontrado.";
        public const string PostponementReasonRequired = "El motivo de la postergación es requerido.";
        public const string AlreadyPostponed = "Esta vacunación ya se encuentra postergada.";
        public const string NoConsultaLinked = "El registro de vacunación no está asociado a una consulta.";
        public const string FacturaNotFoundForConsulta = "No existe una factura generada para la consulta de esta vacunación.";
        public const string AlreadySentToInvoice = "Esta vacunación ya fue enviada a una factura.";
        public static string ReminderEmailSubject(string systemName) => $"Recordatorio de vacunación - {systemName}";
        public static string ReminderEmailBody(string petName, string vaccineName, string dueDate) =>
            $"<p>Hola,</p><p>Te recordamos que <strong>{petName}</strong> tiene programada la vacuna <strong>{vaccineName}</strong> para el día <strong>{dueDate}</strong>.</p><p>Agenda tu cita con anticipación.</p>";
        public static string PendingReminderNotificationTitle(string petName) => $"Refuerzo próximo: {petName}";
        public static string PendingReminderNotificationMessage(string petName, string vaccineName, string dueDate) =>
            $"La mascota {petName} tiene el refuerzo de {vaccineName} programado para el {dueDate}. Da seguimiento telefónico al cliente.";
        public static string DayBeforeReminderEmailSubject(string systemName) => $"Recordatorio de vacunación mañana - {systemName}";
        public static string DayBeforeReminderNotificationTitle(string petName) => $"Vacuna mañana: {petName}";
        public static string DayBeforeReminderNotificationMessage(string petName, string vaccineName, string dueDate) =>
            $"La mascota {petName} tiene programada la vacuna {vaccineName} para mañana {dueDate}. Confirma la asistencia con el cliente.";
    }
    public static class ResponseMessagesEspecie
    {
        public const string EspecieNombreRequired = "El nombre de la especie es requerido.";
        public const string EspecieMaxLength = "El nombre de la especie no puede exceder los 50 caracteres.";
        public const string EspecieAlreadyExists = "Ya existe una especie con ese nombre.";
        public const string EspecieNotFound = "Especie no encontrada.";
    }
}
