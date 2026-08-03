using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using Microsoft.Extensions.Options;
using vet_api_Net.Constants;
using vet_api_Net.Infrastructure.Configuration;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Models;
using vet_api_Net.Utilities;
using static DTOs.CalendarDTO;

//Describe: Servicio de calendario que orquesta la lógica para construir los días de la agenda mensual y slots ocupados de doctores.
namespace vet_api_Net.Services;

//Este servicio se introdujo una forma mas corta de declarar los constructores en C# 12, menos codigo

public class CalendarService(ICalendarRepository calendarRepository, IOptions<ApiSettingsOptions> apiSettings) : ICalendarService
{
private readonly ICalendarRepository _calendarRepository = calendarRepository;
private readonly ApiSettingsOptions _apiSettings = apiSettings.Value;
public async Task<CalendarResponseDTO> GetCalendarAsync(int year, int month, int? doctorId)
{
    if (year <= 0) year = DateTime.Now.Year;
    if (month < 1 || month > 12) month = DateTime.Now.Month;

    string? doctorName = null;
    if (doctorId.HasValue)
    {
        var doctor = await _calendarRepository.GetDoctorByIdAsync(doctorId.Value);
        if (doctor != null)
        {
            doctorName = $"{doctor.Nombre} {doctor.Apellido}".Trim();
        }
    }

    var firstDayOfMonth = new DateTime(year, month, 1);
    int diff = (7 + (int)firstDayOfMonth.DayOfWeek - (int)DayOfWeek.Monday) % 7;
    var startDate = firstDayOfMonth.AddDays(-diff);
    var endDate = startDate.AddDays(41);

    var citas = await _calendarRepository.GetCitasForDoctorAsync(startDate, endDate, doctorId);


    var daysList = new List<CalendarDayDTO>();
    var currentDate = startDate;

    var cultureInfo = new CultureInfo("es-ES");

    for (int i = 0; i < 42; i++)
    {
        var dateStr = currentDate.ToString(_apiSettings.DateFormat);
        var dayAppointments = citas.Where(c => c.FechaCita.Date == currentDate.Date).ToList();

        var allSlots = GenerateDailyAgenda(dayAppointments, MapToCitaDetalleDto);

        var rawDayName = cultureInfo.DateTimeFormat.GetDayName(currentDate.DayOfWeek);
        var dayName = !string.IsNullOrEmpty(rawDayName)
            ? char.ToUpper(rawDayName[0]) + rawDayName[1..]
            : string.Empty;


        daysList.Add(new CalendarDayDTO
        {
            Date = dateStr,
            DayNumber = currentDate.Day,
            MonthNumber = currentDate.Month,
            YearNumber = currentDate.Year,
            IsCurrentMonth = currentDate.Month == month && currentDate.Year == year,
            DayName = dayName,
            HasAppointments = allSlots.Any(s => s.Status == Status.Busy),
            Slots = allSlots
        });

        currentDate = currentDate.AddDays(1);
    }

    int currentSystemYear = DateTime.Now.Year;
    var availableYears = Enumerable.Range(currentSystemYear - 3, 7).ToList();

    var monthsList = CalendarHelperUtilities.GetMonths();

    return new CalendarResponseDTO
    {
        Year = year,
        Month = month,
        DoctorId = doctorId,
        DoctorName = doctorName,
        Years = availableYears,
        Months = monthsList,
        Days = daysList
    };
}

private static string FormatTimeTo12h(TimeOnly time)
{
    return time.ToString("hh:mm tt", CultureInfo.InvariantCulture);
}

private CitaDetalleDto MapToCitaDetalleDto(Cita cita)
{
    return new CitaDetalleDto
    {
        Id = cita.Id,
        PacienteNombre = cita.Mascota?.Nombre ?? string.Empty,
        ClienteNombre = cita.Mascota?.Cliente != null
            ? $"{cita.Mascota.Cliente.Nombre} {cita.Mascota.Cliente.Apellido}".Trim()
            : string.Empty,
        Motivo = cita.Motivo ?? string.Empty,
        Estado = cita.Estado ?? string.Empty
    };
}
public async Task<ValidateSlotResponseDTO> CheckDoctorAvailabilityAsync(
    int doctorId,
    DateTime date,
    TimeOnly requestedTime,
    int durationMinutes = 30)
{
    var requestedStart = date.Date.Add(requestedTime.ToTimeSpan());
    var requestedEnd = requestedStart.AddMinutes(durationMinutes);

    var citasDelDia = await _calendarRepository.GetCitasForDoctorAsync(date.Date, date.Date, doctorId);
    var citasActivas = citasDelDia
        .Where(c =>
            !string.Equals(c.Estado, Status.Cancelled, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(c.Estado, Status.Completed, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(c.Estado, Status.NotAssisted, StringComparison.OrdinalIgnoreCase))
        .Where(c =>
        {
            var existingEnd = c.FechaCita.Date.Add(c.HoraCita.ToTimeSpan()).AddMinutes(30);
            return existingEnd > DateTime.Now;
        })
        .ToList();

    foreach (var cita in citasActivas)
    {
        var existingStart = cita.FechaCita.Date.Add(cita.HoraCita.ToTimeSpan());
        var existingEnd = existingStart.AddMinutes(30);

        if (requestedStart < existingEnd && requestedEnd > existingStart)
        {
            var formattedOccupiedEnd = TimeOnly.FromDateTime(existingEnd).ToString("HH:mm");
            var formattedDisplayEnd = FormatTimeTo12h(TimeOnly.FromDateTime(existingEnd));

            return new ValidateSlotResponseDTO
            {
                IsOccupied = true,
                Message = $"{ResponseMessagesCalendar.DoctorIsBusy} {FormatTimeTo12h(cita.HoraCita)} a {formattedDisplayEnd}.",
                SuggestedNextAvailableTime = formattedOccupiedEnd
            };
        }
    }

    return new ValidateSlotResponseDTO
    {
        IsOccupied = false,
        Message = ResponseMessagesCalendar.DoctorIsAvailable,
        SuggestedNextAvailableTime = requestedTime.ToString("HH:mm")
    };
}

private static List<CalendarDTO.AgendaSlotDto> GenerateDailyAgenda(List<Cita> dayAppointments, Func<Cita, CitaDetalleDto> mapper)
{
    List<string> standardHours = 
    [
        "08:00", "08:30", "09:00", "09:30", "10:00", "10:30",
        "11:00", "11:30", "12:00", "12:30", "13:00", "13:30",
        "14:00", "14:30", "15:00", "15:30", "16:00", "16:30",
        "17:00", "17:30"
    ];

    var conflictiveAppointments = dayAppointments
        .Where(c =>
            !string.Equals(c.Estado, Status.Cancelled, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(c.Estado, Status.Completed, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(c.Estado, Status.NotAssisted, StringComparison.OrdinalIgnoreCase))
        .Where(c =>
        {
            var existingEnd = c.FechaCita.Date.Add(c.HoraCita.ToTimeSpan()).AddMinutes(30);
            return existingEnd > DateTime.Now;
        })
        .ToList();

    var matchedCitas = new HashSet<int>();
    var agendaSlots = new List<CalendarDTO.AgendaSlotDto>();

    foreach (var hourStr in standardHours)
    {
        var slotTime = TimeOnly.ParseExact(hourStr, "HH:mm");
        var slotStartMinutes = slotTime.Hour * 60 + slotTime.Minute;
        var slotEndMinutes = slotStartMinutes + 30;

        CalendarDTO.AgendaSlotDto slotDto = new CalendarDTO.AgendaSlotDto
        {
            Hour = hourStr,
            DisplayHour = slotTime.ToString("hh:mm tt", CultureInfo.InvariantCulture),
            Status = Status.Available,
            Cita = null
        };

        var exactCita = dayAppointments.FirstOrDefault(c => c.HoraCita == slotTime);
        if (exactCita != null)
        {
            slotDto.Cita = mapper(exactCita);
        } 

        foreach (var cita in conflictiveAppointments)
        {
            var citaStartMinutes = cita.HoraCita.Hour * 60 + cita.HoraCita.Minute;
            var citaEndMinutes = citaStartMinutes + 30;

            if (citaStartMinutes < slotEndMinutes && citaEndMinutes > slotStartMinutes)
            {
                if (citaStartMinutes == slotStartMinutes)
                {
                    slotDto.Status = Status.Busy;
                    slotDto.Cita = mapper(cita);
                    matchedCitas.Add(cita.Id);
                }
                else if (slotDto.Status == Status.Available)
                {
                    slotDto.Status = Status.Blocked;
                }
            }
        }

        agendaSlots.Add(slotDto);
    }

    var extraSlots = conflictiveAppointments
        .Where(c => !matchedCitas.Contains(c.Id))
        .Select(cita => new CalendarDTO.AgendaSlotDto
        {
            Hour = cita.HoraCita.ToString("HH:mm"),
            DisplayHour = cita.HoraCita.ToString("hh:mm tt", CultureInfo.InvariantCulture),
            Status = Status.Busy,
            Cita = mapper(cita)
        });

    agendaSlots.AddRange(extraSlots);

    var inactiveAppointments = dayAppointments
        .Where(c => !conflictiveAppointments.Any(ca => ca.Id == c.Id))
        .ToList();

    foreach (var cita in inactiveAppointments)
    {
        var citaTimeStr = cita.HoraCita.ToString("HH:mm");
        
        if (!agendaSlots.Any(s => s.Cita != null && s.Cita.Id == cita.Id))
        {
            agendaSlots.Add(new CalendarDTO.AgendaSlotDto
            {
                Hour = citaTimeStr,
                DisplayHour = cita.HoraCita.ToString("hh:mm tt", CultureInfo.InvariantCulture),
                Status = Status.Available, 
                Cita = mapper(cita)
            });
        }
    }

    return agendaSlots.OrderBy(s => s.Hour).ToList();
}
}