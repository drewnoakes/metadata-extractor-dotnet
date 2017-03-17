#region License
//
// Copyright 2002-2017 Drew Noakes
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

using System.Text;
using JetBrains.Annotations;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="OlympusEquipmentMakernoteDirectory"/>.
    /// </summary>
    /// <remarks>
    /// Some Description functions converted from Exiftool version 10.10 created by Phil Harvey
    /// http://www.sno.phy.queensu.ca/~phil/exiftool/
    /// lib\Image\ExifTool\Olympus.pm
    /// </remarks>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class OlympusRawDevelopmentMakernoteDescriptor : TagDescriptor<OlympusRawDevelopmentMakernoteDirectory>
    {
        public OlympusRawDevelopmentMakernoteDescriptor([NotNull] OlympusRawDevelopmentMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case OlympusRawDevelopmentMakernoteDirectory.TagRawDevVersion:
                    return GetRawDevVersionDescription();
                case OlympusRawDevelopmentMakernoteDirectory.TagRawDevColorSpace:
                    return GetRawDevColorSpaceDescription();
                case OlympusRawDevelopmentMakernoteDirectory.TagRawDevEngine:
                    return GetRawDevEngineDescription();
                case OlympusRawDevelopmentMakernoteDirectory.TagRawDevNoiseReduction:
                    return GetRawDevNoiseReductionDescription();
                case OlympusRawDevelopmentMakernoteDirectory.TagRawDevEditStatus:
                    return GetRawDevEditStatusDescription();
                case OlympusRawDevelopmentMakernoteDirectory.TagRawDevSettings:
                    return GetRawDevSettingsDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetRawDevVersionDescription()
        {
            return GetVersionBytesDescription(OlympusRawDevelopmentMakernoteDirectory.TagRawDevVersion, 4);
        }

        [CanBeNull]
        public string GetRawDevColorSpaceDescription()
        {
            return GetIndexedDescription(OlympusRawDevelopmentMakernoteDirectory.TagRawDevColorSpace,
                "sRGB", "Adobe RGB", "Pro Photo RGB");
        }

        [CanBeNull]
        public string GetRawDevEngineDescription()
        {
            return GetIndexedDescription(OlympusRawDevelopmentMakernoteDirectory.TagRawDevEngine,
                "High Speed", "High Function", "Advanced High Speed", "Advanced High Function");
        }

        [CanBeNull]
        public string GetRawDevNoiseReductionDescription()
        {
            if (!Directory.TryGetInt32(OlympusRawDevelopmentMakernoteDirectory.TagRawDevNoiseReduction, out int value))
                return null;

            if (value == 0)
                return "(none)";

            var sb = new StringBuilder();
            var v = (ushort)value;

            if ((v        & 1) != 0) sb.Append("Noise Reduction, ");
            if (((v >> 1) & 1) != 0) sb.Append("Noise Filter, ");
            if (((v >> 2) & 1) != 0) sb.Append("Noise Filter (ISO Boost), ");

            return sb.ToString(0, sb.Length - 2);
        }

        [CanBeNull]
        public string GetRawDevEditStatusDescription()
        {
            if (!Directory.TryGetInt32(OlympusRawDevelopmentMakernoteDirectory.TagRawDevEditStatus, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "Original";
                case 1:
                    return "Edited (Landscape)";
                case 6:
                case 8:
                    return "Edited (Portrait)";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetRawDevSettingsDescription()
        {
            if (!Directory.TryGetInt32(OlympusRawDevelopmentMakernoteDirectory.TagRawDevSettings, out int value))
                return null;

            if (value == 0)
                return "(none)";

            var sb = new StringBuilder();
            var v = (ushort)value;

            if ((v        & 1) != 0) sb.Append("WB Color Temp, ");
            if (((v >> 1) & 1) != 0) sb.Append("WB Gray Point, ");
            if (((v >> 2) & 1) != 0) sb.Append("Saturation, ");
            if (((v >> 3) & 1) != 0) sb.Append("Contrast, ");
            if (((v >> 4) & 1) != 0) sb.Append("Sharpness, ");
            if (((v >> 5) & 1) != 0) sb.Append("Color Space, ");
            if (((v >> 6) & 1) != 0) sb.Append("High Function, ");
            if (((v >> 7) & 1) != 0) sb.Append("Noise Reduction, ");

            return sb.ToString(0, sb.Length - 2);
        }

    }
}
