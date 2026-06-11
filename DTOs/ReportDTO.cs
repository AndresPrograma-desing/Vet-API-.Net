using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DTOs;

public record ReportDTO
{
    public int Id { get; set; }
    public string? ReportName { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Data { get; set; }
}