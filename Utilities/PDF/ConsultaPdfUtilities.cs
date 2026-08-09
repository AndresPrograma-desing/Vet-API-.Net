using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Utilities;

namespace vet_api_Net.Utilities.Pdf
{
    public class ConsultaPdfUtilities : IConsultaPdfUtilities
    {
        public byte[] GenerateConsultaPdf(ConsultaPdfDTO consulta, string webRootPath, string currencySymbol)
        {
            string logoImagePath = Path.Combine(webRootPath, "images", "HappyPetsLogo.png");

            var document = new ConsultaDocument(consulta, currencySymbol, logoImagePath);
            return document.GeneratePdf();
        }

        public string BuildFileName(ConsultaPdfDTO consulta)
        {
            string clientePart = !string.IsNullOrWhiteSpace(consulta.ClienteNombre)
                ? consulta.ClienteNombre
                : (consulta.ClienteId.HasValue ? $"{PdfText.Clinte}_{consulta.ClienteId}" : "Cliente");

            string mascotaPart = !string.IsNullOrWhiteSpace(consulta.MascotaNombre)
                ? consulta.MascotaNombre
                : $"{PdfText.Mascota}_{consulta.MascotaId}";

            clientePart = SanitizePart(clientePart);
            mascotaPart = SanitizePart(mascotaPart);

            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            string safeConsultaId = SanitizePart(consulta.Id > 0 ? consulta.Id.ToString() : timestamp);

            return $"Consulta_{safeConsultaId}_{clientePart}_{mascotaPart}.pdf";
        }

        private static string SanitizePart(string input)
        {
            string clean = (input ?? string.Empty).Trim().Replace(' ', '_');
            foreach (var c in Path.GetInvalidFileNameChars())
                clean = clean.Replace(c, '-');
            return clean;
        }

        private class ConsultaDocument : IDocument
        {
            private readonly ConsultaPdfDTO _consulta;
            private readonly string _currencySymbol;
            private readonly string _logoImagePath;
            private readonly string _primaryColor = Colors.Blue.Darken3;
            private readonly string _secondaryColor = Colors.Blue.Lighten5;
            private readonly string _textDark = Colors.Grey.Darken4;
            private readonly string _textMuted = Colors.Grey.Darken2;
            private readonly string _borderLight = Colors.Grey.Lighten2;

            public ConsultaDocument(ConsultaPdfDTO consulta, string currencySymbol, string logoImagePath)
            {
                _consulta = consulta;
                _currencySymbol = currencySymbol;
                _logoImagePath = logoImagePath;
            }

            public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

            public void Compose(IDocumentContainer container)
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.PageColor(Colors.White);

                    page.Header().Element(ComposeHeader);

                    page.Content().PaddingVertical(15).Column(column =>
                    {
                        column.Item().Element(ComposePatientAndDoctorInfo);

                        column.Item().PaddingTop(15).Element(ComposeClinicalDetails);

                        column.Item().PaddingTop(15).Element(ComposePrescriptionSection);

                        if (_consulta.Productos != null && _consulta.Productos.Any())
                        {
                            column.Item().PaddingTop(15).Element(BuildProductsTable);
                        }

                        column.Item().PaddingTop(15).Element(ComposeTotals);
                    });

