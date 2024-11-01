using InteropDotNet;
using System.Runtime.InteropServices;

namespace Imatex.Web.Services.Ocr;

public static class TesseractLinuxLoaderFix
{
    /// <summary>
    /// Override .so search path used on Linux by Tesseract
    /// </summary>
    public static void Patch()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            LibraryLoader.Instance.CustomSearchPath = $"{AppDomain.CurrentDomain.BaseDirectory}/runtimes";
        }
    }
}
