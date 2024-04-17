namespace Imatex.Web.Options;

public class TikTokOptions
{
    public const string TikTokOptionsKey = "TikTokOptions";

    public string TikTokDownloadApi { get; set; } = string.Empty;

    public HashSet<string> Agents { get; set; } = [];

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(TikTokDownloadApi))
        {
            throw new ArgumentNullException(nameof(TikTokDownloadApi));
        }

        if (Agents.Count == 0)
        {
            throw new ArgumentNullException(nameof(Agents));
        }
    }
}
