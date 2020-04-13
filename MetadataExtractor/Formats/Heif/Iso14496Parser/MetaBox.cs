using System.Collections.Generic;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496Parser
{
    public class MetaBox : FullBox
    {
        public IList<Box> Boxes { get; }

        public MetaBox(BoxLocation loc, SequentialReader sr) : base(loc, sr)
        {
            Boxes = BoxReader.BoxList(loc, sr);
        }

        public override IEnumerable<Box> Children() => Boxes;
    }
}
