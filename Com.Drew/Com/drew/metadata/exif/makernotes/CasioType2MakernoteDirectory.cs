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
    /// <summary>Describes tags specific to Casio (type 2) cameras.</summary>
    /// <remarks>
    /// Describes tags specific to Casio (type 2) cameras.
    /// A standard TIFF IFD directory but always uses Motorola (Big-Endian) Byte Alignment.
    /// Makernote data begins after a 6-byte header: "QVC\x00\x00\x00"
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class CasioType2MakernoteDirectory : Directory
    {
        /// <summary>2 values - x,y dimensions in pixels.</summary>
        public const int TagThumbnailDimensions = unchecked((int)(0x0002));

        /// <summary>Size in bytes</summary>
        public const int TagThumbnailSize = unchecked((int)(0x0003));

        /// <summary>Offset of Preview Thumbnail</summary>
        public const int TagThumbnailOffset = unchecked((int)(0x0004));

        /// <summary>
        /// 1 = Fine
        /// 2 = Super Fine
        /// </summary>
        public const int TagQualityMode = unchecked((int)(0x0008));

        /// <summary>
        /// 0 = 640 x 480 pixels
        /// 4 = 1600 x 1200 pixels
        /// 5 = 2048 x 1536 pixels
        /// 20 = 2288 x 1712 pixels
        /// 21 = 2592 x 1944 pixels
        /// 22 = 2304 x 1728 pixels
        /// 36 = 3008 x 2008 pixels
        /// </summary>
        public const int TagImageSize = unchecked((int)(0x0009));

        /// <summary>
        /// 0 = Normal
        /// 1 = Macro
        /// </summary>
        public const int TagFocusMode1 = unchecked((int)(0x000D));

        /// <summary>
        /// 3 = 50
        /// 4 = 64
        /// 6 = 100
        /// 9 = 200
        /// </summary>
        public const int TagIsoSensitivity = unchecked((int)(0x0014));

        /// <summary>
        /// 0 = Auto
        /// 1 = Daylight
        /// 2 = Shade
        /// 3 = Tungsten
        /// 4 = Fluorescent
        /// 5 = Manual
        /// </summary>
        public const int TagWhiteBalance1 = unchecked((int)(0x0019));

        /// <summary>Units are tenths of a millimetre</summary>
        public const int TagFocalLength = unchecked((int)(0x001D));

        /// <summary>
        /// 0 = -1
        /// 1 = Normal
        /// 2 = +1
        /// </summary>
        public const int TagSaturation = unchecked((int)(0x001F));

        /// <summary>
        /// 0 = -1
        /// 1 = Normal
        /// 2 = +1
        /// </summary>
        public const int TagContrast = unchecked((int)(0x0020));

        /// <summary>
        /// 0 = -1
        /// 1 = Normal
        /// 2 = +1
        /// </summary>
        public const int TagSharpness = unchecked((int)(0x0021));

        /// <summary>See PIM specification here: http://www.ozhiker.com/electronics/pjmt/jpeg_info/pim.html</summary>
        public const int TagPrintImageMatchingInfo = unchecked((int)(0x0E00));

        /// <summary>Alternate thumbnail offset</summary>
        public const int TagPreviewThumbnail = unchecked((int)(0x2000));

        public const int TagWhiteBalanceBias = unchecked((int)(0x2011));

        /// <summary>
        /// 12 = Flash
        /// 0 = Manual
        /// 1 = Auto?
        /// 4 = Flash?
        /// </summary>
        public const int TagWhiteBalance2 = unchecked((int)(0x2012));

        /// <summary>Units are millimetres</summary>
        public const int TagObjectDistance = unchecked((int)(0x2022));

        /// <summary>0 = Off</summary>
        public const int TagFlashDistance = unchecked((int)(0x2034));

        /// <summary>2 = Normal Mode</summary>
        public const int TagRecordMode = unchecked((int)(0x3000));

        /// <summary>1 = Off?</summary>
        public const int TagSelfTimer = unchecked((int)(0x3001));

        /// <summary>3 = Fine</summary>
        public const int TagQuality = unchecked((int)(0x3002));

        /// <summary>
        /// 1 = Fixation
        /// 6 = Multi-Area Auto Focus
        /// </summary>
        public const int TagFocusMode2 = unchecked((int)(0x3003));

        /// <summary>(string)</summary>
        public const int TagTimeZone = unchecked((int)(0x3006));

        public const int TagBestshotMode = unchecked((int)(0x3007));

        /// <summary>
        /// 0 = Off
        /// 1 = On?
        /// </summary>
        public const int TagCcdIsoSensitivity = unchecked((int)(0x3014));

        /// <summary>0 = Off</summary>
        public const int TagColourMode = unchecked((int)(0x3015));

        /// <summary>0 = Off</summary>
        public const int TagEnhancement = unchecked((int)(0x3016));

        /// <summary>0 = Off</summary>
        public const int TagFilter = unchecked((int)(0x3017));

        [NotNull]
        protected internal static readonly Dictionary<int?, string> _tagNameMap = new Dictionary<int?, string>();

        static CasioType2MakernoteDirectory()
        {
            // TODO add missing names
            _tagNameMap.Put(TagThumbnailDimensions, "Thumbnail Dimensions");
            _tagNameMap.Put(TagThumbnailSize, "Thumbnail Size");
            _tagNameMap.Put(TagThumbnailOffset, "Thumbnail Offset");
            _tagNameMap.Put(TagQualityMode, "Quality Mode");
            _tagNameMap.Put(TagImageSize, "Image Size");
            _tagNameMap.Put(TagFocusMode1, "Focus Mode");
            _tagNameMap.Put(TagIsoSensitivity, "ISO Sensitivity");
            _tagNameMap.Put(TagWhiteBalance1, "White Balance");
            _tagNameMap.Put(TagFocalLength, "Focal Length");
            _tagNameMap.Put(TagSaturation, "Saturation");
            _tagNameMap.Put(TagContrast, "Contrast");
            _tagNameMap.Put(TagSharpness, "Sharpness");
            _tagNameMap.Put(TagPrintImageMatchingInfo, "Print Image Matching (PIM) Info");
            _tagNameMap.Put(TagPreviewThumbnail, "Casio Preview Thumbnail");
            _tagNameMap.Put(TagWhiteBalanceBias, "White Balance Bias");
            _tagNameMap.Put(TagWhiteBalance2, "White Balance");
            _tagNameMap.Put(TagObjectDistance, "Object Distance");
            _tagNameMap.Put(TagFlashDistance, "Flash Distance");
            _tagNameMap.Put(TagRecordMode, "Record Mode");
            _tagNameMap.Put(TagSelfTimer, "Self Timer");
            _tagNameMap.Put(TagQuality, "Quality");
            _tagNameMap.Put(TagFocusMode2, "Focus Mode");
            _tagNameMap.Put(TagTimeZone, "Time Zone");
            _tagNameMap.Put(TagBestshotMode, "BestShot Mode");
            _tagNameMap.Put(TagCcdIsoSensitivity, "CCD ISO Sensitivity");
            _tagNameMap.Put(TagColourMode, "Colour Mode");
            _tagNameMap.Put(TagEnhancement, "Enhancement");
            _tagNameMap.Put(TagFilter, "Filter");
        }

        public CasioType2MakernoteDirectory()
        {
            this.SetDescriptor(new CasioType2MakernoteDescriptor(this));
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
