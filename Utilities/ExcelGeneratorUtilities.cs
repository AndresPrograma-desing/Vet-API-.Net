using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using DTOs;
using vet_api_Net.Interfaze.Utilities;

//Describe: Genera los reportes descargables en Excel (dashboard, citas, clientes) usando ClosedXML.

namespace vet_api_Net.Utilities
{
    public class ExcelGeneratorUtilities : IExcelGeneratorUtilities
    {
        private static readonly XLColor HeaderFill = XLColor.FromHtml("#4472C4");
        private const string CurrencyFormat = "#,##0.00";

        public byte[] GenerateDashboardExcel(DashboardStatsDTO stats)
        {
            using var workbook = new XLWorkbook();

            WriteResumenSheet(workbook, stats);
            WriteGananciasSheet(workbook, stats);
            WriteCitasSheet(workbook, stats);
            WriteProductosSheet(workbook, stats);
            WriteAlertasSheet(workbook, stats);

            return ToBytes(workbook);
        }

        private static void WriteResumenSheet(XLWorkbook workbook, DashboardStatsDTO stats)
        {
            var ws = workbook.Worksheets.Add("Resumen");

            WriteTitle(ws, 1, "Resumen del Dashboard");
            ws.Cell(2, 1).Value = "Generado el";
            ws.Cell(2, 2).Value = DateTime.Now;
            ws.Cell(2, 2).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";

            int row = 4;
            WriteTableHeader(ws, row, "Indicador", "Valor");
            row++;

            WriteKpiRow(ws, ref row, $"Ingresos Totales ({stats.MoneyType})", stats.TotalGanancias, isCurrency: true);
            WriteKpiRow(ws, ref row, $"Pérdidas Totales ({stats.MoneyType})", stats.TotalPerdidas, isCurrency: true);
            WriteKpiRow(ws, ref row, "Total Facturas", stats.TotalFacturas);
            WriteKpiRow(ws, ref row, "Total Citas", stats.TotalCitas);
            WriteKpiRow(ws, ref row, "Total Mascotas", stats.TotalMascotas);
            WriteKpiRow(ws, ref row, "Total Clientes", stats.TotalClientes);
            WriteKpiRow(ws, ref row, "Total Productos", stats.TotalProductos);

            ws.Range(4, 1, row - 1, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range(4, 1, row - 1, 2).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            ws.SheetView.FreezeRows(4);
            ws.Columns().AdjustToContents();
        }

        private static void WriteKpiRow(IXLWorksheet ws, ref int row, string label, object value, bool isCurrency = false)
        {
            ws.Cell(row, 1).Value = label;
            if (isCurrency)
            {
                ws.Cell(row, 2).Value = Convert.ToDecimal(value);
                ws.Cell(row, 2).Style.NumberFormat.Format = CurrencyFormat;
            }
            else
            {
                ws.Cell(row, 2).Value = Convert.ToInt32(value);
            }
            row++;
        }

        private static void WriteGananciasSheet(XLWorkbook workbook, DashboardStatsDTO stats)
        {
            var ws = workbook.Worksheets.Add("Ganancias");

            WriteTitle(ws, 1, $"Ganancias por Periodo ({stats.MoneyType})");

            int row = 3;
            WriteTableHeader(ws, row, "Periodo", "Fecha Inicio", $"Ganancia ({stats.MoneyType})");
            row++;
            var firstDataRow = row;

            foreach (var g in stats.GananciasMensuales)
            {
                ws.Cell(row, 1).Value = g.Periodo;
                ws.Cell(row, 2).Value = g.FechaInicio;
                ws.Cell(row, 2).Style.DateFormat.Format = "dd/MM/yyyy";
                ws.Cell(row, 3).Value = g.Ganancia;
                ws.Cell(row, 3).Style.NumberFormat.Format = CurrencyFormat;
                row++;
            }

            ApplyTableBorders(ws, 3, 1, row - 1, 3, firstDataRow);
            ws.SheetView.FreezeRows(4);
            ws.Columns().AdjustToContents();
        }

        private static void WriteCitasSheet(XLWorkbook workbook, DashboardStatsDTO stats)
        {
            var ws = workbook.Worksheets.Add("Citas");

            WriteTitle(ws, 1, "Citas por Estado");
            int row = 3;
            WriteTableHeader(ws, row, "Estado", "Cantidad");
            row++;
            var estadoFirstRow = row;
            foreach (var kvp in stats.CitasPorEstado)
            {
                ws.Cell(row, 1).Value = kvp.Key;
                ws.Cell(row, 2).Value = kvp.Value;
                row++;
            }
            ApplyTableBorders(ws, 3, 1, row - 1, 2, estadoFirstRow);

            row += 2;
            WriteTitle(ws, row, "Últimas Citas");
            row++;
            WriteTableHeader(ws, row, "ID", "Fecha", "Hora", "Motivo", "Tipo", "Estado", "Mascota", "Cliente", "Doctor");
            row++;
            var citasFirstRow = row;
            foreach (var c in stats.UltimasCitas)
            {
                ws.Cell(row, 1).Value = c.Id;
                ws.Cell(row, 2).Value = c.FechaCita;
                ws.Cell(row, 3).Value = c.HoraCita;
                ws.Cell(row, 4).Value = c.Motivo;
                ws.Cell(row, 5).Value = c.TipoCita;
                ws.Cell(row, 6).Value = c.Estado;
                ws.Cell(row, 7).Value = c.MascotaNombre;
                ws.Cell(row, 8).Value = c.ClienteNombre;
                ws.Cell(row, 9).Value = c.DoctorNombre;
                row++;
            }
            ApplyTableBorders(ws, citasFirstRow - 1, 1, row - 1, 9, citasFirstRow);

            ws.Columns().AdjustToContents();
        }

        private static void WriteProductosSheet(XLWorkbook workbook, DashboardStatsDTO stats)
        {
            var ws = workbook.Worksheets.Add("Productos");

            WriteTitle(ws, 1, "Productos Bajo Stock");
            int row = 3;
            WriteTableHeader(ws, row, "Código", "Nombre", "Stock", "Stock Mínimo", $"Precio Venta ({stats.MoneyType})");
            row++;
            var firstDataRow = row;

            foreach (var p in stats.ProductosBajoStock)
            {
                ws.Cell(row, 1).Value = p.Codigo;
                ws.Cell(row, 2).Value = p.Nombre;
                ws.Cell(row, 3).Value = p.Stock;
                ws.Cell(row, 4).Value = p.StockMinimo;
                ws.Cell(row, 5).Value = p.PrecioVenta;
                ws.Cell(row, 5).Style.NumberFormat.Format = CurrencyFormat;
                row++;
            }

            ApplyTableBorders(ws, 3, 1, row - 1, 5, firstDataRow);
            ws.SheetView.FreezeRows(4);
            ws.Columns().AdjustToContents();
        }

        private static void WriteAlertasSheet(XLWorkbook workbook, DashboardStatsDTO stats)
        {
            var ws = workbook.Worksheets.Add("Alertas");

            WriteTitle(ws, 1, "Alertas Recientes");
            int row = 3;
            WriteTableHeader(ws, row, "Título", "Mensaje", "Tipo", "Prioridad", "Fecha");
            row++;
            var firstDataRow = row;

            foreach (var a in stats.AlertasRecientes)
            {
                ws.Cell(row, 1).Value = a.Titulo;
                ws.Cell(row, 2).Value = a.Mensaje;
                ws.Cell(row, 3).Value = a.Tipo;
                ws.Cell(row, 4).Value = a.Prioridad;
                ws.Cell(row, 5).Value = a.Fecha;
                ws.Cell(row, 5).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
                row++;
            }

            ApplyTableBorders(ws, 3, 1, row - 1, 5, firstDataRow);
            ws.SheetView.FreezeRows(4);
            ws.Columns().AdjustToContents();
        }

        // ---- Helpers compartidos de estilo ----

        private static void WriteTitle(IXLWorksheet ws, int row, string text)
        {
            ws.Cell(row, 1).Value = text;
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Font.FontSize = 13;
        }

        private static IXLRange WriteTableHeader(IXLWorksheet ws, int row, params string[] headers)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(row, i + 1).Value = headers[i];
            }

