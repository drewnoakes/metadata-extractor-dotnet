using System;
using System.IO;

namespace Sharpen
{
    public class InputStream : IDisposable
    {
        private long mark;
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
        
        public virtual int Available ()
        {
            if (Wrapped is WrappedSystemStream)
                return ((WrappedSystemStream)Wrapped).InputStream.Available();
            else
                return 0;
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
            if (Wrapped is WrappedSystemStream)
                ((WrappedSystemStream)Wrapped).InputStream.Mark (readlimit);
            else {
                if (BaseStream is WrappedSystemStream)
                    ((WrappedSystemStream)BaseStream).OnMark (readlimit);
                if (Wrapped != null)
                    this.mark = Wrapped.Position;
            }
        }
        
        public virtual bool MarkSupported ()
        {
            if (Wrapped is WrappedSystemStream)
                return ((WrappedSystemStream)Wrapped).InputStream.MarkSupported ();
            else
                return ((Wrapped != null) && Wrapped.CanSeek);
        }

        public virtual int Read ()
        {
            if (Wrapped == null) {
                throw new NotImplementedException ();
            }
            return Wrapped.ReadByte ();
        }

        public virtual int Read (sbyte[] buf)
        {
            return Read (buf, 0, buf.Length);
        }

        public virtual int Read (sbyte[] b, int off, int len)
        {
            if (Wrapped is WrappedSystemStream)
                return ((WrappedSystemStream)Wrapped).InputStream.Read (b, off, len);
            
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
                b[off + totalRead] = (sbyte)nr;
                totalRead++;
            }
            return totalRead;
        }

        public virtual void Reset ()
        {
            if (Wrapped is WrappedSystemStream)
                ((WrappedSystemStream)Wrapped).InputStream.Reset ();
            else {
                if (Wrapped == null)
                    throw new IOException ();
                Wrapped.Position = mark;
            }
        }

        public virtual long Skip (long cnt)
        {
            if (Wrapped is WrappedSystemStream)
                return ((WrappedSystemStream)Wrapped).InputStream.Skip (cnt);
            
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
            else
                return false;
        }
        
        public long Position {
            get {
                if (Wrapped != null)
                    return Wrapped.Position;
                else
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
