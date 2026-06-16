namespace vet_api_Net.Infrastructure.Configuration;

public class ApiSettingsOptions
{
    public const string SectionName = "ApiSettings";
    public string? DateFormat { get; set; }
    public string? TimeFormat { get; set; }
    public string? USD { get; set; }
    public string? VES { get; set; }
    public string? ApiName { get; set; }
    public string? SystemName { get; set; }
}

public class TokenTemporalOptions
{
    public const string SectionName = "token-temporal";
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Id = 999;
    public string Rol { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
}

public class SeedDataOptions
{
    public const string SectionName = "SeedData";
    public bool Initialize { get; set; } = false;
    public Dictionary<string, UserData> DummyData { get; set; } = new Dictionary<string, UserData>();
}

public class UserData
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
}