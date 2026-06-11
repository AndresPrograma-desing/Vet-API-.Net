using System.ComponentModel.DataAnnotations;

namespace vet_api_Net.Models
{
    public class ReporConfig
    {
        [Key]
        public int Id { get; set; }
        public int Days { get; set; } = 30;
        public bool IsEnabled { get; set; } = true;
        public bool GenerateEnabled { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}