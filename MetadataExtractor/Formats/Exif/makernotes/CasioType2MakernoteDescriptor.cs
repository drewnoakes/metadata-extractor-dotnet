// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="CasioType2MakernoteDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class CasioType2MakernoteDescriptor : TagDescriptor<CasioType2MakernoteDirectory>
    {
        public CasioType2MakernoteDescriptor(CasioType2MakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                CasioType2MakernoteDirectory.TagThumbnailDimensions => GetThumbnailDimensionsDescription(),
                CasioType2MakernoteDirectory.TagThumbnailSize => GetThumbnailSizeDescription(),
                CasioType2MakernoteDirectory.TagThumbnailOffset => GetThumbnailOffsetDescription(),
                CasioType2MakernoteDirectory.TagQualityMode => GetQualityModeDescription(),
                CasioType2MakernoteDirectory.TagImageSize => GetImageSizeDescription(),
                CasioType2MakernoteDirectory.TagFocusMode1 => GetFocusMode1Description(),
                CasioType2MakernoteDirectory.TagIsoSensitivity => GetIsoSensitivityDescription(),
                CasioType2MakernoteDirectory.TagWhiteBalance1 => GetWhiteBalance1Description(),
                CasioType2MakernoteDirectory.TagFocalLength => GetFocalLengthDescription(),
                CasioType2MakernoteDirectory.TagSaturation => GetSaturationDescription(),
                CasioType2MakernoteDirectory.TagContrast => GetContrastDescription(),
                CasioType2MakernoteDirectory.TagSharpness => GetSharpnessDescription(),
                CasioType2MakernoteDirectory.TagPreviewThumbnail => GetCasioPreviewThumbnailDescription(),
                CasioType2MakernoteDirectory.TagWhiteBalanceBias => GetWhiteBalanceBiasDescription(),
                CasioType2MakernoteDirectory.TagWhiteBalance2 => GetWhiteBalance2Description(),
                CasioType2MakernoteDirectory.TagObjectDistance => GetObjectDistanceDescription(),
                CasioType2MakernoteDirectory.TagFlashDistance => GetFlashDistanceDescription(),
                CasioType2MakernoteDirectory.TagRecordMode => GetRecordModeDescription(),
                CasioType2MakernoteDirectory.TagSelfTimer => GetSelfTimerDescription(),
                CasioType2MakernoteDirectory.TagQuality => GetQualityDescription(),
                CasioType2MakernoteDirectory.TagFocusMode2 => GetFocusMode2Description(),
                CasioType2MakernoteDirectory.TagTimeZone => GetTimeZoneDescription(),
                CasioType2MakernoteDirectory.TagCcdIsoSensitivity => GetCcdIsoSensitivityDescription(),
                CasioType2MakernoteDirectory.TagColourMode => GetColourModeDescription(),
                CasioType2MakernoteDirectory.TagEnhancement => GetEnhancementDescription(),
                CasioType2MakernoteDirectory.TagFilter => GetFilterDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetFilterDescription()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagFilter, "Off");
        }

        public string? GetEnhancementDescription()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagEnhancement, "Off");
        }

        public string? GetColourModeDescription()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagColourMode, "Off");
        }

        public string? GetCcdIsoSensitivityDescription()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagCcdIsoSensitivity, "Off", "On");
        }

        public string? GetTimeZoneDescription()
        {
            return Directory.GetString(CasioType2MakernoteDirectory.TagTimeZone);
        }

        public string? GetFocusMode2Description()
        {
            if (!Directory.TryGetInt32(CasioType2MakernoteDirectory.TagFocusMode2, out int value))
                return null;

            return value switch
            {
                1 => "Fixation",
                6 => "Multi-Area Focus",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetQualityDescription()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagQuality, 3, "Fine");
        }

        public string? GetSelfTimerDescription()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagSelfTimer, 1, "Off");
        }

        public string? GetRecordModeDescription()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagRecordMode, 2, "Normal");
        }

        public string? GetFlashDistanceDescription()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagFlashDistance, "Off");
        }

        public string? GetObjectDistanceDescription()
        {
            if (!Directory.TryGetInt32(CasioType2MakernoteDirectory.TagObjectDistance, out int value))
                return null;
            return value + " mm";
        }

        public string? GetWhiteBalance2Description()
        {
            if (!Directory.TryGetInt32(CasioType2MakernoteDirectory.TagWhiteBalance2, out int value))
                return null;
            return value switch
            {
                0 => "Manual",
                1 => "Auto",
                4 => "Flash",
                12 => "Flash",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetWhiteBalanceBiasDescription()
        {
            return Directory.GetString(CasioType2MakernoteDirectory.TagWhiteBalanceBias);
        }

        public string? GetCasioPreviewThumbnailDescription()
        {
            var bytes = Directory.GetByteArray(CasioType2MakernoteDirectory.TagPreviewThumbnail);
            if (bytes == null)
                return null;

            return "<" + bytes.Length + " bytes of image data>";
        }

        public string? GetSharpnessDescription()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagSharpness, "-1", "Normal", "+1");
        }

        public string? GetContrastDescription()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagContrast, "-1", "Normal", "+1");
        }

        public string? GetSaturationDescription()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagSaturation, "-1", "Normal", "+1");
        }

        public string? GetFocalLengthDescription()
        {
            if (!Directory.TryGetDouble(CasioType2MakernoteDirectory.TagFocalLength, out double value))
                return null;
            return GetFocalLengthDescription(value/10d);
        }

        public string? GetWhiteBalance1Description()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagWhiteBalance1, "Auto", "Daylight", "Shade", "Tungsten", "Florescent", "Manual");
        }

        public string? GetIsoSensitivityDescription()
        {
            if (!Directory.TryGetInt32(CasioType2MakernoteDirectory.TagIsoSensitivity, out int value))
                return null;

            return value switch
            {
                3 => "50",
                4 => "64",
                6 => "100",
                9 => "200",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetFocusMode1Description()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagFocusMode1, "Normal", "Macro");
        }

        public string? GetImageSizeDescription()
        {
            if (!Directory.TryGetInt32(CasioType2MakernoteDirectory.TagImageSize, out int value))
                return null;

            return value switch
            {
                0 => "640 x 480 pixels",
                4 => "1600 x 1200 pixels",
                5 => "2048 x 1536 pixels",
                20 => "2288 x 1712 pixels",
                21 => "2592 x 1944 pixels",
                22 => "2304 x 1728 pixels",
                36 => "3008 x 2008 pixels",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetQualityModeDescription()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagQualityMode, 1, "Fine", "Super Fine");
        }

        public string? GetThumbnailOffsetDescription()
        {
            return Directory.GetString(CasioType2MakernoteDirectory.TagThumbnailOffset);
        }

        public string? GetThumbnailSizeDescription()
        {
            if (!Directory.TryGetInt32(CasioType2MakernoteDirectory.TagThumbnailSize, out int value))
                return null;

            return value + " bytes";
        }

        public string? GetThumbnailDimensionsDescription()
        {
            var dimensions = Directory.GetInt32Array(CasioType2MakernoteDirectory.TagThumbnailDimensions);
            if (dimensions == null || dimensions.Length != 2)
                return Directory.GetString(CasioType2MakernoteDirectory.TagThumbnailDimensions);
            return dimensions[0] + " x " + dimensions[1] + " pixels";
        }
    }
}
