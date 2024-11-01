using Imatex.Web.Models;
using System.Drawing;

namespace Imatex.Web.Services.Ocr
{
    public interface ITextConverterService : IDisposable
    {
        IAsyncEnumerable<ExtractedTextResult> GetTextFromImagesAsync(CancellationToken cancellationToken, float confidenceThreshold = 0.5F, params Bitmap[] images);
    }
}