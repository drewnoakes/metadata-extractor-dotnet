using ICSharpCode.SharpZipLib.Zip.Compression;

namespace Sharpen
{
    public class InflaterInputStream : InputStream
    {
        public InflaterInputStream(InputStream s)
        {
            Wrapped = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream(s.GetWrappedStream(), new Inflater());
        }
    }
}