            var range = ws.Range(row, 1, row, headers.Length);
            range.Style.Font.Bold = true;
            range.Style.Font.FontColor = XLColor.White;
            range.Style.Fill.BackgroundColor = HeaderFill;
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            range.SetAutoFilter();
            return range;
        }

        private static void ApplyTableBorders(IXLWorksheet ws, int headerRow, int firstCol, int lastRow, int lastCol, int firstDataRow)
        {
            if (lastRow < firstDataRow) return; // sin filas de datos, solo el encabezado
            ws.Range(headerRow, firstCol, lastRow, lastCol).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range(headerRow, firstCol, lastRow, lastCol).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        }

        private static byte[] ToBytes(XLWorkbook workbook)
        {
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] GenerateCitasExcel(List<CitasRequestDTO> citas)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Citas");

            int row = 1;
            WriteTableHeader(ws, row, "ID", "Fecha", "Hora", "Motivo", "Tipo", "Estado", "Mascota", "Cliente", "Doctor");
            row++;
            var firstDataRow = row;

            foreach (var c in citas)
            {
                ws.Cell(row, 1).Value = c.Id;
                ws.Cell(row, 2).Value = c.FechaCita;
                ws.Cell(row, 3).Value = c.HoraCita;
                ws.Cell(row, 4).Value = c.Motivo;
                ws.Cell(row, 5).Value = c.TipoCita;
                ws.Cell(row, 6).Value = c.Estado;
                ws.Cell(row, 7).Value = c.Mascotum?.Nombre;
                ws.Cell(row, 8).Value = c.Mascotum?.Cliente != null ? $"{c.Mascotum.Cliente.Nombre} {c.Mascotum.Cliente.Apellido}".Trim() : "";
                ws.Cell(row, 9).Value = c.Doctor != null ? $"{c.Doctor.Nombre} {c.Doctor.Apellido}".Trim() : "";
                row++;
            }

