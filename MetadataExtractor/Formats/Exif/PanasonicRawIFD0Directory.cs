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

namespace MetadataExtractor.Formats.Exif
{
    /// <remarks>These tags are found in IFD0 of Panasonic/Leica RAW, RW2 and RWL images.</remarks>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class PanasonicRawIfd0Directory : Directory
    {
        public const int TagPanasonicRawVersion = 0x0001;
        public const int TagSensorWidth = 0x0002;
        public const int TagSensorHeight = 0x0003;
        public const int TagSensorTopBorder = 0x0004;
        public const int TagSensorLeftBorder = 0x0005;
        public const int TagSensorBottomBorder = 0x0006;
        public const int TagSensorRightBorder = 0x0007;

        public const int TagBlackLevel1 = 0x0008;
        public const int TagBlackLevel2 = 0x0009;
        public const int TagBlackLevel3 = 0x000a;
        public const int TagLinearityLimitRed = 0x000e;
        public const int TagLinearityLimitGreen = 0x000f;
        public const int TagLinearityLimitBlue = 0x0010;
        public const int TagRedBalance = 0x0011;
        public const int TagBlueBalance = 0x0012;
        public const int TagWbInfo = 0x0013;

        public const int TagIso = 0x0017;
        public const int TagHighIsoMultiplierRed = 0x0018;
        public const int TagHighIsoMultiplierGreen = 0x0019;
        public const int TagHighIsoMultiplierBlue = 0x001a;
        public const int TagBlackLevelRed = 0x001c;
        public const int TagBlackLevelGreen = 0x001d;
        public const int TagBlackLevelBlue = 0x001e;
        public const int TagWbRedLevel = 0x0024;
        public const int TagWbGreenLevel = 0x0025;
        public const int TagWbBlueLevel = 0x0026;

        public const int TagWbInfo2 = 0x0027;

        public const int TagJpgFromRaw = 0x002e;

        public const int TagCropTop = 0x002f;
        public const int TagCropLeft = 0x0030;
        public const int TagCropBottom = 0x0031;
        public const int TagCropRight = 0x0032;

        public const int TagMake = 0x010f;
        public const int TagModel = 0x0110;
        public const int TagStripOffsets = 0x0111;
        public const int TagOrientation = 0x0112;
        public const int TagRowsPerStrip = 0x0116;
        public const int TagStripByteCounts = 0x0117;
        public const int TagRawDataOffset = 0x0118;

        public const int TagDistortionInfo = 0x0119;


        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagPanasonicRawVersion, "Panasonic Raw Version" },
            { TagSensorWidth, "Sensor Width" },
            { TagSensorHeight, "Sensor Height" },
            { TagSensorTopBorder, "Sensor Top Border" },
            { TagSensorLeftBorder, "Sensor Left Border" },
            { TagSensorBottomBorder, "Sensor Bottom Border" },
            { TagSensorRightBorder, "Sensor Right Border" },

            { TagBlackLevel1, "Black Level 1" },
            { TagBlackLevel2, "Black Level 2" },
            { TagBlackLevel3, "Black Level 3" },
            { TagLinearityLimitRed, "Linearity Limit Red" },
            { TagLinearityLimitGreen, "Linearity Limit Green" },
            { TagLinearityLimitBlue, "Linearity Limit Blue" },
            { TagRedBalance, "Red Balance" },
            { TagBlueBalance, "Blue Balance" },

            { TagIso, "ISO" },
            { TagHighIsoMultiplierRed, "High ISO Multiplier Red" },
            { TagHighIsoMultiplierGreen, "High ISO Multiplier Green" },
            { TagHighIsoMultiplierBlue, "High ISO Multiplier Blue" },
            { TagBlackLevelRed, "Black Level Red" },
            { TagBlackLevelGreen, "Black Level Green" },
            { TagBlackLevelBlue, "Black Level Blue" },
            { TagWbRedLevel, "WB Red Level" },
            { TagWbGreenLevel, "WB Green Level" },
            { TagWbBlueLevel, "WB Blue Level" },

            { TagJpgFromRaw, "Jpg From Raw" },

            { TagCropTop, "Crop Top" },
            { TagCropLeft, "Crop Left" },
            { TagCropBottom, "Crop Bottom" },
            { TagCropRight, "Crop Right" },

            { TagMake, "Make" },
            { TagModel, "Model" },
            { TagStripOffsets, "Strip Offsets" },
            { TagOrientation, "Orientation" },
            { TagRowsPerStrip, "Rows Per Strip" },
            { TagStripByteCounts, "Strip Byte Counts" },
            { TagRawDataOffset, "Raw Data Offset" }
        };

        public PanasonicRawIfd0Directory()
        {
            SetDescriptor(new PanasonicRawIfd0Descriptor(this));
        }

        public override string Name => "PanasonicRaw Exif IFD0";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
