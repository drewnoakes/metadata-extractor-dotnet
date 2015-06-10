/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using System.Collections.Generic;
using JetBrains.Annotations;

namespace Com.Drew.Metadata.Exif.Makernotes
{
    /// <summary>Describes tags specific to Sigma / Foveon cameras.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class SigmaMakernoteDirectory : Directory
    {
        public const int TagSerialNumber = unchecked(0x2);

        public const int TagDriveMode = unchecked(0x3);

        public const int TagResolutionMode = unchecked(0x4);

        public const int TagAutoFocusMode = unchecked(0x5);

        public const int TagFocusSetting = unchecked(0x6);

        public const int TagWhiteBalance = unchecked(0x7);

        public const int TagExposureMode = unchecked(0x8);

        public const int TagMeteringMode = unchecked(0x9);

        public const int TagLensRange = unchecked(0xa);

        public const int TagColorSpace = unchecked(0xb);

        public const int TagExposure = unchecked(0xc);

        public const int TagContrast = unchecked(0xd);

        public const int TagShadow = unchecked(0xe);

        public const int TagHighlight = unchecked(0xf);

        public const int TagSaturation = unchecked(0x10);

        public const int TagSharpness = unchecked(0x11);

        public const int TagFillLight = unchecked(0x12);

        public const int TagColorAdjustment = unchecked(0x14);

        public const int TagAdjustmentMode = unchecked(0x15);

        public const int TagQuality = unchecked(0x16);

        public const int TagFirmware = unchecked(0x17);

        public const int TagSoftware = unchecked(0x18);

        public const int TagAutoBracket = unchecked(0x19);

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

        public override string GetName()
        {
            return "Sigma Makernote";
        }

        protected override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
