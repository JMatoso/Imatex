using Imatex.Models;
using System.Drawing;

namespace Imatex.Services.OCR
{
    public interface IOcrService
    {
        void Dispose();
        IEnumerable<ExtractedText> GetTextFromImages(params Bitmap[] image);
    }
}