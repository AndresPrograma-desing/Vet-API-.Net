using System;

/*
 describe: 
 - Este archivo es para configurar las rutas de los endpoints, 
   Organiza los strings de las rutas de los endpoints en clases para su facil orden y escalabilidad
*/

namespace vet_api_Net.Routes
{
    public static class Endpoints
    {
        public static class Auth
        {
            public const string Login = "login";
            public const string Me = "me";
            public const string UpdatePassword = "update-password";
            public const string TempToken = "Token";
        }
        public static class Users
        {
            public const string Create = "create";
            public const string Secretaries = "secretarias";
            public const string UserDisabled = "{id}/disable";
            public const string UserEnabled = "{id}/enable";
            public const string Delete = "{id}";
            public const string Status = "{id}/status";
            public const string RequestPasswordReset = "request-password-reset";
            public const string ConfirmTicket = "confirm-ticket";
            public const string ResetStatus = "reset-status/{email}";
            public const string AssignNewPassword = "assign-new-password";
            public const string PendingResets = "pending-resets";
            public const string UpdateUserName = "update-user-name";
            public const string GetRoles = "roles";
        }

        public static class UserPermissions
        {
            public const string GetByUser = "{userId}";
            public const string Update = "{userId}";
        }

        public static class Permissions
        {
            public const string Catalog = "permission";
        }

        public static class Citas
        {
            public const string GetAllRequests = "requests";
            public const string Delete = "{id}";
            public const string Update = "update/{id}";
            public const string Finalizar = "{id}/finalizar";
            public const string NewStatus = "{id}/statusnew";
            public const string Status = "{id}/status";
            public const string Create = "create";
            public const string CitaToday = "today";
            public const string Statuses = "statuses";
            public const string Types = "types";
            public const string PortalByCliente = "portal/cliente/{clienteId}";
        }
        public static class Client
        {
            public const string GetById = "{id}";
            public const string GetMascotas = "{id}/mascotas";
            public const string ValidateClient = "Validate";
        }
        public static class Clients
        {
            public const string Update = "update/{id}";
            public const string Delete = "delete/{id}";
        }
        public static class ClientPets
        {
            public const string CreateWithPet = "create-with-pet";
            public const string CreatePetForExistingClient = "add-pet";
            public const string GetClientsWithPetsLookup = "lookup/clients";
        }
        public static class Consultas
        {
            public const string Create = "create";
            public const string GetById = "{id}";
            public const string UpdateReceta = "{id}/receta";
        }
        public static class Facturas
        {
            public const string CitaId = "cita/{citaId}";
            public const string CitaPdf = "cita/{citaId}/pdf";
            public const string Files = "files";
            public const string Download = "files/download/{fileName}";
            public const string StatusPago = "{id}/estado-pago";
            public const string Settings = "settings-facturas";
            public const string UpdateSettings = "settings-facturas/auto-delete";
        }
        public static class UsersPetsController
        {
            public const string UserPets = "{nombre}/pets";
        }
        public static class MassagesController
        {
            public const string Conversation = "{userId}/{otherId}/conversations";
            public const string UserMessages = "{userId}/message-user";
            public const string MarkRead = "{id}/read";
        }
        public static class PetsController
        {
            public const string GetById = "{id}";
            public const string Update = "update/{id}";
            public const string Delete = "delete/{id}";
        }
        public static class ProductsController
        {
            public const string Create = "Create";
            public const string Delete = "delete/{id}";
            public const string Quantity = "{id}/quantity";
            public const string UpdateProducto = "update/{id}";
            public const string Categories = "categories";
        }
        public static class ReportController
        {
            public const string GetById = "{id}";
            public const string Delete = "{id}";
            public const string GenerateFull = "generate-full";
            public const string GenerateFullReport = "full/{id}";
            public const string CreateExcel = "excel/{id}";
            public const string GetExcel = "excel/{id}";
            public const string UpdateRetentionDays = "settings/retention-days";
            public const string ToggleAutoDelete = "settings/autodelete";
            public const string ToggleAutoGenerate = "settings/autogenerate";
            public const string Settings = "settings";
        }
        public static class WorkerConfigController
        {
            public const string GetByName = "{workerName}";
            public const string UpdateByName = "{workerName}";
        }
        public static class ExcellController
        {
            public const string GenerateFull = "generate-full";
            public const string CreateExcel = "excel/{id}";
            public const string GetExcel = "excel/{id}";
            public const string DownloadDashboard = "download-dashboard";
            public const string DownloadCitas = "download-citas";
            public const string DownloadClientes = "download-clientes";
        }
        public static class PdfController
        {
            public const string DownloadConsulta = "consulta/{id}";
            public const string DownloadCredencial = "credencial/{id}";
            public const string DownloadCarnet = "mascota/{mascotaId}/carnet";
        }
        public static class Vaccine
        {
            public const string Create = "create";
            public const string GetById = "{id}";
            public const string Update = "{id}";
            public const string Delete = "{id}";
            public const string Batches = "{id}/batches";
            public const string CreateBatch = "{id}/batches";
            public const string SchemeStages = "{id}/scheme-stages";
            public const string DeleteSchemeStage = "{id}/scheme-stages/{stageId}";
            public const string GetSpecies = "species";
            public const string CreateSpecies = "species";
        }
        public static class PetVaccination
        {
            public const string Register = "register";
            public const string Postpone = "{id}/postpone";
            public const string SendToInvoice = "{id}/send-to-invoice";
            public const string StatusByMascota = "mascota/{mascotaId}/status";
            public const string HistoryByMascota = "mascota/{mascotaId}/history";
            public const string PendingThisWeek = "pending-this-week";
        }
        public static class NowGrod
        {
            public const string Analyze = "analyze";
            public const string Chat = "chat";
            public const string Consultar = "consultar";
            public const string SessionToken = "session-token";
        }
        public static class HistoriaClinica
        {
            public const string GetById = "{id}";
            public const string GetByMascota = "mascota/{mascotaId}";
            public const string Create = "create";
            public const string Update = "{id}";
            public const string GetByMascotaId = "mascota/{mascotaId}";
            public const string UpdateNotes = "{id}/notes";
            public const string RefreshIa = "mascota/{mascotaId}/refrescar";
        }
        public static class IaConocimiento
        {
            public const string GetAll = "";
            public const string GetById = "{id}";
            public const string Create = "create";
            public const string Update = "{id}";
            public const string Categoria = "categoria/{categoria}";
            public const string Guardar = "guardar";
        }

