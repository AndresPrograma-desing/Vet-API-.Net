using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ClosedXML.Excel; 
using vet_api_Net.Models;
using vet_api_Net.Constants;

namespace vet_api_Net.Services
{
    public class GenerateReportExcel
    {
        public byte[] GenerateExcelFromReport(Reporte reporte)
        {
            if (reporte == null || string.IsNullOrWhiteSpace(reporte.Datos))
                throw new ArgumentException(Exceltext.InvalidReport);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = JsonSerializer.Deserialize<ReportData>(reporte.Datos, options);

            if (data == null)
                throw new Exception(Exceltext.DeserializationError);
 
            using (var workbook = new XLWorkbook())
            {
                AgregarHoja(workbook, Exceltext.Clientes, data.Clientes ?? new List<Dictionary<string, object>>());
                AgregarHoja(workbook, Exceltext.Mascotas, data.Mascotas ?? new List<Dictionary<string, object>>());
                AgregarHoja(workbook, Exceltext.Productos, data.Productos ?? new List<Dictionary<string, object>>());
                AgregarHoja(workbook, Exceltext.Facturas, data.Facturas ?? new List<Dictionary<string, object>>());
                AgregarHoja(workbook, Exceltext.Usuarios, data.Usuarios ?? new List<Dictionary<string, object>>());
                AgregarHoja(workbook, Exceltext.Appointments, data.Appointments ?? new List<Dictionary<string, object>>());
                AgregarHoja(workbook, Exceltext.Consultations, data.Consultations ?? new List<Dictionary<string, object>>());
                AgregarHoja(workbook, Exceltext.InventoryMovements, data.InventoryMovements ?? new List<Dictionary<string, object>>());
                AgregarHojaResumen(workbook, Exceltext.Summary, data.Summary ?? new Dictionary<string, object>());

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        private void AgregarHoja(XLWorkbook workbook, string nombre, IEnumerable<Dictionary<string, object>> datos)
        {
            if (datos == null) return;
            var ws = workbook.Worksheets.Add(nombre);
            var list = new List<Dictionary<string, object>>(datos);
            if (list.Count == 0)
                return;
 
            var headers = new List<string>(list[0].Keys);
            for (int i = 0; i < headers.Count; i++)
                ws.Cell(1, i + 1).Value = headers[i];
 
            for (int row = 0; row < list.Count; row++)
            {
                var dict = list[row];
                for (int col = 0; col < headers.Count; col++)
                {
                    ws.Cell(row + 2, col + 1).Value = dict[headers[col]]?.ToString() ?? string.Empty;
                }
            }
            ws.Columns().AdjustToContents();
        }

        private void AgregarHojaResumen(XLWorkbook workbook, string nombre, Dictionary<string, object> resumen)
        {
            var ws = workbook.Worksheets.Add(nombre);
            ws.Cell(1, 1).Value = Exceltext.SummaryMetricColumn;
            ws.Cell(1, 2).Value = Exceltext.SummaryValueColumn;

            var row = 2;
            foreach (var (metrica, valor) in resumen)
            {
                ws.Cell(row, 1).Value = metrica;
                ws.Cell(row, 2).Value = valor?.ToString() ?? string.Empty;
                row++;
            }

            ws.Columns().AdjustToContents();
        }

        public class ReportData
        {
            public DateTime FechaGeneracion { get; set; }
            public List<Dictionary<string, object>>? Clientes { get; set; }
            public List<Dictionary<string, object>>? Mascotas { get; set; }
            public List<Dictionary<string, object>>? Productos { get; set; }
            public List<Dictionary<string, object>>? Facturas { get; set; }
            public List<Dictionary<string, object>>? Usuarios { get; set; }
            public List<Dictionary<string, object>>? Appointments { get; set; }
            public List<Dictionary<string, object>>? Consultations { get; set; }
            public List<Dictionary<string, object>>? InventoryMovements { get; set; }
            public Dictionary<string, object>? Summary { get; set; }
        }
    }
}