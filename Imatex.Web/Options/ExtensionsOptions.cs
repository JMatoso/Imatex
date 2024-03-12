namespace Imatex.Web.Options;

public class ExtensionsOptions
{
    public const string ExtensionOptionsKey = "ExtensionsOptions";

    public int MaxFileCount { get; set; } = 5;
    public long MaxFileSize { get; set; } = 5 * 1024 * 1024;
    public HashSet<string> AllowedImageExtensions { get; set; } = [];
    public HashSet<string> AllowedDocumentExtensions { get; set; } = [];

    public bool IsAllowedImageExtension(string extension) 
        => AllowedImageExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);

    public bool IsAllowedDocumentExtension(string extension) 
        => AllowedDocumentExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);

    public string GetAllowedImageExtensionsAsString(string separator = ",") 
        => string.Join(separator, AllowedImageExtensions);

    public string GetAllowedDocumentExtensionsAsString(string separator = ",") 
        => string.Join(separator, AllowedDocumentExtensions);
}
