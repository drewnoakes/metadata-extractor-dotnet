// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="LeicaType5MakernoteDirectory"/>.
    /// <para />
    /// Tag reference from: http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/Panasonic.html
    /// </summary>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class LeicaType5MakernoteDescriptor : TagDescriptor<LeicaType5MakernoteDirectory>
    {
        public LeicaType5MakernoteDescriptor(LeicaType5MakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                LeicaType5MakernoteDirectory.TagExposureMode => GetExposureModeDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        /// <summary>
        /// 4 values
        /// </summary>
        /// <returns></returns>
        public string? GetExposureModeDescription()
        {
            if (!(Directory.GetObject(LeicaType5MakernoteDirectory.TagExposureMode) is byte[] values) || values.Length < 4)
                return null;

            var join = $"{values[0]} {values[1]} {values[2]} {values[3]}";
            var ret = join switch
            {
                "0 0 0 0" => "Program AE",
                "1 0 0 0" => "Aperture-priority AE",
                "1 1 0 0" => "Aperture-priority AE (1)",
                "2 0 0 0" => "Shutter speed priority AE",  // guess
                "3 0 0 0" => "Manual",
                _ => "Unknown (" + join + ")",
            };
            return ret;
        }
    }
}
