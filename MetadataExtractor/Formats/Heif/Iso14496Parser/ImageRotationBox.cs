// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496Parser
{
    public class ImageRotationBox : Box
    {
        public uint Rotation { get; } // rotation is anti-clockwise and valid values are 0,90,180, and 270

        public ImageRotationBox(BoxLocation boxLocation, SequentialReader sr) : base(boxLocation)
        {
            Rotation = (uint)((sr.GetByte() & 3) * 90);
        }
    }
}
