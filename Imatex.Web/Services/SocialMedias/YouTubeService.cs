using YoutubeExplode;
using Imatex.Web.Models.SocialMedias;

namespace Imatex.Web.Services.SocialMedias;

public interface IYouTubeService
{
    Task<VideoResultBase> DownloadVideoAsync(string url, CancellationToken cancellationToken = default);
}

public class YouTubeService(IHttpClientFactory httpClientFactory, ILogger<YouTubeService> logger) : IYouTubeService
{
    private readonly ILogger<YouTubeService> _logger = logger;

    public async Task<VideoResultBase> DownloadVideoAsync(string url, CancellationToken cancellationToken = default)
    {
        var videoResult = new VideoResultBase();

        try
        {
            var youtubeClient = new YoutubeClient(httpClientFactory.CreateClient(nameof(Program)));
            var video = await youtubeClient.Videos.GetAsync(url, cancellationToken);
            if (video is null)
            {
                return videoResult.SetError("Video not found.");
            }

            var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
            var muxedStreams = streamManifest.GetMuxedStreams().OrderByDescending(s => s.VideoQuality).ToList();
            var audioStreams = streamManifest.GetAudioOnlyStreams().OrderByDescending(s => s.Bitrate).ToList();

            foreach (var item in muxedStreams)
            {
                videoResult.VideoInfos.Add(new VideoInfo()
                {
                    Url = item.Url,
                    Size = item.Size.Bytes,
                    Quality = item.VideoQuality.Label,
                    Resolution = item.VideoResolution.ToString()
                });
            }

            foreach (var item in audioStreams)
            {
                videoResult.VideoInfos.Add(new VideoInfo()
                {
                    Url = item.Url,
                    Extension = item.Container.Name,
                    Resolution = $"{item.Bitrate.BitsPerSecond}bps",
                    Size = item.Size.Bytes,
                    Quality = item.AudioCodec,
                    IsAudio = true
                });
            }

            videoResult.Title = video.Title;
            videoResult.Duration = video.Duration;
            videoResult.Keywords = [.. video.Keywords];
            videoResult.Description = video.Description;

            var thumbnail = video.Thumbnails.OrderByDescending(x => x.Resolution.Area).FirstOrDefault();
            videoResult.ThumbnailUrl = thumbnail?.Url;

            videoResult.OriginalVideoUrl = video.Url;
            videoResult.AuthorNickname = video.Author.ToString();
            videoResult.AuthorUsername = video.Author.ChannelTitle;

            return videoResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading video from YouTube: {Message}", ex.Message);
            return videoResult.SetError("Error downloading video from YouTube.");
        }
    }
}