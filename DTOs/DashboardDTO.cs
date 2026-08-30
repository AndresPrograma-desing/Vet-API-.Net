using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DTOs;

public record DashboardStatsDTO
{
    [JsonPropertyName("total_ganancias")]
    public decimal TotalGanancias { get; set; }

    [JsonPropertyName("total_perdidas")]
    public decimal TotalPerdidas { get; set; }

    [JsonPropertyName("total_facturas")]
    public int TotalFacturas { get; set; }

    [JsonPropertyName("total_citas")]
    public int TotalCitas { get; set; }

    [JsonPropertyName("total_mascotas")]
    public int TotalMascotas { get; set; }

    [JsonPropertyName("total_clientes")]
    public int TotalClientes { get; set; }

    [JsonPropertyName("total_productos")]
    public int TotalProductos { get; set; }

    [JsonPropertyName("citas_por_estado")]
    public Dictionary<string, int> CitasPorEstado { get; set; } = new();

    [JsonPropertyName("ultimas_citas")]
    public List<DashboardCitaDTO> UltimasCitas { get; set; } = new();

    [JsonPropertyName("productos_bajo_stock")]
    public List<DashboardProductDTO> ProductosBajoStock { get; set; } = new();

    [JsonPropertyName("alertas_recientes")]
    public List<DashboardAlertDTO> AlertasRecientes { get; set; } = new();

    [JsonPropertyName("ganancias_mensuales")]
    public List<DashboardEarningByMonthDTO> GananciasMensuales { get; set; } = new();

    [JsonPropertyName("money_type")]
    public string MoneyType { get; set; } = string.Empty;
}

public record DashboardCitaDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("fecha_cita")]
    public string FechaCita { get; set; } = string.Empty;

    [JsonPropertyName("hora_cita")]
    public string HoraCita { get; set; } = string.Empty;

    [JsonPropertyName("motivo")]
    public string Motivo { get; set; } = string.Empty;

    [JsonPropertyName("tipo_cita")]
    public string TipoCita { get; set; } = string.Empty;

    [JsonPropertyName("estado")]
    public string Estado { get; set; } = string.Empty;

    [JsonPropertyName("mascota_nombre")]
    public string MascotaNombre { get; set; } = string.Empty;

    [JsonPropertyName("cliente_nombre")]
    public string ClienteNombre { get; set; } = string.Empty;

    [JsonPropertyName("doctor_nombre")]
    public string DoctorNombre { get; set; } = string.Empty;
}

public record DashboardProductDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("codigo")]
    public string Codigo { get; set; } = string.Empty;

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("stock")]
    public int Stock { get; set; }

    [JsonPropertyName("stock_minimo")]
    public int StockMinimo { get; set; }

    [JsonPropertyName("precio_venta")]
    public decimal PrecioVenta { get; set; }
}

public record DashboardEarningByMonthDTO
{
    [JsonPropertyName("periodo")]
    public string Periodo { get; set; } = string.Empty;

    [JsonPropertyName("fecha_inicio")]
    public DateTime FechaInicio { get; set; }

    [JsonPropertyName("mes")]
    public string Mes { get; set; } = string.Empty;

    [JsonPropertyName("anio")]
    public int Anio { get; set; }

    [JsonPropertyName("ganancia")]
    public decimal Ganancia { get; set; }
}

public record DashboardAlertDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("titulo")]
    public string Titulo { get; set; } = string.Empty;

    [JsonPropertyName("mensaje")]
    public string Mensaje { get; set; } = string.Empty;

    [JsonPropertyName("tipo")]
    public string Tipo { get; set; } = string.Empty;

    [JsonPropertyName("prioridad")]
    public string Prioridad { get; set; } = string.Empty;

    [JsonPropertyName("fecha")]
    public DateTime Fecha { get; set; }
}
