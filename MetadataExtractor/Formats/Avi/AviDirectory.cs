// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Avi
{
    /// <author>Payton Garland</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class AviDirectory : Directory
    {
        public const int TAG_FRAMES_PER_SECOND = 1;
        public const int TAG_SAMPLES_PER_SECOND = 2;
        public const int TAG_DURATION = 3;
        public const int TAG_VIDEO_CODEC = 4;
        public const int TAG_AUDIO_CODEC = 5;
        public const int TAG_WIDTH = 6;
        public const int TAG_HEIGHT = 7;
        public const int TAG_STREAMS = 8;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            {TAG_FRAMES_PER_SECOND, "Frames Per Second"},
            {TAG_SAMPLES_PER_SECOND, "Samples Per Second"},
            {TAG_DURATION, "Duration"},
            {TAG_VIDEO_CODEC, "Video Codec"},
            {TAG_AUDIO_CODEC, "Audio Codec"},
            {TAG_WIDTH, "Width"},
            {TAG_HEIGHT, "Height"},
            {TAG_STREAMS, "Stream Count"}
        };

        public AviDirectory()
        {
            SetDescriptor(new AviDescriptor(this));
        }

        public override string Name => "Avi";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
