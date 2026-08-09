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

namespace vet_api_Net.Utilities.Pdf
{
    public class GeneratePdfUtilities
    {
        public string GenerateInvoicePdf(FacturationDTO invoice, string webRootPath, string currencySymbol)
        {
            string clientePart = !string.IsNullOrWhiteSpace(invoice.ClienteNombre) ? invoice.ClienteNombre : $"{PdfText.Clinte}{invoice.ClienteId}";
            string mascotaPart = !string.IsNullOrWhiteSpace(invoice.MascotaNombre) ? invoice.MascotaNombre : (invoice.MascotaId.HasValue ? $"{PdfText.Mascota}{invoice.MascotaId.Value}" : PdfText.Mascota);

            clientePart = SanitizePart(clientePart);
            mascotaPart = SanitizePart(mascotaPart);

            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            string safeFactura = SanitizePart(invoice.NumeroFactura ?? timestamp);
            string fileName = $"Recibo_{safeFactura}_{clientePart}_{mascotaPart}.pdf";
            string dir = Path.Combine(webRootPath, "facturas");
            Directory.CreateDirectory(dir);
            string filePath = Path.Combine(dir, fileName);

            string logoImagePath = Path.Combine(webRootPath, "images", "HappyPetsLogo.png");

            var document = new InvoiceDocument(invoice, currencySymbol, logoImagePath);
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

        private class InvoiceDocument : IDocument
        {
            private readonly FacturationDTO _inv;
            private readonly string _currencySymbol;
            private readonly string _logoImagePath;
            private readonly string _primaryColor = Colors.Blue.Darken3;
            private readonly string _secondaryColor = Colors.Blue.Lighten5;
            private readonly string _textDark = Colors.Grey.Darken4;
            private readonly string _textMuted = Colors.Grey.Darken2;
            private readonly string _borderLight = Colors.Grey.Lighten2;

            public InvoiceDocument(FacturationDTO inv, string currencySymbol, string logoImagePath)
            {
                _inv = inv;
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

                    page.Content().PaddingVertical(20).Column(column =>
                    {
                        column.Item().Element(ComposeClientAndPetInfo);
                        
                        column.Item().PaddingTop(20).Element(c => BuildStyledItemsTable(c, _inv.Items ?? new List<FacturationItemDTO>()));

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

                    row.ConstantItem(200).AlignRight().Column(col =>
                    {
                        col.Item().Text("RECIBO DE PAGO")
                            .FontSize(12).SemiBold().FontColor(_textMuted);
                        
                        col.Item().Text(_inv.NumeroFactura ?? "S/N")
                            .FontSize(18).Bold().FontColor(_primaryColor);
                    });
                });
            }

            private void ComposeClientAndPetInfo(IContainer container)
            {
                container.Row(r =>
                {
                    r.RelativeItem().Column(c =>
                    {
                        c.Item().Text(PdfText.Clinte.ToUpper()).FontSize(9).Bold().FontColor(_primaryColor);
                        c.Item().PaddingTop(2).Text(_inv.ClienteNombre ?? "-").FontSize(12).Bold().FontColor(_textDark);
                        c.Item().Text($"{PdfText.IdCliente}: {_inv.ClienteId}").FontSize(9).FontColor(_textMuted);
                    });

                    r.RelativeItem().Column(c =>
                    {
                        c.Item().Text(PdfText.Mascota.ToUpper()).FontSize(9).Bold().FontColor(_primaryColor);
                        c.Item().PaddingTop(2).Text(_inv.MascotaNombre ?? "-").FontSize(12).Bold().FontColor(_textDark);
                    });

                    r.ConstantItem(150).Column(c =>
                    {
                        c.Item().AlignRight().Text(text =>
                        {
                            text.Span($"{PdfText.Fecha}: ").FontSize(9).SemiBold().FontColor(_textMuted);
                            text.Span($"{_inv.FechaEmision:dd/MM/yyyy}").FontSize(9).FontColor(_textDark);
                        });
                        
                        c.Item().PaddingTop(2).AlignRight().Text(text =>
                        {
                            text.Span($"{PdfText.Vencimiento}: ").FontSize(9).SemiBold().FontColor(_textMuted);
                            text.Span($"{_inv.FechaEmision.AddDays(30):dd/MM/yyyy}").FontSize(9).FontColor(_textDark);
                        });
                    });
                });
            }

          private void BuildStyledItemsTable(IContainer container, List<FacturationItemDTO> items)
{
    container.Table(table =>
    {
        table.ColumnsDefinition(columns =>
        {
            columns.ConstantColumn(70);
            columns.RelativeColumn();
            columns.ConstantColumn(50);
            columns.ConstantColumn(80);
            columns.ConstantColumn(80);
        });

        table.Header(header =>
        {
            IContainer StyleHeaderCell(IContainer c, string bg) => c.Background(bg).Padding(6).AlignMiddle();

            header.Cell().Element(c => StyleHeaderCell(c, _primaryColor)).Text(PdfText.Code).FontColor(Colors.White).FontSize(9).Bold();
            header.Cell().Element(c => StyleHeaderCell(c, _primaryColor)).Text(PdfText.Description).FontColor(Colors.White).FontSize(9).Bold();
            header.Cell().Element(c => StyleHeaderCell(c, _primaryColor)).AlignRight().Text(PdfText.Quantity).FontColor(Colors.White).FontSize(9).Bold();
            header.Cell().Element(c => StyleHeaderCell(c, _primaryColor)).AlignRight().Text(PdfText.Price).FontColor(Colors.White).FontSize(9).Bold();
            header.Cell().Element(c => StyleHeaderCell(c, _primaryColor)).AlignRight().Text(PdfText.Total).FontColor(Colors.White).FontSize(9).Bold();
        });

        if (!items.Any())
        {
            table.Cell().ColumnSpan(5).Padding(12).AlignCenter().Text(PdfText.NotServices)
                .FontColor(_textMuted).Italic().FontSize(10);
            return;
        }

        bool alternate = false;
        foreach (var it in items)
        { 
            var rowColor = alternate ? _secondaryColor : "#FFFFFF";
            alternate = !alternate;

            IContainer StyleContentCell(IContainer c, string bg, string border) => 
                c.Background(bg).PaddingHorizontal(6).PaddingVertical(6).BorderBottom(1).BorderColor(border).AlignMiddle();

            table.Cell().Element(c => StyleContentCell(c, rowColor, _borderLight)).Text(it.Codigo ?? "").FontSize(9).FontColor(_textDark);
            table.Cell().Element(c => StyleContentCell(c, rowColor, _borderLight)).Text(it.Nombre ?? "").FontSize(9).FontColor(_textDark);
            table.Cell().Element(c => StyleContentCell(c, rowColor, _borderLight)).AlignRight().Text(it.Cantidad.ToString()).FontSize(9).FontColor(_textDark);
            table.Cell().Element(c => StyleContentCell(c, rowColor, _borderLight)).AlignRight().Text($"{_currencySymbol}{it.PrecioUnitario:N2}").FontSize(9).FontColor(_textDark);
            table.Cell().Element(c => StyleContentCell(c, rowColor, _borderLight)).AlignRight().Text($"{_currencySymbol}{it.Total:N2}").FontSize(9).Bold().FontColor(_textDark);
        }
    });
}

            private void ComposeTotals(IContainer container)
            {
                container.Row(r =>
                {
                    r.RelativeItem();

                    r.ConstantItem(220).Column(c =>
                    {
                        c.Item().PaddingVertical(2).Row(rr =>
                        {
                            rr.RelativeItem().Text(PdfText.Subtotal).FontSize(9).FontColor(_textMuted);
                            rr.ConstantItem(90).AlignRight().Text($"{_currencySymbol}{_inv.Subtotal:N2}").FontSize(9).FontColor(_textDark);
                        });
                        
                        // c.Item().PaddingVertical(2).Row(rr =>
                        // {
                        //     rr.RelativeItem().Text(PdfText.IVA).FontSize(9).FontColor(_textMuted);
                        //     rr.ConstantItem(90).AlignRight().Text(PdfText.NumberPrecio).FontSize(9).FontColor(_textDark);
                        // });
                        
                        c.Item().PaddingVertical(2).Row(rr =>
                        {
                            rr.RelativeItem().Text(PdfText.ConsultaPrice).FontSize(9).FontColor(_textMuted);
                            rr.ConstantItem(90).AlignRight().Text(_inv.Descuento.HasValue ? $"{_currencySymbol}{_inv.Descuento.Value:N2}" : $"{_currencySymbol}0.00").FontSize(9).FontColor(_textDark);
                        });
                        
                        c.Item().PaddingVertical(4).LineHorizontal(1).LineColor(_primaryColor);
                         
                        c.Item().Background(_secondaryColor).Padding(6).Row(rr =>
                        {
                            rr.RelativeItem().AlignMiddle().Text(PdfText.Total).FontSize(11).Bold().FontColor(_primaryColor);
                            rr.ConstantItem(90).AlignRight().AlignMiddle().Text($"{_currencySymbol}{_inv.Total:N2}")
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