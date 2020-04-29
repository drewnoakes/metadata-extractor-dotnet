// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496.Boxes
{
    internal sealed class PixelInformationBox : FullBox
    {
        public byte ChannelCount { get; }
        public byte[] BitsPerChannel { get; }

        public PixelInformationBox(BoxLocation location, SequentialReader reader)
            : base(location, reader)
        {
            ChannelCount = reader.GetByte();
            BitsPerChannel = reader.GetBytes(ChannelCount);
        }
    }
}
