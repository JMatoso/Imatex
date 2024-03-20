using Imatex.Web.Models;
using System.IO.Compression;

namespace Imatex.Web.Services.Compression;

public class ZipCompressorService(ILogger<ZipCompressorService> logger) : IZipCompressorService
{
    private readonly ILogger<ZipCompressorService> _logger = logger;

    public byte[] CreateZipFileInMemory(IEnumerable<ExtractedImage> extractedImages)
    {
        if (extractedImages is null || !extractedImages.Any()) return [];

        using MemoryStream memoryStream = new();

        try
        {
            using ZipArchive archive = new(memoryStream, ZipArchiveMode.Create, true);
            foreach (var image in extractedImages)
            {
                image.Image.Seek(0, SeekOrigin.Begin);
                AddImageToZip(archive, image.FileName, image.Image);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error creating the zip file: {Message}", ex.Message);
            return [];
        }

        return memoryStream.ToArray();
    }

    private void AddImageToZip(ZipArchive archive, string fileName, Stream imageStream)
    {
        try
        {
            ZipArchiveEntry entry = archive.CreateEntry(fileName);
            using Stream entryStream = entry.Open();
            imageStream.CopyTo(entryStream);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error adding the image to the zip file: {Message}", ex.Message);
        }
    }
}
