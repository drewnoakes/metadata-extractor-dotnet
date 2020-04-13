// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496
{
    public class ItemPropertyAssociationBox : FullBox
    {
        public uint EntryCount { get; }
        public ItemPropertyAssociationEntry[] Entries { get; }

        public ItemPropertyAssociationBox(BoxLocation loc, SequentialReader sr) : base(loc, sr)
        {
            EntryCount = sr.GetUInt32();
            Entries = ParseEntries(sr);
        }

        private ItemPropertyAssociationEntry[] ParseEntries(SequentialReader sr)
        {
            var bits = new BitReader(sr);
            var ret = new ItemPropertyAssociationEntry[EntryCount];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = ParseAssociationEntry(bits);
            }

            return ret;
        }

        private ItemPropertyAssociationEntry ParseAssociationEntry(BitReader bits)
        {
            return new ItemPropertyAssociationEntry(
                bits.GetUInt32(ItemIndexWidth),
                ReadAssociatedItems(bits));
        }

        private AssociatedProperty[] ReadAssociatedItems(BitReader bits)
        {
            var ret = new AssociatedProperty[bits.GetByte(8)];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = ParseAssociatedItem(bits);
            }

            return ret;
        }

        private AssociatedProperty ParseAssociatedItem(BitReader bits)
        {
            return new AssociatedProperty(
                bits.GetBit(),
                bits.GetUInt16(PropertyIndexWidth));
        }

        private int ItemIndexWidth => Version < 1 ? 16 : 32;

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
        public bool Essential { get; }
        public ushort Index { get; }

        public AssociatedProperty(bool essential, ushort index)
        {
            Essential = essential;
            Index = index;
        }
    }
}
