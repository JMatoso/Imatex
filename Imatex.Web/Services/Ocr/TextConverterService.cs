using Imatex.Web.Models;
using System.Drawing;
using System.Runtime.CompilerServices;
using Tesseract;

namespace Imatex.Web.Services.Ocr;

public class TextConverterService : ITextConverterService
{
    private readonly TesseractEngine _tesseractEngine;
    private readonly string _tessDataPath = Path.Combine(Directory.GetCurrentDirectory(), "tessdata");

    public TextConverterService()
    {
        TesseractLinuxLoaderFix.Patch();

        if (!Directory.Exists(_tessDataPath))
        {
            throw new DirectoryNotFoundException($"Tesseract data directory not found at {_tessDataPath}");
        }

        _tesseractEngine = new TesseractEngine(_tessDataPath, "eng+por+fra+spa+ita", EngineMode.Default);
    }

    /// <summary>
    /// Extract the text from the images.
    /// </summary>
    /// <param name="images">Bitmap image files.</param>
    /// <returns>A list of <see cref="ExtractedTextResult"/></returns>
    public async IAsyncEnumerable<ExtractedTextResult> GetTextFromImagesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken,
        float confidenceThreshold = 0.5f,
        params Bitmap[] images)
    {
        foreach (var image in images)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            using var page = _tesseractEngine.Process(image, PageSegMode.AutoOsd);

            string text = page.GetText();

            if (string.IsNullOrWhiteSpace(text) || page.GetMeanConfidence() < confidenceThreshold)
            {
                continue;
            }

            yield return new ExtractedTextResult
            {
                Text = text,
                FileName = page.ImageName,
                DateIssued = DateTimeOffset.UtcNow,
                Confidence = page.GetMeanConfidence()
            };
        }

        await Task.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_tesseractEngine.IsDisposed)
        {
            if (disposing)
            {
                _tesseractEngine?.Dispose();
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