                    page.Footer().Element(ComposeFooter);
                });
            }

            private void ComposeHeader(IContainer container)
            {
                container.BorderBottom(2).BorderColor(_primaryColor).PaddingBottom(12).Row(row =>
                {
                    row.RelativeItem().Row(headerRow =>
                    {
                        if (File.Exists(_logoImagePath))
                        {
                            headerRow.ConstantItem(65).PaddingRight(10).AlignMiddle().Image(_logoImagePath);
                        }

                        headerRow.RelativeItem().Column(col =>
                        {
                            col.Item().AlignMiddle().Text(PdfText.HappyPets)
                                .FontSize(22).Bold().FontColor(_primaryColor);

                            col.Item().PaddingTop(2).Text(PdfText.SpecialVet)
                                .FontSize(10).SemiBold().FontColor(_textMuted);
                        });
                    });

                    row.ConstantItem(220).AlignRight().Column(col =>
                    {
                        col.Item().Text("CONSULTA MÉDICA")
                            .FontSize(12).SemiBold().FontColor(_textMuted);

                        col.Item().Text($"N° {_consulta.Id:D6}")
                            .FontSize(18).Bold().FontColor(_primaryColor);
                    });
                });
            }

            private void ComposePatientAndDoctorInfo(IContainer container)
            {
                container.Background(_secondaryColor).Padding(10).Row(r =>
                {
                    r.RelativeItem().Column(c =>
                    {
                        c.Item().Text(PdfText.Mascota.ToUpper()).FontSize(9).Bold().FontColor(_primaryColor);
                        c.Item().PaddingTop(2).Text(_consulta.MascotaNombre ?? "-").FontSize(12).Bold().FontColor(_textDark);
                        c.Item().Text($"ID Mascota: {_consulta.MascotaId}").FontSize(8).FontColor(_textMuted);
                    });

                    r.RelativeItem().Column(c =>
                    {
                        c.Item().Text("DATOS DEL CLIENTE").FontSize(9).Bold().FontColor(_primaryColor);
                        c.Item().PaddingTop(2).Text(!string.IsNullOrEmpty(_consulta.ClienteNombre) ? _consulta.ClienteNombre : "N/A").FontSize(10).Bold().FontColor(_textDark);
                        c.Item().Text($"Tel: {(!string.IsNullOrEmpty(_consulta.ClienteTelefono) ? _consulta.ClienteTelefono : _consulta.TelefonoCliente)}").FontSize(8).FontColor(_textMuted);
                        if (!string.IsNullOrEmpty(_consulta.CorreoCliente))
                            c.Item().Text($"Email: {_consulta.CorreoCliente}").FontSize(8).FontColor(_textMuted);
                    });

                    r.ConstantItem(140).Column(c =>
                    {
                        c.Item().AlignRight().Text(text =>
                        {
                            text.Span($"{PdfText.Fecha}: ").FontSize(9).SemiBold().FontColor(_textMuted);
                            text.Span($"{_consulta.FechaConsulta:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(_textDark);
                        });

                        c.Item().PaddingTop(2).AlignRight().Text(text =>
                        {
                            text.Span("Cita ID: ").FontSize(8).SemiBold().FontColor(_textMuted);
                            text.Span($"{_consulta.CitaId}").FontSize(8).FontColor(_textDark);
                        });

                        c.Item().PaddingTop(2).AlignRight().Text(text =>
                        {
                            text.Span("Doctor ID: ").FontSize(8).SemiBold().FontColor(_textMuted);
                            text.Span($"{_consulta.DoctorId}").FontSize(8).FontColor(_textDark);
                        });
                    });
                });
            }

            private void ComposeClinicalDetails(IContainer container)
            {
                container.Column(c =>
                {
                    BuildBlock(c, "SÍNTOMAS / MOTIVO DE CONSULTA", _consulta.Sintomas);
                    BuildBlock(c, "DIAGNÓSTICO", _consulta.Diagnostico);
                    BuildBlock(c, "TRATAMIENTO APLICADO", _consulta.Tratamiento);
                    BuildBlock(c, "OBSERVACIONES", _consulta.Observaciones);
                });
            }

            private void BuildBlock(ColumnDescriptor c, string title, string? textValue)
            {
                string value = string.IsNullOrWhiteSpace(textValue) ? "Sin observaciones registradas." : textValue.Trim();

                c.Item().PaddingBottom(8).Column(col =>
                {
                    col.Item().Text(title).FontSize(9).Bold().FontColor(_primaryColor);
                    col.Item().PaddingTop(2).BorderBottom(1).BorderColor(_borderLight).PaddingBottom(4)
                        .Text(value).FontSize(9.5f).FontColor(_textDark);
                });
            }

            private void ComposePrescriptionSection(IContainer container)
            {
                string recetaTexto = string.IsNullOrWhiteSpace(_consulta.Receta)
                    ? "No se considera enviar receta."
                    : _consulta.Receta.Trim();

                container.Border(1).BorderColor(_primaryColor).Padding(8).Column(c =>
                {
                    c.Item().Text("RECETA / INDICACIONES MÉDICAS").FontSize(10).Bold().FontColor(_primaryColor);
                    c.Item().PaddingTop(4).Text(recetaTexto).FontSize(9.5f).FontColor(_textDark);
                });
            }

            private void BuildProductsTable(IContainer container)
            {
                container.Column(col =>
                {
                    col.Item().PaddingBottom(4).Text("PRODUCTOS Y MEDICAMENTOS SUMINISTRADOS").FontSize(10).Bold().FontColor(_primaryColor);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.ConstantColumn(45);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.ConstantColumn(65);
                            columns.ConstantColumn(65);
                        });

                        table.Header(header =>
                        {
                            IContainer StyleHeaderCell(IContainer c, string bg) => c.Background(bg).Padding(5).AlignMiddle();

                            header.Cell().Element(c => StyleHeaderCell(c, _primaryColor)).Text("Producto").FontColor(Colors.White).FontSize(8.5f).Bold();
                            header.Cell().Element(c => StyleHeaderCell(c, _primaryColor)).AlignCenter().Text("Cant.").FontColor(Colors.White).FontSize(8.5f).Bold();
                            header.Cell().Element(c => StyleHeaderCell(c, _primaryColor)).Text("Dosis / Vía").FontColor(Colors.White).FontSize(8.5f).Bold();
                            header.Cell().Element(c => StyleHeaderCell(c, _primaryColor)).Text("Frec. / Duración").FontColor(Colors.White).FontSize(8.5f).Bold();
                            header.Cell().Element(c => StyleHeaderCell(c, _primaryColor)).AlignRight().Text("P. Unit").FontColor(Colors.White).FontSize(8.5f).Bold();
                            header.Cell().Element(c => StyleHeaderCell(c, _primaryColor)).AlignRight().Text("Total").FontColor(Colors.White).FontSize(8.5f).Bold();
                        });

                        bool alternate = false;
                        foreach (var prod in _consulta.Productos)
                        {
                            var rowColor = alternate ? _secondaryColor : "#FFFFFF";
                            alternate = !alternate;

                            IContainer StyleContentCell(IContainer c, string bg, string border) =>
                                c.Background(bg).PaddingHorizontal(5).PaddingVertical(4).BorderBottom(1).BorderColor(border).AlignMiddle();

                            string dosisVia = string.Join(" / ", new[] { prod.Dosis, prod.ViaAdministracion }.Where(s => !string.IsNullOrWhiteSpace(s)));
                            string frecDur = string.Join(" / ", new[] { prod.Frecuencia, prod.Duracion }.Where(s => !string.IsNullOrWhiteSpace(s)));

                            decimal subtotal = prod.Cantidad * prod.PrecioUnitario;

                            table.Cell().Element(c => StyleContentCell(c, rowColor, _borderLight)).Text(prod.NombreProducto ?? "").FontSize(8.5f).FontColor(_textDark);
                            table.Cell().Element(c => StyleContentCell(c, rowColor, _borderLight)).AlignCenter().Text(prod.Cantidad.ToString()).FontSize(8.5f).FontColor(_textDark);
                            table.Cell().Element(c => StyleContentCell(c, rowColor, _borderLight)).Text(string.IsNullOrEmpty(dosisVia) ? "-" : dosisVia).FontSize(8f).FontColor(_textDark);
                            table.Cell().Element(c => StyleContentCell(c, rowColor, _borderLight)).Text(string.IsNullOrEmpty(frecDur) ? "-" : frecDur).FontSize(8f).FontColor(_textDark);
                            table.Cell().Element(c => StyleContentCell(c, rowColor, _borderLight)).AlignRight().Text($"{_currencySymbol}{prod.PrecioUnitario:N2}").FontSize(8.5f).FontColor(_textDark);
                            table.Cell().Element(c => StyleContentCell(c, rowColor, _borderLight)).AlignRight().Text($"{_currencySymbol}{subtotal:N2}").FontSize(8.5f).Bold().FontColor(_textDark);
                        }
                    });
                });
            }

            private void ComposeTotals(IContainer container)
            {
                decimal totalProductos = _consulta.Productos?.Sum(p => p.Cantidad * p.PrecioUnitario) ?? 0;
                decimal granTotal = _consulta.ConsultaPrice + totalProductos;

                container.Row(r =>
                {
                    r.RelativeItem();

                    r.ConstantItem(230).Column(c =>
                    {
                        c.Item().PaddingVertical(2).Row(rr =>
                        {
                            rr.RelativeItem().Text("Precio Consulta:").FontSize(9).FontColor(_textMuted);
                            rr.ConstantItem(90).AlignRight().Text($"{_currencySymbol}{_consulta.ConsultaPrice:N2}").FontSize(9).FontColor(_textDark);
                        });

                        c.Item().PaddingVertical(2).Row(rr =>
                        {
                            rr.RelativeItem().Text("Total Insumos:").FontSize(9).FontColor(_textMuted);
                            rr.ConstantItem(90).AlignRight().Text($"{_currencySymbol}{totalProductos:N2}").FontSize(9).FontColor(_textDark);
                        });

                        c.Item().PaddingVertical(4).LineHorizontal(1).LineColor(_primaryColor);

                        c.Item().Background(_secondaryColor).Padding(6).Row(rr =>
                        {
                            rr.RelativeItem().AlignMiddle().Text("TOTAL:").FontSize(11).Bold().FontColor(_primaryColor);
                            rr.ConstantItem(90).AlignRight().AlignMiddle().Text($"{_currencySymbol}{granTotal:N2}")
                                .FontSize(12).Bold().FontColor(_primaryColor);
                        });
                    });
                });
            }

            private void ComposeFooter(IContainer container)
            {
                container.Column(col =>
                {
                    col.Item().LineHorizontal(1).LineColor(_borderLight);
                    col.Item().PaddingTop(8).AlignCenter().Text(PdfText.ThankYou)
                        .FontSize(10).Medium().FontColor(_primaryColor);

                    col.Item().PaddingTop(4).AlignCenter().Text(PdfText.FooterText)
                        .FontSize(7).FontColor(_textMuted);
                });
            }
        }
    }
}
