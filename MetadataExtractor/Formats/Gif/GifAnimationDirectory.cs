// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Gif
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class GifAnimationDirectory : Directory
    {
        public const int TagIterationCount = 1;

        private static readonly Dictionary<int, string> _tagNameMap = new()
        {
            { TagIterationCount, "Iteration Count" }
        };

        public GifAnimationDirectory() : base(_tagNameMap)
        {
            SetDescriptor(new GifAnimationDescriptor(this));
        }

        public override string Name => "GIF Animation";
    }
}
