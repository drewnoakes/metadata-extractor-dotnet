using System.Collections.Generic;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496Parser
{
    public class ItemReferenceBox : FullBox
    {
        public IList<SingleItemTypeReferenceBox> Boxes { get; }

        public ItemReferenceBox(BoxLocation loc, SequentialReader sr): base(loc, sr)
        {
            var list = new List<SingleItemTypeReferenceBox>();
            Boxes = list;
            while (!loc.DoneReading(sr)) {
                list.Add((SingleItemTypeReferenceBox)BoxReader.ReadBox(sr, (l,r)=> new SingleItemTypeReferenceBox(l,r,Version)));
            }
        }

    }
}