        public static class MoneyController
        {
            public const string Update = "update";
            public const string TasaDollarBcv = "tasa-dollar-bcv-scraping";
            public const string TasaDollarBcvToDb = "tasa-dollar-bcv";
            public const string UpdateBcvManual = "tasa-dollar-bcv/manual";
            public const string ScrapeAndSaveBcv = "bcv-dollar-rate/scrape";
        }
        public static class NotificationController
        {
            public const string Create = "Created";
            public const string MarkAsRead = "{alertId}/mark-as-read";
            public const string NotificactionByUser = "user/{userId}/notifications";
        }
        public static class WSMessageController
        {
            public const string DispatchInvoiceByCita = "dispatch/appointment/{citaId}";
            public const string SessionInit = "session/init";
            public const string SessionStatus = "session/status";
        }

        public static class EmailsController
        {
            public const string DispatchEmail = "send-email/{entityId}";
            public const string TestEmail = "test-email";
            public const string GetTemplates = "templates";
            public const string GetTemplateById = "templates/{id}";
            public const string UpdateTemplate = "templates/{id}";
            public const string DataResend = "data-resend";
            public const string UpdateResend = "update-resend";
        }
        public static class PasswordRecovery
        {
            public const string RequestCode = "request-code";
            public const string VerifyCode = "verify-code";
            public const string ResetPassword = "reset-password";
        }
        public static class DashboardController
        {
            public const string Stats = "dashboard";
        }
        public static class ProfileController
        {
            public const string UpdateProfile = "update-profile";
        }
        public static class Calendar
        {
            public const string GetCalendar = "get-calendar";
            public const string CheckAvailability = "check-availability";
        }
        public static class Logs
        {
            public const string GetLogs = "get-logs";
        }

    }
}