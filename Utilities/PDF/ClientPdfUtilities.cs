using System;
using System.Globalization;
using System.IO;
using System.Linq;
using DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using vet_api_Net.Constants;

namespace vet_api_Net.Utilities.Pdf
{
    public class GeneratePerfilPdfService
    {
        public string GeneratePerfilPdf(PerfilPdfDTO perfil, string webRootPath, string currencySymbol = "$")
        {
            string nombreCliente = $"{perfil.Nombre}_{perfil.Apellido}".Trim();
            nombreCliente = SanitizePart(string.IsNullOrWhiteSpace(nombreCliente) ? $"Cliente_{perfil.Id}" : nombreCliente);

            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            string fileName = $"Perfil_{perfil.Id}_{nombreCliente}_{timestamp}.pdf";

            string dir = Path.Combine(webRootPath, "perfiles");
            Directory.CreateDirectory(dir);
            string filePath = Path.Combine(dir, fileName);

            string logoImagePath = Path.Combine(webRootPath, "images", "HappyPetsLogo.png");

            var document = new PerfilDocument(perfil, currencySymbol, logoImagePath);
            document.GeneratePdf(filePath);

            return fileName;
        }

        private static string SanitizePart(string input)
        {
            string clean = (input ?? string.Empty).Trim().Replace(' ', '_');
            foreach (var c in Path.GetInvalidFileNameChars())
                clean = clean.Replace(c, '-');
            return clean;
        }

        private class PerfilDocument : IDocument
        {
            private readonly PerfilPdfDTO _perfil;
            private readonly string _currencySymbol;
            private readonly string _logoImagePath;
            private readonly string _primaryColor = Colors.Blue.Darken3;
            private readonly string _secondaryColor = Colors.Blue.Lighten5;
            private readonly string _textDark = Colors.Grey.Darken4;
            private readonly string _textMuted = Colors.Grey.Darken2;
            private readonly string _borderLight = Colors.Grey.Lighten2;

