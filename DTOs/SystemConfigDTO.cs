using System;

namespace DTOs;

public record SystemConfigDTO
{
    public int Id {get; set;}
    public string? FrontendUrl {get; set;}
    public string? BackendExternalUrl {get; set;}

}