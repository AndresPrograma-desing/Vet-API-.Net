using System.Text.Json.Serialization;

namespace DTOs;

public record AuthClientsDTO
{
   public string? Password {get; set;}
   public string? Email {get; set;} 
   public int Cedula {get; set;}
}