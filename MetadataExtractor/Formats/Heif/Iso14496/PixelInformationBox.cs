// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496
{
    internal class PixelInformationBox : FullBox
    {
        public byte ChannelCount { get; }
        public byte[] BitsPerChannel { get; }

        public PixelInformationBox(BoxLocation loc, SequentialReader sr) : base(loc, sr)
        {
            ChannelCount = sr.GetByte();
            BitsPerChannel = new byte[ChannelCount];
            for (int i = 0; i < BitsPerChannel.Length; i++)
            {
                BitsPerChannel[i] = sr.GetByte();
            }
        }
    }
}
