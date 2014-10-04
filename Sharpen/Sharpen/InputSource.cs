using System.IO;

namespace Sharpen
{
    public class InputSource
    {
        private readonly StreamReader m_inputStream;

        public InputSource(InputStream inputStream)
        {
            m_inputStream = new InputStreamReader(inputStream);
        }

        public InputSource(StreamReader inputStream)
        {
            m_inputStream = inputStream;
        }

        public Stream GetStream()
        {
            return m_inputStream.BaseStream;
        }
    }
}