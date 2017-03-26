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

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

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
        public OlympusFocusInfoMakernoteDescriptor([NotNull] OlympusFocusInfoMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case OlympusFocusInfoMakernoteDirectory.TagFocusInfoVersion:
                    return GetFocusInfoVersionDescription();
                case OlympusFocusInfoMakernoteDirectory.TagAutoFocus:
                    return GetAutoFocusDescription();
                case OlympusFocusInfoMakernoteDirectory.TagFocusDistance:
                    return GetFocusDistanceDescription();
                case OlympusFocusInfoMakernoteDirectory.TagAfPoint:
                    return GetAfPointDescription();
                case OlympusFocusInfoMakernoteDirectory.TagExternalFlash:
                    return GetExternalFlashDescription();
                case OlympusFocusInfoMakernoteDirectory.TagExternalFlashBounce:
                    return GetExternalFlashBounceDescription();
                case OlympusFocusInfoMakernoteDirectory.TagExternalFlashZoom:
                    return GetExternalFlashZoomDescription();
                case OlympusFocusInfoMakernoteDirectory.TagManualFlash:
                    return GetManualFlashDescription();
                case OlympusFocusInfoMakernoteDirectory.TagMacroLed:
                    return GetMacroLedDescription();
                case OlympusFocusInfoMakernoteDirectory.TagSensorTemperature:
                    return GetSensorTemperatureDescription();
                case OlympusFocusInfoMakernoteDirectory.TagImageStabilization:
                    return GetImageStabilizationDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetFocusInfoVersionDescription()
        {
            return GetVersionBytesDescription(OlympusFocusInfoMakernoteDirectory.TagFocusInfoVersion, 4);
        }

        [CanBeNull]
        public string GetAutoFocusDescription()
        {
            return GetIndexedDescription(OlympusFocusInfoMakernoteDirectory.TagAutoFocus,
                "Off", "On");
        }

        /// <remarks>
        /// this rational value looks like it is in mm when the denominator is
        /// 1 (E-1), and cm when denominator is 10 (E-300), so if we ignore the
        /// denominator we are consistently in mm - PH
        /// </remarks>
        [CanBeNull]
        public string GetFocusDistanceDescription()
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
        [CanBeNull]
        public string GetAfPointDescription()
        {
            if (!Directory.TryGetInt16(OlympusFocusInfoMakernoteDirectory.TagAfPoint, out short value))
                return null;

            return value.ToString();
        }

        [CanBeNull]
        public string GetExternalFlashDescription()
        {
            var values = Directory.GetObject(OlympusFocusInfoMakernoteDirectory.TagExternalFlash) as ushort[];
            if (values == null || values.Length < 2)
                return null;

            var join = $"{values[0]} {values[1]}";

            switch (join)
            {
                case "0 0":
                    return "Off";
                case "1 0":
                    return "On";
                default:
                    return "Unknown (" + join + ")";
            }
        }

        [CanBeNull]
        public string GetExternalFlashBounceDescription()
        {
            return GetIndexedDescription(OlympusFocusInfoMakernoteDirectory.TagExternalFlashBounce,
                "Bounce or Off", "Direct");
        }

        [CanBeNull]
        public string GetExternalFlashZoomDescription()
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

            switch (join)
            {
                case "0":
                    return "Off";
                case "1":
                    return "On";
                case "0 0":
                    return "Off";
                case "1 0":
                    return "On";
                default:
                    return "Unknown (" + join + ")";
            }
        }

        [CanBeNull]
        public string GetManualFlashDescription()
        {
            var values = Directory.GetObject(OlympusFocusInfoMakernoteDirectory.TagManualFlash) as short[];
            if (values == null)
                return null;

            if (values[0] == 0)
                return "Off";

            if (values[1] == 1)
                return "Full";
            return "On (1/" + values[1] + " strength)";
        }

        [CanBeNull]
        public string GetMacroLedDescription()
        {
            return GetIndexedDescription(OlympusFocusInfoMakernoteDirectory.TagMacroLed,
                "Off", "On");
        }

        /// <remarks>
        /// <para>TODO: Complete when Camera Model is available.</para>
        /// <para>There are differences in how to interpret this tag that can only be reconciled by knowing the model.</para>
        /// </remarks>
        [CanBeNull]
        public string GetSensorTemperatureDescription()
        {
            return Directory.GetString(OlympusFocusInfoMakernoteDirectory.TagSensorTemperature);
        }

        /// <remarks>
        /// <para> if the first 4 bytes are non-zero, then bit 0x01 of byte 44 gives the stabilization mode</para>
        /// <notes>(the other value is more reliable, so ignore this totally if the other exists)</notes>
        /// </remarks>
        [CanBeNull]
        public string GetImageStabilizationDescription()
        {
            var values = Directory.GetByteArray(OlympusFocusInfoMakernoteDirectory.TagImageStabilization);
            if (values == null)
                return null;

            if((values[0] | values[1] | values[2] | values[3]) == 0x0)
                return "Off";
            return "On, " + ((values[43] & 1) > 0 ? "Mode 1" : "Mode 2");
        }
    }
}
