using System.IO;

namespace Sharpen
{
    public class BufferedOutputStream : OutputStream
    {
        public BufferedOutputStream (OutputStream outs)
        {
            base.Wrapped = new BufferedStream (outs == null ? new MemoryStream () : outs.GetWrappedStream ());
        }

        public BufferedOutputStream (OutputStream outs, int bufferSize)
        {
            base.Wrapped = new BufferedStream (outs.GetWrappedStream (), bufferSize);
        }
    }
}
