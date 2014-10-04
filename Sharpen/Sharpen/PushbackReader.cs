using System;
using System.IO;

namespace Sharpen
{
    /// <summary>
    /// http://grepcode.com/file_/repository.grepcode.com/java/root/jdk/openjdk/6-b14/java/io/PushbackReader.java/?v=source
    /// </summary>
    public class PushbackReader : StreamReader
    {
        private char[] buf;
        private int pos;
        private object _lock;

        public PushbackReader(StreamReader stream) : this(stream, 1)
        {
        }

        public PushbackReader(StreamReader stream, int size)
            : base(stream.BaseStream)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException("size", "size <= 0");
            }
            _lock = this;
            buf = new char[size];
            pos = size;
        }

        public override int Read()
        {
            lock (_lock)
            {
                EnsureOpen();
                if (pos < buf.Length)
                    return buf[pos++];

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
                        else if ((off < 0) || (off > cbuf.Length))
                        {
                            throw new ArgumentException();
                        }
                        return 0;
                    }
                    int avail = buf.Length - pos;
                    if (avail > 0)
                    {
                        if (len < avail)
                            avail = len;
                        System.Array.Copy(buf, pos, cbuf, off, avail);
                        pos += avail;
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

        public void Unread(int c)
        {
            lock (_lock)
            {
                EnsureOpen();
                if (pos == 0)
                    throw new IOException("Pushback buffer overflow");
                buf[--pos] = (char) c;
            }
        }

        public void Unread(char[] cbuf, int off, int len)
        {
            lock (_lock)
            {
                EnsureOpen();
                if (len > pos)
                    throw new IOException("Pushback buffer overflow");
                pos -= len;
                System.Array.Copy(cbuf, off, buf, pos, len);
            }
        }

        public void unread(char[] cbuf)
        {
            Unread(cbuf, 0, cbuf.Length);
        }

        private void EnsureOpen()
        {
            if (buf == null)
                throw new IOException("Stream closed");
        }
    }
}