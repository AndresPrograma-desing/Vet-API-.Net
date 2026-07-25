using System;

/*
 describe: 
 - Este archivo es para configurar las variables de entorno del worker,
   Su funcion es traer las variables de entorno del archivo appsettings.json y
   agruparlas en clases para su facil acceso
*/


namespace vet_api_Net.WorkerSettings
{
    public class WorkerSetting
    {
        public int IntervalValues { get; set; }
        public string? IntervalUnits { get; set; }
        public int RetentionValues { get; set; }
        public string? RetentionUnits { get; set; }
        public bool Enabled { get; set; }
    }

}