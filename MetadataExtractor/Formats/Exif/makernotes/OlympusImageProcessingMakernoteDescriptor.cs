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
    /// Provides human-readable string representations of tag values stored in a <see cref="OlympusImageProcessingMakernoteDirectory"/>.
    /// </summary>
    /// <remarks>
    /// Some Description functions converted from Exiftool version 10.33 created by Phil Harvey
    /// http://www.sno.phy.queensu.ca/~phil/exiftool/
    /// lib\Image\ExifTool\Olympus.pm
    /// </remarks>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class OlympusImageProcessingMakernoteDescriptor : TagDescriptor<OlympusImageProcessingMakernoteDirectory>
    {
        public OlympusImageProcessingMakernoteDescriptor([NotNull] OlympusImageProcessingMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case OlympusImageProcessingMakernoteDirectory.TagImageProcessingVersion:
                    return GetImageProcessingVersionDescription();
                case OlympusImageProcessingMakernoteDirectory.TagColorMatrix:
                    return GetColorMatrixDescription();
                case OlympusImageProcessingMakernoteDirectory.TagNoiseReduction2:
                    return GetNoiseReduction2Description();
                case OlympusImageProcessingMakernoteDirectory.TagDistortionCorrection2:
                    return GetDistortionCorrection2Description();
                case OlympusImageProcessingMakernoteDirectory.TagShadingCompensation2:
                    return GetShadingCompensation2Description();
                case OlympusImageProcessingMakernoteDirectory.TagMultipleExposureMode:
                    return GetMultipleExposureModeDescription();
                case OlympusImageProcessingMakernoteDirectory.TagAspectRatio:
                    return GetAspectRatioDescription();
                case OlympusImageProcessingMakernoteDirectory.TagKeystoneCompensation:
                    return GetKeystoneCompensationDescription();
                case OlympusImageProcessingMakernoteDirectory.TagKeystoneDirection:
                    return GetKeystoneDirectionDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetImageProcessingVersionDescription()
        {
            return GetVersionBytesDescription(OlympusImageProcessingMakernoteDirectory.TagImageProcessingVersion, 4);
        }

        [CanBeNull]
        public string GetColorMatrixDescription()
        {
            var values = Directory.GetObject(OlympusImageProcessingMakernoteDirectory.TagColorMatrix) as short[];
            if (values == null)
                return null;

            var str = new StringBuilder();
            for (var i = 0; i < values.Length; i++)
            {
                if (i != 0)
                    str.Append(' ');
                str.Append(values[i]);
            }

            return str.ToString();
        }

        [CanBeNull]
        public string GetNoiseReduction2Description()
        {
            if (!Directory.TryGetInt32(OlympusImageProcessingMakernoteDirectory.TagNoiseReduction2, out int value))
                return null;

            if (value == 0)
                return "(none)";

            var sb = new StringBuilder();
            var v = (ushort)value;

            if (( v       & 1) != 0) sb.Append("Noise Reduction, ");
            if (((v >> 1) & 1) != 0) sb.Append("Noise Filter, ");
            if (((v >> 2) & 1) != 0) sb.Append("Noise Filter (ISO Boost), ");

            return sb.ToString(0, sb.Length - 2);
        }

        [CanBeNull]
        public string GetDistortionCorrection2Description()
        {
            return GetIndexedDescription(OlympusImageProcessingMakernoteDirectory.TagDistortionCorrection2,
                "Off", "On");
        }

        [CanBeNull]
        public string GetShadingCompensation2Description()
        {
            return GetIndexedDescription(OlympusImageProcessingMakernoteDirectory.TagShadingCompensation2,
                "Off", "On");
        }

        [CanBeNull]
        public string GetMultipleExposureModeDescription()
        {
            var values = Directory.GetObject(OlympusImageProcessingMakernoteDirectory.TagMultipleExposureMode) as ushort[];
            if (values == null)
            {
                // check if it's only one value long also
                if (!Directory.TryGetInt32(OlympusImageProcessingMakernoteDirectory.TagMultipleExposureMode, out int value))
                    return null;

                values = new ushort[1];
                values[0] = (ushort)value;
            }

            if (values.Length == 0)
                return null;

            var sb = new StringBuilder();

            switch (values[0])
            {
                case 0:
                    sb.Append("Off");
                    break;
                case 2:
                    sb.Append("On (2 frames)");
                    break;
                case 3:
                    sb.Append("On (3 frames)");
                    break;
                default:
                    sb.Append("Unknown (" + values[0] + ")");
                    break;
            }

            if (values.Length > 1)
                sb.Append("; " + values[1]);

            return sb.ToString();
        }

        [CanBeNull]
        public string GetAspectRatioDescription()
        {
            var values = Directory.GetObject(OlympusImageProcessingMakernoteDirectory.TagAspectRatio) as byte[];
            if (values == null || values.Length < 2)
                return null;

            var join = $"{values[0]} {values[1]}";

            string ret;
            switch (join)
            {
                case "1 1":
                    ret = "4:3";
                    break;
                case "1 4":
                    ret = "1:1";
                    break;
                case "2 1":
                    ret = "3:2 (RAW)";
                    break;
                case "2 2":
                    ret = "3:2";
                    break;
                case "3 1":
                    ret = "16:9 (RAW)";
                    break;
                case "3 3":
                    ret = "16:9";
                    break;
                case "4 1":
                    ret = "1:1 (RAW)";
                    break;
                case "4 4":
                    ret = "6:6";
                    break;
                case "5 5":
                    ret = "5:4";
                    break;
                case "6 6":
                    ret = "7:6";
                    break;
                case "7 7":
                    ret = "6:5";
                    break;
                case "8 8":
                    ret = "7:5";
                    break;
                case "9 1":
                    ret = "3:4 (RAW)";
                    break;
                case "9 9":
                    ret = "3:4";
                    break;
                default:
                    ret = "Unknown (" + join + ")";
                    break;
            }

            return ret;
        }

        [CanBeNull]
        public string GetKeystoneCompensationDescription()
        {
            var values = Directory.GetObject(OlympusImageProcessingMakernoteDirectory.TagKeystoneCompensation) as byte[];
            if (values == null || values.Length < 2)
                return null;

            var join = $"{values[0]} {values[1]}";

            string ret;
            switch (join)
            {
                case "0 0":
                    ret = "Off";
                    break;
                case "0 1":
                    ret = "On";
                    break;
                default:
                    ret = "Unknown (" + join + ")";
                    break;
            }

            return ret;
        }

        [CanBeNull]
        public string GetKeystoneDirectionDescription()
        {
            return GetIndexedDescription(OlympusImageProcessingMakernoteDirectory.TagKeystoneDirection,
                "Vertical", "Horizontal");
        }
    }
}
