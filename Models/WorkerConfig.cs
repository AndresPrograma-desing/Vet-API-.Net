using System.ComponentModel.DataAnnotations;

namespace vet_api_Net.Models
{
    public class WorkerConfig
    {
        [Key]
        public int Id { get; set; }
        public string WorkerName { get; set; } = string.Empty;
        public bool IsEnabled { get; set; } = true;
        public int IntervalValue { get; set; } = 30;
        public string IntervalUnit { get; set; } = "minutes";
        public int? RetentionValue { get; set; }
        public string? RetentionUnit { get; set; }
        public bool? GenerateEnabled { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
