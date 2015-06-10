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
    /// <summary>Describes tags specific to certain Leica cameras.</summary>
    /// <remarks>
    /// Describes tags specific to certain Leica cameras.
    /// <para />
    /// Tag reference from: http://gvsoft.homedns.org/exif/makernote-leica-type1.html
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class LeicaMakernoteDirectory : Directory
    {
        public const int TagQuality = unchecked(0x0300);

        public const int TagUserProfile = unchecked(0x0302);

        public const int TagSerialNumber = unchecked(0x0303);

        public const int TagWhiteBalance = unchecked(0x0304);

        public const int TagLensType = unchecked(0x0310);

        public const int TagExternalSensorBrightnessValue = unchecked(0x0311);

        public const int TagMeasuredLv = unchecked(0x0312);

        public const int TagApproximateFNumber = unchecked(0x0313);

        public const int TagCameraTemperature = unchecked(0x0320);

        public const int TagColorTemperature = unchecked(0x0321);

        public const int TagWbRedLevel = unchecked(0x0322);

        public const int TagWbGreenLevel = unchecked(0x0323);

        public const int TagWbBlueLevel = unchecked(0x0324);

        public const int TagCcdVersion = unchecked(0x0330);

        public const int TagCcdBoardVersion = unchecked(0x0331);

        public const int TagControllerBoardVersion = unchecked(0x0332);

        public const int TagM16CVersion = unchecked(0x0333);

        public const int TagImageIdNumber = unchecked(0x0340);

        [NotNull]
        protected static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static LeicaMakernoteDirectory()
        {
            TagNameMap[TagQuality] = "Quality";
            TagNameMap[TagUserProfile] = "User Profile";
            TagNameMap[TagSerialNumber] = "Serial Number";
            TagNameMap[TagWhiteBalance] = "White Balance";
            TagNameMap[TagLensType] = "Lens Type";
            TagNameMap[TagExternalSensorBrightnessValue] = "External Sensor Brightness Value";
            TagNameMap[TagMeasuredLv] = "Measured LV";
            TagNameMap[TagApproximateFNumber] = "Approximate F Number";
            TagNameMap[TagCameraTemperature] = "Camera Temperature";
            TagNameMap[TagColorTemperature] = "Color Temperature";
            TagNameMap[TagWbRedLevel] = "WB Red Level";
            TagNameMap[TagWbGreenLevel] = "WB Green Level";
            TagNameMap[TagWbBlueLevel] = "WB Blue Level";
            TagNameMap[TagCcdVersion] = "CCD Version";
            TagNameMap[TagCcdBoardVersion] = "CCD Board Version";
            TagNameMap[TagControllerBoardVersion] = "Controller Board Version";
            TagNameMap[TagM16CVersion] = "M16 C Version";
            TagNameMap[TagImageIdNumber] = "Image ID Number";
        }

        public LeicaMakernoteDirectory()
        {
            SetDescriptor(new LeicaMakernoteDescriptor(this));
        }

        public override string GetName()
        {
            return "Leica Makernote";
        }

        protected override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
