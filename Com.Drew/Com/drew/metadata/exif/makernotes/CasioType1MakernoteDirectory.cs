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
    /// <summary>Describes tags specific to Casio (type 1) cameras.</summary>
    /// <remarks>
    /// Describes tags specific to Casio (type 1) cameras.
    /// A standard TIFF IFD directory but always uses Motorola (Big-Endian) Byte Alignment.
    /// Makernote data begins immediately (no header).
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class CasioType1MakernoteDirectory : Directory
    {
        public const int TagRecordingMode = unchecked((int)(0x0001));

        public const int TagQuality = unchecked((int)(0x0002));

        public const int TagFocusingMode = unchecked((int)(0x0003));

        public const int TagFlashMode = unchecked((int)(0x0004));

        public const int TagFlashIntensity = unchecked((int)(0x0005));

        public const int TagObjectDistance = unchecked((int)(0x0006));

        public const int TagWhiteBalance = unchecked((int)(0x0007));

        public const int TagUnknown1 = unchecked((int)(0x0008));

        public const int TagUnknown2 = unchecked((int)(0x0009));

        public const int TagDigitalZoom = unchecked((int)(0x000A));

        public const int TagSharpness = unchecked((int)(0x000B));

        public const int TagContrast = unchecked((int)(0x000C));

        public const int TagSaturation = unchecked((int)(0x000D));

        public const int TagUnknown3 = unchecked((int)(0x000E));

        public const int TagUnknown4 = unchecked((int)(0x000F));

        public const int TagUnknown5 = unchecked((int)(0x0010));

        public const int TagUnknown6 = unchecked((int)(0x0011));

        public const int TagUnknown7 = unchecked((int)(0x0012));

        public const int TagUnknown8 = unchecked((int)(0x0013));

        public const int TagCcdSensitivity = unchecked((int)(0x0014));

        [NotNull]
        protected internal static readonly Dictionary<int?, string> _tagNameMap = new Dictionary<int?, string>();

        static CasioType1MakernoteDirectory()
        {
            _tagNameMap.Put(TagCcdSensitivity, "CCD Sensitivity");
            _tagNameMap.Put(TagContrast, "Contrast");
            _tagNameMap.Put(TagDigitalZoom, "Digital Zoom");
            _tagNameMap.Put(TagFlashIntensity, "Flash Intensity");
            _tagNameMap.Put(TagFlashMode, "Flash Mode");
            _tagNameMap.Put(TagFocusingMode, "Focusing Mode");
            _tagNameMap.Put(TagObjectDistance, "Object Distance");
            _tagNameMap.Put(TagQuality, "Quality");
            _tagNameMap.Put(TagRecordingMode, "Recording Mode");
            _tagNameMap.Put(TagSaturation, "Saturation");
            _tagNameMap.Put(TagSharpness, "Sharpness");
            _tagNameMap.Put(TagUnknown1, "Makernote Unknown 1");
            _tagNameMap.Put(TagUnknown2, "Makernote Unknown 2");
            _tagNameMap.Put(TagUnknown3, "Makernote Unknown 3");
            _tagNameMap.Put(TagUnknown4, "Makernote Unknown 4");
            _tagNameMap.Put(TagUnknown5, "Makernote Unknown 5");
            _tagNameMap.Put(TagUnknown6, "Makernote Unknown 6");
            _tagNameMap.Put(TagUnknown7, "Makernote Unknown 7");
            _tagNameMap.Put(TagUnknown8, "Makernote Unknown 8");
            _tagNameMap.Put(TagWhiteBalance, "White Balance");
        }

        public CasioType1MakernoteDirectory()
        {
            this.SetDescriptor(new CasioType1MakernoteDescriptor(this));
        }

        [NotNull]
        public override string GetName()
        {
            return "Casio Makernote";
        }

        [NotNull]
        protected internal override Dictionary<int?, string> GetTagNameMap()
        {
            return _tagNameMap;
        }
    }
}
