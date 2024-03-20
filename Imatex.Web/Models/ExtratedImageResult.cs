namespace Imatex.Web.Models;

public class ExtratedImageResult
{
    public string? FileName { get; private set; } 
    public IEnumerable<Stream> Images { get; private set; } = [];

    public bool Success { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;

    /// <summary>
    /// Constructor for the <see cref="ExtratedImageResult"/> class. Sets Success to false and ErrorMessage to "No images were extracted from the document." if the images are null and sets Success to true and Images to the images if the images are not null.
    /// </summary>
    /// <param name="images">A list of images. <see cref="IEnumerable{Stream}"/></param>
    public ExtratedImageResult(IEnumerable<Stream>? images, string? fileName = "")
    {
        if (images is null)
        {
            ErrorMessage = "No images were extracted from the document.";
        }

        Success = true;
        FileName = fileName;
        Images = images ?? [];
    }

    /// <summary>
    /// Constructor for the <see cref="ExtratedImageResult"/> class. Sets Success to false and ErrorMessage to the error message.
    /// </summary>
    /// <param name="errorMessage">Error message.</param>
    public ExtratedImageResult(string errorMessage)
    {
        Success = false;
        ErrorMessage = errorMessage;
    }
}

public class ExtractedImage
{
    public Stream Image { get; set; } = default!;
    public string FileName { get; set; } = string.Empty;
    public string ImageBase64 { get; set; } = string.Empty;
    public string? BaseFileName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
}
