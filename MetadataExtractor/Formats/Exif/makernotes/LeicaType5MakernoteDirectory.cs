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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to certain Leica cameras.</summary>
    /// <remarks>
    /// Tag reference from: http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/Panasonic.html
    /// </remarks>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class LeicaType5MakernoteDirectory : Directory
    {
        public const int TagLensModel = 0x0303;
        public const int TagOriginalFileName = 0x0407;
        public const int TagOriginalDirectory = 0x0408;
        public const int TagExposureMode = 0x040d;
        public const int TagShotInfo = 0x0410;
        public const int TagFilmMode = 0x0412;
        public const int TagWbRgbLevels = 0x0413;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagLensModel, "Lens Model" },
            { TagOriginalFileName, "Original File Name" },
            { TagOriginalDirectory, "Original Directory" },
            { TagExposureMode, "Exposure Mode" },
            { TagShotInfo, "Shot Info" },
            { TagFilmMode, "Film Mode" },
            { TagWbRgbLevels, "WB RGB Levels" }
        };

        public LeicaType5MakernoteDirectory()
        {
            SetDescriptor(new LeicaType5MakernoteDescriptor(this));
        }

        public override string Name => "Leica Makernote";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
