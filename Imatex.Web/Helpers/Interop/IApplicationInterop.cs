
namespace Imatex.Web.Helpers.Interop
{
    public interface IApplicationInterop
    {
        Task CopyToClipboardAsync(string text);
        string GetRandomColor(int count = 0);
        Task<string> PasteFromClipboardAsync();
    }
}