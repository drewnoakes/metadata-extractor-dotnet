// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace MetadataExtractor.Formats.Mpeg
{
    public sealed class Mp3Directory : Directory
    {
        public const int TAG_ID = 1;
        public const int TAG_LAYER = 2;
        public const int TAG_BITRATE = 3;
        public const int TAG_FREQUENCY = 4;
        public const int TAG_MODE = 5;
        public const int TAG_EMPHASIS = 6;
        public const int TAG_COPYRIGHT = 7;
        public const int TAG_FRAME_SIZE = 8;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TAG_ID, "ID" },
            { TAG_LAYER, "Layer" },
            { TAG_BITRATE, "Bitrate" },
            { TAG_FREQUENCY, "Frequency" },
            { TAG_MODE, "Mode" },
            { TAG_EMPHASIS, "Emphasis Method" },
            { TAG_COPYRIGHT, "Copyright" },
            { TAG_FRAME_SIZE, "Frame Size" }
        };

        public Mp3Directory()
        {
            SetDescriptor(new Mp3Descriptor(this));
        }

        public override string Name => "MP3";

        protected override bool TryGetTagName(int tagType, out string? tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
