using System;

namespace DTOs;

public class PushNotificationDTO
{
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? UserId { get; set; }
    public string? NameUser { get; set; }
    public int? AlertId { get; set; }
}