// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496
{
    internal sealed class PrimaryItemBox : FullBox
    {
        public uint PrimaryItem { get; }

        public PrimaryItemBox(BoxLocation location, SequentialReader reader)
            : base(location, reader)
        {
            PrimaryItem = Version == 0 ? reader.GetUInt16() : reader.GetUInt32();
        }
    }
}
