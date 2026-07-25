
/*
 describe: 
 - Este archivo es para configurar las variables de entorno de la generacion de reportes,
   Su funcion es traer las variables de entorno del archivo appsettings.json y
   agruparlas en clases para su facil acceso
*/

namespace vet_api_Net.ReportSettings
{
    public class ReportGeneratingSetting
    {
        public int Id { get; set; }
        public int DaysBeforeDeletion { get; set; } = 30;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}