namespace Sharpen
{
    public class ByteBuffer
    {
        private readonly byte[] buffer;
        private int index;
        private int limit;
        private int offset;

        public ByteBuffer ()
        {
        }

        private ByteBuffer (byte[] buf, int start, int len)
        {
            this.buffer = buf;
            this.offset = 0;
            this.limit = start + len;
            this.index = start;
        }

        public byte[] Array ()
        {
            return buffer;
        }

        public int ArrayOffset ()
        {
            return offset;
        }

        public int Limit ()
        {
            return (limit - offset);
        }

        public int Position ()
        {
            return (index - offset);
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
