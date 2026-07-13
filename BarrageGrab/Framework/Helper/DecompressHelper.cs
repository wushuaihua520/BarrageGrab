using System.IO.Compression;

namespace BarrageGrab.Framework.Helper
{
    public static class DecompressHelper
    {
        public static byte[] Decompress(byte[] zippedData)
        {
            using var input = new MemoryStream(zippedData);
            using var gzip = new GZipStream(input, CompressionMode.Decompress);
            using var output = new MemoryStream();
            gzip.CopyTo(output);
            return output.ToArray();
        }
    }
}
