// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496
{
    internal class DataEntryLocationBox : FullBox
    {
        public string Name { get; }
        public string Location { get; }

        public DataEntryLocationBox(BoxLocation loc, SequentialReader sr, bool hasName) : base(loc, sr)
        {
            Name = hasName ? sr.GetNullTerminatedString((int)loc.BytesLeft(sr), Encoding.UTF8) : "";
            Location = loc.DoneReading(sr) ? "" : sr.GetNullTerminatedString((int)loc.BytesLeft(sr), Encoding.UTF8);
        }
    }
}
