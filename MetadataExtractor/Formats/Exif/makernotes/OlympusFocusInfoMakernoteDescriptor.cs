// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="OlympusFocusInfoMakernoteDirectory"/>.
    /// </summary>
    /// <remarks>
    /// Some Description functions converted from Exiftool version 10.10 created by Phil Harvey
    /// http://www.sno.phy.queensu.ca/~phil/exiftool/
    /// lib\Image\ExifTool\Olympus.pm
    /// </remarks>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class OlympusFocusInfoMakernoteDescriptor : TagDescriptor<OlympusFocusInfoMakernoteDirectory>
    {
        public OlympusFocusInfoMakernoteDescriptor(OlympusFocusInfoMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                OlympusFocusInfoMakernoteDirectory.TagFocusInfoVersion => GetFocusInfoVersionDescription(),
                OlympusFocusInfoMakernoteDirectory.TagAutoFocus => GetAutoFocusDescription(),
                OlympusFocusInfoMakernoteDirectory.TagFocusDistance => GetFocusDistanceDescription(),
                OlympusFocusInfoMakernoteDirectory.TagAfPoint => GetAfPointDescription(),
                OlympusFocusInfoMakernoteDirectory.TagExternalFlash => GetExternalFlashDescription(),
                OlympusFocusInfoMakernoteDirectory.TagExternalFlashBounce => GetExternalFlashBounceDescription(),
                OlympusFocusInfoMakernoteDirectory.TagExternalFlashZoom => GetExternalFlashZoomDescription(),
                OlympusFocusInfoMakernoteDirectory.TagManualFlash => GetManualFlashDescription(),
                OlympusFocusInfoMakernoteDirectory.TagMacroLed => GetMacroLedDescription(),
                OlympusFocusInfoMakernoteDirectory.TagSensorTemperature => GetSensorTemperatureDescription(),
                OlympusFocusInfoMakernoteDirectory.TagImageStabilization => GetImageStabilizationDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetFocusInfoVersionDescription()
        {
            return GetVersionBytesDescription(OlympusFocusInfoMakernoteDirectory.TagFocusInfoVersion, 4);
        }

        public string? GetAutoFocusDescription()
        {
            return GetIndexedDescription(OlympusFocusInfoMakernoteDirectory.TagAutoFocus,
                "Off", "On");
        }

        /// <remarks>
        /// this rational value looks like it is in mm when the denominator is
        /// 1 (E-1), and cm when denominator is 10 (E-300), so if we ignore the
        /// denominator we are consistently in mm - PH
        /// </remarks>
        public string? GetFocusDistanceDescription()
        {
            if (!Directory.TryGetRational(OlympusFocusInfoMakernoteDirectory.TagFocusDistance, out Rational value))
                return "inf";
            if (value.Numerator == 0xFFFFFFFF || value.Numerator == 0x00000000)
                return "inf";

            return value.Numerator / 1000.0 + " m";
        }

        /// <remarks>
        /// <para>TODO: Complete when Camera Model is available.</para>
        /// <para>There are differences in how to interpret this tag that can only be reconciled by knowing the model.</para>
        /// </remarks>
        public string? GetAfPointDescription()
        {
            if (!Directory.TryGetInt16(OlympusFocusInfoMakernoteDirectory.TagAfPoint, out short value))
                return null;

            return value.ToString();
        }

        public string? GetExternalFlashDescription()
        {
            if (!(Directory.GetObject(OlympusFocusInfoMakernoteDirectory.TagExternalFlash) is ushort[] values) || values.Length < 2)
                return null;

            var join = $"{values[0]} {values[1]}";

            return join switch
            {
                "0 0" => "Off",
                "1 0" => "On",
                _ => "Unknown (" + join + ")",
            };
        }

        public string? GetExternalFlashBounceDescription()
        {
            return GetIndexedDescription(OlympusFocusInfoMakernoteDirectory.TagExternalFlashBounce,
                "Bounce or Off", "Direct");
        }

        public string? GetExternalFlashZoomDescription()
        {
            var values = Directory.GetObject(OlympusFocusInfoMakernoteDirectory.TagExternalFlashZoom) as ushort[];
            if (values == null)
            {
                // check if it's only one value long also
                if (!Directory.TryGetInt16(OlympusFocusInfoMakernoteDirectory.TagExternalFlashZoom, out short value))
                    return null;

                values = new ushort[1];
                values[0] = (ushort)value;
            }

            if (values.Length == 0)
                return null;

            var join = $"{values[0]}" + (values.Length > 1 ? $"{ values[1]}" : "");

            return join switch
            {
                "0" => "Off",
                "1" => "On",
                "0 0" => "Off",
                "1 0" => "On",
                _ => "Unknown (" + join + ")",
            };
        }

        public string? GetManualFlashDescription()
        {
            if (!(Directory.GetObject(OlympusFocusInfoMakernoteDirectory.TagManualFlash) is short[] values))
                return null;

            if (values[0] == 0)
                return "Off";

            if (values[1] == 1)
                return "Full";
            return "On (1/" + values[1] + " strength)";
        }

        public string? GetMacroLedDescription()
        {
            return GetIndexedDescription(OlympusFocusInfoMakernoteDirectory.TagMacroLed,
                "Off", "On");
        }

        /// <remarks>
        /// <para>TODO: Complete when Camera Model is available.</para>
        /// <para>There are differences in how to interpret this tag that can only be reconciled by knowing the model.</para>
        /// </remarks>
        public string? GetSensorTemperatureDescription()
        {
            return Directory.GetString(OlympusFocusInfoMakernoteDirectory.TagSensorTemperature);
        }

        /// <remarks>
        /// <para> if the first 4 bytes are non-zero, then bit 0x01 of byte 44 gives the stabilization mode</para>
        /// <notes>(the other value is more reliable, so ignore this totally if the other exists)</notes>
        /// </remarks>
        public string? GetImageStabilizationDescription()
        {
            var values = Directory.GetByteArray(OlympusFocusInfoMakernoteDirectory.TagImageStabilization);
            if (values == null)
                return null;

            if ((values[0] | values[1] | values[2] | values[3]) == 0x0)
                return "Off";
            return "On, " + ((values[43] & 1) > 0 ? "Mode 1" : "Mode 2");
        }
    }
}
