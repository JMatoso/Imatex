using Imatex.Web.Extensions;
using Imatex.Web.Models;
using Microsoft.IO;
using System.IO.Compression;

namespace Imatex.Web.Services.Compression;

public class ZipCompressorService(ILogger<ZipCompressorService> logger, RecyclableMemoryStreamManager memoryStreamManager) : IZipCompressorService
{
    private readonly ILogger<ZipCompressorService> _logger = logger;
    private readonly RecyclableMemoryStreamManager _memoryStreamManager = memoryStreamManager;

    public async Task<byte[]> CreateZipFileInMemoryAsync(List<ExtractedImage> extractedImages, CancellationToken cancellationToken = default)
    {
        if (extractedImages is null || extractedImages.Count == 0)
        {
            return [];
        }

        await using var memoryStream = _memoryStreamManager.GetStream();
        var semaphore = new SemaphoreSlim(5);

        try
        {
            using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true);

            var tasks = extractedImages.Select(async image =>
            {
                await semaphore.WaitAsync(cancellationToken);

                try
                {
                    if (image.Image.Position > 0)
                    {
                        image.Image.Seek(0, SeekOrigin.Begin);
                    }

                    await AddImageToZipAsync(archive, image.FileName, image.Image);
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error creating the zip file: {Message}", ex.Message);
            return [];
        }

        return memoryStream.ToByteArray();
    }

    private async Task AddImageToZipAsync(ZipArchive archive, string fileName, Stream imageStream, CancellationToken cancellationToken = default)
    {
        try
        {
            var entry = archive.CreateEntry(fileName);
            await using var entryStream = entry.Open();
            await imageStream.CopyToAsync(entryStream, bufferSize: 81920, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Adding image to zip was canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Error adding the image to the zip file: {Message}", ex.Message);
        }
    }
}
