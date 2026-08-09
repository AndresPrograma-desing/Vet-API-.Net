using System;
using System.Text;
using ImageMagick;
using Microsoft.Extensions.Logging;
using vet_api_Net.Interfaze.Utilities;

namespace vet_api_Net.Utilities;

public class ImageConversionUtilities : IImageConversionUtilities
{
    private readonly ILogger<ImageConversionUtilities> _logger;

    public ImageConversionUtilities(ILogger<ImageConversionUtilities> logger)
    {
        _logger = logger;
    }

    public byte[]? ConvertToRenderableImage(byte[] imageBytes)
    {
        if (imageBytes == null || imageBytes.Length == 0)
        {
            _logger.LogWarning("ConvertToRenderableImage recibió un arreglo de bytes vacío o nulo.");
            return null;
        }

        if (IsHtmlOrJsonErrorPayload(imageBytes))
        {
            _logger.LogWarning("ConvertToRenderableImage recibió un payload que parece HTML/JSON en vez de una imagen. Primeros bytes: {Preview}", PreviewBytes(imageBytes));
            return null;
        }

        try
        {
            using var image = new MagickImage(imageBytes);

            image.AutoOrient();
            image.ColorSpace = ColorSpace.sRGB;

            if (image.Width > 512 || image.Height > 512)
            {
                image.Resize(new MagickGeometry(512, 512)
                {
                    IgnoreAspectRatio = false,
                    Greater = true
                });
            }

            image.Strip();
            image.Format = MagickFormat.Png;

            return image.ToByteArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ConvertToRenderableImage falló al decodificar/convertir una imagen de {SizeBytes} bytes.", imageBytes.Length);
            return null;
        }
    }

    private static string PreviewBytes(byte[] bytes)
        => Encoding.UTF8.GetString(bytes, 0, Math.Min(bytes.Length, 100)).Replace('\n', ' ').Replace('\r', ' ');

    private static bool IsHtmlOrJsonErrorPayload(byte[] bytes)
    {
        if (bytes.Length < 10) return false;

        string header = Encoding.UTF8.GetString(bytes, 0, Math.Min(bytes.Length, 100)).TrimStart();

        return header.StartsWith("<", StringComparison.OrdinalIgnoreCase) ||
               header.StartsWith("{", StringComparison.OrdinalIgnoreCase);
    }
}