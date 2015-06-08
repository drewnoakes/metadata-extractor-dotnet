namespace Sharpen
{
    public class FilterInputStream : InputStream
    {
        protected InputStream In;

        public FilterInputStream (InputStream s)
        {
            this.In = s;
        }

        public override int Available ()
        {
            return this.In.Available ();
        }

        public override void Close ()
        {
            this.In.Close ();
        }

        public override void Mark (int readlimit)
        {
            this.In.Mark (readlimit);
        }

        public override bool MarkSupported ()
        {
            return this.In.MarkSupported ();
        }

        public override int Read ()
        {
            return this.In.Read ();
        }

        public override int Read (sbyte[] buf)
        {
            return this.In.Read (buf);
        }

        public override int Read (sbyte[] b, int off, int len)
        {
            return this.In.Read (b, off, len);
        }

        public override void Reset ()
        {
            this.In.Reset ();
        }

        public override long Skip (long cnt)
        {
            return this.In.Skip (cnt);
        }
    }
}
