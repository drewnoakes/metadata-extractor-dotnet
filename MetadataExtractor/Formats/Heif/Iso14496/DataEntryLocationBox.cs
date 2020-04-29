// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496
{
    internal class DataEntryLocationBox : FullBox
    {
        public string Name { get; }
        public string Location { get; }

        public DataEntryLocationBox(BoxLocation location, SequentialReader reader, bool hasName)
            : base(location, reader)
        {
            Name = hasName ? reader.GetNullTerminatedString((int)reader.BytesRemainingInBox(location), Encoding.UTF8) : "";
            Location = !reader.IsWithinBox(location) ? "" : reader.GetNullTerminatedString((int)reader.BytesRemainingInBox(location), Encoding.UTF8);
        }
    }
}
