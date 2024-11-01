using GroupDocs.Parser.Options;
using GroupDocs.Parser;
using Imatex.Web.Models;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Microsoft.IO;
using System.Collections.Concurrent;

namespace Imatex.Web.Services.Extractor;

public class ImageExtractorService(ILogger<ImageExtractorService> logger, RecyclableMemoryStreamManager memoryStreamManager) : IImageExtractorService
{
    private readonly RecyclableMemoryStreamManager _memoryStreamManager = memoryStreamManager;
    private readonly ConcurrentBag<RecyclableMemoryStream> _processedImages = [];
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
    public async Task<ExtratedImageResult> ExtractImagesFromDocumentAsync(Stream document, string? fileName = "", bool resizeImages = false, int width = 640, int height = 480, bool keepAspectRatio = true, CancellationToken cancellationToken = default)
    {
        try
        {
            await ClearListAsync();

            using var parser = new Parser(document);

            var semaphore = new SemaphoreSlim(5);
            var extractedImages = parser.GetImages();
            var options = new ImageOptions(ImageFormat.Png);

            var imageTasks = extractedImages.Select(async image =>
            {
                try
                {
                    await semaphore.WaitAsync(cancellationToken);
                    await using var imageStream = image.GetImageStream(options);
                    Stream? finalImageStream = imageStream;

                    if (resizeImages)
                    {
                        finalImageStream = await ResizeImageAsync(
                            imageStream, width, height, keepAspectRatio, cancellationToken);
                    }

                    if (finalImageStream is not null)
                    {
                        var memoryStream = _memoryStreamManager.GetStream();
                        await finalImageStream.CopyToAsync(memoryStream, cancellationToken);
                        await finalImageStream.DisposeAsync();

                        if (memoryStream.Position > 0)
                        {
                            memoryStream.Seek(0, SeekOrigin.Begin);
                        }

                        _processedImages.Add(memoryStream);
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(imageTasks);

            return new ExtratedImageResult(_processedImages, fileName?.ToLower());
        }
        catch (Exception ex)
        {
            _logger.LogError("Error extracting the images from the document: {Message}", ex.Message);
            return new ExtratedImageResult("Error extracting the images from the document.");
        }
    }

    private async Task ClearListAsync()
    {
        foreach (var image in _processedImages)
        {
            await image.DisposeAsync();
            _ = _processedImages.TryTake(out _);
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
    public async Task<Stream?> ResizeImageAsync(Stream image, int width = 640, int height = 480, bool keepAspectRatio = true, CancellationToken cancellationToken = default)
    {
        try
        {
            var memoryStream = _memoryStreamManager.GetStream();
            var resizedImage = await ResizeAsync(image, width, height, keepAspectRatio, cancellationToken);

            await ApplyTransparentBackground(resizedImage).SaveAsPngAsync(memoryStream, cancellationToken);

            return memoryStream;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error resizing the image: {Message}", ex.Message);
            return null;
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
    private static async Task<Image> ResizeAsync(Stream originalImage, int width = 640, int height = 480, bool keepAspectRatio = true, CancellationToken cancellationToken = default)
    {
        if (originalImage.Position > 0)
        {
            originalImage.Seek(0, SeekOrigin.Begin);
        }

        var loadedImage = await Image.LoadAsync(originalImage, cancellationToken);
        var resizeOptions = new ResizeOptions
        {
            Mode = keepAspectRatio ? ResizeMode.Max : ResizeMode.Stretch,
            Size = new Size(width, height)
        };

        loadedImage.Mutate(x => x.Resize(resizeOptions));

        return loadedImage;
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
