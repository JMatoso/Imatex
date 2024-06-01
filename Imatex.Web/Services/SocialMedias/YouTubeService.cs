using YoutubeExplode;
using Imatex.Web.Models.SocialMedias;

namespace Imatex.Web.Services.SocialMedias;

public interface IYouTubeService
{
    Task<VideoResultBase> DownloadVideoAsync(string url);
}

public class YouTubeService(IHttpClientFactory httpClientFactory, ILogger<YouTubeService> logger) : IYouTubeService
{
    readonly ILogger<YouTubeService> _logger = logger;
    readonly CancellationToken _cancellationToken = new();
    readonly YoutubeClient _youtubeClient = new(httpClientFactory.CreateClient());

    public async Task<VideoResultBase> DownloadVideoAsync(string url)
    {
        var videoResult = new VideoResultBase();

        try
        {
            var video = await _youtubeClient.Videos.GetAsync(url, _cancellationToken);

            if (video is null)
            {
                return videoResult.SetError("Video not found.");
            }

            var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(video.Id, _cancellationToken);
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
            videoResult.Description = video.Description;
            videoResult.Keywords = [.. video.Keywords];
            videoResult.Duration = video.Duration;

            var thumbnail = video.Thumbnails.OrderByDescending(x => x.Resolution.Area).FirstOrDefault();
            videoResult.ThumbnailUrl = thumbnail?.Url;

            videoResult.OriginalVideoUrl = video.Url;
            videoResult.AuthorUsername = video.Author.ChannelTitle;
            videoResult.AuthorNickname = video.Author.ToString();

            return videoResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading video from YouTube.");
            return videoResult.SetError("Error downloading video from YouTube.");
        }
    }
}
