using System;
// This file contains all the constant response messages used across the application for consistency and maintainability.
// Este archivo contiene todos los mensajes de respuesta constantes utilizados en toda la aplicación para garantizar la coherencia y la facilidad de mantenimiento.
namespace vet_api_Net.Constants
{
    public static class ResponseErrors
    {
        public const string InternalServerError = "Error interno del servidor.";
        public const string NotFound = "Recurso no encontrado.";
        public const string BadRequest = "Solicitud incorrecta.";
        public const string Unauthorized = "No autorizado.";
        public const string Forbidden = "Prohibido.";
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
        public const string GenericError = "Ocurrió un error interno, por favor intenta nuevamente.";
        public const string ErrorSavingPassword = "Ocurrió un error interno al guardar la contraseña.";


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

    }
    public static class ResponseMessagesUsers
    {
        public const string UserNotFound = "Usuario no encontrado.";
        public const string DoctorNotFound = "Doctor no encontrado.";
        public const string AdminNotFound = "Administrador no encontrado.";
        public const string SecretarialNotFound = "Secretaria no encontrada.";
        public const string ErrorGettingUser = "Ocurrió un error interno al obtener el usuario.";
        public const string ErrorGettingUsers = "Ocurrió un error interno al obtener los usuarios.";
        public const string ExistingUsername = "El nombre de usuario ya está en uso.";
        public static string DeletingUser(int id) => $"Usuario con ID {id} eliminado exitosamente";
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
        public const string ConsultaNotFound = "Consulta no encontrada para facturación.";
        public const string CitaNotFound = "Cita no encontrada para facturación.";
        public const string MascotaNotFound = "Mascota no encontrada para facturación.";
        public const string OnlyCitaAllowed = "La factura solo puede generarse para citas completadas.";
        public const string ErrorProcessingInvoice = "Error al procesar la facturación interna. Consulta creada pero no se pudo generar la factura.";
        public const string InvoiceGenerationError = "Error al guardar la información de la factura generada. Sin embargo, el PDF se ha creado correctamente.";

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
        public const string Factura = "facturas";
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
        public const string Facturas = "Facturas";
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

    }
    public static class ResponseMessagesFactura
    {
        public const string FacturaCreada = "Factura creada exitosamente.";
        public const string FacturaNoEncontrada = "Factura no encontrada.";
        public const string ErrorInterno = "Ocurrió un error interno al procesar la solicitud de factura.";
        public const string FacturaActualizada = "Factura actualizada exitosamente.";
        public const string EmptyProducts = "No se pueden generar facturas sin productos asociados.";
        public const string NoData = "N/A";
        public const int EmptyProductId = 0;
        public const string EmptyProductName = "Sin productos asociados";
        public const string EmptyProductDescription = "No se encontraron productos vinculados a esta factura.";
        public const decimal EmptyProductPrice = 0.00m;
        public const int EmptyProductQuantity = 0;
        public const int EmptyProductTotal = 0;
        public const string Notes = "No hay notas disponibles";
        public const string NotFactura = "No hay facturas registradas en el sistema.";
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
    }
    public static class ResponseMessagesAuthController
    {
        public const string Unauthorized = "Email, Contraseña Actual y Nueva Contraseña son requeridos.";
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
    public static class ResponseMessagesReportController
    {
        public const string Enabled = "activada";
        public const string Disabled = "desactivada";
        public const string AutoDeleteStatus = "La eliminación de reportes automática ha sido ";
        public const string AutoGenerateStatus = "La generación de reportes automática ha sido ";
        public static string RetentionDaysUpdated(int days) => $"Los reportes ahora se eliminarán tras {days} días.";
        public const string MinNumber = "La cantidad de días debe ser mayor a 0.";
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
        public const string Success = "El envío del comprobante a través de WhatsApp fue exitoso.";
        public const string PartialFailure = "La factura se procesó localmente pero el envío por WhatsApp falló.";
        public const string CriticalFailure = "Hubo un fallo crítico en el procesamiento y despacho del comprobante.";
        public const string SessionInitSuccess = "Solicitud de inicialización de sesión enviada con éxito.";
        public const string SessionInitFailure = "No se pudo completar la inicialización de la sesión con el servidor de mensajería.";
        public const string SessionInitCriticalFailure = "Hubo un error crítico al procesar la solicitud de inicialización.";
        public const string SessionStatusNotFound = "No se encontró información o la instancia de WhatsApp no está inicializada.";
        public const string SessionStatusCriticalFailure = "Hubo un error crítico al recuperar el estado del canal.";
    }
}