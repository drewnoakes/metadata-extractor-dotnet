using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496Parser
{
    public class ItemDataBox : Box
    {
        public byte[] Data { get; }

        public ItemDataBox(BoxLocation location, SequentialReader sr) : base(location)
        {
            Data = ReadRemainingData(sr);
        }
    }
}
