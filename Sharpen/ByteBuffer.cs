namespace Sharpen
{
    public class ByteBuffer
    {
        private readonly byte[] _buffer;
        private int _index;
        private int _limit;
        private int _offset;

        public ByteBuffer ()
        {
        }

        private ByteBuffer (byte[] buf, int start, int len)
        {
            _buffer = buf;
            _offset = 0;
            _limit = start + len;
            _index = start;
        }

        public byte[] Array ()
        {
            return _buffer;
        }

        public int ArrayOffset ()
        {
            return _offset;
        }

        public int Limit ()
        {
            return (_limit - _offset);
        }

        public int Position ()
        {
            return (_index - _offset);
        }

        public static ByteBuffer Wrap(byte[] buf)
        {
            return new ByteBuffer (buf, 0, buf.Length);
        }
    }
}
