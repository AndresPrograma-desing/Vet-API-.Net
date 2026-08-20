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
        public const string DeleteReportWorker = "DeleteReportWorker";
        public const string AutoGenerateReportWorker = "AutoGenerateReportWorker";
        public const string VaccinationReminderWorker = "VaccinationReminderWorker";
    }

    public static class VaccinationVariables
    {
        public const int AlertWindowDays = 30; // Ventana amarilla: próxima a vencer (0-30 días)
        public const int ReminderSevenDays = 7;
        public const int ReminderThreeDays = 3;
        public const string StatusApplied = "Applied";
        public const string StatusPostponed = "Postponed";
        public const string StageInitial = "Initial";
        public const string StageBooster = "Booster";
        public const string TrafficLightGreen = "Green";
        public const string TrafficLightYellow = "Yellow";
        public const string TrafficLightRed = "Red";
        public const string FrequencyDays = "days";
        public const string FrequencyMonths = "months";
        public const string FrequencyYears = "years";
    }
}