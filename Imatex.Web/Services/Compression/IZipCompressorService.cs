using Imatex.Web.Models;

namespace Imatex.Web.Services.Compression
{
    public interface IZipCompressorService
    {
        byte[] CreateZipFileInMemory(IEnumerable<ExtractedImage> extractedImages);
    }
}