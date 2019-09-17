// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to certain Leica cameras.</summary>
    /// <remarks>
    /// Tag reference from: http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/Panasonic.html
    /// </remarks>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class LeicaType5MakernoteDirectory : Directory
    {
        public const int TagLensModel = 0x0303;
        public const int TagOriginalFileName = 0x0407;
        public const int TagOriginalDirectory = 0x0408;
        public const int TagExposureMode = 0x040d;
        public const int TagShotInfo = 0x0410;
        public const int TagFilmMode = 0x0412;
        public const int TagWbRgbLevels = 0x0413;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagLensModel, "Lens Model" },
            { TagOriginalFileName, "Original File Name" },
            { TagOriginalDirectory, "Original Directory" },
            { TagExposureMode, "Exposure Mode" },
            { TagShotInfo, "Shot Info" },
            { TagFilmMode, "Film Mode" },
            { TagWbRgbLevels, "WB RGB Levels" }
        };

        public LeicaType5MakernoteDirectory()
        {
            SetDescriptor(new LeicaType5MakernoteDescriptor(this));
        }

        public override string Name => "Leica Makernote";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
