using System.Text.RegularExpressions;

namespace Imatex.Bot.Extensions;

public static partial class RegexPatterns
{
    [GeneratedRegex("\\s+")]
    public static partial Regex HyphenRegex();

    [GeneratedRegex("[^a-z0-9\\s-]", RegexOptions.IgnoreCase)]
    public static partial Regex NumbersRegex();

    [GeneratedRegex(@"^(http|https)://", RegexOptions.IgnoreCase)]
    public static partial Regex HttpRegex();
}
