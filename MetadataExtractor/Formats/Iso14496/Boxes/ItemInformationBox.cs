// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Iso14496.Boxes
{
    internal sealed class ItemInformationBox : FullBox
    {
        public uint Count { get; }
        public IList<Box> Boxes { get; }

        public ItemInformationBox(BoxLocation location, SequentialReader reader)
            : base(location, reader)
        {
            Count = Version == 0 ? reader.GetUInt16() : reader.GetUInt32();
            Boxes = BoxReader.ReadBoxes(reader, location);
        }

        public override IEnumerable<Box> Children() => Boxes;
    }
}
