// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Globalization;
// using DTOs;
// using Xceed.Document.NET;
// using Xceed.Words.NET;
// using Xceed.Drawing;

// namespace vet_api_Net.Services
// {
//     public class GenerateDocxService
//     {
//         public string GenerateInvoiceDocx(FacturationDTO invoice, string webRootPath)
//         {
//             var safeNumero = string.IsNullOrWhiteSpace(invoice.NumeroFactura) ? DateTime.UtcNow.ToString("yyyyMMddHHmmss") : invoice.NumeroFactura;
//             foreach (var c in Path.GetInvalidFileNameChars()) safeNumero = safeNumero.Replace(c, '-');
//             string fileName = $"Factura_{safeNumero}.docx";
//             string dir = Path.Combine(webRootPath, "facturas");
//             Directory.CreateDirectory(dir);
//             string filePath = Path.Combine(dir, fileName);

//             using (DocX doc = DocX.Create(filePath))
//             {
//                 // Header table (1x2)
//                 var headerTable = doc.AddTable(1, 2);

//                 // Left column: logo/name
//                 var leftCell = headerTable.Rows[0].Cells[0];
//                 var leftPara = leftCell.Paragraphs[0];
//                 leftPara.Append("🐾 VetClinic").FontSize(22).Bold().Font("Calibri");
//                 leftCell.InsertParagraph("Clínica Veterinaria Especializada").FontSize(14);
//                 leftCell.InsertParagraph("Av. Principal 123 · Ciudad · CP 12345").FontSize(10);

//                 // Right column: badge
//                 var rightCell = headerTable.Rows[0].Cells[1];
//                 var pFactura = rightCell.Paragraphs[0];
//                 pFactura.Alignment = Alignment.center;
//                 pFactura.Append("FACTURA").FontSize(19).Bold();
//                 rightCell.InsertParagraph(invoice.NumeroFactura ?? string.Empty).FontSize(13).Bold().Alignment = Alignment.center;
//                 rightCell.InsertParagraph($"Fecha: {invoice.FechaEmision:yyyy-MM-dd}").FontSize(10).Alignment = Alignment.center;

//                 doc.InsertTable(headerTable);
//                 doc.InsertParagraph();

//                 // Client / Pet info
//                 var infoTable = doc.AddTable(1, 2);
//                 var clientCell = infoTable.Rows[0].Cells[0];
//                 clientCell.Paragraphs[0].Append("● CLIENTE").FontSize(12).Bold();
//                 clientCell.InsertParagraph(invoice.ClienteNombre ?? string.Empty).FontSize(11).Bold();
//                 clientCell.InsertParagraph($"ID: {invoice.ClienteId}").FontSize(9);

//                 var petCell = infoTable.Rows[0].Cells[1];
//                 petCell.Paragraphs[0].Append("● MASCOTA").FontSize(12).Bold().Alignment = Alignment.right;
//                 petCell.InsertParagraph((invoice.MascotaNombre ?? string.Empty)).FontSize(11).Bold().Alignment = Alignment.right;
//                 petCell.InsertParagraph($"ID: {invoice.MascotaId}").FontSize(9).Alignment = Alignment.right;

//                 doc.InsertTable(infoTable);
//                 doc.InsertParagraph();

//                 // Items table
//                 int itemCount = invoice.Items?.Count ?? 0;
//                 var headers = new[] { "CÓDIGO", "PRODUCTO", "DESCRIPCIÓN", "CANT.", "P. UNIT.", "TOTAL" };
//                 var itemsTable = doc.AddTable(Math.Max(1, itemCount + 1), 6);

//                 // Header row
//                 for (int i = 0; i < headers.Length; i++)
//                 {
//                     var cell = itemsTable.Rows[0].Cells[i];
//                     cell.Paragraphs[0].Append(headers[i]).FontSize(9).Bold();
//                     if (i >= 3) cell.Paragraphs[0].Alignment = Alignment.right;
//                 }

//                 // Item rows
//                 for (int r = 0; r < itemCount; r++)
//                 {
//                     var item = invoice.Items[r];
//                     var row = itemsTable.Rows[r + 1];
//                     if ((r + 1) % 2 == 0)
//                     {
//                         // keep alternating row background simple (omit fill color to avoid Xceed color dependency)
//                     }

//                     row.Cells[0].Paragraphs[0].Append((item.Codigo ?? string.Empty)).FontSize(9);
//                     row.Cells[1].Paragraphs[0].Append((item.Nombre ?? string.Empty)).FontSize(11).Bold();
//                     row.Cells[2].Paragraphs[0].Append((item.Descripcion ?? string.Empty)).FontSize(9);
//                     row.Cells[3].Paragraphs[0].Append(item.Cantidad.ToString()).Alignment = Alignment.right;
//                     row.Cells[4].Paragraphs[0].Append(item.PrecioUnitario.ToString("C", CultureInfo.CurrentCulture)).Alignment = Alignment.right;
//                     row.Cells[5].Paragraphs[0].Append(item.Total.ToString("C", CultureInfo.CurrentCulture)).Bold().Alignment = Alignment.right;
//                 }

//                 doc.InsertTable(itemsTable);

//                 // Totals
//                 doc.InsertParagraph();
//                 var subtotalPara = doc.InsertParagraph();
//                 subtotalPara.Append($"Subtotal: {invoice.Subtotal.ToString("C", CultureInfo.CurrentCulture)}").FontSize(11).Alignment = Alignment.right;
//                 // IVA possibly missing; guard with try
//                 decimal iva = 0m;
//                 try { /* no iva on DTO - keep zero */ } catch { }
//                 subtotalPara.AppendLine($" IVA (16%): {iva.ToString("C", CultureInfo.CurrentCulture)}").FontSize(11).Alignment = Alignment.right;

//                 var grandTotal = doc.InsertParagraph();
//                 grandTotal.Append("TOTAL: ").FontSize(16).Bold();
//                 grandTotal.Append(invoice.Total.ToString("C", CultureInfo.CurrentCulture)).FontSize(16).Bold();
//                 grandTotal.Alignment = Alignment.right;

//                 doc.InsertParagraph();
//                 doc.InsertParagraph("________________________________________________________").Alignment = Alignment.center;
//                 doc.InsertParagraph("✨ Gracias por confiar en VetClinic. ✨").Bold().Alignment = Alignment.center;

//                 doc.Save();
//             }

//             return fileName;
//         }
//     }
// }
