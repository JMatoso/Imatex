using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Imatex.Web.Extensions;

public static class StringExtensions
{
    [return: NotNullIfNotNull(nameof(value))]
    public static string? ToFriendlyUrl(this string value)
    {
        if (string.IsNullOrEmpty(value)) return value;

        value = value.RemoveSpecialChars();

        value = Regex.Replace(value, "[^a-z0-9\\s-]", "");
        value = Regex.Replace(value, "[\\s-]+", " ");
        value = Regex.Replace(value, "\\s", "-");

        return value.ToLower();
    }

    private static string RemoveSpecialChars(this string value)
    {
        value = value.ToLower();
        value = Regex.Replace(value, "[àåáâäãåąā]", "a");
        value = Regex.Replace(value, "[çćčĉ]", "c");
        value = Regex.Replace(value, "[đ]", "d");
        value = Regex.Replace(value, "[èéêëę]", "e");
        value = Regex.Replace(value, "[ğĝ]", "g");
        value = Regex.Replace(value, "[ĥ]", "h");
        value = Regex.Replace(value, "[ìíîïı]", "i");
        value = Regex.Replace(value, "[ĵ]", "j");
        value = Regex.Replace(value, "[ł]", "l");
        value = Regex.Replace(value, "[ñń]", "n");
        value = Regex.Replace(value, "[òóôõöøőð]", "o");
        value = Regex.Replace(value, "[ř]", "r");
        value = Regex.Replace(value, "[śşšŝ]", "s");
        value = Regex.Replace(value, "[ùúûüŭů]", "u");
        value = Regex.Replace(value, "[ýÿ]", "y");
        value = Regex.Replace(value, "[żźž]", "z");
        value = Regex.Replace(value, "[ß]", "ss");
        value = Regex.Replace(value, "[þ]", "th");

        return value.Trim();
    }
}
