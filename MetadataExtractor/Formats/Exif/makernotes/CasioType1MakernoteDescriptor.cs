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
    /// Provides human-readable string representations of tag values stored in a <see cref="CasioType1MakernoteDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class CasioType1MakernoteDescriptor : TagDescriptor<CasioType1MakernoteDirectory>
    {
        public CasioType1MakernoteDescriptor([NotNull] CasioType1MakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case CasioType1MakernoteDirectory.TagRecordingMode:
                    return GetRecordingModeDescription();
                case CasioType1MakernoteDirectory.TagQuality:
                    return GetQualityDescription();
                case CasioType1MakernoteDirectory.TagFocusingMode:
                    return GetFocusingModeDescription();
                case CasioType1MakernoteDirectory.TagFlashMode:
                    return GetFlashModeDescription();
                case CasioType1MakernoteDirectory.TagFlashIntensity:
                    return GetFlashIntensityDescription();
                case CasioType1MakernoteDirectory.TagObjectDistance:
                    return GetObjectDistanceDescription();
                case CasioType1MakernoteDirectory.TagWhiteBalance:
                    return GetWhiteBalanceDescription();
                case CasioType1MakernoteDirectory.TagDigitalZoom:
                    return GetDigitalZoomDescription();
                case CasioType1MakernoteDirectory.TagSharpness:
                    return GetSharpnessDescription();
                case CasioType1MakernoteDirectory.TagContrast:
                    return GetContrastDescription();
                case CasioType1MakernoteDirectory.TagSaturation:
                    return GetSaturationDescription();
                case CasioType1MakernoteDirectory.TagCcdSensitivity:
                    return GetCcdSensitivityDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetCcdSensitivityDescription()
        {
            if (!Directory.TryGetInt32(CasioType1MakernoteDirectory.TagCcdSensitivity, out int value))
                return null;
            switch (value)
            {
                // these four for QV3000
                case 64:
                    return "Normal";
                case 125:
                    return "+1.0";
                case 250:
                    return "+2.0";
                case 244:
                    return "+3.0";

                // these two for QV8000/2000
                case 80:
                    return "Normal (ISO 80 equivalent)";
                case 100:
                    return "High";

                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetSaturationDescription()
        {
            return GetIndexedDescription(CasioType1MakernoteDirectory.TagSaturation,
                "Normal", "Low", "High");
        }

        [CanBeNull]
        public string GetContrastDescription()
        {
            return GetIndexedDescription(CasioType1MakernoteDirectory.TagContrast,
                "Normal", "Low", "High");
        }

        [CanBeNull]
        public string GetSharpnessDescription()
        {
            return GetIndexedDescription(CasioType1MakernoteDirectory.TagSharpness,
                "Normal", "Soft", "Hard");
        }

        [CanBeNull]
        public string GetDigitalZoomDescription()
        {
            if (!Directory.TryGetInt32(CasioType1MakernoteDirectory.TagDigitalZoom, out int value))
                return null;

            switch (value)
            {
                case 0x10000:
                    return "No digital zoom";
                case 0x10001:
                    return "2x digital zoom";
                case 0x20000:
                    return "2x digital zoom";
                case 0x40000:
                    return "4x digital zoom";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetWhiteBalanceDescription()
        {
            if (!Directory.TryGetInt32(CasioType1MakernoteDirectory.TagWhiteBalance, out int value))
                return null;

            switch (value)
            {
                case 1:
                    return "Auto";
                case 2:
                    return "Tungsten";
                case 3:
                    return "Daylight";
                case 4:
                    return "Florescent";
                case 5:
                    return "Shade";
                case 129:
                    return "Manual";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetObjectDistanceDescription()
        {
            if (!Directory.TryGetInt32(CasioType1MakernoteDirectory.TagObjectDistance, out int value))
                return null;
            return value + " mm";
        }

        [CanBeNull]
        public string GetFlashIntensityDescription()
        {
            if (!Directory.TryGetInt32(CasioType1MakernoteDirectory.TagFlashIntensity, out int value))
                return null;

            switch (value)
            {
                case 11:
                    return "Weak";
                case 13:
                    return "Normal";
                case 15:
                    return "Strong";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetFlashModeDescription()
        {
            return GetIndexedDescription(CasioType1MakernoteDirectory.TagFlashMode,
                1,
                "Auto", "On", "Off", "Red eye reduction");
        }

        [CanBeNull]
        public string GetFocusingModeDescription()
        {
            return GetIndexedDescription(CasioType1MakernoteDirectory.TagFocusingMode,
                2,
                "Macro", "Auto focus", "Manual focus", "Infinity");
        }

        [CanBeNull]
        public string GetQualityDescription()
        {
            return GetIndexedDescription(CasioType1MakernoteDirectory.TagQuality,
                1,
                "Economy", "Normal", "Fine");
        }

        [CanBeNull]
        public string GetRecordingModeDescription()
        {
            return GetIndexedDescription(CasioType1MakernoteDirectory.TagRecordingMode,
                1,
                "Single shutter", "Panorama", "Night scene", "Portrait", "Landscape");
        }
    }
}
