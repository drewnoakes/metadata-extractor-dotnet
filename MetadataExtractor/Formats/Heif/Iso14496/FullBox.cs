// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496
{
    public class FullBox : Box
    {
        private readonly uint _typeAndFlags;

        public byte Version => (byte)(_typeAndFlags >> 24);
        public uint Flags => _typeAndFlags & 0x00FFFFFF;

        public FullBox(BoxLocation loc, SequentialReader sr) : base(loc)
        {
            _typeAndFlags = sr.GetUInt32();
        }
    }
}
