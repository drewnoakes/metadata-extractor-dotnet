// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Gif
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public class GifControlDirectory : Directory
    {
        public const int TagDelay = 1;
        public const int TagDisposalMethod = 2;
        public const int TagUserInputFlag = 3;
        public const int TagTransparentColorFlag = 4;
        public const int TagTransparentColorIndex = 5;

        private static readonly Dictionary<int, string> _tagNameMap = new()
        {
            { TagDelay, "Delay" },
            { TagDisposalMethod, "Disposal Method" },
            { TagUserInputFlag, "User Input Flag" },
            { TagTransparentColorFlag, "Transparent Color Flag" },
            { TagTransparentColorIndex, "Transparent Color Index" }
        };

        public GifControlDirectory() : base(_tagNameMap)
        {
            SetDescriptor(new GifControlDescriptor(this));
        }

        public override string Name => "GIF Control";
    }
}
