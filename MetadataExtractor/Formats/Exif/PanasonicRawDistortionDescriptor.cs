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
        public PanasonicRawDistortionDescriptor([NotNull] PanasonicRawDistortionDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case PanasonicRawDistortionDirectory.TagDistortionParam02:
                    return GetDistortionParam02Description();
                case PanasonicRawDistortionDirectory.TagDistortionParam04:
                    return GetDistortionParam04Description();
                case PanasonicRawDistortionDirectory.TagDistortionScale:
                    return GetDistortionScaleDescription();
                case PanasonicRawDistortionDirectory.TagDistortionCorrection:
                    return GetDistortionCorrectionDescription();
                case PanasonicRawDistortionDirectory.TagDistortionParam08:
                    return GetDistortionParam08Description();
                case PanasonicRawDistortionDirectory.TagDistortionParam09:
                    return GetDistortionParam09Description();
                case PanasonicRawDistortionDirectory.TagDistortionParam11:
                    return GetDistortionParam11Description();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetDistortionParam02Description()
        {
            if (!Directory.TryGetInt16(PanasonicRawDistortionDirectory.TagDistortionParam02, out short value))
                return null;

            return new Rational(value, 32678).ToString();
            //return ((double)value / 32768.0d).ToString();
        }

        [CanBeNull]
        public string GetDistortionParam04Description()
        {
            if (!Directory.TryGetInt16(PanasonicRawDistortionDirectory.TagDistortionParam04, out short value))
                return null;

            return new Rational(value, 32678).ToString();
        }

        [CanBeNull]
        public string GetDistortionScaleDescription()
        {
            if (!Directory.TryGetInt16(PanasonicRawDistortionDirectory.TagDistortionScale, out short value))
                return null;

            return (1 / (1 + value / 32768)).ToString();
        }

        [CanBeNull]
        public string GetDistortionCorrectionDescription()
        {
            if (!Directory.TryGetInt32(PanasonicRawDistortionDirectory.TagDistortionCorrection, out int value))
                return null;

            // (have seen the upper 4 bits set for GF5 and GX1, giving a value of -4095 - PH)
            var mask = 0x000f;
            switch (value & mask)
            {
                case 0:
                    return "Off";
                case 1:
                    return "On";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        public string GetDistortionParam08Description()
        {
            if (!Directory.TryGetInt16(PanasonicRawDistortionDirectory.TagDistortionParam08, out short value))
                return null;

            return new Rational(value, 32678).ToString();
        }

        [CanBeNull]
        public string GetDistortionParam09Description()
        {
            if (!Directory.TryGetInt16(PanasonicRawDistortionDirectory.TagDistortionParam09, out short value))
                return null;

            return new Rational(value, 32678).ToString();
        }

        [CanBeNull]
        public string GetDistortionParam11Description()
        {
            if (!Directory.TryGetInt16(PanasonicRawDistortionDirectory.TagDistortionParam11, out short value))
                return null;

            return new Rational(value, 32678).ToString();
        }

    }
}
