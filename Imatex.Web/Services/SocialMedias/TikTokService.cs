using Imatex.Web.Extensions;
using Imatex.Web.Models.SocialMedias;
using Imatex.Web.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Imatex.Web.Services.SocialMedias;

public interface ITikTokService
{
    Task<VideoResultBase> DownloadVideoAsync(string? videoUrl);
}

public partial class TikTokService(IHttpClientFactory httpClientFactory, IOptions<TikTokOptions> tikTokOptions, ILogger<TikTokService> logger) : ITikTokService
{
    readonly Random _random = new();
    readonly ILogger<TikTokService> _logger = logger;
    readonly CancellationToken _cancellationToken = new();
    readonly TikTokOptions _tiktokOptions = tikTokOptions.Value;
    readonly HttpClient _client = httpClientFactory.CreateClient();

    static string? ExtractVideoId(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;
        var match = Regex.Match(url, SocialMediaExtensions.TikTokPattern);

        if (!match.Success)
        {
            return string.Empty;
        }

        if (!string.IsNullOrWhiteSpace(match.Groups[1].Value))
        {
            return match.Groups[1].Value;
        }

        if (!string.IsNullOrWhiteSpace(match.Groups[2].Value))
        {
            return match.Groups[2].Value;
        }

        return string.Empty;
    }

    void SetRandomUserAgent()
    {
        string randomAgent = _tiktokOptions.Agents.ElementAt(_random.Next(_tiktokOptions.Agents.Count));
        _client.DefaultRequestHeaders.UserAgent.ParseAdd(randomAgent);
    }

    public async Task<VideoResultBase> DownloadVideoAsync(string? videoUrl)
    {
        var videoResult = new VideoResultBase();
        string? videoId = ExtractVideoId(videoUrl);

        if (string.IsNullOrWhiteSpace(videoId))
        {
            return videoResult.SetError("Unable to get TikTok video id.");
        }

        SetRandomUserAgent();

        string apiUrl = string.Format(_tiktokOptions.TikTokDownloadApi, videoId);

        try
        {
            HttpResponseMessage response = await _client.GetAsync(apiUrl, _cancellationToken);
            string content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to fetch video for Video ID: {VideoId}. Status code: {StatusCode}", videoId, response.StatusCode);
                return videoResult.SetError($"Failed to fetch video for Video ID: {videoId}.");
            }

            videoResult.OriginalVideoUrl = videoUrl!;
            ExtractDownloadUrl(content, videoResult);

            return videoResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while fetching video for Video ID: {VideoId}", videoId);
            return videoResult.SetError("Exception occurred, try again.");
        }
    }

    void ExtractDownloadUrl(string apiResponse, VideoResultBase videoResult)
    {
        try
        {
            using var jsonDocument = JsonDocument.Parse(apiResponse);

            var awemeList = jsonDocument.RootElement.GetProperty("aweme_list")[0];
            var video = awemeList.GetProperty("video");
            var videoPlayAddress = video.GetProperty("play_addr");

            videoResult.Title = nameof(SocialMedia.TikTok);
            videoResult.Description = awemeList.GetProperty("desc").GetString();
            videoResult.AuthorNickname = awemeList.GetProperty("author").GetProperty("nickname").GetString();
            videoResult.AuthorUsername = awemeList.GetProperty("author").GetProperty("unique_id").GetString();
            videoResult.Duration = TimeSpan.FromMilliseconds(video.GetProperty("duration").GetInt64());
            videoResult.ThumbnailUrl = video.GetProperty("cover").GetProperty("url_list")[0].GetString()?.Replace("\\u0026", "&");

            var ratio = video.GetProperty("ratio").GetString();
            var size = videoPlayAddress.GetProperty("data_size").GetInt64();
            var height = videoPlayAddress.GetProperty("height").GetInt32();
            var width = videoPlayAddress.GetProperty("width").GetInt32();
            var resolution = $"{width}x{height}";

            var keywords = videoResult.Description?.GetHashTags();
            videoResult.Keywords = [.. videoResult.Keywords, .. keywords ?? []];

            foreach (var key in videoResult.Keywords)
            {
                videoResult.Description = videoResult.Description?.Replace(key, "");
            }

            int serverCount = 1;
            foreach (var item in videoPlayAddress.GetProperty("url_list").EnumerateArray())
            {
                videoResult.VideoInfos.Add(new VideoInfo()
                {
                    Url = item.GetString()?.Replace("\\u0026", "&"),
                    Size = size,
                    Quality = ratio,
                    ServerCount = serverCount++,
                    Resolution = resolution
                });
            }
        }
        catch (Exception ex)
        {
            videoResult.SetError($"Error downloading video, try again.");
            _logger.LogError(ex, "Error parsing JSON response: {Message}", ex.Message);
        }
    }
}