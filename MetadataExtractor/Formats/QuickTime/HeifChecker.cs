using System.IO;
using System.Linq;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.QuickTime
{
    public static class HeifChecker
    {
        private static readonly uint[] heicTypes = new[]
        {
            0x68656963u, // heic
            0x68656966u, // heif
            0x68657663u, //hevc
        };
        public static bool IsHeicStream(Stream str)
        {
            try
            {
                str.Seek(0, SeekOrigin.Begin);
                var reader = new SequentialStreamReader(str);
                var fTypeLen = reader.GetUInt32();
                while (reader.Position < fTypeLen)
                {
                    var candidate = reader.GetUInt32();
                    if (heicTypes.Contains(candidate)) return true;
                }

                return false;
            }
            finally
            {
                str.Seek(0, SeekOrigin.Begin);
            }
        }
    }
}
