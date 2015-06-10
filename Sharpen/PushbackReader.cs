using System;
using System.IO;

namespace Sharpen
{
    /// <summary>
    /// http://grepcode.com/file_/repository.grepcode.com/java/root/jdk/openjdk/6-b14/java/io/PushbackReader.java/?v=source
    /// </summary>
    public class PushbackReader : StreamReader
    {
        private readonly char[] _buf;
        private int _pos;
        private readonly object _lock;

        public PushbackReader(StreamReader stream, int size)
            : base(stream.BaseStream)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException("size", "size <= 0");
            }
            _lock = this;
            _buf = new char[size];
            _pos = size;
        }

        public override int Read()
        {
            lock (_lock)
            {
                EnsureOpen();
                if (_pos < _buf.Length)
                    return _buf[_pos++];

                return base.Read();
            }
        }

        public override int Read(char[] cbuf, int off, int len)
        {
            lock (_lock)
            {
                EnsureOpen();
                try
                {
                    if (len <= 0)
                    {
                        if (len < 0)
                        {
                            throw new ArgumentException();
                        }
                        if ((off < 0) || (off > cbuf.Length))
                        {
                            throw new ArgumentException();
                        }
                        return 0;
                    }
                    var avail = _buf.Length - _pos;
                    if (avail > 0)
                    {
                        if (len < avail)
                            avail = len;
                        Array.Copy(_buf, _pos, cbuf, off, avail);
                        _pos += avail;
                        off += avail;
                        len -= avail;
                    }
                    if (len > 0)
                    {
                        len = base.Read(cbuf, off, len);
                        if (len == -1)
                        {
                            return (avail == 0) ? -1 : avail;
                        }
                        return avail + len;
                    }
                    return avail;
                }
                catch (IndexOutOfRangeException)
                {
                    throw new ArgumentException();
                }
            }
        }

        public void Unread(char[] cbuf, int off, int len)
        {
            lock (_lock)
            {
                EnsureOpen();
                if (len > _pos)
                    throw new IOException("Pushback buffer overflow");
                _pos -= len;
                Array.Copy(cbuf, off, _buf, _pos, len);
            }
        }

        private void EnsureOpen()
        {
            if (_buf == null)
                throw new IOException("Stream closed");
        }
    }
}