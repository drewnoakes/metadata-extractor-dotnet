using MetadataExtractor.Util;
using System.Text;

namespace MetadataExtractor.Formats.Riff
{
    internal sealed class RiffTypeChecker : ITypeChecker
    {
        public int ByteCount => 12;

        public Util.FileType CheckType(byte[] bytes)
        {
            if (!bytes.RegionEquals(0, 4, Encoding.UTF8.GetBytes("RIFF")))
                return Util.FileType.Unknown;
            var fourCC = Encoding.UTF8.GetString(bytes, index: 8, count: 4);
            return fourCC switch
            {
                "WAVE" => Util.FileType.Wav,
                "AVI " => Util.FileType.Avi,
                "WEBP" => Util.FileType.WebP,
                _ => Util.FileType.Riff,
            };
        }
    }
}
