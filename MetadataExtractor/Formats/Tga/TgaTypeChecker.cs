using MetadataExtractor.Util;

namespace MetadataExtractor.Formats.Tga
{
    internal sealed class TgaTypeChecker : ITypeChecker
    {
        public int ByteCount => TgaHeaderReader.HeaderSize;

        public Util.FileType CheckType(byte[] bytes)
        {
            if (TgaHeaderReader.Instance.TryExtract(bytes, out var _))
                return Util.FileType.Tga;
            return Util.FileType.Unknown;
        }
    }
}
