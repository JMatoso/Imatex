
namespace Imatex.Web.Helpers.Interop
{
    public interface IApplicationInterop
    {
        Task CopyToClipboardAsync(string text);
    }
}