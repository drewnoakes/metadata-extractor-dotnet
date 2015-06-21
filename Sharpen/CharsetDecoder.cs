using System.Text;

namespace Sharpen
{
    public class CharsetDecoder
    {
        private readonly Encoding _enc;
        readonly Decoder _decoder;

        public CharsetDecoder (Encoding enc)
        {
            _enc = enc;
            _decoder = enc.GetDecoder ();
        }

        public string Decode (ByteBuffer b)
        {
            string res = _enc.Decode (b);
            if (res.IndexOf ('\uFFFD') != -1 && _decoder.Fallback == DecoderFallback.ExceptionFallback)
                throw new CharacterCodingException ();
            return res;
        }
    }
}
