using System;
using System.IO;

namespace Sharpen
{
    public class InputStream : IDisposable
    {
        private long _mark;
        protected Stream Wrapped;
        protected Stream BaseStream;

        public static implicit operator InputStream (Stream s)
        {
            return Wrap (s);
        }

        public static implicit operator Stream (InputStream s)
        {
            return s.GetWrappedStream ();
        }

        public virtual void Close ()
        {
            if (Wrapped != null) {
                Wrapped.Close ();
            }
        }

        public void Dispose ()
        {
            Close ();
        }

        public Stream GetWrappedStream ()
        {
            // Always create a wrapper stream (not directly Wrapped) since the subclass
            // may be overriding methods that need to be called when used through the Stream class
            return new WrappedSystemStream (this);
        }

        internal Stream GetNativeStream()
        {
            return Wrapped;
        }

        public virtual void Mark (int readlimit)
        {
            var stream = Wrapped as WrappedSystemStream;
            if (stream != null)
                stream.InputStream.Mark (readlimit);
            else {
                stream = BaseStream as WrappedSystemStream;
                if (stream != null)
                    stream.OnMark (readlimit);
                if (Wrapped != null)
                    _mark = Wrapped.Position;
            }
        }

        public virtual bool MarkSupported ()
        {
            var stream = Wrapped as WrappedSystemStream;
            if (stream != null)
                return stream.InputStream.MarkSupported ();
            return ((Wrapped != null) && Wrapped.CanSeek);
        }

        public virtual int Read ()
        {
            if (Wrapped == null) {
                throw new NotImplementedException ();
            }
            return Wrapped.ReadByte ();
        }

        public virtual int Read (byte[] buf)
        {
            return Read (buf, 0, buf.Length);
        }

        public virtual int Read (byte[] b, int off, int len)
        {
            var stream = Wrapped as WrappedSystemStream;
            if (stream != null)
                return stream.InputStream.Read (b, off, len);

            if (Wrapped != null) {
                byte[] buffer = new byte[len];
                int num = Wrapped.Read (buffer, 0, len);
                if (num > 0)
                    Extensions.CopyCastBuffer(buffer, 0, len, b, off);
                return ((num <= 0) ? -1 : num);
            }
            int totalRead = 0;
            while (totalRead < len) {
                int nr = Read ();
                if (nr == -1)
                    return -1;
                b[off + totalRead] = (byte)nr;
                totalRead++;
            }
            return totalRead;
        }

        public virtual void Reset ()
        {
            var stream = Wrapped as WrappedSystemStream;
            if (stream != null)
                stream.InputStream.Reset ();
            else {
                if (Wrapped == null)
                    throw new IOException ();
                Wrapped.Position = _mark;
            }
        }

        public virtual long Skip (long cnt)
        {
            var stream = Wrapped as WrappedSystemStream;
            if (stream != null)
                return stream.InputStream.Skip (cnt);

            long n = cnt;
            while (n > 0) {
                if (Read () == -1)
                    return cnt - n;
                n--;
            }
            return cnt - n;
        }

        public bool CanSeek ()
        {
            if (Wrapped != null)
                return Wrapped.CanSeek;
            return false;
        }

        public long Position {
            get
            {
                if (Wrapped != null)
                    return Wrapped.Position;
                throw new NotSupportedException ();
            }
            set {
                if (Wrapped != null)
                    Wrapped.Position = value;
                else
                    throw new NotSupportedException ();
            }
        }

        static public InputStream Wrap (Stream s)
        {
            InputStream stream = new InputStream ();
            stream.Wrapped = s;
            return stream;
        }
    }
}
