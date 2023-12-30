using Imatex.Models;
using System.Drawing;

namespace Imatex.Services.OCR
{
    public interface IOcrService
    {
        void Dispose();

        /// <summary>
        /// Extract the text from the images.
        /// </summary>
        /// <param name="images">Bitmap image files.</param>
        /// <returns>A list of <see cref="ExtractedText"/></returns>
        IEnumerable<ExtractedText> GetTextFromImages(params Bitmap[] image);
    }
}