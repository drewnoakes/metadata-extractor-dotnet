using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496Parser
{
    public class PrimaryItemBox : FullBox
    {
        public uint PrimaryItem { get; }

        public PrimaryItemBox(BoxLocation loc, SequentialReader sr) : base(loc, sr)
        {
            PrimaryItem = Version == 0 ? sr.GetUInt16() : sr.GetUInt32();
        }
    }
}
