using System;

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