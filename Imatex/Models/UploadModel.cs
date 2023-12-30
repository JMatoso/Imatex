using System.ComponentModel.DataAnnotations;

namespace Imatex.Models;

public class UploadModel
{
    [Range(1, int.MaxValue)]
    public int Width { get; set; } = 640;

    [Range(1, int.MaxValue)]
    public int Height { get; set; } = 480;
    public bool KeepAspectRatio { get; set; } = true;
    public bool ResizeImages { get; set; } = false;
}
