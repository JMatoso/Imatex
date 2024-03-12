using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace Imatex.Web.Helpers.Interop;

public class ApplicationInterop(IJSRuntime jsRuntime, ISnackbar snackBar) : ComponentBase, IApplicationInterop
{
    private readonly ISnackbar _snackBar = snackBar;
    private readonly IJSRuntime _jsRuntime = jsRuntime;

    public async Task CopyToClipboardAsync(string text)
    {
        await _jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
        _snackBar.Add("Text copied to clipboard", Severity.Success);
    }
}
