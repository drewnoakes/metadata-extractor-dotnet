// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace MetadataExtractor.Formats.Mpeg
{
    public sealed class Mp3Directory : Directory
    {
        public const int TagId = 1;
        public const int TagLayer = 2;
        public const int TagBitrate = 3;
        public const int TagFrequency = 4;
        public const int TagMode = 5;
        public const int TagEmphasis = 6;
        public const int TagCopyright = 7;
        public const int TagFrameSize = 8;

        private static readonly Dictionary<int, string> _tagNameMap = new()
        {
            { TagId, "ID" },
            { TagLayer, "Layer" },
            { TagBitrate, "Bitrate" },
            { TagFrequency, "Frequency" },
            { TagMode, "Mode" },
            { TagEmphasis, "Emphasis Method" },
            { TagCopyright, "Copyright" },
            { TagFrameSize, "Frame Size" }
        };

        public Mp3Directory() : base(_tagNameMap)
        {
            SetDescriptor(new Mp3Descriptor(this));
        }

        public override string Name => "MP3";
    }
}
