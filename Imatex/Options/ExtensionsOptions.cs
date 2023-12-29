namespace Imatex.Options;

public class ExtensionsOptions
{
    public int MaxFileCount { get; set; } = 5;
    public long MaxFileSize { get; set; } = 5 * 1024 * 1024;
    public HashSet<string> AllowedExtensions { get; set; } = new HashSet<string>();

    public bool IsAllowed(string extension) => AllowedExtensions.Contains(extension);
    public string GetAllowedExtensions() => string.Join(", ", AllowedExtensions);
}