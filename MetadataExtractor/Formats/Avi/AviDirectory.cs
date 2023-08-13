// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Avi
{
    /// <author>Payton Garland</author>
    public class AviDirectory : Directory
    {
        public const int TagFramesPerSecond = 1;
        public const int TagSamplesPerSecond = 2;
        public const int TagDuration = 3;
        public const int TagVideoCodec = 4;
        public const int TagAudioCodec = 5;
        public const int TagWidth = 6;
        public const int TagHeight = 7;
        public const int TagStreams = 8;
        public const int TagDateTimeOriginal = 320;

        private static readonly Dictionary<int, string> _tagNameMap = new()
        {
            {TagFramesPerSecond, "Frames Per Second"},
            {TagSamplesPerSecond, "Samples Per Second"},
            {TagDuration, "Duration"},
            {TagVideoCodec, "Video Codec"},
            {TagAudioCodec, "Audio Codec"},
            {TagWidth, "Width"},
            {TagHeight, "Height"},
            {TagStreams, "Stream Count"},
            {TagDateTimeOriginal, "Date/Time Original"}
        };

        public AviDirectory() : base(_tagNameMap)
        {
            SetDescriptor(new AviDescriptor(this));
        }

        public override string Name => "AVI";
    }
}
