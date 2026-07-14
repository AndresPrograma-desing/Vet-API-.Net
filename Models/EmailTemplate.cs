using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace vet_api_Net.Models;

public partial class EmailTemplate
{
    public int Id { get; set; }
    public string HtmlCode { get; set; } = "<h1> Email template vacio <h1>";
    public string? TypeEmail { get; set; }
    public DateTime Update { get; set; }
    public DateTime CreatedAt { get; set; }
}