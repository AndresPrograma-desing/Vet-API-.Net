using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

//Describe: DTOs para estructurar las respuestas del calendario del sistema, sus días, slots de agenda y detalles simplificados de las citas.
namespace DTOs;

public record CalendarResponseDTO
{
    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("month")]
    public int Month { get; set; }

    [JsonPropertyName("doctor_id")]
    public int? DoctorId { get; set; }

    [JsonPropertyName("doctor_name")]
    public string? DoctorName { get; set; }

    [JsonPropertyName("years")]
    public List<int> Years { get; set; } = new();

    [JsonPropertyName("months")]
    public List<MonthInfoDTO> Months { get; set; } = new();

    [JsonPropertyName("days")]
    public List<CalendarDayDTO> Days { get; set; } = new();
}

public record MonthInfoDTO
{
    [JsonPropertyName("number")]
    public int Number { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public record CalendarDayDTO
{
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty; 

    [JsonPropertyName("day_number")]
    public int DayNumber { get; set; }

    [JsonPropertyName("month_number")]
    public int MonthNumber { get; set; }

    [JsonPropertyName("year_number")]
    public int YearNumber { get; set; }

    [JsonPropertyName("is_current_month")]
    public bool IsCurrentMonth { get; set; }

    [JsonPropertyName("day_name")]
    public string DayName { get; set; } = string.Empty;     

    [JsonPropertyName("has_appointments")]
    public bool HasAppointments { get; set; }

    [JsonPropertyName("slots")]
    public List<CalendarDTO.AgendaSlotDto> Slots { get; set; } = new();
}

public record CalendarDTO
{
    public class AgendaSlotDto
    {
        [JsonPropertyName("hour")]
        public string Hour { get; set; } = string.Empty; 

        [JsonPropertyName("display_hour")]
        public string DisplayHour { get; set; } = string.Empty;  

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty; 

        [JsonPropertyName("cita")]
        public CitaDetalleDto? Cita { get; set; }
    }

    public class CitaDetalleDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("paciente_nombre")]
        public string PacienteNombre { get; set; } = string.Empty;

        [JsonPropertyName("cliente_nombre")]
        public string ClienteNombre { get; set; } = string.Empty;

        [JsonPropertyName("motivo")]
        public string Motivo { get; set; } = string.Empty;

        [JsonPropertyName("estado")]
        public string Estado { get; set; } = string.Empty;
    }

    public class DiaAgendaDto
    {
        [JsonPropertyName("fecha")]
        public string Fecha { get; set; } = string.Empty;

        [JsonPropertyName("formatted_date")]
        public string FormattedDate { get; set; } = string.Empty;

        [JsonPropertyName("is_doctor")]
        public bool IsDoctor { get; set; }

        [JsonPropertyName("doctor_name")]
        public string? DoctorName { get; set; }

        [JsonPropertyName("slots")]
        public List<AgendaSlotDto> Slots { get; set; } = new();
    }

    public class AgendaRangoFiltroDto
    {
        [JsonPropertyName("fecha_inicio")]
        public DateTime FechaInicio { get; set; }

        [JsonPropertyName("fecha_fin")]
        public DateTime FechaFin { get; set; }

        [JsonPropertyName("veterinario_id")]
        public int? VeterinarioId { get; set; }
    }
    public class ValidateSlotResponseDTO
    {
        public bool IsOccupied { get; set; }
        public string Message { get; set; } = string.Empty;
        public string SuggestedNextAvailableTime { get; set; } = string.Empty; 
    }
}