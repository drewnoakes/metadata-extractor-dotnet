using System.IO;

namespace Sharpen
{
    public class RandomAccessFile
    {
        private readonly FileStream _stream;

        public RandomAccessFile (FilePath file, string mode) : this(file.GetPath (), mode)
        {
        }

        public RandomAccessFile (string file, string mode)
        {
            if (mode.IndexOf ('w') != -1)
                _stream = new FileStream (file, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            else
                _stream = new FileStream (file, FileMode.Open, FileAccess.Read);
        }

        public void Close ()
        {
            _stream.Close ();
        }

        public long Length ()
        {
            return _stream.Length;
        }

        public int Read(byte[] sbuffer)
        {
            if (sbuffer.Length == 0)
            {
                return 0;
            }

            byte[] buffer = new byte[sbuffer.Length];
            int r = _stream.Read(buffer, 0, buffer.Length);
            if (r > 0)
            {
                Extensions.Copy(buffer, sbuffer);
                return r;
            }

            return -1;
        }

        public int Read ()
        {
            byte[] buffer = new byte[1];
            int r = _stream.Read(buffer, 0, buffer.Length);
            return r > 0 ? buffer[0] : -1;
        }

        public void Seek (long pos)
        {
            _stream.Position = pos;
        }
    }
}
