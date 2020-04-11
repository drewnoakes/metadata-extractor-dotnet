using System.Collections.Generic;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496Parser
{
    public class ItemInformationBox : FullBox
    {
        public uint Count {get;}
        public IList<Box> Boxes { get; }
        public ItemInformationBox(BoxLocation loc, SequentialReader sr) : base(loc, sr)
        {
            Count = Version == 0?sr.GetUInt16():sr.GetUInt32();
            Boxes = BoxReader.BoxList(loc, sr);
        }
        public override IEnumerable<Box> Children() => Boxes;

    }
}
