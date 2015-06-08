using System.IO;

namespace Sharpen
{
    public class BufferedInputStream : InputStream
    {
        public BufferedInputStream (InputStream s)
        {
            BaseStream = s.GetWrappedStream ();
            Wrapped = new BufferedStream (BaseStream);
        }
    }
}
