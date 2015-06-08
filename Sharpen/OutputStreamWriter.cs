using System.IO;

namespace Sharpen
{
    public class OutputStreamWriter : StreamWriter
    {
        public OutputStreamWriter (OutputStream stream, string encoding)
            : base(stream.GetWrappedStream (), Extensions.GetEncoding (encoding))
        {
        }

        public OutputStreamWriter(PrintStream printStream)
            : base(printStream.InternalWriter.BaseStream)
        {
        }
    }
}
