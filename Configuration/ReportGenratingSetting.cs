namespace vet_api_Net.ReportSettings
{
    public class ReportGeneratingSetting
    {
        public int Id { get; set; } 
        public int DaysBeforeDeletion { get; set; } = 30; 
       public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}