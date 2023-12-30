using Imatex.Models;
using System.Drawing;
using Tesseract;

namespace Imatex.Services.OCR;

public class OcrService : IDisposable, IOcrService
{
    private readonly TesseractEngine _tesseractEngine;

    public OcrService()
    {
        _tesseractEngine = new TesseractEngine(@"./tessdata", "eng+por+fra+spa+ita", EngineMode.Default);
    }

    /// <summary>
    /// Extract the text from the images.
    /// </summary>
    /// <param name="images">Bitmap image files.</param>
    /// <returns>A list of <see cref="ExtractedText"/></returns>
    public IEnumerable<ExtractedText> GetTextFromImages(params Bitmap[] images)
    {
        foreach (var img in images)
        {
            using var page = _tesseractEngine.Process(img);

            string text = page.GetText();  
            
            if(string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
            {
                continue;
            }
            
            yield return new ExtractedText
            {
                Text = text,
                FileName = page.ImageName,
                DateIssued = DateTimeOffset.UtcNow,
                Confidence = page.GetMeanConfidence()
            };
        }
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
