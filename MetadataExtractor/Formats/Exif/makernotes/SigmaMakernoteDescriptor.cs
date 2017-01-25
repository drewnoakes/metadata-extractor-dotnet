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
    /// Provides human-readable string representations of tag values stored in a <see cref="SigmaMakernoteDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class SigmaMakernoteDescriptor : TagDescriptor<SigmaMakernoteDirectory>
    {
        public SigmaMakernoteDescriptor([NotNull] SigmaMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case SigmaMakernoteDirectory.TagExposureMode:
                    return GetExposureModeDescription();
                case SigmaMakernoteDirectory.TagMeteringMode:
                    return GetMeteringModeDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        private string GetMeteringModeDescription()
        {
            var value = Directory.GetString(SigmaMakernoteDirectory.TagMeteringMode);
            if (string.IsNullOrEmpty(value))
                return null;

            switch (value[0])
            {
                case '8':
                    return "Multi Segment";
                case 'A':
                    return "Average";
                case 'C':
                    return "Center Weighted Average";
                default:
                    return value;
            }
        }

        [CanBeNull]
        private string GetExposureModeDescription()
        {
            var value = Directory.GetString(SigmaMakernoteDirectory.TagExposureMode);
            if (string.IsNullOrEmpty(value))
                return null;

            switch (value[0])
            {
                case 'A':
                    return "Aperture Priority AE";
                case 'M':
                    return "Manual";
                case 'P':
                    return "Program AE";
                case 'S':
                    return "Shutter Speed Priority AE";
                default:
                    return value;
            }
        }
    }
}
