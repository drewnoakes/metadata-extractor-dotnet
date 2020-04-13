using System.Collections.Generic;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496Parser
{
    public class DataReferenceBox : FullBox
    {
        public IList<Box> Boxes { get; }
        public uint BoxCount { get; }

        public DataReferenceBox(BoxLocation loc, SequentialReader sr) : base(loc, sr)
        {
            BoxCount = sr.GetUInt32();
            Boxes = BoxReader.BoxList(loc, sr);
        }

        public override IEnumerable<Box> Children() => Boxes;
    }
}
