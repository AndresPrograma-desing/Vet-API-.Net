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
    public class VacunaCertifyPdfUtilities : IVaccinationCertificatePdfUtilities
    {
        public byte[] GenerateCarnetPdf(PetVaccinationCarnetPdfDTO carnet, string webRootPath)
        {
            string logoImagePath = Path.Combine(webRootPath, "images", "HappyPetsLogo.png");

            var document = new CarnetDocument(carnet, logoImagePath);
            return document.GeneratePdf();
        }

        public string BuildFileName(PetVaccinationCarnetPdfDTO carnet)
        {
            string mascotaPart = !string.IsNullOrWhiteSpace(carnet.MascotaNombre)
                ? carnet.MascotaNombre
                : $"{PdfText.Mascota}_{carnet.MascotaId}";

            mascotaPart = SanitizePart(mascotaPart);

            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            return $"Carnet_Vacunacion_{mascotaPart}_{timestamp}.pdf";
        }

        private static string SanitizePart(string input)
        {
            string clean = (input ?? string.Empty).Trim().Replace(' ', '_');
            foreach (var c in Path.GetInvalidFileNameChars())
                clean = clean.Replace(c, '-');
            return clean;
        }

        private class CarnetDocument : IDocument
        {
            private readonly PetVaccinationCarnetPdfDTO _carnet;
            private readonly string _logoImagePath;
            private readonly string _primaryColor = Colors.Blue.Darken3;
            private readonly string _secondaryColor = Colors.Blue.Lighten5;
            private readonly string _textDark = Colors.Grey.Darken4;
            private readonly string _textMuted = Colors.Grey.Darken2;
            private readonly string _borderLight = Colors.Grey.Lighten2;

            public CarnetDocument(PetVaccinationCarnetPdfDTO carnet, string logoImagePath)
            {
                _carnet = carnet;
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
                        column.Item().PaddingTop(15).Element(BuildVaccinationsTable);
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
                        col.Item().Text("CARNET DE VACUNACIÓN")
                            .FontSize(12).SemiBold().FontColor(_textMuted);

                        col.Item().Text($"Mascota N° {_carnet.MascotaId:D6}")
                            .FontSize(14).Bold().FontColor(_primaryColor);
                    });
                });
            }

            private void ComposePetInfo(IContainer container)
            {
                container.Background(_secondaryColor).Padding(10).Row(r =>
                {
                    r.RelativeItem().Column(c =>
                    {
                        c.Item().Text(PdfText.Mascota.ToUpper()).FontSize(9).Bold().FontColor(_primaryColor);
                        c.Item().PaddingTop(2).Text(_carnet.MascotaNombre).FontSize(12).Bold().FontColor(_textDark);
                        c.Item().Text($"{_carnet.Especie}{(!string.IsNullOrWhiteSpace(_carnet.Raza) ? $" - {_carnet.Raza}" : string.Empty)}").FontSize(8).FontColor(_textMuted);
                    });

                    r.RelativeItem().Column(c =>
                    {
                        c.Item().Text("DATOS DEL CLIENTE").FontSize(9).Bold().FontColor(_primaryColor);
                        c.Item().PaddingTop(2).Text(!string.IsNullOrEmpty(_carnet.ClienteNombre) ? _carnet.ClienteNombre : "N/A").FontSize(10).Bold().FontColor(_textDark);
                        if (!string.IsNullOrEmpty(_carnet.ClienteTelefono))
                            c.Item().Text($"Tel: {_carnet.ClienteTelefono}").FontSize(8).FontColor(_textMuted);
                    });
                });
            }

            private void BuildVaccinationsTable(IContainer container)
            {
                container.Column(col =>
                {
                    col.Item().PaddingBottom(4).Text("HISTORIAL DE VACUNAS APLICADAS").FontSize(10).Bold().FontColor(_primaryColor);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            IContainer StyleHeaderCell(IContainer c, string bg) => c.Background(bg).Padding(5).AlignMiddle();

                            header.Cell().Element(c => StyleHeaderCell(c, _primaryColor)).Text("Vacuna").FontColor(Colors.White).FontSize(8.5f).Bold();
                            header.Cell().Element(c => StyleHeaderCell(c, _primaryColor)).Text(PdfText.Fecha).FontColor(Colors.White).FontSize(8.5f).Bold();
                            header.Cell().Element(c => StyleHeaderCell(c, _primaryColor)).Text("Lote / Laboratorio").FontColor(Colors.White).FontSize(8.5f).Bold();
                            header.Cell().Element(c => StyleHeaderCell(c, _primaryColor)).Text("Próxima Dosis").FontColor(Colors.White).FontSize(8.5f).Bold();
                            header.Cell().Element(c => StyleHeaderCell(c, _primaryColor)).Text(PdfText.Doctor).FontColor(Colors.White).FontSize(8.5f).Bold();
                        });

                        bool alternate = false;
                        foreach (var item in _carnet.Items)
                        {
                            var rowColor = alternate ? _secondaryColor : "#FFFFFF";
                            alternate = !alternate;

                            IContainer StyleContentCell(IContainer c, string bg, string border) =>
                                c.Background(bg).PaddingHorizontal(5).PaddingVertical(4).BorderBottom(1).BorderColor(border).AlignMiddle();

                            string loteLab = string.Join(" / ", new[] { item.BatchNumber, item.Laboratory }.Where(s => !string.IsNullOrWhiteSpace(s)));

                            table.Cell().Element(c => StyleContentCell(c, rowColor, _borderLight)).Text(item.VaccineName).FontSize(8.5f).FontColor(_textDark);
                            table.Cell().Element(c => StyleContentCell(c, rowColor, _borderLight)).Text(item.ApplicationDate.ToString(PdfText.DMY)).FontSize(8.5f).FontColor(_textDark);
                            table.Cell().Element(c => StyleContentCell(c, rowColor, _borderLight)).Text(string.IsNullOrEmpty(loteLab) ? "-" : loteLab).FontSize(8f).FontColor(_textDark);
                            table.Cell().Element(c => StyleContentCell(c, rowColor, _borderLight)).Text(item.NextDoseDate.HasValue ? item.NextDoseDate.Value.ToString(PdfText.DMY) : "-").FontSize(8.5f).FontColor(_textDark);
                            table.Cell().Element(c => StyleContentCell(c, rowColor, _borderLight)).Text(item.DoctorName ?? "-").FontSize(8f).FontColor(_textDark);
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
