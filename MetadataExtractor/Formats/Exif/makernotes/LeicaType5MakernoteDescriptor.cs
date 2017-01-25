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
    /// Provides human-readable string representations of tag values stored in a <see cref="LeicaType5MakernoteDirectory"/>.
    /// <para />
    /// Tag reference from: http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/Panasonic.html
    /// </summary>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class LeicaType5MakernoteDescriptor : TagDescriptor<LeicaType5MakernoteDirectory>
    {
        public LeicaType5MakernoteDescriptor([NotNull] LeicaType5MakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case LeicaType5MakernoteDirectory.TagExposureMode:
                    return GetExposureModeDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        /// <summary>
        /// 4 values
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        public string GetExposureModeDescription()
        {
            var values = Directory.GetObject(LeicaType5MakernoteDirectory.TagExposureMode) as byte[];
            if (values == null || values.Length < 4)
                return null;

            var join = $"{values[0]} {values[1]} {values[2]} {values[3]}";

            string ret;
            switch (join)
            {
                case "0 0 0 0":
                    ret = "Program AE";
                    break;
                case "1 0 0 0":
                    ret = "Aperture-priority AE";
                    break;
                case "1 1 0 0":
                    ret = "Aperture-priority AE (1)";
                    break;
                case "2 0 0 0":
                    ret = "Shutter speed priority AE";  // guess
                    break;
                case "3 0 0 0":
                    ret = "Manual";
                    break;
                default:
                    ret = "Unknown (" + join + ")";
                    break;
            }

            return ret;
        }
    }
}
