using System;

/*
 describe: 
 - Este archivo es para configurar las variables del sistema, 
   Organiza los strings de las variables en clases para su facil orden y escalabilidad
*/

namespace vet_api_Net.Constants
{
    public static class Variables
    {
        public const int TargetId = 1; // Esta variable se encarga de tener o pasar el valor constante de la fila o columna de la tabla de configuracion de monedas
    }

    public static class PaginationVariables
    {
        public const int DefaultPageNumber = 1;
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 100;
    }

    public static class WorkerNames
    {
        public const string DeleteFacturaWorker = "DeleteFacturaWorker";
        public const string DeleteConsultaPdfWorker = "DeleteConsultaPdfWorker";
        public const string DeleteReportWorker = "DeleteReportWorker";
        public const string AutoGenerateReportWorker = "AutoGenerateReportWorker";
        public const string VaccinationReminderWorker = "VaccinationReminderWorker";
        public const string HistoriaClinicaSummaryWorker = "HistoriaClinicaSummaryWorker";
        public const string ProcessEventsWorker = "ProcessEventsWorker";
        public const string BcvWorker = "BcvWorker";
        public const string ClearNotificationsWorker = "ClearNotificationsWorker";
    }

    public static class ProcessEventTypes
    {
        public const string Error = "Error";
        public const string Warning = "Warning";
        public const string Success = "Success";
    }

    public static class AiProviders
    {
        public const string Gemini = "gemini";
    }

    public static class VaccinationVariables
    {
        public const int AlertWindowDays = 30; // Ventana amarilla: próxima a vencer (0-30 días)
        public const int ReminderSevenDays = 7;
        public const int ReminderThreeDays = 3;
        public static readonly TimeSpan DayBeforeReminderTime = new TimeSpan(17, 0, 0);
        public const string StatusApplied = "Applied";
        public const string StatusPostponed = "Postponed";
        public const string StageInitial = "Initial";
        public const string StageBooster = "Booster";
        public const string TrafficLightGreen = "#22C55E";
        public const string TrafficLightYellow = "#EAB308";
        public const string TrafficLightRed = "#EF4444";
        public const string FrequencyDays = "days";
        public const string FrequencyMonths = "months";
        public const string FrequencyYears = "years";
    }
}