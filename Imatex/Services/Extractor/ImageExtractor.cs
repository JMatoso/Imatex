using GroupDocs.Parser.Options;
using GroupDocs.Parser;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Imatex.Services.Extractor;

public class ImageExtractor : IImageExtractor
{
    private readonly ILogger<ImageExtractor> _logger;

    public ImageExtractor(ILogger<ImageExtractor> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Extracts images from a document, with the option to resize them.
    /// </summary>
    /// <param name="document">The stream of the document from which the images will be extracted.</param>
    /// <param name="resizeImages">A boolean indicating whether the extracted images should be resized. The default value is false.</param>
    /// <param name="width">The desired width of the resized images. The default value is 640.</param>
    /// <param name="height">The desired height of the resized images. The default value is 480.</param>
    /// <param name="keepAspectRatio">A boolean indicating whether the aspect ratio of the images should be maintained when resizing. The default value is true.</param>
    /// <returns>A tuple containing a list of streams of the extracted images (or empty if the extraction fails) and a boolean indicating whether the operation was successful.</returns>

    public (IEnumerable<Stream>? images, bool success) ExtractImagesFromDocument(Stream document, bool resizeImages = false, int width = 640, int height = 480, bool keepAspectRatio = true)
    {
        try
        {
            using var parser = new Parser(document);

            var extractedImages = parser.GetImages();
            var proccessedImages = new List<Stream>();
            var options = new ImageOptions(ImageFormat.Png);

            foreach (var image in extractedImages)
            {
                var streamImage = image.GetImageStream(options);

                if (resizeImages)
                {
                    var (resizedImages, success) = ResizeImage(streamImage, width, height, keepAspectRatio);

                    if (success && resizedImages != null)
                    {
                        proccessedImages.Add(resizedImages);
                        continue;
                    }
                }

                proccessedImages.Add(streamImage);
            }

            return (proccessedImages, true);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error extracting the images from the document: {Message}", ex.Message);
            return (Enumerable.Empty<Stream>(), false);
        }
    }

    /// <summary>
    /// Resizes an image, with the option to maintain its original proportions, and applies a transparent background.
    /// </summary>
    /// <param name="image">The stream of the image to be resized.</param>
    /// <param name="width">The desired width of the resized image. The default value is 640.</param>
    /// <param name="height">The desired height of the resized image. The default value is 480.</param>
    /// <param name="keepAspectRatio">A boolean indicating whether the image's proportion should be maintained when resizing. The default value is true.</param>
    /// <returns>A tuple containing the stream of the resized image (or null if the operation fails) and a boolean indicating whether the operation was successful.</returns>
    public (Stream? resizedImage, bool success) ResizeImage(Stream image, int width = 640, int height = 480, bool keepAspectRatio = true)
    {
        try
        {
            var resizedImage = Resize(image, width, height, keepAspectRatio);

            var ms = new MemoryStream();

            ApplyTransparentBackground(resizedImage).SaveAsWebp(ms);

            return (ms, true);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error resizing the image: {Message}", ex.Message);
            return (null, false);
        }
    }

    /// <summary>
    /// Resizes an image, with the option to maintain its original proportions.
    /// </summary>
    /// <param name="originalImage">The stream of the image to be resized.</param>
    /// <param name="width">The desired width of the resized image. The default value is 640.</param>
    /// <param name="height">The desired height of the resized image. The default value is 480.</param>
    /// <param name="keepAspectRatio">A boolean indicating whether the image's proportion should be maintained when resizing. The default value is true.</param>
    /// <returns>The resized image.</returns>
    private static Image Resize(Stream originalImage, int width = 640, int height = 480, bool keepAspectRatio = true)
    {
        originalImage.Seek(0, SeekOrigin.Begin);

        var image = Image.Load(originalImage);

        var resizeOptions = new ResizeOptions
        {
            Mode = keepAspectRatio ? ResizeMode.Max : ResizeMode.Stretch,
            Size = new Size(width, height)
        };

        image.Mutate(x => x.Resize(resizeOptions));

        return image;
    }

    /// <summary>
    /// Applies a transparent background to an image and centers it.
    /// </summary>
    /// <param name="image">The image to be processed.</param>
    /// <param name="width">The desired width of the final image. The default value is 640.</param>
    /// <param name="height">The desired height of the final image. The default value is 480.</param>
    /// <returns>The processed image with a transparent background and centered.</returns>
    private static Image ApplyTransparentBackground(Image image, int width = 640, int height = 480)
    {
        var outputImage = new Image<Rgba32>(Configuration.Default, width, height, Color.Transparent);

        var position = new Point((width - image.Width) / 2, (height - image.Height) / 2);

        outputImage.Mutate(o => o.DrawImage(image, position, 1));

        return outputImage;
    }
}
