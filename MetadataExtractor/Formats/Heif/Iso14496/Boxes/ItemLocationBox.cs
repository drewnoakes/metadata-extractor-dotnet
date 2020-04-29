// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.IO;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496.Boxes
{
    internal sealed class ItemLocationBox : FullBox
    {
        public byte OffsetSize { get; }
        public byte LengthSize { get; }
        public byte BaseOffsetSize { get; }
        public byte IndexSize { get; }
        public uint ItemCount { get; }
        public ItemLocation[] ItemLocations { get; }

        public ItemLocationBox(BoxLocation location, SequentialReader reader)
            : base(location, reader)
        {
            var bitReader = new BitReader(reader);
            OffsetSize = bitReader.GetByte(4);
            LengthSize = bitReader.GetByte(4);
            BaseOffsetSize = bitReader.GetByte(4);
            if (Version == 1 || Version == 2)
            {
                IndexSize = bitReader.GetByte(4);
            }
            else
            {
                bitReader.GetByte(4);
            }

            ItemCount = Version < 2 ? bitReader.GetUInt32(16) : bitReader.GetUInt32(32);
            ItemLocations = ParseLocationArray(reader);
        }

        private ItemLocation[] ParseLocationArray(SequentialReader sr)
        {
            var ret = new ItemLocation[ItemCount];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = ParseLocation(sr);
            }

            return ret;
        }

        private ItemLocation ParseLocation(SequentialReader sr)
        {
            return new ItemLocation(
                ReadItemNumber(sr), ReadConstructionMethod(sr), sr.GetUInt16(),
                ReadSizedPointer(sr, BaseOffsetSize), ReadExtentList(sr));
        }

        private uint ReadItemNumber(SequentialReader sr) => Version < 2 ? sr.GetUInt16() : sr.GetUInt32();

        private ConstructionMethod ReadConstructionMethod(SequentialReader sr) =>
            (ConstructionMethod)((Version == 1 || Version == 2) ? (sr.GetUInt16() & 0x0F) : 0);

        private static ulong ReadSizedPointer(SequentialReader sr, byte pointerSize) =>
            pointerSize switch
            {
                0 => 0,
                4 => sr.GetUInt32(),
                8 => sr.GetUInt64(),
                _ => throw new InvalidDataException("Pointer size must be 0, 4, or 8 bytes")
            };

        private ItemLocationExtent[] ReadExtentList(SequentialReader sr)
        {
            var ret = new ItemLocationExtent[sr.GetUInt16()];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = ReadExtent(sr);
            }

            return ret;
        }

        private ItemLocationExtent ReadExtent(SequentialReader sr) =>
            new ItemLocationExtent(
                ReadItemIndex(sr), ReadSizedPointer(sr, OffsetSize),
                ReadSizedPointer(sr, LengthSize));

        private ulong ReadItemIndex(SequentialReader sr) =>
            (Version == 1 || Version == 2) ? ReadSizedPointer(sr, IndexSize) : 0;
    }

    internal enum ConstructionMethod
    {
        FileOffset = 0,
        IdatOffset = 1,
        ItemOffset = 2,
    }

    internal class ItemLocation
    {
        public uint ItemId { get; }
        public ConstructionMethod ConstructionMethod { get; }
        public ushort DataReferenceIndex { get; }
        public ulong BaseOffset { get; }
        public ItemLocationExtent[] ExtentList { get; }

        public ItemLocation(
            uint itemId,
            ConstructionMethod constructionMethod,
            ushort dataReferenceIndex,
            ulong baseOffset,
            ItemLocationExtent[] extentList)
        {
            ItemId = itemId;
            ConstructionMethod = constructionMethod;
            DataReferenceIndex = dataReferenceIndex;
            BaseOffset = baseOffset;
            ExtentList = extentList;
        }
    }

    internal class ItemLocationExtent
    {
        public ulong ExtentIndex { get; }
        public ulong ExtentOffset { get; }
        public ulong ExtentLength { get; }

        public ItemLocationExtent(ulong extentIndex, ulong extentOffset, ulong extentLength)
        {
            ExtentIndex = extentIndex;
            ExtentOffset = extentOffset;
            ExtentLength = extentLength;
        }
    }
}
