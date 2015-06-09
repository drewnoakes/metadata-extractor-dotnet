using System.IO;
using System.Text;

namespace Sharpen
{
    public class ByteArrayOutputStream : OutputStream
    {
        public ByteArrayOutputStream (int bufferSize)
        {
            Wrapped = new MemoryStream (bufferSize);
        }

        public byte[] ToByteArray ()
        {
            return Extensions.ConvertToByteArray(ToByteArrayInternal());
        }

        private  byte[] ToByteArrayInternal()
        {
            return ((MemoryStream)Wrapped).ToArray ();
        }

        public override void Close ()
        {
            // Closing a ByteArrayOutputStream has no effect.
        }

        public override string ToString ()
        {
            return Encoding.UTF8.GetString(ToByteArrayInternal());
        }

        public string ToString(string encoding)
        {
            //  TODO: check encoding
            return Encoding.GetEncoding(encoding).GetString(ToByteArrayInternal());
        }
    }
}
