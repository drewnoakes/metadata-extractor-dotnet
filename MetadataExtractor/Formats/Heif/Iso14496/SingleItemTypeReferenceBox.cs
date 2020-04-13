// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496
{
    public class SingleItemTypeReferenceBox : Box
    {
        public uint FromItemId { get; }
        public ushort ReferenceCount { get; }
        public uint[] ToItemIds { get; }

        public SingleItemTypeReferenceBox(BoxLocation loc, SequentialReader sr, byte parentVersion) : base(loc)
        {
            FromItemId = parentVersion == 0 ? sr.GetUInt16() : sr.GetUInt32();
            ReferenceCount = sr.GetUInt16();
            ToItemIds = new uint[ReferenceCount];
            for (int i = 0; i < ReferenceCount; i++)
            {
                ToItemIds[i] = parentVersion == 0 ? sr.GetUInt16() : sr.GetUInt32();
            }
        }
    }
}
