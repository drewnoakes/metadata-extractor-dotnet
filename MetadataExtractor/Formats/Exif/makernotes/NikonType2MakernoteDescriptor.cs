// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using MetadataExtractor.Util;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="NikonType2MakernoteDirectory"/>.
    /// Type-2 applies to the E990 and D-series cameras such as the D1, D70 and D100.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class NikonType2MakernoteDescriptor : TagDescriptor<NikonType2MakernoteDirectory>
    {
        public NikonType2MakernoteDescriptor(NikonType2MakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                NikonType2MakernoteDirectory.TagProgramShift => GetProgramShiftDescription(),
                NikonType2MakernoteDirectory.TagExposureDifference => GetExposureDifferenceDescription(),
                NikonType2MakernoteDirectory.TagLens => GetLensDescription(),
                NikonType2MakernoteDirectory.TagCameraHueAdjustment => GetHueAdjustmentDescription(),
                NikonType2MakernoteDirectory.TagCameraColorMode => GetColorModeDescription(),
                NikonType2MakernoteDirectory.TagAutoFlashCompensation => GetAutoFlashCompensationDescription(),
                NikonType2MakernoteDirectory.TagFlashExposureCompensation => GetFlashExposureCompensationDescription(),
                NikonType2MakernoteDirectory.TagFlashBracketCompensation => GetFlashBracketCompensationDescription(),
                NikonType2MakernoteDirectory.TagExposureTuning => GetExposureTuningDescription(),
                NikonType2MakernoteDirectory.TagLensStops => GetLensStopsDescription(),
                NikonType2MakernoteDirectory.TagColorSpace => GetColorSpaceDescription(),
                NikonType2MakernoteDirectory.TagActiveDLighting => GetActiveDLightingDescription(),
                NikonType2MakernoteDirectory.TagVignetteControl => GetVignetteControlDescription(),
                NikonType2MakernoteDirectory.TagIso1 => GetIsoSettingDescription(),
                NikonType2MakernoteDirectory.TagDigitalZoom => GetDigitalZoomDescription(),
                NikonType2MakernoteDirectory.TagFlashUsed => GetFlashUsedDescription(),
                NikonType2MakernoteDirectory.TagAfFocusPosition => GetAutoFocusPositionDescription(),
                NikonType2MakernoteDirectory.TagFirmwareVersion => GetFirmwareVersionDescription(),
                NikonType2MakernoteDirectory.TagLensType => GetLensTypeDescription(),
                NikonType2MakernoteDirectory.TagShootingMode => GetShootingModeDescription(),
                NikonType2MakernoteDirectory.TagNefCompression => GetNefCompressionDescription(),
                NikonType2MakernoteDirectory.TagHighIsoNoiseReduction => GetHighIsoNoiseReductionDescription(),
                NikonType2MakernoteDirectory.TagPowerUpTime => GetPowerUpTimeDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetPowerUpTimeDescription()
        {
            // this is generally a byte[] of length 8 directly representing a date and time.
            // the format is : first 2 bytes together are the year, and then each byte after
            //                 is month, day, hour, minute, second with the eighth byte unused
            // e.g., 2011:04:25 01:54:58

            var o = Directory.GetByteArray(NikonType2MakernoteDirectory.TagPowerUpTime);
            if (o == null)
                return null;

            ushort year = ByteConvert.FromBigEndianToNative(BitConverter.ToUInt16(o, 0));
            return string.Format($"{year}:{o[2]:D2}:{o[3]:D2} {o[4]:D2}:{o[5]:D2}:{o[6]:D2}");
        }

        public string? GetHighIsoNoiseReductionDescription()
        {
            return GetIndexedDescription(NikonType2MakernoteDirectory.TagHighIsoNoiseReduction,
                "Off", "Minimal", "Low", null, "Normal", null, "High");
        }

        public string? GetFlashUsedDescription()
        {
            return GetIndexedDescription(NikonType2MakernoteDirectory.TagFlashUsed,
                "Flash Not Used", "Manual Flash", null, "Flash Not Ready", null, null, null, "External Flash", "Fired, Commander Mode", "Fired, TTL Mode");
        }

        public string? GetNefCompressionDescription()
        {
            return GetIndexedDescription(NikonType2MakernoteDirectory.TagNefCompression,
                1,
                "Lossy (Type 1)", null, "Uncompressed", null, null, null, "Lossless", "Lossy (Type 2)");
        }

        public string? GetShootingModeDescription()
        {
            return GetBitFlagDescription(NikonType2MakernoteDirectory.TagShootingMode,
                new[] { "Single Frame", "Continuous" }, "Delay", null, "PC Control", "Exposure Bracketing", "Auto ISO", "White-Balance Bracketing", "IR Control");
        }

        // LSB [low label, high label]
        public string? GetLensTypeDescription()
        {
            return GetBitFlagDescription(NikonType2MakernoteDirectory.TagLensType,
                new[] { "AF", "MF" }, "D", "G", "VR");
        }

        // LSB [low label, high label]
        public string? GetColorSpaceDescription()
        {
            return GetIndexedDescription(NikonType2MakernoteDirectory.TagColorSpace,
                1,
                "sRGB", "Adobe RGB");
        }

        public string? GetActiveDLightingDescription()
        {
            if (!Directory.TryGetInt32(NikonType2MakernoteDirectory.TagActiveDLighting, out int value))
                return null;

            return value switch
            {
                0 => "Off",
                1 => "Light",
                3 => "Normal",
                5 => "High",
                7 => "Extra High",
                65535 => "Auto",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetVignetteControlDescription()
        {
            if (!Directory.TryGetInt32(NikonType2MakernoteDirectory.TagVignetteControl, out int value))
                return null;

            return value switch
            {
                0 => "Off",
                1 => "Low",
                3 => "Normal",
                5 => "High",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetAutoFocusPositionDescription()
        {
            var values = Directory.GetInt32Array(NikonType2MakernoteDirectory.TagAfFocusPosition);
            if (values == null)
                return null;

            if (values.Length != 4 || values[0] != 0 || values[2] != 0 || values[3] != 0)
                return "Unknown (" + Directory.GetString(NikonType2MakernoteDirectory.TagAfFocusPosition) + ")";

            return (values[1]) switch
            {
                0 => "Centre",
                1 => "Top",
                2 => "Bottom",
                3 => "Left",
                4 => "Right",
                _ => "Unknown (" + values[1] + ")",
            };
        }

        public string? GetDigitalZoomDescription()
        {
            if (!Directory.TryGetRational(NikonType2MakernoteDirectory.TagDigitalZoom, out Rational value))
                return null;

            return value.ToInt32() == 1
                ? "No digital zoom"
                : value.ToSimpleString() + "x digital zoom";
        }

        public string? GetProgramShiftDescription()
        {
            return GetEvDescription(NikonType2MakernoteDirectory.TagProgramShift);
        }

        public string? GetExposureDifferenceDescription()
        {
            return GetEvDescription(NikonType2MakernoteDirectory.TagExposureDifference);
        }

        public string? GetAutoFlashCompensationDescription()
        {
            return GetEvDescription(NikonType2MakernoteDirectory.TagAutoFlashCompensation);
        }

        public string? GetFlashExposureCompensationDescription()
        {
            return GetEvDescription(NikonType2MakernoteDirectory.TagFlashExposureCompensation);
        }

        public string? GetFlashBracketCompensationDescription()
        {
            return GetEvDescription(NikonType2MakernoteDirectory.TagFlashBracketCompensation);
        }

        public string? GetExposureTuningDescription()
        {
            return GetEvDescription(NikonType2MakernoteDirectory.TagExposureTuning);
        }

        public string? GetLensStopsDescription()
        {
            return GetEvDescription(NikonType2MakernoteDirectory.TagLensStops);
        }

        private string? GetEvDescription(int tagType)
        {
            var values = Directory.GetInt32Array(tagType);
            if (values == null || values.Length < 3 || values[2] == 0)
                return null;
            return $"{(sbyte)values[0]*(sbyte)values[1]/(double)(sbyte)values[2]:0.##} EV";
        }

        public string? GetIsoSettingDescription()
        {
            var values = Directory.GetInt32Array(NikonType2MakernoteDirectory.TagIso1);
            if (values == null || values.Length < 2)
                return null;
            if (values[0] != 0 || values[1] == 0)
                return "Unknown (" + Directory.GetString(NikonType2MakernoteDirectory.TagIso1) + ")";
            return "ISO " + values[1];
        }

        public string? GetLensDescription()
        {
            return GetLensSpecificationDescription(NikonType2MakernoteDirectory.TagLens);
        }

        public string? GetHueAdjustmentDescription()
        {
            return GetFormattedString(NikonType2MakernoteDirectory.TagCameraHueAdjustment, "{0} degrees");
        }

        public string? GetColorModeDescription()
        {
            var value = Directory.GetString(NikonType2MakernoteDirectory.TagCameraColorMode);
            return value == null ? null : value.StartsWith("MODE1") ? "Mode I (sRGB)" : value;
        }

        public string? GetFirmwareVersionDescription()
        {
            return GetVersionBytesDescription(NikonType2MakernoteDirectory.TagFirmwareVersion, 2);
        }
    }
}
