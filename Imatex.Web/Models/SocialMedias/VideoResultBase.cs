using Humanizer;
using Imatex.Web.Extensions;

namespace Imatex.Web.Models.SocialMedias;

public class VideoResultBase
{
    private string? title;
    private string? description;

    public string? Title
    {
        get => title;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                title = value;
                return;
            }

            title = string.Join("_", value.Split(Path.GetInvalidFileNameChars())).Trim().Titleize();
        }
    }

    public string? Description { get => description; set => description = value?.Trim(); }
    public string? AuthorNickname { get; set; }
    public string? AuthorUsername { get; set; }
    public HashSet<string> Keywords { get; set; } = [];
    public TimeSpan? Duration { get; set; }
    public string? ThumbnailUrl { get; set; }

    public string OriginalVideoUrl { get; set; } = string.Empty;

    public List<VideoInfo> VideoInfos { get; set; } = [];

    public string? FileName => Title?.GenerateFileNameWithExtension(".mp4");

    public bool Success => string.IsNullOrEmpty(ErrorMessage);
    public string? ErrorMessage { get; private set; } 

    public VideoResultBase SetError(string errorMessage)
    {
        ErrorMessage = errorMessage;
        return this;
    }
}

public class VideoInfo
{
    public long Size { get; set; }
    public string? Url { get; set; }
    public string? Quality { get; set; }
    public int? ServerCount { get; set; } = null;
    public bool IsAudio { get; set; }
    public string? Resolution { get; set; }
    public string Extension { get; set; } = ".mp4";
    public bool HasUrl => !string.IsNullOrWhiteSpace(Url);
}