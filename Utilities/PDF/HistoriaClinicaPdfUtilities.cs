using System;
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
    public class HistoriaClinicaPdfUtilities : IHistoriaClinicaPdfUtilities
    {
        public byte[] GenerateHistoriaClinicaPdf(HistoriaClinicaResponseDTO historia, string webRootPath)
        {
            string logoImagePath = Path.Combine(webRootPath, "images", "HappyPetsLogo.png");

            var document = new HistoriaClinicaDocument(historia, logoImagePath);
            return document.GeneratePdf();
        }

        public string BuildFileName(HistoriaClinicaResponseDTO historia)
        {
            string mascotaPart = !string.IsNullOrWhiteSpace(historia.Mascota?.Nombre)
                ? historia.Mascota.Nombre
                : $"{PdfText.Mascota}_{historia.MascotaId}";

            mascotaPart = SanitizePart(mascotaPart);

            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            return $"Historial_Medico_{mascotaPart}_{timestamp}.pdf";
        }

        private static string SanitizePart(string input)
        {
            string clean = (input ?? string.Empty).Trim().Replace(' ', '_');
            foreach (var c in Path.GetInvalidFileNameChars())
                clean = clean.Replace(c, '-');
            return clean;
        }

        private class HistoriaClinicaDocument : IDocument
        {
            private readonly HistoriaClinicaResponseDTO _historia;
            private readonly string _logoImagePath;
            private readonly string _primaryColor = Colors.Green.Darken3;
            private readonly string _secondaryColor = Colors.Green.Lighten5;
            private readonly string _textDark = Colors.Grey.Darken4;
            private readonly string _textMuted = Colors.Grey.Darken2;
            private readonly string _borderLight = Colors.Grey.Lighten2;
            private readonly string _alertColor = Colors.Red.Darken2;

            public HistoriaClinicaDocument(HistoriaClinicaResponseDTO historia, string logoImagePath)
            {
                _historia = historia;
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
                        column.Item().Element(ComposePetInfo);

                        if (!string.IsNullOrWhiteSpace(_historia.ResumenIa))
                            column.Item().PaddingTop(15).Element(c => ComposeTextBlock(c, "RESUMEN CLÍNICO", _historia.ResumenIa, _primaryColor));

                        if (!string.IsNullOrWhiteSpace(_historia.AlertasRiesgoIa))
                            column.Item().PaddingTop(10).Element(c => ComposeTextBlock(c, "ALERTAS Y RIESGOS", _historia.AlertasRiesgoIa, _alertColor));

                        if (!string.IsNullOrWhiteSpace(_historia.SugerenciasIa))
                            column.Item().PaddingTop(10).Element(c => ComposeTextBlock(c, "SUGERENCIAS", _historia.SugerenciasIa, _primaryColor));

                        column.Item().PaddingTop(15).Element(BuildConsultasTable);
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
                        col.Item().Text("HISTORIAL MÉDICO")
                            .FontSize(12).SemiBold().FontColor(_textMuted);

                        col.Item().Text($"Mascota N° {_historia.MascotaId:D6}")
                            .FontSize(14).Bold().FontColor(_primaryColor);
                    });
                });
            }

            private void ComposePetInfo(IContainer container)
            {
                var mascota = _historia.Mascota;

                container.Background(_secondaryColor).Padding(10).Row(r =>
                {
                    r.RelativeItem().Column(c =>
                    {
                        c.Item().Text(PdfText.Mascota.ToUpper()).FontSize(9).Bold().FontColor(_primaryColor);
                        c.Item().PaddingTop(2).Text(mascota?.Nombre ?? "-").FontSize(12).Bold().FontColor(_textDark);
                        c.Item().Text($"{mascota?.Especie}{(!string.IsNullOrWhiteSpace(mascota?.Raza) ? $" - {mascota.Raza}" : string.Empty)}").FontSize(8).FontColor(_textMuted);
                    });

                    r.RelativeItem().Column(c =>
                    {
                        c.Item().Text("ALERGIAS").FontSize(9).Bold().FontColor(_primaryColor);
                        c.Item().PaddingTop(2).Text(string.IsNullOrWhiteSpace(mascota?.Alergias) ? "Ninguna registrada" : mascota.Alergias).FontSize(9).FontColor(_textDark);
                    });

                    r.RelativeItem().Column(c =>
                    {
                        c.Item().Text("CONDICIONES MÉDICAS").FontSize(9).Bold().FontColor(_primaryColor);
                        c.Item().PaddingTop(2).Text(string.IsNullOrWhiteSpace(mascota?.CondicionesMedicas) ? "Ninguna registrada" : mascota.CondicionesMedicas).FontSize(9).FontColor(_textDark);
                    });
                });
            }

            private void ComposeTextBlock(IContainer container, string title, string text, string accentColor)
            {
                container.Column(col =>
                {
                    col.Item().PaddingBottom(4).Text(title).FontSize(10).Bold().FontColor(accentColor);
                    col.Item().Background(Colors.Grey.Lighten4).Padding(8).Text(text).FontSize(9).FontColor(_textDark);
                });
            }

            private void BuildConsultasTable(IContainer container)
            {
                container.Column(col =>
                {
                    col.Item().PaddingBottom(4).Text("CONSULTAS REGISTRADAS").FontSize(10).Bold().FontColor(_primaryColor);

                    if (_historia.Consultas == null || _historia.Consultas.Count == 0)
                    {
                        col.Item().Text("Sin consultas registradas.").FontSize(9).FontColor(_textMuted);
                        return;
                    }

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            IContainer StyleHeaderCell(IContainer c) => c.Background(_primaryColor).Padding(5).AlignMiddle();

                            header.Cell().Element(StyleHeaderCell).Text(PdfText.Fecha).FontColor(Colors.White).FontSize(8.5f).Bold();
                            header.Cell().Element(StyleHeaderCell).Text("Síntomas / Diagnóstico").FontColor(Colors.White).FontSize(8.5f).Bold();
                            header.Cell().Element(StyleHeaderCell).Text("Tratamiento").FontColor(Colors.White).FontSize(8.5f).Bold();
                            header.Cell().Element(StyleHeaderCell).Text(PdfText.Doctor).FontColor(Colors.White).FontSize(8.5f).Bold();
                        });

                        bool alternate = false;
                        foreach (var consulta in _historia.Consultas)
                        {
                            var rowColor = alternate ? _secondaryColor : "#FFFFFF";
                            alternate = !alternate;

                            IContainer StyleContentCell(IContainer c) =>
                                c.Background(rowColor).PaddingHorizontal(5).PaddingVertical(4).BorderBottom(1).BorderColor(_borderLight).AlignMiddle();

                            string sintomasDiagnostico = string.Join(" / ", new[] { consulta.Sintomas, consulta.Diagnostico }.Where(s => !string.IsNullOrWhiteSpace(s)));

                            table.Cell().Element(StyleContentCell).Text(consulta.FechaConsulta.ToString(PdfText.DMY)).FontSize(8.5f).FontColor(_textDark);
                            table.Cell().Element(StyleContentCell).Text(string.IsNullOrEmpty(sintomasDiagnostico) ? "-" : sintomasDiagnostico).FontSize(8f).FontColor(_textDark);
                            table.Cell().Element(StyleContentCell).Text(string.IsNullOrEmpty(consulta.Tratamiento) ? "-" : consulta.Tratamiento).FontSize(8f).FontColor(_textDark);
                            table.Cell().Element(StyleContentCell).Text(string.IsNullOrEmpty(consulta.DoctorNombre) ? "-" : consulta.DoctorNombre).FontSize(8f).FontColor(_textDark);
                        }
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
