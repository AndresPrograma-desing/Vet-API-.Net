using System.IO;
using ClosedXML.Excel;
using DTOs;
using vet_api_Net.Interfaze.Utilities;

namespace vet_api_Net.Utilities
{
    public class ExcelGenerator : IExcelGenerator
    {
        public byte[] GenerateDashboardExcel(DashboardStatsDTO stats)
        {
            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Dashboard Stats");

                ws.Cell(1, 1).Value = "Métricas Principales";
                ws.Cell(1, 1).Style.Font.Bold = true;
                
                ws.Cell(2, 1).Value = "Ingresos Totales";
                ws.Cell(2, 2).Value = stats.TotalGanancias;
                
                ws.Cell(3, 1).Value = "Total Pérdidas";
                ws.Cell(3, 2).Value = stats.TotalPerdidas;
                
                ws.Cell(4, 1).Value = "Total Facturas";
                ws.Cell(4, 2).Value = stats.TotalFacturas;
                
                ws.Cell(5, 1).Value = "Total Citas";
                ws.Cell(5, 2).Value = stats.TotalCitas;
                
                ws.Cell(6, 1).Value = "Total Mascotas";
                ws.Cell(6, 2).Value = stats.TotalMascotas;
                
                ws.Cell(7, 1).Value = "Total Clientes";
                ws.Cell(7, 2).Value = stats.TotalClientes;
                
                ws.Cell(8, 1).Value = "Total Productos";
                ws.Cell(8, 2).Value = stats.TotalProductos;

                int row = 10;
                
                // Ganancias Mensuales
                ws.Cell(row, 1).Value = "Ingresos Mensuales";
                ws.Cell(row, 1).Style.Font.Bold = true;
                row++;
                ws.Cell(row, 1).Value = "Mes";
                ws.Cell(row, 2).Value = "Año";
                ws.Cell(row, 3).Value = "Ingreso";
                ws.Range(row, 1, row, 3).Style.Font.Bold = true;
                row++;
                foreach (var g in stats.GananciasMensuales)
                {
                    ws.Cell(row, 1).Value = g.Mes;
                    ws.Cell(row, 2).Value = g.Anio;
                    ws.Cell(row, 3).Value = g.Ganancia;
                    row++;
                }
                
                row++;

                // Citas por Estado
                ws.Cell(row, 1).Value = "Citas por Estado";
                ws.Cell(row, 1).Style.Font.Bold = true;
                row++;
                foreach (var kvp in stats.CitasPorEstado)
                {
                    ws.Cell(row, 1).Value = kvp.Key;
                    ws.Cell(row, 2).Value = kvp.Value;
                    row++;
                }

                row++;

                // Ultimas citas
                ws.Cell(row, 1).Value = "Últimas Citas";
                ws.Cell(row, 1).Style.Font.Bold = true;
                row++;
                ws.Cell(row, 1).Value = "ID";
                ws.Cell(row, 2).Value = "Fecha";
                ws.Cell(row, 3).Value = "Hora";
                ws.Cell(row, 4).Value = "Motivo";
                ws.Cell(row, 5).Value = "Tipo";
                ws.Cell(row, 6).Value = "Estado";
                ws.Cell(row, 7).Value = "Mascota";
                ws.Cell(row, 8).Value = "Cliente";
                ws.Cell(row, 9).Value = "Doctor";
                ws.Range(row, 1, row, 9).Style.Font.Bold = true;
                row++;
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

                row++;



                ws.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public byte[] GenerateCitasExcel(System.Collections.Generic.List<CitasRequestDTO> citas)
        {
            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Citas");

                int row = 1;
                ws.Cell(row, 1).Value = "ID";
                ws.Cell(row, 2).Value = "Fecha";
                ws.Cell(row, 3).Value = "Hora";
                ws.Cell(row, 4).Value = "Motivo";
                ws.Cell(row, 5).Value = "Tipo";
                ws.Cell(row, 6).Value = "Estado";
                ws.Cell(row, 7).Value = "Mascota";
                ws.Cell(row, 8).Value = "Cliente";
                ws.Cell(row, 9).Value = "Doctor";
                ws.Range(row, 1, row, 9).Style.Font.Bold = true;
                row++;

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

                ws.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public byte[] GenerateClientesExcel(System.Collections.Generic.List<ClientListWithMascotasDTO> clientes)
        {
            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Clientes");

                int row = 1;
                ws.Cell(row, 1).Value = "ID";
                ws.Cell(row, 2).Value = "Nombre";
                ws.Cell(row, 3).Value = "Apellido";
                ws.Cell(row, 4).Value = "Email";
                ws.Cell(row, 5).Value = "Teléfono";
                ws.Cell(row, 6).Value = "Identificación";
                ws.Cell(row, 7).Value = "Dirección";
                ws.Cell(row, 8).Value = "Mascotas (Nombres)";
                ws.Range(row, 1, row, 8).Style.Font.Bold = true;
                row++;

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

                ws.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}
