using Imatex.Web.Models;
using System.Drawing;
using System.Runtime.CompilerServices;
using Tesseract;

namespace Imatex.Web.Services.Ocr;

public class TextConverterService : ITextConverterService
{
    private static bool _isConfigured = false;
    private readonly ILogger<TextConverterService> _logger;
    private readonly TesseractEngine _tesseractEngine = default!;
    private readonly string _tessDataPath = Path.Combine(Directory.GetCurrentDirectory(), "tessdata");

    public TextConverterService(ILogger<TextConverterService> logger)
    {
        _logger = logger;

        try
        {
            if (!Directory.Exists(_tessDataPath))
            {
                _isConfigured = false;
                _logger.LogError("Tesseract data directory not found at {TessDataPath}", _tessDataPath);
                return;
            }

            _tesseractEngine = new TesseractEngine(_tessDataPath, "eng+por+fra+spa+ita", EngineMode.Default);
            _isConfigured = true;
        }
        catch (Exception ex)
        {
            _isConfigured = false;
            _logger.LogError(ex, "Failed to initialize Tesseract engine");
        }
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
        if (!_isConfigured)
        {
            yield return new ExtractedTextResult
            {
                Text = "Tesseract engine is not configured",
                FileName = string.Empty,
                DateIssued = DateTimeOffset.UtcNow,
                Confidence = 0
            };
        }

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
