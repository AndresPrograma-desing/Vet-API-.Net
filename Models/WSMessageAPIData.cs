using System;
using System.ComponentModel.DataAnnotations;

namespace vet_api_Net.Models
{
    public class WSMessageAPIData
    {
        [Key]
        public string? ClientId { get; set; }
        public string? ApiKey { get; set; }
        public string? Message { get; set; }// es nuevo
        public DateTime Update { get; set; }
    }

}