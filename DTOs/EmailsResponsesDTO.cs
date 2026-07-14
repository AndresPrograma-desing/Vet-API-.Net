using System;
using System.Text.Json.Serialization;


namespace DTOs;

public record EmailsResponsesDTO
{
    public int Id { get; set; }
    public string? HtmlCode { get; set; }
    public string? TypeEmail { get; set; }
    public DateTime Update { get; set; }
}