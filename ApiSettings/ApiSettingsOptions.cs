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
    public int PasswordRecoveryExpirationMinutes { get; set; } = 1;
    public string? GroqApiKey { get; set; }
    public string? GroqApiUrl { get; set; }
    public string? GroqModel { get; set; }
}

public class TokenTemporalOptions
{
    public const string SectionName = "token-temporal";
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Id { get; set; } = 999;
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
    public string? AvatarUrl { get; set; }
}
public class TemplatesHTML
{
    public const string SectionName = "TemplatesHTML";
    public string? Recibos { get; set; }
    public string? ResetPassword { get; set; }
    public string? ResendPassword { get; set; }
    public string? ConfirmChangePass { get; set; }
    public string? ConfirmChangePassPage { get; set; }
    public string? BirthdayPets { get; set; }

}

public class SupabaseSettingsOptions
{
    public const string SectionName = "Supabase";
    public string? Url { get; set; }
    public string? Key { get; set; }
    public string? Bucket { get; set; } = "avatars";
}
