using Imatex.Web.Models;
using System.Drawing;

namespace Imatex.Web.Services.Ocr
{
    public interface ITextConverterService
    {
        void Dispose();

        /// <summary>
        /// Extract the text from the images.
        /// </summary>
        /// <param name="images">Bitmap image files.</param>
        /// <returns>A list of <see cref="ExtractedTextResult"/></returns>
        IEnumerable<ExtractedTextResult> GetTextFromImages(params Bitmap[] images);
    }
}