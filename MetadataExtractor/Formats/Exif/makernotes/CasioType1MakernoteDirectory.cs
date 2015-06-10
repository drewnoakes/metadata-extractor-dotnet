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

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to Casio (type 1) cameras.</summary>
    /// <remarks>
    /// Describes tags specific to Casio (type 1) cameras.
    /// A standard TIFF IFD directory but always uses Motorola (Big-Endian) Byte Alignment.
    /// Makernote data begins immediately (no header).
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class CasioType1MakernoteDirectory : Directory
    {
        public const int TagRecordingMode = unchecked(0x0001);

        public const int TagQuality = unchecked(0x0002);

        public const int TagFocusingMode = unchecked(0x0003);

        public const int TagFlashMode = unchecked(0x0004);

        public const int TagFlashIntensity = unchecked(0x0005);

        public const int TagObjectDistance = unchecked(0x0006);

        public const int TagWhiteBalance = unchecked(0x0007);

        public const int TagUnknown1 = unchecked(0x0008);

        public const int TagUnknown2 = unchecked(0x0009);

        public const int TagDigitalZoom = unchecked(0x000A);

        public const int TagSharpness = unchecked(0x000B);

        public const int TagContrast = unchecked(0x000C);

        public const int TagSaturation = unchecked(0x000D);

        public const int TagUnknown3 = unchecked(0x000E);

        public const int TagUnknown4 = unchecked(0x000F);

        public const int TagUnknown5 = unchecked(0x0010);

        public const int TagUnknown6 = unchecked(0x0011);

        public const int TagUnknown7 = unchecked(0x0012);

        public const int TagUnknown8 = unchecked(0x0013);

        public const int TagCcdSensitivity = unchecked(0x0014);

        [NotNull]
        protected static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static CasioType1MakernoteDirectory()
        {
            TagNameMap[TagCcdSensitivity] = "CCD Sensitivity";
            TagNameMap[TagContrast] = "Contrast";
            TagNameMap[TagDigitalZoom] = "Digital Zoom";
            TagNameMap[TagFlashIntensity] = "Flash Intensity";
            TagNameMap[TagFlashMode] = "Flash Mode";
            TagNameMap[TagFocusingMode] = "Focusing Mode";
            TagNameMap[TagObjectDistance] = "Object Distance";
            TagNameMap[TagQuality] = "Quality";
            TagNameMap[TagRecordingMode] = "Recording Mode";
            TagNameMap[TagSaturation] = "Saturation";
            TagNameMap[TagSharpness] = "Sharpness";
            TagNameMap[TagUnknown1] = "Makernote Unknown 1";
            TagNameMap[TagUnknown2] = "Makernote Unknown 2";
            TagNameMap[TagUnknown3] = "Makernote Unknown 3";
            TagNameMap[TagUnknown4] = "Makernote Unknown 4";
            TagNameMap[TagUnknown5] = "Makernote Unknown 5";
            TagNameMap[TagUnknown6] = "Makernote Unknown 6";
            TagNameMap[TagUnknown7] = "Makernote Unknown 7";
            TagNameMap[TagUnknown8] = "Makernote Unknown 8";
            TagNameMap[TagWhiteBalance] = "White Balance";
        }

        public CasioType1MakernoteDirectory()
        {
            SetDescriptor(new CasioType1MakernoteDescriptor(this));
        }

        public override string Name
        {
            get { return "Casio Makernote"; }
        }

        protected override IReadOnlyDictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
