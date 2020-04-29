// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496.Boxes
{
    internal sealed class ImageSpatialExtentsBox : FullBox
    {
        public uint X { get; }
        public uint Y { get; }

        public ImageSpatialExtentsBox(BoxLocation location, SequentialReader reader)
            : base(location, reader)
        {
            X = reader.GetUInt32();
            Y = reader.GetUInt32();
        }
    }
}
