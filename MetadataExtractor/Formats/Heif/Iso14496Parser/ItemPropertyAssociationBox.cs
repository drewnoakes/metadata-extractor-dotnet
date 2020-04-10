using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496Parser
{
    public class ItemPropertyAssociationBox : FullBox
    {
        public uint EntryCount { get; }
        public ItemPropertyAssociationEntry[] Entries;

        public ItemPropertyAssociationBox(BoxLocation loc, SequentialReader sr) : base(loc, sr)
        {
            EntryCount = sr.GetUInt32();
            Entries = ParseEntries(sr);
        }

        private ItemPropertyAssociationEntry[] ParseEntries(SequentialReader sr)
        {
            var bits = new BitsReader(sr);
            var ret = new ItemPropertyAssociationEntry[EntryCount];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = ParseAssociationEntry(bits);
            }
            return ret;
        }

        private ItemPropertyAssociationEntry ParseAssociationEntry(BitsReader bits)
        {
            return new ItemPropertyAssociationEntry(bits.GetUInt32(ItemIndexWidth),
                ReadAssociatedItems(bits));

        }

        private AssociatedProperty[] ReadAssociatedItems(BitsReader bits)
        {
            var ret = new AssociatedProperty[bits.GetByte(8)];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = ParseAssociatedItem(bits);
            }

            return ret;
        }

        private AssociatedProperty ParseAssociatedItem(BitsReader bits)
        {
            return new AssociatedProperty(
                bits.GetBit(),
                bits.GetUInt16(PropertyIndexWidth));
        }

        private int ItemIndexWidth => Version<1?16:32;

        private int PropertyIndexWidth => (Flags & 0x1) == 0x1 ? 15 : 7;
    }

    public class ItemPropertyAssociationEntry
    {
        public uint ItemId { get; }
        public AssociatedProperty[] Properties { get; }

        public ItemPropertyAssociationEntry(uint itemId, AssociatedProperty[] properties)
        {
            ItemId = itemId;
            Properties = properties;
        }
    }

    public class AssociatedProperty
    {
        public bool Essential { get;}
        public ushort Index { get;}

        public AssociatedProperty(bool essential, ushort index)
        {
            Essential = essential;
            Index = index;
        }
    }
}
