using Imatex.Web.Models;

namespace Imatex.Web.Services.Compression
{
    public interface IZipCompressorService
    {
        Task<byte[]> CreateZipFileInMemoryAsync(List<ExtractedImage> extractedImages, CancellationToken cancellationToken = default);
    }
}