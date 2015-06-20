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

using System.Collections.Generic;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to Sigma / Foveon cameras.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class SigmaMakernoteDirectory : Directory
    {
        public const int TagSerialNumber = 0x2;

        public const int TagDriveMode = 0x3;

        public const int TagResolutionMode = 0x4;

        public const int TagAutoFocusMode = 0x5;

        public const int TagFocusSetting = 0x6;

        public const int TagWhiteBalance = 0x7;

        public const int TagExposureMode = 0x8;

        public const int TagMeteringMode = 0x9;

        public const int TagLensRange = 0xa;

        public const int TagColorSpace = 0xb;

        public const int TagExposure = 0xc;

        public const int TagContrast = 0xd;

        public const int TagShadow = 0xe;

        public const int TagHighlight = 0xf;

        public const int TagSaturation = 0x10;

        public const int TagSharpness = 0x11;

        public const int TagFillLight = 0x12;

        public const int TagColorAdjustment = 0x14;

        public const int TagAdjustmentMode = 0x15;

        public const int TagQuality = 0x16;

        public const int TagFirmware = 0x17;

        public const int TagSoftware = 0x18;

        public const int TagAutoBracket = 0x19;

        [NotNull]
        protected static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static SigmaMakernoteDirectory()
        {
            TagNameMap[TagSerialNumber] = "Serial Number";
            TagNameMap[TagDriveMode] = "Drive Mode";
            TagNameMap[TagResolutionMode] = "Resolution Mode";
            TagNameMap[TagAutoFocusMode] = "Auto Focus Mode";
            TagNameMap[TagFocusSetting] = "Focus Setting";
            TagNameMap[TagWhiteBalance] = "White Balance";
            TagNameMap[TagExposureMode] = "Exposure Mode";
            TagNameMap[TagMeteringMode] = "Metering Mode";
            TagNameMap[TagLensRange] = "Lens Range";
            TagNameMap[TagColorSpace] = "Color Space";
            TagNameMap[TagExposure] = "Exposure";
            TagNameMap[TagContrast] = "Contrast";
            TagNameMap[TagShadow] = "Shadow";
            TagNameMap[TagHighlight] = "Highlight";
            TagNameMap[TagSaturation] = "Saturation";
            TagNameMap[TagSharpness] = "Sharpness";
            TagNameMap[TagFillLight] = "Fill Light";
            TagNameMap[TagColorAdjustment] = "Color Adjustment";
            TagNameMap[TagAdjustmentMode] = "Adjustment Mode";
            TagNameMap[TagQuality] = "Quality";
            TagNameMap[TagFirmware] = "Firmware";
            TagNameMap[TagSoftware] = "Software";
            TagNameMap[TagAutoBracket] = "Auto Bracket";
        }

        public SigmaMakernoteDirectory()
        {
            SetDescriptor(new SigmaMakernoteDescriptor(this));
        }

        public override string Name
        {
            get { return "Sigma Makernote"; }
        }

        protected override IReadOnlyDictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
