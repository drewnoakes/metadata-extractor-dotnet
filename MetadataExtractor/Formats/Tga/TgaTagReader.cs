using MetadataExtractor.IO;
using System.IO;

namespace MetadataExtractor.Formats.Tga
{
    internal readonly struct TgaTagInfo
    {
        public short Id { get; }
        public int Offset { get; }
        public int Size { get; }

        public TgaTagInfo(short id, int offset, int size)
        {
            Id = id;
            Offset = offset;
            Size = size;
        }
    }

    internal sealed class TgaTagReader : TgaReader<TgaTagInfo[]>
    {
        protected override TgaTagInfo[] Extract(Stream stream, int _)
        {
            var reader = new SequentialStreamReader(stream, isMotorolaByteOrder: false);
            var count = reader.GetUInt16();
            var tags = new TgaTagInfo[count];
            for (int i = 0; i < count; i++)
                tags[i] = GetTag(reader);
            return tags;

            static TgaTagInfo GetTag(SequentialReader reader)
            {
                return new TgaTagInfo(reader.GetInt16(), reader.GetInt32(), reader.GetInt32());
            }
        }
    }
}
