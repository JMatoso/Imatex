using Imatex.Web.Models;

namespace Imatex.Web.Services.Extractor
{
    public interface IImageExtractorService
    {
        Task<ExtratedImageResult> ExtractImagesFromDocumentAsync(Stream document, string? fileName = "", bool resizeImages = false, int width = 640, int height = 480, bool keepAspectRatio = true, CancellationToken cancellationToken = default);
        Task<Stream?> ResizeImageAsync(Stream image, int width = 640, int height = 480, bool keepAspectRatio = true, CancellationToken cancellationToken = default);
    }
}