using System.IO;

namespace Sharpen
{
    public class ByteArrayInputStream : InputStream
    {
        public ByteArrayInputStream (byte[] data)
        {
            Wrapped = new MemoryStream(Extensions.ConvertToByteArray(data));
        }

        public ByteArrayInputStream (byte[] data, int off, int len)
        {
            Wrapped = new MemoryStream (Extensions.ConvertToByteArray(data), off, len);
        }

        public override int Available ()
        {
            MemoryStream ms = (MemoryStream) Wrapped;
            return (int)(ms.Length - ms.Position);
        }
    }
}
