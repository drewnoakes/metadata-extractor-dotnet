using System.IO;

namespace Sharpen
{
    public class BufferedReader : StreamReader
    {
        public BufferedReader (InputStreamReader r) : base(r.BaseStream)
        {
        }
    }
}
