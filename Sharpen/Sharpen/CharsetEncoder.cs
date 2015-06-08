using System.Text;

namespace Sharpen
{
    public class CharsetEncoder
    {
        private readonly Encoding enc;

        public CharsetEncoder (Encoding enc)
        {
            this.enc = enc;
        }

        public ByteBuffer Encode (CharSequence str)
        {
            return Encode (str.ToString ());
        }

        public ByteBuffer Encode (string str)
        {
            return ByteBuffer.Wrap (enc.GetBytes (str));
        }
    }
}
