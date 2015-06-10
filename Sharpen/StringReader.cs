using System.IO;
using System.Text;

namespace Sharpen
{
    public class StringReader : StreamReader
    {
        public StringReader(string input) : base(GetStreamFromString(input))
        {
        }

        private static Stream GetStreamFromString(string input)
        {
            var byteArray = Encoding.UTF8.GetBytes(input);
            return new MemoryStream(byteArray);
        }
    }
}