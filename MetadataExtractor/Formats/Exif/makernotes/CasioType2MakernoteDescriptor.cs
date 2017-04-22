#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="CasioType2MakernoteDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class CasioType2MakernoteDescriptor : TagDescriptor<CasioType2MakernoteDirectory>
    {
        public CasioType2MakernoteDescriptor([NotNull] CasioType2MakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case CasioType2MakernoteDirectory.TagThumbnailDimensions:
                    return GetThumbnailDimensionsDescription();
                case CasioType2MakernoteDirectory.TagThumbnailSize:
                    return GetThumbnailSizeDescription();
                case CasioType2MakernoteDirectory.TagThumbnailOffset:
                    return GetThumbnailOffsetDescription();
                case CasioType2MakernoteDirectory.TagQualityMode:
                    return GetQualityModeDescription();
                case CasioType2MakernoteDirectory.TagImageSize:
                    return GetImageSizeDescription();
                case CasioType2MakernoteDirectory.TagFocusMode1:
                    return GetFocusMode1Description();
                case CasioType2MakernoteDirectory.TagIsoSensitivity:
                    return GetIsoSensitivityDescription();
                case CasioType2MakernoteDirectory.TagWhiteBalance1:
                    return GetWhiteBalance1Description();
                case CasioType2MakernoteDirectory.TagFocalLength:
                    return GetFocalLengthDescription();
                case CasioType2MakernoteDirectory.TagSaturation:
                    return GetSaturationDescription();
                case CasioType2MakernoteDirectory.TagContrast:
                    return GetContrastDescription();
                case CasioType2MakernoteDirectory.TagSharpness:
                    return GetSharpnessDescription();
                case CasioType2MakernoteDirectory.TagPreviewThumbnail:
                    return GetCasioPreviewThumbnailDescription();
                case CasioType2MakernoteDirectory.TagWhiteBalanceBias:
                    return GetWhiteBalanceBiasDescription();
                case CasioType2MakernoteDirectory.TagWhiteBalance2:
                    return GetWhiteBalance2Description();
                case CasioType2MakernoteDirectory.TagObjectDistance:
                    return GetObjectDistanceDescription();
                case CasioType2MakernoteDirectory.TagFlashDistance:
                    return GetFlashDistanceDescription();
                case CasioType2MakernoteDirectory.TagRecordMode:
                    return GetRecordModeDescription();
                case CasioType2MakernoteDirectory.TagSelfTimer:
                    return GetSelfTimerDescription();
                case CasioType2MakernoteDirectory.TagQuality:
                    return GetQualityDescription();
                case CasioType2MakernoteDirectory.TagFocusMode2:
                    return GetFocusMode2Description();
                case CasioType2MakernoteDirectory.TagTimeZone:
                    return GetTimeZoneDescription();
                case CasioType2MakernoteDirectory.TagCcdIsoSensitivity:
                    return GetCcdIsoSensitivityDescription();
                case CasioType2MakernoteDirectory.TagColourMode:
                    return GetColourModeDescription();
                case CasioType2MakernoteDirectory.TagEnhancement:
                    return GetEnhancementDescription();
                case CasioType2MakernoteDirectory.TagFilter:
                    return GetFilterDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetFilterDescription()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagFilter, "Off");
        }

        [CanBeNull]
        public string GetEnhancementDescription()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagEnhancement, "Off");
        }

        [CanBeNull]
        public string GetColourModeDescription()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagColourMode, "Off");
        }

        [CanBeNull]
        public string GetCcdIsoSensitivityDescription()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagCcdIsoSensitivity, "Off", "On");
        }

        [CanBeNull]
        public string GetTimeZoneDescription()
        {
            return Directory.GetString(CasioType2MakernoteDirectory.TagTimeZone);
        }

        [CanBeNull]
        public string GetFocusMode2Description()
        {
            if (!Directory.TryGetInt32(CasioType2MakernoteDirectory.TagFocusMode2, out int value))
                return null;

            switch (value)
            {
                case 1:  return "Fixation";
                case 6:  return "Multi-Area Focus";
                default: return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetQualityDescription()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagQuality, 3, "Fine");
        }

        [CanBeNull]
        public string GetSelfTimerDescription()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagSelfTimer, 1, "Off");
        }

        [CanBeNull]
        public string GetRecordModeDescription()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagRecordMode, 2, "Normal");
        }

        [CanBeNull]
        public string GetFlashDistanceDescription()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagFlashDistance, "Off");
        }

        [CanBeNull]
        public string GetObjectDistanceDescription()
        {
            if (!Directory.TryGetInt32(CasioType2MakernoteDirectory.TagObjectDistance, out int value))
                return null;
            return value + " mm";
        }

        [CanBeNull]
        public string GetWhiteBalance2Description()
        {
            if (!Directory.TryGetInt32(CasioType2MakernoteDirectory.TagWhiteBalance2, out int value))
                return null;
            switch (value)
            {
                case 0:
                    return "Manual";
                case 1:
                    return "Auto";
                case 4:
                    // unsure about this
                    return "Flash";
                case 12:
                    // unsure about this
                    return "Flash";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetWhiteBalanceBiasDescription()
        {
            return Directory.GetString(CasioType2MakernoteDirectory.TagWhiteBalanceBias);
        }

        [CanBeNull]
        public string GetCasioPreviewThumbnailDescription()
        {
            var bytes = Directory.GetByteArray(CasioType2MakernoteDirectory.TagPreviewThumbnail);
            if (bytes == null)
                return null;

            return "<" + bytes.Length + " bytes of image data>";
        }

        [CanBeNull]
        public string GetSharpnessDescription()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagSharpness, "-1", "Normal", "+1");
        }

        [CanBeNull]
        public string GetContrastDescription()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagContrast, "-1", "Normal", "+1");
        }

        [CanBeNull]
        public string GetSaturationDescription()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagSaturation, "-1", "Normal", "+1");
        }

        [CanBeNull]
        public string GetFocalLengthDescription()
        {
            if (!Directory.TryGetDouble(CasioType2MakernoteDirectory.TagFocalLength, out double value))
                return null;
            return GetFocalLengthDescription(value/10d);
        }

        [CanBeNull]
        public string GetWhiteBalance1Description()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagWhiteBalance1, "Auto", "Daylight", "Shade", "Tungsten", "Florescent", "Manual");
        }

        [CanBeNull]
        public string GetIsoSensitivityDescription()
        {
            if (!Directory.TryGetInt32(CasioType2MakernoteDirectory.TagIsoSensitivity, out int value))
                return null;

            switch (value)
            {
                case 3:
                    return "50";
                case 4:
                    return "64";
                case 6:
                    return "100";
                case 9:
                    return "200";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetFocusMode1Description()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagFocusMode1, "Normal", "Macro");
        }

        [CanBeNull]
        public string GetImageSizeDescription()
        {
            if (!Directory.TryGetInt32(CasioType2MakernoteDirectory.TagImageSize, out int value))
                return null;

            switch (value)
            {
                case 0:  return "640 x 480 pixels";
                case 4:  return "1600 x 1200 pixels";
                case 5:  return "2048 x 1536 pixels";
                case 20: return "2288 x 1712 pixels";
                case 21: return "2592 x 1944 pixels";
                case 22: return "2304 x 1728 pixels";
                case 36: return "3008 x 2008 pixels";
                default: return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetQualityModeDescription()
        {
            return GetIndexedDescription(CasioType2MakernoteDirectory.TagQualityMode, 1, "Fine", "Super Fine");
        }

        [CanBeNull]
        public string GetThumbnailOffsetDescription()
        {
            return Directory.GetString(CasioType2MakernoteDirectory.TagThumbnailOffset);
        }

        [CanBeNull]
        public string GetThumbnailSizeDescription()
        {
            if (!Directory.TryGetInt32(CasioType2MakernoteDirectory.TagThumbnailSize, out int value))
                return null;

            return value + " bytes";
        }

        [CanBeNull]
        public string GetThumbnailDimensionsDescription()
        {
            var dimensions = Directory.GetInt32Array(CasioType2MakernoteDirectory.TagThumbnailDimensions);
            if (dimensions == null || dimensions.Length != 2)
                return Directory.GetString(CasioType2MakernoteDirectory.TagThumbnailDimensions);
            return dimensions[0] + " x " + dimensions[1] + " pixels";
        }
    }
}
