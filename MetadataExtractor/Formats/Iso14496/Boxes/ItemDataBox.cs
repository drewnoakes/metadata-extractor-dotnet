// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Iso14496.Boxes
{
    internal sealed class ItemDataBox : Box
    {
        public byte[] Data { get; }

        public ItemDataBox(BoxLocation location, SequentialReader sr)
            : base(location)
        {
            Data = ReadRemainingData(sr);
        }
    }
}
