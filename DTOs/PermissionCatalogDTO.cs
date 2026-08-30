using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DTOs;

public record PermissionItemDTO
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;
}

public record PermissionModuleDTO
{
    [JsonPropertyName("module")]
    public string Module { get; set; } = string.Empty;

    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;

    [JsonPropertyName("permissions")]
    public List<PermissionItemDTO> Permissions { get; set; } = new();
}

public record RoleCatalogItemDTO
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;
}

public record PermissionCatalogResponseDTO
{
    [JsonPropertyName("modules")]
    public List<PermissionModuleDTO> Modules { get; set; } = new();

    [JsonPropertyName("roles")]
    public List<RoleCatalogItemDTO> Roles { get; set; } = new();
}