            ApplyTableBorders(ws, 1, 1, row - 1, 9, firstDataRow);
            ws.SheetView.FreezeRows(1);
            ws.Columns().AdjustToContents();

            return ToBytes(workbook);
        }

        public byte[] GenerateClientesExcel(List<ClientListWithMascotasDTO> clientes)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Clientes");

            int row = 1;
            WriteTableHeader(ws, row, "ID", "Nombre", "Apellido", "Email", "Teléfono", "Identificación", "Dirección", "Mascotas (Nombres)");
            row++;
            var firstDataRow = row;

            foreach (var c in clientes)
            {
                ws.Cell(row, 1).Value = c.Id;
                ws.Cell(row, 2).Value = c.Nombre;
                ws.Cell(row, 3).Value = c.Apellido;
                ws.Cell(row, 4).Value = c.Email;
                ws.Cell(row, 5).Value = c.Telefono;
                ws.Cell(row, 6).Value = c.Identificacion;
                ws.Cell(row, 7).Value = c.Direccion;
                ws.Cell(row, 8).Value = c.Mascotas != null ? string.Join(", ", c.Mascotas.ConvertAll(m => m.Nombre)) : "";
                row++;
            }

            ApplyTableBorders(ws, 1, 1, row - 1, 8, firstDataRow);
            ws.SheetView.FreezeRows(1);
            ws.Columns().AdjustToContents();

            return ToBytes(workbook);
        }
    }
}
