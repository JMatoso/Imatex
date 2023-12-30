namespace Imatex.Options;

public class ExtensionsOptions
{
    public int MaxFileCount { get; set; } = 5;
    public long MaxFileSize { get; set; } = 5 * 1024 * 1024;
    public HashSet<string> AllowedImageExtensions { get; set; } = new HashSet<string>();
    public HashSet<string> AllowedDocumentExtensions { get; set; } = new HashSet<string>();

    public bool IsAllowedImageExtension(string extension) => AllowedImageExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
    public bool IsAllowedDocumentExtension(string extension) => AllowedDocumentExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);

    public string GetAllowedImageExtensions() => string.Join(", ", AllowedImageExtensions);
    public string GetAllowedDocumentExtensions() => string.Join(", ", AllowedDocumentExtensions);
}