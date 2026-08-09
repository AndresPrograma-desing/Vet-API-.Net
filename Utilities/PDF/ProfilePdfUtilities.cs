using System;
using System.IO;
using System.Net.Http;
using DTOs;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Utilities;

//Describe: Genera el PDF de credencial de usuario. Si el avatar descargado no puede decodificarse (ej. formato AVIF no soportado por SkiaSharp/QuestPDF), se regenera el documento sin foto en vez de fallar la descarga completa.
namespace vet_api_Net.Utilities.Pdf
{
    public class ProfilePdfUtilities : IProfilePdfUtilities
    {
        private readonly HttpClient _httpClient;
        private readonly IImageConversionUtilities _imageConversion;
        private readonly ILogger<ProfilePdfUtilities> _logger;

        public ProfilePdfUtilities(HttpClient httpClient, IImageConversionUtilities imageConversion, ILogger<ProfilePdfUtilities> logger)
        {
            _httpClient = httpClient;
            _imageConversion = imageConversion;
            _logger = logger;
        }

        public byte[] GenerateCredencialPdf(CredencialUsuarioPdfDTO usuario, string webRootPath)
        {
            string logoImagePath = Path.Combine(webRootPath, "images", "HappyPetsLogo.png");
            string fallbackAvatarSvgPath = ResolveFallbackAvatarSvgPath(usuario, webRootPath);

            byte[]? avatarBytes = DownloadAvatarBytes(usuario.Avatar);

            try
            {
                var document = new CredencialDocument(usuario, avatarBytes, logoImagePath, fallbackAvatarSvgPath);
                return document.GeneratePdf();
            }
            catch (Exception ex) when (avatarBytes != null)
            {
                _logger.LogError(ex, "GenerateCredencialPdf falló al componer el PDF con avatar para el usuario {UsuarioId}. Se reintenta sin foto.", usuario.Id);
                var fallbackDocument = new CredencialDocument(usuario, null, logoImagePath, fallbackAvatarSvgPath);
                return fallbackDocument.GeneratePdf();
            }
        }

        private static string ResolveFallbackAvatarSvgPath(CredencialUsuarioPdfDTO usuario, string webRootPath)
        {
            string fileName = string.Equals(usuario.Rol, "secretaria", StringComparison.OrdinalIgnoreCase)
                ? "female-avatar.svg"
                : "male-avatar.svg";

            return Path.Combine(webRootPath, "assets", fileName);
        }

        public string BuildFileName(CredencialUsuarioPdfDTO usuario)
        {
            string safeNombre = SanitizePart(usuario.Name ?? $"Usuario_{usuario.Id}");
            return $"Credencial_{usuario.Id}_{safeNombre}.pdf";
        }

        private byte[]? DownloadAvatarBytes(string avatarUrl)
        {
            if (string.IsNullOrWhiteSpace(avatarUrl)) return null;

            try
            {
                var rawBytes = _httpClient.GetByteArrayAsync(avatarUrl).GetAwaiter().GetResult();
                return _imageConversion.ConvertToRenderableImage(rawBytes);
            }
            catch
            {
                return null;
            }
        }

        private static string SanitizePart(string input)
        {
            string clean = (input ?? string.Empty).Trim().Replace(' ', '_');
            foreach (var c in Path.GetInvalidFileNameChars())
                clean = clean.Replace(c, '-');
            return clean;
        }

        private class CredencialDocument : IDocument
        {
            private readonly CredencialUsuarioPdfDTO _user;
            private readonly byte[]? _avatarBytes;
            private readonly string _logoImagePath;
            private readonly string _fallbackAvatarSvgPath;

            private readonly string _primaryColor = Colors.Blue.Darken3;
            private readonly string _secondaryColor = Colors.Blue.Lighten5;
            private readonly string _accentColor = Colors.Blue.Darken1;
            private readonly string _textDark = Colors.Grey.Darken4;
            private readonly string _textMuted = Colors.Grey.Darken2;

            public CredencialDocument(CredencialUsuarioPdfDTO user, byte[]? avatarBytes, string logoImagePath, string fallbackAvatarSvgPath)
            {
                _user = user;
                _avatarBytes = avatarBytes;
                _logoImagePath = logoImagePath;
                _fallbackAvatarSvgPath = fallbackAvatarSvgPath;
            }

            public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

            public void Compose(IDocumentContainer container)
            {
                container.Page(page =>
                {
                    page.Size(new PageSize(242.6f, 153.0f));
                    page.Margin(8);
                    page.PageColor(Colors.White);

                    page.Content().Border(1).BorderColor(_primaryColor).Column(col =>
                    {
                        col.Item().Element(ComposeHeader);

                        col.Item().Padding(6).Element(ComposeBody);

                        col.Item().Element(ComposeFooterBadge);
                    });
                });
            }

            private void ComposeHeader(IContainer container)
            {
                container.Background(_secondaryColor).PaddingHorizontal(6).PaddingVertical(4).Row(r =>
                {
                    r.RelativeItem().Row(hr =>
                    {
                        if (File.Exists(_logoImagePath))
                        {
                            hr.ConstantItem(20).PaddingRight(4).AlignMiddle().Image(_logoImagePath);
                        }

                        hr.RelativeItem().Column(c =>
                        {
                            c.Item().Text(PdfText.HappyPets).FontSize(9).Bold().FontColor(_primaryColor);
                            c.Item().Text("IDENTIFICACIÓN").FontSize(5).SemiBold().FontColor(_textMuted);
                        });
                    });

                    r.ConstantItem(50).AlignRight().AlignMiddle().Text($"ID: {_user.Id}")
                        .FontSize(7).Bold().FontColor(_primaryColor);
                });
            }

            private void ComposeBody(IContainer container)
            {
                container.Row(row =>
                {
                    row.ConstantItem(48).Height(48).Element(c =>
                    {
                        if (_avatarBytes != null && _avatarBytes.Length > 0)
                        {
                            c.Border(1).BorderColor(_primaryColor).Image(_avatarBytes).FitArea();
                        }
                        else if (File.Exists(_fallbackAvatarSvgPath))
                        {
                            c.Border(1).BorderColor(_primaryColor).Svg(File.ReadAllText(_fallbackAvatarSvgPath));
                        }
                        else
                        {
                            c.Background(_secondaryColor).Border(1).BorderColor(_primaryColor)
                             .AlignCenter().AlignMiddle().Text(_user.Name?.Substring(0, 1) ?? "U")
                             .FontSize(18).Bold().FontColor(_primaryColor);
                        }
                    });

                    row.RelativeItem().PaddingLeft(6).Column(col =>
                    {
                        col.Item().Text(_user.Name ?? "Sin Nombre")
                            .FontSize(8.5f).Bold().FontColor(_textDark);

                        col.Item().PaddingTop(1).Text(_user.Email ?? "")
                            .FontSize(6f).FontColor(_textMuted);

                        col.Item().Text($"Tel: {(_user.Phone ?? "N/A")}")
                            .FontSize(6f).FontColor(_textMuted);
                    });
                });
            }

            private void ComposeFooterBadge(IContainer container)
            {
                string rolTexto = (_user.Rol ?? "PERSONAL").ToUpper();

                if (rolTexto == "ADMIN")
                {
                    rolTexto = "ADMINISTRADOR";
                }
                container.Background(_primaryColor).PaddingVertical(3).AlignCenter()
                    .Text(rolTexto)
                    .FontSize(7.5f).Bold().FontColor(Colors.White);
            }
        }
    }
}
