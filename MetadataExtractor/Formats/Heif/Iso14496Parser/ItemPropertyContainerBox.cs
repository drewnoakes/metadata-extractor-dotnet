using System.Collections.Generic;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496Parser
{
    public class ItemPropertyContainerBox : Box
    {
        public IList<Box> Boxes { get; }

        public ItemPropertyContainerBox(BoxLocation loc, SequentialReader sr) : base(loc)
        {
            Boxes = BoxReader.BoxList(loc, sr);
        }

        public override IEnumerable<Box> Children() => Boxes;
    }
}
