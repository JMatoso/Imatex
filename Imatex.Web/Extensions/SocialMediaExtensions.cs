using Imatex.Web.Models.SocialMedias;
using System.Text.RegularExpressions;

namespace Imatex.Web.Extensions;

public static class SocialMediaExtensions
{
    public static readonly string TikTokPattern = @"https:\/\/(?:www\.tiktok\.com\/@[^\/]+\/video\/(\d+)|vm\.tiktok\.com\/([^\/?]+))";

    static readonly Dictionary<SocialMedia, Regex> _regexes = [];

    static readonly Dictionary<SocialMedia, string> _patterns = new()
    {
        { SocialMedia.Facebook, @"https?:\/\/(www\.)?facebook\.com\/[a-zA-Z0-9._-]+\/?" },
        { SocialMedia.YouTube, @"https?:\/\/(?:www\.)?youtube\.com\/(?:watch\?v=|shorts\/)[a-zA-Z0-9_-]+(?:\?[\w=&-]*)?|https?:\/\/youtu\.be\/[a-zA-Z0-9_-]+(?:\?[\w=&-]*)?" },
        { SocialMedia.Instagram, @"https?:\/\/(www\.)?instagram\.com\/[a-zA-Z0-9._]+\/?" },
        { SocialMedia.TikTok, @"https?:\/\/(?:www\.)?tiktok\.com\/@?[a-zA-Z0-9._-]+\/video\/[0-9]+|https?:\/\/vm\.tiktok\.com\/[\w\-]+" }
    };

    static readonly Random _random = new();

    static readonly HashSet<string> Examples =
    [
        "https://www.facebook.com/username/videos/fsfgf4a4fd5",
        "https://www.youtube.com/watch?v=5f6d5f4s",
        "https://youtu.be/fw8fes54s",
        "https://www.instagram.com/p/f7d64sd8g",
        "https://www.tiktok.com/@username/w4r65w22",
        "https://vm.tiktok.com/gdBffrfe4e5"
    ];

    public static string GetRandomExample() => Examples.ElementAt(_random.Next(Examples.Count));

    public static SocialMedia GetSocialMediaLink(this Uri? url)
    {
        if (url == null || !url.IsWellFormedOriginalString())
            return SocialMedia.Unknown;

        var input = url.ToString();

        foreach (var kvp in _patterns)
        {
            if (!_regexes.ContainsKey(kvp.Key))
                _regexes[kvp.Key] = new Regex(kvp.Value, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            if (_regexes[kvp.Key].IsMatch(input))
                return kvp.Key;
        }

        return SocialMedia.Unknown;
    }
}
