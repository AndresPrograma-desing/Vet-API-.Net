namespace vet_api_Net.Infrastructure.Configuration;

public class ApiSettingsOptions
{
    public const string SectionName = "ApiSettings";
    public string? DateFormat { get; set; }
    public string? TimeFormat { get; set; }
    public string? USD { get; set;}
    public string? VES { get; set;}
    public string? ApiName { get; set; }
    public string? SystemName { get; set; }
}