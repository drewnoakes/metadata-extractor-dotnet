using System;
using System.IO;

namespace Sharpen
{
    public class OutputStream : IDisposable
    {
        protected Stream Wrapped;

        public static implicit operator OutputStream (Stream s)
        {
            return Wrap (s);
        }

        public static implicit operator Stream (OutputStream s)
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

        public virtual void Flush ()
        {
            if (Wrapped != null) {
                Wrapped.Flush ();
            }
        }

        public Stream GetWrappedStream ()
        {
            // Always create a wrapper stream (not directly Wrapped) since the subclass
            // may be overriding methods that need to be called when used through the Stream class
            return new WrappedSystemStream (this);
        }

        static public OutputStream Wrap (Stream s)
        {
            OutputStream stream = new OutputStream ();
            stream.Wrapped = s;
            return stream;
        }

        public virtual void Write (int b)
        {
            var stream = Wrapped as WrappedSystemStream;
            if (stream != null)
                stream.OutputStream.Write (b);
            else {
                if (Wrapped == null)
                    throw new NotImplementedException ();
                Wrapped.WriteByte ((byte)b);
            }
        }

        public virtual void Write (sbyte[] b)
        {
            Write (b, 0, b.Length);
        }

        public virtual void Write (sbyte[] b, int offset, int len)
        {
            var stream = Wrapped as WrappedSystemStream;
            if (stream != null)
                stream.OutputStream.Write (b, offset, len);
            else {
                if (Wrapped != null) {
                    Wrapped.Write (Extensions.ConvertToByteArray(b), offset, len);
                } else {
                    for (int i = 0; i < len; i++) {
                        Write (b[i + offset]);
                    }
                }
            }
        }
    }
}
