using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace Imatex.Web.Helpers.Interop;

public class ApplicationInterop(IJSRuntime jsRuntime, ISnackbar snackBar) : ComponentBase, IApplicationInterop
{
    private readonly ISnackbar _snackBar = snackBar;
    private readonly IJSRuntime _jsRuntime = jsRuntime;

    private readonly HashSet<string> _colors = 
    [
        Colors.Red.Default,
        Colors.Blue.Default,
        Colors.Green.Default,
        Colors.Pink.Default,
        Colors.Orange.Default,
        Colors.Indigo.Default,
        Colors.LightBlue.Default,
        Colors.Teal.Default,
        Colors.Purple.Default,
        Colors.DeepPurple.Default,
        Colors.DeepOrange.Default,
        Colors.Brown.Default,
        Colors.BlueGrey.Default,
        Colors.Amber.Default,
    ];

    public async Task CopyToClipboardAsync(string text)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
            _snackBar.Add("Text copied to clipboard", Severity.Success);
        }
        catch (Exception)
        {
            _snackBar.Add("Unable to copy from rhe clipboard. Try allow it to copy.", Severity.Warning);
        }
    }

    public async Task<string> PasteFromClipboardAsync()
    {
        try
        {
            var paste = await _jsRuntime.InvokeAsync<string>("navigator.clipboard.readText");
            _snackBar.Add($"Text pasted from clipboard", Severity.Success);
            return paste;
        }
        catch (Exception)
        {
            _snackBar.Add("Unable to paste from the clipboard. Try allow it to paste.", Severity.Warning);
            return string.Empty;
        }
    }

    public string GetRandomColor(int count = 0)
    {
        bool outOfIndex = count > _colors.Count || count < 0;
        return outOfIndex ? _colors.Last() : _colors.ElementAt(count);
    }
}
