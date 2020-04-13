// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496
{
    public class ImageSpatialExtentsBox : FullBox
    {
        public uint X { get; }
        public uint Y { get; }

        public ImageSpatialExtentsBox(BoxLocation loc, SequentialReader sr) : base(loc, sr)
        {
            X = sr.GetUInt32();
            Y = sr.GetUInt32();
        }
    }
}
