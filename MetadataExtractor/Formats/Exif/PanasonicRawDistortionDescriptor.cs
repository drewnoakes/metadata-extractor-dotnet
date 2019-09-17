// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="PanasonicRawDistortionDirectory"/>.
    /// </summary>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class PanasonicRawDistortionDescriptor : TagDescriptor<PanasonicRawDistortionDirectory>
    {
        public PanasonicRawDistortionDescriptor(PanasonicRawDistortionDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                PanasonicRawDistortionDirectory.TagDistortionParam02 => GetDistortionParam02Description(),
                PanasonicRawDistortionDirectory.TagDistortionParam04 => GetDistortionParam04Description(),
                PanasonicRawDistortionDirectory.TagDistortionScale => GetDistortionScaleDescription(),
                PanasonicRawDistortionDirectory.TagDistortionCorrection => GetDistortionCorrectionDescription(),
                PanasonicRawDistortionDirectory.TagDistortionParam08 => GetDistortionParam08Description(),
                PanasonicRawDistortionDirectory.TagDistortionParam09 => GetDistortionParam09Description(),
                PanasonicRawDistortionDirectory.TagDistortionParam11 => GetDistortionParam11Description(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetDistortionParam02Description()
        {
            if (!Directory.TryGetInt16(PanasonicRawDistortionDirectory.TagDistortionParam02, out short value))
                return null;

            return new Rational(value, 32678).ToString();
            //return ((double)value / 32768.0d).ToString();
        }

        public string? GetDistortionParam04Description()
        {
            if (!Directory.TryGetInt16(PanasonicRawDistortionDirectory.TagDistortionParam04, out short value))
                return null;

            return new Rational(value, 32678).ToString();
        }

        public string? GetDistortionScaleDescription()
        {
            if (!Directory.TryGetInt16(PanasonicRawDistortionDirectory.TagDistortionScale, out short value))
                return null;

            return (1 / (1 + value / 32768)).ToString();
        }

        public string? GetDistortionCorrectionDescription()
        {
            if (!Directory.TryGetInt32(PanasonicRawDistortionDirectory.TagDistortionCorrection, out int value))
                return null;

            // (have seen the upper 4 bits set for GF5 and GX1, giving a value of -4095 - PH)
            var mask = 0x000f;
            return (value & mask) switch
            {
                0 => "Off",
                1 => "On",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetDistortionParam08Description()
        {
            if (!Directory.TryGetInt16(PanasonicRawDistortionDirectory.TagDistortionParam08, out short value))
                return null;

            return new Rational(value, 32678).ToString();
        }

        public string? GetDistortionParam09Description()
        {
            if (!Directory.TryGetInt16(PanasonicRawDistortionDirectory.TagDistortionParam09, out short value))
                return null;

            return new Rational(value, 32678).ToString();
        }

        public string? GetDistortionParam11Description()
        {
            if (!Directory.TryGetInt16(PanasonicRawDistortionDirectory.TagDistortionParam11, out short value))
                return null;

            return new Rational(value, 32678).ToString();
        }

    }
}
