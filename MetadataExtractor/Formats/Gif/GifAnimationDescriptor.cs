// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Gif
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public class GifAnimationDescriptor : TagDescriptor<GifAnimationDirectory>
    {
        public GifAnimationDescriptor(GifAnimationDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                GifAnimationDirectory.TagIterationCount => GetIterationCountDescription(),
                _ => null,
            };
        }

        private string? GetIterationCountDescription()
        {
            if (!Directory.TryGetUInt16(GifAnimationDirectory.TagIterationCount, out ushort count))
                return null;
            return count == 0 ? "Infinite" : count == 1 ? "Once" : count == 2 ? "Twice" : $"{count} times";
        }
    }
}
