using Microsoft.IO;

namespace Imatex.Web.Extensions;

public static class MemoryStreamExtensions
{
    public static byte[] ToByteArray(this RecyclableMemoryStream memoryStream)
    {
        return memoryStream.GetBuffer()[..(int)memoryStream.Length];
    }
}
