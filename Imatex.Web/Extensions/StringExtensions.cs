using Imatex.Bot.Extensions;
using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Imatex.Web.Extensions;

public static partial class StringExtensions
{
    [return: NotNullIfNotNull(nameof(value))]
    public static string? ToFriendlyUrl(this string value)
    {
        if (string.IsNullOrEmpty(value)) return value;

        value = value.Normalize(NormalizationForm.FormD);

        value = RegexPatterns.NumbersRegex().Replace(value, "");
        value = RegexPatterns.HyphenRegex().Replace(value, "-");

        return value.Trim('-').ToLower();
    }

    public static bool TryGetValidLink(this string? text, out Uri? uri)
    {
        uri = null;

        if (string.IsNullOrEmpty(text)) return false;
        if (!RegexPatterns.HttpRegex().IsMatch(text)) return false;

        try
        {
            uri = new Uri(text, UriKind.Absolute);
            return true;
        }
        catch (UriFormatException)
        {
            return false;
        }
    }

    public static string GenerateFileNameWithExtension(this string? fileName, string extension, string? sufix = "media")
    {
        fileName ??= "file";
        return $"{fileName}-{sufix}-{DateTime.Now.Ticks}".ToFriendlyUrl() + extension;
    }

    public static MarkupString ToMarkupHtml(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return new MarkupString(string.Empty);

        var builder = new StringBuilder(text);

        builder.Replace("<", "&lt;");
        builder.Replace(">", "&gt;");
        builder.Replace("\n", "<br>");

        return new MarkupString(builder.ToString());
    }

    public static string[]? GetHashTags(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return [];
        return text.Split(" ").Where(x => x.StartsWith('#')).Select(x => x).ToArray();
    }

    public static string AsString(this IEnumerable<string>? text)
    {
        return string.Join(" ", text ?? []);
    }
}
