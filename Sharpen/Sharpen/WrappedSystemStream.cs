using System;
using System.IO;

namespace Sharpen
{
    public class WrappedSystemStream : Stream
    {
        private readonly InputStream _ist;
        private readonly OutputStream _ost;
        int _position;
        int _markedPosition;

        public WrappedSystemStream (InputStream ist)
        {
            this._ist = ist;
        }

        public WrappedSystemStream (OutputStream ost)
        {
            this._ost = ost;
        }

        public InputStream InputStream {
            get { return _ist; }
        }

        public OutputStream OutputStream {
            get { return _ost; }
        }

        public override void Close ()
        {
            if (this._ist != null) {
                this._ist.Close ();
            }
            if (this._ost != null) {
                this._ost.Close ();
            }
        }

        public override void Flush ()
        {
            this._ost.Flush ();
        }

        public override int Read (byte[] buffer, int offset, int count)
        {
            sbyte[] sbuffer = new sbyte[count];
            int res = this._ist.Read(sbuffer, 0, count);
            Extensions.CopyCastBuffer(sbuffer,0, count,  buffer, offset);
            if (res != -1) {
                _position += res;
                return res;
            } else
                return 0;
        }

        public override int ReadByte ()
        {
            int res = this._ist.Read ();
            if (res != -1)
                _position++;
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
            this._ost.Write (sbuffer, 0, count);
            _position += count;
        }

        public override void WriteByte (byte value)
        {
            this._ost.Write (value);
            _position++;
        }

        public override bool CanRead {
            get { return (this._ist != null); }
        }

        public override bool CanSeek {
            get { return true; }
        }

        public override bool CanWrite {
            get { return (this._ost != null); }
        }

        public override long Length {
            get
            {
                if (_ist != null)
                {
                    return _ist.GetNativeStream().Length;
                }

                throw new NotSupportedException ();
            }
        }

        public void OnMark (int nb)
        {
            _markedPosition = _position;
            _ist.Mark (nb);
        }

        public override long Position {
            get {
                if (_ist != null && _ist.CanSeek ())
                    return _ist.Position;
                else
                    return _position;
            }
            set {
                if (value == _position)
                    return;
                else if (value == _markedPosition)
                    _ist.Reset ();
                else if (_ist != null && _ist.CanSeek ()) {
                    _ist.Position = value;
                }
                else
                    throw new NotSupportedException ();
            }
        }
    }
}