            public PerfilDocument(PerfilPdfDTO perfil, string currencySymbol, string logoImagePath)
            {
                _perfil = perfil;
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
                        // 1. Tarjeta con Datos Personales del Cliente
                        column.Item().Element(ComposeClientInfo);

                        // 2. Sección de Mascotas Registradas
                        column.Item().PaddingTop(20).Element(ComposeMascotasSection);

                        // 3. Sección de Historial de Facturas
                        column.Item().PaddingTop(20).Element(ComposeFacturasSection);
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
                        col.Item().Text("EXPEDIENTE DE CLIENTE")
                            .FontSize(12).SemiBold().FontColor(_textMuted);

                        col.Item().Text($"ID CLIENTE: {_perfil.Id:D6}")
                            .FontSize(16).Bold().FontColor(_primaryColor);
                    });
                });
            }

            private void ComposeClientInfo(IContainer container)
            {
                container.Background(_secondaryColor).Padding(12).Column(col =>
                {
                    col.Item().Text("INFORMACIÓN DEL CLIENTE").FontSize(10).Bold().FontColor(_primaryColor);

                    col.Item().PaddingTop(6).Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Nombre Completo").FontSize(8).SemiBold().FontColor(_textMuted);
                            c.Item().Text($"{_perfil.Nombre} {_perfil.Apellido}").FontSize(11).Bold().FontColor(_textDark);
                        });

                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Correo Electrónico").FontSize(8).SemiBold().FontColor(_textMuted);
                            c.Item().Text(!string.IsNullOrWhiteSpace(_perfil.Email) ? _perfil.Email : "N/A").FontSize(10).FontColor(_textDark);
                        });

                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Teléfono / Móvil").FontSize(8).SemiBold().FontColor(_textMuted);
                            c.Item().Text(!string.IsNullOrWhiteSpace(_perfil.Telefono) ? _perfil.Telefono : "N/A").FontSize(10).FontColor(_textDark);
                        });
                    });

                    // Si hay fechas de registro válidas las mostramos
                    if (_perfil.Creado != DateTime.MinValue && _perfil.Creado != default)
                    {
                        col.Item().PaddingTop(6).Row(row =>
                        {
                            row.RelativeItem().Text(t =>
                            {
                                t.Span("Fecha de Registro: ").FontSize(8).SemiBold().FontColor(_textMuted);
                                t.Span($"{_perfil.Creado:dd/MM/yyyy}").FontSize(8).FontColor(_textDark);
                            });
                        });
                    }
                });
            }

            private void ComposeMascotasSection(IContainer container)
            {
                container.Column(col =>
                {
                    col.Item().PaddingBottom(6).Row(r =>
                    {
                        r.RelativeItem().Text("MASCOTAS REGISTRADAS").FontSize(11).Bold().FontColor(_primaryColor);
                        r.ConstantItem(100).AlignRight().Text($"Total: {_perfil.Mascota?.Count ?? 0}").FontSize(9).FontColor(_textMuted);
                    });

                    if (_perfil.Mascota == null || !_perfil.Mascota.Any())
                    {
                        col.Item().Border(1).BorderColor(_borderLight).Padding(10)
                            .Text("No hay mascotas asociadas a este perfil.").FontSize(9.5f).Italic().FontColor(_textMuted);
                    }
                    else
                    {
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(50);  // ID
                                columns.RelativeColumn(3);  // Nombre
                                columns.RelativeColumn(2);  // Especie
                                columns.RelativeColumn(2);  // Raza
                                columns.ConstantColumn(60);  // Edad
                                columns.ConstantColumn(60);  // Peso
                            });

                            table.Header(header =>
                            {
                                IContainer StyleHeader(IContainer c) => c.Background(_primaryColor).Padding(5).AlignMiddle();

                                header.Cell().Element(StyleHeader).Text("ID").FontColor(Colors.White).FontSize(8.5f).Bold();
                                header.Cell().Element(StyleHeader).Text("Nombre").FontColor(Colors.White).FontSize(8.5f).Bold();
                                header.Cell().Element(StyleHeader).Text("Especie").FontColor(Colors.White).FontSize(8.5f).Bold();
                                header.Cell().Element(StyleHeader).Text("Raza").FontColor(Colors.White).FontSize(8.5f).Bold();
                                header.Cell().Element(StyleHeader).AlignCenter().Text("Edad").FontColor(Colors.White).FontSize(8.5f).Bold();
                                header.Cell().Element(StyleHeader).AlignRight().Text("Peso").FontColor(Colors.White).FontSize(8.5f).Bold();
                            });

                            bool alternate = false;
                            foreach (var m in _perfil.Mascota)
                            {
                                var rowBg = alternate ? _secondaryColor : "#FFFFFF";
                                alternate = !alternate;

                                IContainer StyleCell(IContainer c) => c.Background(rowBg).Padding(5).BorderBottom(1).BorderColor(_borderLight).AlignMiddle();

                                table.Cell().Element(StyleCell).Text($"#{m.Id}").FontSize(8.5f).FontColor(_textMuted);
                                table.Cell().Element(StyleCell).Text(m.Nombre ?? "-").FontSize(8.5f).Bold().FontColor(_textDark);
                                table.Cell().Element(StyleCell).Text(m.Especie ?? "-").FontSize(8.5f).FontColor(_textDark);
                                table.Cell().Element(StyleCell).Text(m.Raza ?? "-").FontSize(8.5f).FontColor(_textDark);
                                table.Cell().Element(StyleCell).AlignCenter().Text(m.Edad.HasValue ? $"{m.Edad} años" : "-").FontSize(8.5f).FontColor(_textDark);
                                table.Cell().Element(StyleCell).AlignRight().Text(m.Peso.HasValue ? $"{m.Peso:N1} kg" : "-").FontSize(8.5f).FontColor(_textDark);
                            }
                        });
                    }
                });
            }

            private void ComposeFacturasSection(IContainer container)
            {
                container.Column(col =>
                {
                    col.Item().PaddingBottom(6).Row(r =>
                    {
                        r.RelativeItem().Text("HISTORIAL DE FACTURACIÓN").FontSize(11).Bold().FontColor(_primaryColor);
                        r.ConstantItem(100).AlignRight().Text($"Total: {_perfil.Facturas?.Count ?? 0}").FontSize(9).FontColor(_textMuted);
                    });

                    if (_perfil.Facturas == null || !_perfil.Facturas.Any())
                    {
                        col.Item().Border(1).BorderColor(_borderLight).Padding(10)
                            .Text("No hay facturas registradas para este cliente.").FontSize(9.5f).Italic().FontColor(_textMuted);
                    }
                    else
                    {
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(70);  // N° Factura
                                columns.RelativeColumn(2);  // Fecha
                                columns.RelativeColumn(2);  // Estado
                                columns.RelativeColumn(2);  // Total
                            });

                            table.Header(header =>
                            {
                                IContainer StyleHeader(IContainer c) => c.Background(_primaryColor).Padding(5).AlignMiddle();

                                header.Cell().Element(StyleHeader).Text("N° Factura").FontColor(Colors.White).FontSize(8.5f).Bold();
                                header.Cell().Element(StyleHeader).Text("Fecha").FontColor(Colors.White).FontSize(8.5f).Bold();
                                header.Cell().Element(StyleHeader).Text("Estado").FontColor(Colors.White).FontSize(8.5f).Bold();
                                header.Cell().Element(StyleHeader).AlignRight().Text("Monto Total").FontColor(Colors.White).FontSize(8.5f).Bold();
                            });

                            bool alternate = false;
                            foreach (var f in _perfil.Facturas)
                            {
                                var rowBg = alternate ? _secondaryColor : "#FFFFFF";
                                alternate = !alternate;

                                IContainer StyleCell(IContainer c) => c.Background(rowBg).Padding(5).BorderBottom(1).BorderColor(_borderLight).AlignMiddle();

                                table.Cell().Element(StyleCell).Text($"#{f.Id:D6}").FontSize(8.5f).Bold().FontColor(_textDark);
                                table.Cell().Element(StyleCell).Text(f.Fecha != default ? f.Fecha.ToString("dd/MM/yyyy HH:mm") : "-").FontSize(8.5f).FontColor(_textDark);
                                table.Cell().Element(StyleCell).Text(f.Estado ?? "Completada").FontSize(8.5f).FontColor(_textDark);
                                table.Cell().Element(StyleCell).AlignRight().Text($"{_currencySymbol}{f.Total:N2}").FontSize(8.5f).Bold().FontColor(_primaryColor);
                            }
                        });

                        // Sumatoria total gastado
                        decimal totalAcumulado = _perfil.Facturas.Sum(f => f.Total);
                        col.Item().PaddingTop(6).AlignRight().Text(t =>
                        {
                            t.Span("Total Acumulado: ").FontSize(10).Bold().FontColor(_textDark);
                            t.Span($"{_currencySymbol}{totalAcumulado:N2}").FontSize(11).Bold().FontColor(_primaryColor);
                        });
                    }
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