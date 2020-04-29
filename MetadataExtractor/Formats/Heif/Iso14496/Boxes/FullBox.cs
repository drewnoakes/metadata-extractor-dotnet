// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496.Boxes
{
    internal abstract class FullBox : Box
    {
        private readonly uint _typeAndFlags;

        public byte Version => (byte)(_typeAndFlags >> 24);
        public uint Flags => _typeAndFlags & 0x00FFFFFF;

        protected FullBox(BoxLocation location, SequentialReader reader)
            : base(location)
        {
            _typeAndFlags = reader.GetUInt32();
        }
    }
}
