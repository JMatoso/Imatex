using GroupDocs.Parser.Options;
using GroupDocs.Parser;
using Imatex.Web.Models;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Imatex.Web.Services.Extractor;

public class ImageExtractorService(ILogger<ImageExtractorService> logger) : IImageExtractorService
{
    private readonly ILogger<ImageExtractorService> _logger = logger;

    /// <summary>
    /// Extracts images from a document.
    /// </summary>
    /// <param name="document">The stream of the document from which the images will be extracted.</param>
    /// <param name="fileName">The name of the file from which the images will be extracted. The default value is an empty string.</param>
    /// <param name="resizeImages">A boolean indicating whether the extracted images should be resized. The default value is false.</param>
    /// <param name="width">The desired width of the resized images. The default value is 640.</param>
    /// <param name="height">The desired height of the resized images. The default value is 480.</param>
    /// <param name="keepAspectRatio">A boolean indicating whether the aspect ratio of the images should be maintained when resizing. The default value is true.</param>
    /// <returns><see cref="ExtratedImageResult"/></returns>
    public ExtratedImageResult ExtractImagesFromDocument(Stream document, string? fileName = "", bool resizeImages = false, int width = 640, int height = 480, bool keepAspectRatio = true)
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

                    if (success && resizedImages is not null)
                    {
                        proccessedImages.Add(resizedImages);
                        continue;
                    }
                }

                proccessedImages.Add(streamImage);
            }

            return new ExtratedImageResult(proccessedImages, fileName?.ToLower());
        }
        catch (Exception ex)
        {
            _logger.LogError("Error extracting the images from the document: {Message}", ex.Message);
            return new ExtratedImageResult("Error extracting the images from the document.");
        }
    }

    /// <summary>
    /// Resizes an image, with the option to maintain its original proportions, and applies a transparent background.
    /// </summary>
    /// <param name="image">The stream of the image to be resized.</param>
    /// <param name="width">The desired width of the resized image. The default value is 640.</param>
    /// <param name="height">The desired height of the resized image. The default value is 480.</param>
    /// <param name="keepAspectRatio">A boolean indicating whether the image's proportion should be maintained when resizing. The default value is true.</param>
    /// <returns>A tuple containing the <see cref="Stream"/> of the resized image (or null if the operation fails) and a boolean indicating whether the operation was successful.</returns>
    public (Stream? resizedImage, bool success) ResizeImage(Stream image, int width = 640, int height = 480, bool keepAspectRatio = true)
    {
        try
        {
            var resizedImage = Resize(image, width, height, keepAspectRatio);

            var ms = new MemoryStream();

            ApplyTransparentBackground(resizedImage).SaveAsPng(ms);

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
    /// <returns>The resized image of <see cref="Image"/> type.</returns>
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
    /// <returns>The processed of <see cref="Image"/> type with a transparent background and centered.</returns>
    private static Image<Rgba32> ApplyTransparentBackground(Image image, int width = 640, int height = 480)
    {
        var outputImage = new Image<Rgba32>(Configuration.Default, width, height, Color.Transparent);

        var position = new Point((width - image.Width) / 2, (height - image.Height) / 2);

        outputImage.Mutate(o => o.DrawImage(image, position, 1));

        return outputImage;
    }
}
