namespace Sharpen
{
    using System;
    using System.IO;

    public class WrappedSystemStream : Stream
    {
        private InputStream ist;
        private OutputStream ost;
        int position;
        int markedPosition;

        public WrappedSystemStream (InputStream ist)
        {
            this.ist = ist;
        }

        public WrappedSystemStream (OutputStream ost)
        {
            this.ost = ost;
        }
        
        public InputStream InputStream {
            get { return ist; }
        }

        public OutputStream OutputStream {
            get { return ost; }
        }

        public override void Close ()
        {
            if (this.ist != null) {
                this.ist.Close ();
            }
            if (this.ost != null) {
                this.ost.Close ();
            }
        }

        public override void Flush ()
        {
            this.ost.Flush ();
        }

        public override int Read (byte[] buffer, int offset, int count)
        {
            sbyte[] sbuffer = new sbyte[count];
            int res = this.ist.Read(sbuffer, 0, count);
            Extensions.CopyCastBuffer(sbuffer,0, count,  buffer, offset);
            if (res != -1) {
                position += res;
                return res;
            } else
                return 0;
        }

        public override int ReadByte ()
        {
            int res = this.ist.Read ();
            if (res != -1)
                position++;
            return res;
        }

        public override long Seek (long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Begin)
                Position = offset;
            else if (origin == SeekOrigin.Current)
                Position = Position + offset;
            else if (origin == SeekOrigin.End)
                Position = Length + offset;
            return Position;
        }

        public override void SetLength (long value)
        {
            throw new NotSupportedException ();
        }

        public override void Write (byte[] buffer, int offset, int count)
        {
            sbyte[] sbuffer = new sbyte[count];
            Extensions.CopyCastBuffer(buffer, offset, count, sbuffer,0);
            this.ost.Write (sbuffer, 0, count);
            position += count;
        }

        public override void WriteByte (byte value)
        {
            this.ost.Write (value);
            position++;
        }

        public override bool CanRead {
            get { return (this.ist != null); }
        }

        public override bool CanSeek {
            get { return true; }
        }

        public override bool CanWrite {
            get { return (this.ost != null); }
        }

        public override long Length {
            get 
            {
                if (ist != null)
                {
                    return ist.GetNativeStream().Length;
                }
                
                throw new NotSupportedException ();
            }
        }
        
        public void OnMark (int nb)
        {
            markedPosition = position;
            ist.Mark (nb);
        }
        
        public override long Position {
            get {
                if (ist != null && ist.CanSeek ())
                    return ist.Position;
                else
                    return position;
            }
            set {
                if (value == position)
                    return;
                else if (value == markedPosition)
                    ist.Reset ();
                else if (ist != null && ist.CanSeek ()) {
                    ist.Position = value;
                }
                else
                    throw new NotSupportedException ();
            }
        }
    }
}
