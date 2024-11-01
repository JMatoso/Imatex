using Imatex.Web.Models;
using Newtonsoft.Json;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
            //TesseractLinuxLoaderFix.Patch();

            if (!Directory.Exists(_tessDataPath))
            {
                _isConfigured = false;
                _logger.LogCritical("Tesseract data directory not found at {TessDataPath} | {@OSInformation}", _tessDataPath, GetOSInformation());
                return;
            }

            _tesseractEngine = new TesseractEngine(_tessDataPath, "eng+por+fra+spa+ita", EngineMode.Default);
            _isConfigured = true;
        }
        catch (Exception ex)
        {
            _isConfigured = false;
            _logger.LogCritical(ex, "Failed to initialize Tesseract engine: {Message} | {@OSInformation}", ex.Message, GetOSInformation());
        }

        static string GetOSInformation()
        {
            object values = new
            {
                RuntimeInformation.OSDescription,
                RuntimeInformation.FrameworkDescription,
                OSArchitecture = RuntimeInformation.OSArchitecture.ToString(),
                OSVersion = Environment.OSVersion.ToString(),
                ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
                RuntimeInformation.RuntimeIdentifier,
                IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux),
                IsMacOS = RuntimeInformation.IsOSPlatform(OSPlatform.OSX),
                IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows),
                Environment.Is64BitOperatingSystem,
                Environment.Is64BitProcess,
                Environment.ProcessorCount,
                Environment.Version,
                Environment.CurrentDirectory,
                Environment.SystemDirectory,
                Environment.MachineName,
            };

            return JsonConvert.SerializeObject(values, Formatting.Indented);
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
