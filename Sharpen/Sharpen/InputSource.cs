using System.IO;

namespace Sharpen
{
    public class InputSource
    {
        private readonly StreamReader _mInputStream;

        public InputSource(InputStream inputStream)
        {
            _mInputStream = new InputStreamReader(inputStream);
        }

        public InputSource(StreamReader inputStream)
        {
            _mInputStream = inputStream;
        }

        public Stream GetStream()
        {
            return _mInputStream.BaseStream;
        }
    }
}