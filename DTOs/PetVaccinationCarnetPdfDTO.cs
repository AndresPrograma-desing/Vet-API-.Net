using System;
using System.Collections.Generic;

namespace DTOs;

public record PetVaccinationCarnetPdfDTO
{
    public int MascotaId { get; set; }
    public string MascotaNombre { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public string? Raza { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public string? ClienteTelefono { get; set; }
    public List<PetVaccinationCarnetItemDTO> Items { get; set; } = new();
}

public record PetVaccinationCarnetItemDTO
{
    public string VaccineName { get; set; } = string.Empty;
    public DateOnly ApplicationDate { get; set; }
    public string? BatchNumber { get; set; }
    public string? Laboratory { get; set; }
    public DateOnly? NextDoseDate { get; set; }
    public string? DoctorName { get; set; }
}
