using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496Parser
{
    public class FullBox : Box
    {
        private readonly uint typeAndFlags;
        public byte Version => (byte)(typeAndFlags >> 24);
        public uint Flags => typeAndFlags & 0x00FFFFFF;

        public FullBox(BoxLocation loc, SequentialReader sr) : base(loc)
        {
            typeAndFlags = sr.GetUInt32();
        }
    }
}
