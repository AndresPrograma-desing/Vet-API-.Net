using System;

namespace DTOs;

public record LoginSecurityDTO
{
    public string? Message {get; set;}
    public int TryAgainTime {get; set;}
}