// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496
{
    public class PrimaryItemBox : FullBox
    {
        public uint PrimaryItem { get; }

        public PrimaryItemBox(BoxLocation loc, SequentialReader sr) : base(loc, sr)
        {
            PrimaryItem = Version == 0 ? sr.GetUInt16() : sr.GetUInt32();
        }
    }
}
