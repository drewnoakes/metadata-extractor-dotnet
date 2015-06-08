namespace Sharpen
{
    using System;
    using System.IO;
    using System.Text;

    public class OutputStreamWriter : StreamWriter
    {
        public OutputStreamWriter (OutputStream stream) : base(stream.GetWrappedStream ())
        {
        }

        public OutputStreamWriter (OutputStream stream, string encoding) : base(stream.GetWrappedStream (), Extensions.GetEncoding (encoding))
        {
        }

        public OutputStreamWriter (OutputStream stream, Encoding encoding) : base(stream.GetWrappedStream (), encoding)
        {
        }

        public OutputStreamWriter(PrintStream printStream) : base(printStream.InternalWriter.BaseStream)
        {
        }
    }
}
