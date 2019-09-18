// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif
{
    /// <remarks>These tags are found in Panasonic/Leica RAW, RW2 and RWL images. The index values are 'fake' but
    /// chosen specifically to make processing easier</remarks>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class PanasonicRawDistortionDirectory : Directory
    {
        // 0 and 1 are checksums

        public const int TagDistortionParam02 = 2;

        public const int TagDistortionParam04 = 4;
        public const int TagDistortionScale = 5;

        public const int TagDistortionCorrection = 7;
        public const int TagDistortionParam08 = 8;
        public const int TagDistortionParam09 = 9;

        public const int TagDistortionParam11 = 11;
        public const int TagDistortionN = 12;


        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagDistortionParam02, "Distortion Param 2" },
            { TagDistortionParam04, "Distortion Param 4" },
            { TagDistortionScale, "Distortion Scale" },
            { TagDistortionCorrection, "Distortion Correction" },
            { TagDistortionParam08, "Distortion Param 8" },
            { TagDistortionParam09, "Distortion Param 9" },
            { TagDistortionParam11, "Distortion Param 11" },
            { TagDistortionN, "Distortion N" }
        };

        public PanasonicRawDistortionDirectory()
        {
            SetDescriptor(new PanasonicRawDistortionDescriptor(this));
        }

        public override string Name => "PanasonicRaw DistortionInfo";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
