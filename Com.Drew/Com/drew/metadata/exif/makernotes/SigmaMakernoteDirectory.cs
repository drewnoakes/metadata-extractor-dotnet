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
using Sharpen;

namespace Com.Drew.Metadata.Exif.Makernotes
{
    /// <summary>Describes tags specific to Sigma / Foveon cameras.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class SigmaMakernoteDirectory : Directory
    {
        public const int TagSerialNumber = unchecked((int)(0x2));

        public const int TagDriveMode = unchecked((int)(0x3));

        public const int TagResolutionMode = unchecked((int)(0x4));

        public const int TagAutoFocusMode = unchecked((int)(0x5));

        public const int TagFocusSetting = unchecked((int)(0x6));

        public const int TagWhiteBalance = unchecked((int)(0x7));

        public const int TagExposureMode = unchecked((int)(0x8));

        public const int TagMeteringMode = unchecked((int)(0x9));

        public const int TagLensRange = unchecked((int)(0xa));

        public const int TagColorSpace = unchecked((int)(0xb));

        public const int TagExposure = unchecked((int)(0xc));

        public const int TagContrast = unchecked((int)(0xd));

        public const int TagShadow = unchecked((int)(0xe));

        public const int TagHighlight = unchecked((int)(0xf));

        public const int TagSaturation = unchecked((int)(0x10));

        public const int TagSharpness = unchecked((int)(0x11));

        public const int TagFillLight = unchecked((int)(0x12));

        public const int TagColorAdjustment = unchecked((int)(0x14));

        public const int TagAdjustmentMode = unchecked((int)(0x15));

        public const int TagQuality = unchecked((int)(0x16));

        public const int TagFirmware = unchecked((int)(0x17));

        public const int TagSoftware = unchecked((int)(0x18));

        public const int TagAutoBracket = unchecked((int)(0x19));

        [NotNull]
        protected internal static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static SigmaMakernoteDirectory()
        {
            TagNameMap.Put(TagSerialNumber, "Serial Number");
            TagNameMap.Put(TagDriveMode, "Drive Mode");
            TagNameMap.Put(TagResolutionMode, "Resolution Mode");
            TagNameMap.Put(TagAutoFocusMode, "Auto Focus Mode");
            TagNameMap.Put(TagFocusSetting, "Focus Setting");
            TagNameMap.Put(TagWhiteBalance, "White Balance");
            TagNameMap.Put(TagExposureMode, "Exposure Mode");
            TagNameMap.Put(TagMeteringMode, "Metering Mode");
            TagNameMap.Put(TagLensRange, "Lens Range");
            TagNameMap.Put(TagColorSpace, "Color Space");
            TagNameMap.Put(TagExposure, "Exposure");
            TagNameMap.Put(TagContrast, "Contrast");
            TagNameMap.Put(TagShadow, "Shadow");
            TagNameMap.Put(TagHighlight, "Highlight");
            TagNameMap.Put(TagSaturation, "Saturation");
            TagNameMap.Put(TagSharpness, "Sharpness");
            TagNameMap.Put(TagFillLight, "Fill Light");
            TagNameMap.Put(TagColorAdjustment, "Color Adjustment");
            TagNameMap.Put(TagAdjustmentMode, "Adjustment Mode");
            TagNameMap.Put(TagQuality, "Quality");
            TagNameMap.Put(TagFirmware, "Firmware");
            TagNameMap.Put(TagSoftware, "Software");
            TagNameMap.Put(TagAutoBracket, "Auto Bracket");
        }

        public SigmaMakernoteDirectory()
        {
            this.SetDescriptor(new SigmaMakernoteDescriptor(this));
        }

        [NotNull]
        public override string GetName()
        {
            return "Sigma Makernote";
        }

        [NotNull]
        protected internal override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
