using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vet_api_Net.Models
{
    [Table("system_configs")]
    public class SystemConfig
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("frontend_url")]
        [StringLength(512)]
        public string? FrontendUrl { get; set; }

        [Column("backend_external_url")]
        [StringLength(512)]
        public string? BackendExternalUrl { get; set; }

        [Column("last_updated")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        [Column("bcv_api_url")]
        [StringLength(512)]
        public string? BcvApiUrl { get; set; }

        [Column("email_provider_api_url")]
        [StringLength(512)]
        public string? EmailProviderApiUrl { get; set; }

        [Column("email_provider_api_key")]
        [StringLength(512)]
        public string? EmailProviderApiKey { get; set; }

        [Column("email_provider_from_email")]
        [StringLength(512)]
        public string? EmailProviderFromEmail { get; set; }
    }
}
