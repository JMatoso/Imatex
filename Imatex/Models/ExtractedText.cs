namespace Imatex.Models;

public class ExtractedText
{
    public float Confidence { get; set; }
    public string Text { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public DateTimeOffset DateIssued { get; set; }
}
