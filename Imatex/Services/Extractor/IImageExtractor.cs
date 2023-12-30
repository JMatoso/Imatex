
namespace Imatex.Services.Extractor
{
    public interface IImageExtractor
    {
        /// <summary>
        /// Extracts images from a document, with the option to resize them.
        /// </summary>
        /// <param name="document">The stream of the document from which the images will be extracted.</param>
        /// <param name="resizeImages">A boolean indicating whether the extracted images should be resized. The default value is false.</param>
        /// <param name="width">The desired width of the resized images. The default value is 640.</param>
        /// <param name="height">The desired height of the resized images. The default value is 480.</param>
        /// <param name="keepAspectRatio">A boolean indicating whether the aspect ratio of the images should be maintained when resizing. The default value is true.</param>
        /// <returns>A tuple containing a list of streams of the extracted images (or empty if the extraction fails) and a boolean indicating whether the operation was successful.</returns>
        (IEnumerable<Stream>? images, bool success) ExtractImagesFromDocument(Stream document, bool resizeImages = false, int width = 640, int height = 480, bool keepAspectRatio = true);

        /// <summary>
        /// Resizes an image, with the option to maintain its original proportions, and applies a transparent background.
        /// </summary>
        /// <param name="image">The stream of the image to be resized.</param>
        /// <param name="width">The desired width of the resized image. The default value is 640.</param>
        /// <param name="height">The desired height of the resized image. The default value is 480.</param>
        /// <param name="keepAspectRatio">A boolean indicating whether the image's proportion should be maintained when resizing. The default value is true.</param>
        /// <returns>A tuple containing the stream of the resized image (or null if the operation fails) and a boolean indicating whether the operation was successful.</returns>
        (Stream? resizedImage, bool success) ResizeImage(Stream image, int width = 640, int height = 480, bool keepAspectRatio = true);
    }
}