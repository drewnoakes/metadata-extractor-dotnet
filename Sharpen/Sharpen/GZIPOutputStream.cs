using System.IO.Compression;

namespace Sharpen
{
    public class GZIPOutputStream : OutputStream
    {
        public GZIPOutputStream (OutputStream os)
        {
            Wrapped = new GZipStream (os, CompressionMode.Compress);
        }
    }
}
