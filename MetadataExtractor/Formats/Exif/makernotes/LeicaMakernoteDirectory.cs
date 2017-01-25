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
    /// Tag reference from: http://gvsoft.homedns.org/exif/makernote-leica-type1.html
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class LeicaMakernoteDirectory : Directory
    {
        public const int TagQuality = 0x0300;
        public const int TagUserProfile = 0x0302;
        public const int TagSerialNumber = 0x0303;
        public const int TagWhiteBalance = 0x0304;
        public const int TagLensType = 0x0310;
        public const int TagExternalSensorBrightnessValue = 0x0311;
        public const int TagMeasuredLV = 0x0312;
        public const int TagApproximateFNumber = 0x0313;
        public const int TagCameraTemperature = 0x0320;
        public const int TagColorTemperature = 0x0321;
        public const int TagWBRedLevel = 0x0322;
        public const int TagWBGreenLevel = 0x0323;
        public const int TagWBBlueLevel = 0x0324;
        public const int TagCcdVersion = 0x0330;
        public const int TagCcdBoardVersion = 0x0331;
        public const int TagControllerBoardVersion = 0x0332;
        public const int TagM16CVersion = 0x0333;
        public const int TagImageIdNumber = 0x0340;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagQuality, "Quality" },
            { TagUserProfile, "User Profile" },
            { TagSerialNumber, "Serial Number" },
            { TagWhiteBalance, "White Balance" },
            { TagLensType, "Lens Type" },
            { TagExternalSensorBrightnessValue, "External Sensor Brightness Value" },
            { TagMeasuredLV, "Measured LV" },
            { TagApproximateFNumber, "Approximate F Number" },
            { TagCameraTemperature, "Camera Temperature" },
            { TagColorTemperature, "Color Temperature" },
            { TagWBRedLevel, "WB Red Level" },
            { TagWBGreenLevel, "WB Green Level" },
            { TagWBBlueLevel, "WB Blue Level" },
            { TagCcdVersion, "CCD Version" },
            { TagCcdBoardVersion, "CCD Board Version" },
            { TagControllerBoardVersion, "Controller Board Version" },
            { TagM16CVersion, "M16 C Version" },
            { TagImageIdNumber, "Image ID Number" }
        };

        public LeicaMakernoteDirectory()
        {
            SetDescriptor(new LeicaMakernoteDescriptor(this));
        }

        public override string Name => "Leica Makernote";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
