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
    /// <summary>Describes tags specific to certain Leica cameras.</summary>
    /// <remarks>
    /// Describes tags specific to certain Leica cameras.
    /// <p>
    /// Tag reference from: http://gvsoft.homedns.org/exif/makernote-leica-type1.html
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class LeicaMakernoteDirectory : Directory
    {
        public const int TagQuality = unchecked((int)(0x0300));

        public const int TagUserProfile = unchecked((int)(0x0302));

        public const int TagSerialNumber = unchecked((int)(0x0303));

        public const int TagWhiteBalance = unchecked((int)(0x0304));

        public const int TagLensType = unchecked((int)(0x0310));

        public const int TagExternalSensorBrightnessValue = unchecked((int)(0x0311));

        public const int TagMeasuredLv = unchecked((int)(0x0312));

        public const int TagApproximateFNumber = unchecked((int)(0x0313));

        public const int TagCameraTemperature = unchecked((int)(0x0320));

        public const int TagColorTemperature = unchecked((int)(0x0321));

        public const int TagWbRedLevel = unchecked((int)(0x0322));

        public const int TagWbGreenLevel = unchecked((int)(0x0323));

        public const int TagWbBlueLevel = unchecked((int)(0x0324));

        public const int TagCcdVersion = unchecked((int)(0x0330));

        public const int TagCcdBoardVersion = unchecked((int)(0x0331));

        public const int TagControllerBoardVersion = unchecked((int)(0x0332));

        public const int TagM16CVersion = unchecked((int)(0x0333));

        public const int TagImageIdNumber = unchecked((int)(0x0340));

        [NotNull]
        protected internal static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static LeicaMakernoteDirectory()
        {
            TagNameMap.Put(TagQuality, "Quality");
            TagNameMap.Put(TagUserProfile, "User Profile");
            TagNameMap.Put(TagSerialNumber, "Serial Number");
            TagNameMap.Put(TagWhiteBalance, "White Balance");
            TagNameMap.Put(TagLensType, "Lens Type");
            TagNameMap.Put(TagExternalSensorBrightnessValue, "External Sensor Brightness Value");
            TagNameMap.Put(TagMeasuredLv, "Measured LV");
            TagNameMap.Put(TagApproximateFNumber, "Approximate F Number");
            TagNameMap.Put(TagCameraTemperature, "Camera Temperature");
            TagNameMap.Put(TagColorTemperature, "Color Temperature");
            TagNameMap.Put(TagWbRedLevel, "WB Red Level");
            TagNameMap.Put(TagWbGreenLevel, "WB Green Level");
            TagNameMap.Put(TagWbBlueLevel, "WB Blue Level");
            TagNameMap.Put(TagCcdVersion, "CCD Version");
            TagNameMap.Put(TagCcdBoardVersion, "CCD Board Version");
            TagNameMap.Put(TagControllerBoardVersion, "Controller Board Version");
            TagNameMap.Put(TagM16CVersion, "M16 C Version");
            TagNameMap.Put(TagImageIdNumber, "Image ID Number");
        }

        public LeicaMakernoteDirectory()
        {
            this.SetDescriptor(new LeicaMakernoteDescriptor(this));
        }

        [NotNull]
        public override string GetName()
        {
            return "Leica Makernote";
        }

        [NotNull]
        protected internal override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
