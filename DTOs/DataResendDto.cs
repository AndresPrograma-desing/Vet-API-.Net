using System;

namespace DTOs;

public class DataResendDto
{
    public string ClientEmail { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string UrlResend { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = string.Empty;
    public bool Active { get; set; } = false;
}
