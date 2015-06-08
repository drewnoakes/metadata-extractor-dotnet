using System.IO;

namespace Sharpen
{
    public class BufferedInputStream : InputStream
    {
        public BufferedInputStream (InputStream s)
        {
            BaseStream = s.GetWrappedStream ();
            base.Wrapped = new BufferedStream (BaseStream);
        }
    }
}
