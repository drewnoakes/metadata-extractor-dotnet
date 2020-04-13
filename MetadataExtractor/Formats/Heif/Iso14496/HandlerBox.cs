// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496
{
    internal class HandlerBox : FullBox
    {
        public uint HandlerType { get; }
        public string TrackType { get; }

        public string HandlerTypeString => TypeStringConverter.ToTypeString(HandlerType);

        public HandlerBox(BoxLocation loc, SequentialReader sr) : base(loc, sr)
        {
            sr.GetUInt32(); // should be Zero
            HandlerType = sr.GetUInt32();
            sr.GetUInt32(); // should be Zero
            sr.GetUInt32(); // should be Zero
            sr.GetUInt32(); // should be Zero
            TrackType = sr.GetString((int)loc.BytesLeft(sr), Encoding.UTF8);
        }
    }
}
