using System;
using System.Text.Json.Serialization;

namespace DTOs;

public class ProcessEventsDTO
{
    public string NameEvent { get; set; } = string.Empty;
    public string TypeEvent { get; set; } = string.Empty;
    public string? Tittle { get; set; }
    public string? Error { get; set; }
    public string? ProcessWhitError { get; set; }
    public int? Intents { get; set; }
    public string? VinculationData { get; set; }
    public DateTime Created { get; set; }
    public DateTime Resolved { get; set; }
    public DateTime ultimateIntent { get; set;}
    
}