using System.Text;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496Parser
{
    public class DataEntryLocationBox : FullBox
    {
        public string Name { get; }
        public string Location { get; }

        public DataEntryLocationBox(BoxLocation loc, SequentialReader sr, bool hasName) : base(loc, sr)
        {
            Name = hasName ? sr.GetNullTerminatedString((int)loc.BytesLeft(sr), Encoding.UTF8) : "";
            Location = loc.DoneReading(sr) ? "" : sr.GetNullTerminatedString((int)loc.BytesLeft(sr), Encoding.UTF8);
        }
    }
}
