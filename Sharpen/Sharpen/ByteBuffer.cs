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
            this._buffer = buf;
            this._offset = 0;
            this._limit = start + len;
            this._index = start;
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

        public static ByteBuffer Wrap(sbyte[] buf)
        {
            return Wrap(Extensions.ConvertToByteArray(buf));
        }
    }
}
