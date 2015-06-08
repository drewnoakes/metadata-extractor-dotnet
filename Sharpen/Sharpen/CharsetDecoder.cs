using System.Text;

namespace Sharpen
{
    public class CharsetDecoder
    {
        private readonly Encoding enc;
        readonly Decoder decoder;

        public CharsetDecoder (Encoding enc)
        {
            this.enc = enc;
            this.decoder = enc.GetDecoder ();
        }

        public string Decode (ByteBuffer b)
        {
            string res = enc.Decode (b);
            if (res.IndexOf ('\uFFFD') != -1 && decoder.Fallback == DecoderFallback.ExceptionFallback)
                throw new CharacterCodingException ();
            return res;
        }
    }
}
