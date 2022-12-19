// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Iso14496.Boxes
{
    internal sealed class HandlerBox : FullBox
    {
        public uint HandlerType { get; }
        public string TrackType { get; }

        public HandlerBox(BoxLocation location, SequentialReader reader)
            : base(location, reader)
        {
            reader.GetUInt32(); // should be Zero
            HandlerType = reader.GetUInt32();
            reader.GetUInt32(); // should be Zero
            reader.GetUInt32(); // should be Zero
            reader.GetUInt32(); // should be Zero
            TrackType = reader.GetString((int)reader.BytesRemainingInBox(location), Encoding.UTF8);
        }
    }
}
