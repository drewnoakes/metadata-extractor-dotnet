#region License
//
// Copyright 2002-2015 Drew Noakes
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

using System;
using JetBrains.Annotations;
using MetadataExtractor.Util;

namespace MetadataExtractor.Formats.Xmp
{
    /// <summary>Contains logic for the presentation of data stored in an <see cref="XmpDirectory"/>.</summary>
    /// <author>Torsten Skadell, Drew Noakes https://drewnoakes.com</author>
    public sealed class XmpDescriptor : TagDescriptor<XmpDirectory>
    {
        public XmpDescriptor([NotNull] XmpDirectory directory)
            : base(directory)
        {
        }

        // TODO some of these methods look similar to those found in Exif*Descriptor... extract common functionality from both

        /// <summary>Do some simple formatting, dependent upon <paramref name="tagType"/>.</summary>
        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case XmpDirectory.TagMake:
                case XmpDirectory.TagModel:
                    return Directory.GetString(tagType);
                case XmpDirectory.TagExposureTime:
                    return GetExposureTimeDescription();
                case XmpDirectory.TagExposureProgram:
                    return GetExposureProgramDescription();
                case XmpDirectory.TagShutterSpeed:
                    return GetShutterSpeedDescription();
                case XmpDirectory.TagFNumber:
                    return GetFNumberDescription();
                case XmpDirectory.TagLens:
                case XmpDirectory.TagLensInfo:
                case XmpDirectory.TagCameraSerialNumber:
                case XmpDirectory.TagFirmware:
                    return Directory.GetString(tagType);
                case XmpDirectory.TagFocalLength:
                    return GetFocalLengthDescription();
                case XmpDirectory.TagApertureValue:
                    return GetApertureValueDescription();
                case XmpDirectory.TagRating:
                    return GetRatingDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetExposureTimeDescription()
        {
            var value = Directory.GetString(XmpDirectory.TagExposureTime);
            if (value == null)
                return null;
            return value + " sec";
        }

        [CanBeNull]
        public string GetExposureProgramDescription()
        {
            int value;
            if (!Directory.TryGetInt32(XmpDirectory.TagExposureProgram, out value))
                return null;

            return GetIndexedDescription(XmpDirectory.TagExposureProgram, 1,
                "Manual control",
                "Program normal",
                "Aperture priority",
                "Shutter priority",
                "Program creative (slow program)",
                "Program action (high-speed program)",
                "Portrait mode",
                "Landscape mode");
        }

        [CanBeNull]
        public string GetShutterSpeedDescription()
        {
            var value = Directory.GetSingleNullable(XmpDirectory.TagShutterSpeed);
            if (value == null)
                return null;

            // thanks to Mark Edwards for spotting and patching a bug in the calculation of this
            // description (spotted bug using a Canon EOS 300D)
            // thanks also to Gli Blr for spotting this bug
            if (value <= 1)
            {
                var apexPower = (float)(1 / (Math.Exp((double)value * Math.Log(2))));
                var apexPower10 = (long)Math.Round(apexPower * 10.0);
                var fApexPower = apexPower10 / 10.0f;
                return fApexPower + " sec";
            }
            else
            {
                var apexPower = (int)((Math.Exp((double)value * Math.Log(2))));
                return "1/" + apexPower + " sec";
            }
        }

        [CanBeNull]
        public string GetFNumberDescription()
        {
            var value = Directory.GetRational(XmpDirectory.TagFNumber);
            if (value == null)
                return null;
            return GetFStopDescription(value.ToDouble());
        }

        [CanBeNull]
        public string GetFocalLengthDescription()
        {
            var value = Directory.GetRational(XmpDirectory.TagFocalLength);
            return value == null ? null : string.Format("{0:0.0##} mm", value.ToDouble());
        }

        [CanBeNull]
        public string GetApertureValueDescription()
        {
            var value = Directory.GetDoubleNullable(XmpDirectory.TagApertureValue);
            return value == null ? null : GetFStopDescription(PhotographicConversions.ApertureToFStop((double)value));
        }

        [CanBeNull]
        public string GetRatingDescription()
        {
            var value = Directory.GetDoubleNullable(XmpDirectory.TagRating);
            return value == null
                ? base.GetDescription(XmpDirectory.TagRating)
                : value.Value.ToString("0.0");
        }
    }
}
