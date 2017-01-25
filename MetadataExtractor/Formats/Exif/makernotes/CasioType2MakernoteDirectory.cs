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
    /// <summary>Describes tags specific to Casio (type 2) cameras.</summary>
    /// <remarks>
    /// A standard TIFF IFD directory but always uses Motorola (Big-Endian) Byte Alignment.
    /// Makernote data begins after a 6-byte header: <c>"QVC\x00\x00\x00"</c>.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class CasioType2MakernoteDirectory : Directory
    {
        /// <summary>2 values - x,y dimensions in pixels.</summary>
        public const int TagThumbnailDimensions = 0x0002;

        /// <summary>Size in bytes</summary>
        public const int TagThumbnailSize = 0x0003;

        /// <summary>Offset of Preview Thumbnail</summary>
        public const int TagThumbnailOffset = 0x0004;

        /// <summary>
        /// 1 = Fine
        /// 2 = Super Fine
        /// </summary>
        public const int TagQualityMode = 0x0008;

        /// <summary>
        /// 0 = 640 x 480 pixels
        /// 4 = 1600 x 1200 pixels
        /// 5 = 2048 x 1536 pixels
        /// 20 = 2288 x 1712 pixels
        /// 21 = 2592 x 1944 pixels
        /// 22 = 2304 x 1728 pixels
        /// 36 = 3008 x 2008 pixels
        /// </summary>
        public const int TagImageSize = 0x0009;

        /// <summary>
        /// 0 = Normal
        /// 1 = Macro
        /// </summary>
        public const int TagFocusMode1 = 0x000D;

        /// <summary>
        /// 3 = 50
        /// 4 = 64
        /// 6 = 100
        /// 9 = 200
        /// </summary>
        public const int TagIsoSensitivity = 0x0014;

        /// <summary>
        /// 0 = Auto
        /// 1 = Daylight
        /// 2 = Shade
        /// 3 = Tungsten
        /// 4 = Fluorescent
        /// 5 = Manual
        /// </summary>
        public const int TagWhiteBalance1 = 0x0019;

        /// <summary>Units are tenths of a millimetre</summary>
        public const int TagFocalLength = 0x001D;

        /// <summary>
        /// 0 = -1
        /// 1 = Normal
        /// 2 = +1
        /// </summary>
        public const int TagSaturation = 0x001F;

        /// <summary>
        /// 0 = -1
        /// 1 = Normal
        /// 2 = +1
        /// </summary>
        public const int TagContrast = 0x0020;

        /// <summary>
        /// 0 = -1
        /// 1 = Normal
        /// 2 = +1
        /// </summary>
        public const int TagSharpness = 0x0021;

        /// <summary>See PIM specification here: http://www.ozhiker.com/electronics/pjmt/jpeg_info/pim.html</summary>
        public const int TagPrintImageMatchingInfo = 0x0E00;

        /// <summary>Alternate thumbnail offset</summary>
        public const int TagPreviewThumbnail = 0x2000;

        public const int TagWhiteBalanceBias = 0x2011;

        /// <summary>
        /// 12 = Flash
        /// 0 = Manual
        /// 1 = Auto?
        /// 4 = Flash?
        /// </summary>
        public const int TagWhiteBalance2 = 0x2012;

        /// <summary>Units are millimetres</summary>
        public const int TagObjectDistance = 0x2022;

        /// <summary>0 = Off</summary>
        public const int TagFlashDistance = 0x2034;

        /// <summary>2 = Normal Mode</summary>
        public const int TagRecordMode = 0x3000;

        /// <summary>1 = Off?</summary>
        public const int TagSelfTimer = 0x3001;

        /// <summary>3 = Fine</summary>
        public const int TagQuality = 0x3002;

        /// <summary>
        /// 1 = Fixation
        /// 6 = Multi-Area Auto Focus
        /// </summary>
        public const int TagFocusMode2 = 0x3003;

        /// <summary>(string)</summary>
        public const int TagTimeZone = 0x3006;

        public const int TagBestShotMode = 0x3007;

        /// <summary>
        /// 0 = Off
        /// 1 = On?
        /// </summary>
        public const int TagCcdIsoSensitivity = 0x3014;

        /// <summary>0 = Off</summary>
        public const int TagColourMode = 0x3015;

        /// <summary>0 = Off</summary>
        public const int TagEnhancement = 0x3016;

        /// <summary>0 = Off</summary>
        public const int TagFilter = 0x3017;

        // TODO add missing names
        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagThumbnailDimensions, "Thumbnail Dimensions" },
            { TagThumbnailSize, "Thumbnail Size" },
            { TagThumbnailOffset, "Thumbnail Offset" },
            { TagQualityMode, "Quality Mode" },
            { TagImageSize, "Image Size" },
            { TagFocusMode1, "Focus Mode" },
            { TagIsoSensitivity, "ISO Sensitivity" },
            { TagWhiteBalance1, "White Balance" },
            { TagFocalLength, "Focal Length" },
            { TagSaturation, "Saturation" },
            { TagContrast, "Contrast" },
            { TagSharpness, "Sharpness" },
            { TagPrintImageMatchingInfo, "Print Image Matching (PIM) Info" },
            { TagPreviewThumbnail, "Casio Preview Thumbnail" },
            { TagWhiteBalanceBias, "White Balance Bias" },
            { TagWhiteBalance2, "White Balance" },
            { TagObjectDistance, "Object Distance" },
            { TagFlashDistance, "Flash Distance" },
            { TagRecordMode, "Record Mode" },
            { TagSelfTimer, "Self Timer" },
            { TagQuality, "Quality" },
            { TagFocusMode2, "Focus Mode" },
            { TagTimeZone, "Time Zone" },
            { TagBestShotMode, "BestShot Mode" },
            { TagCcdIsoSensitivity, "CCD ISO Sensitivity" },
            { TagColourMode, "Colour Mode" },
            { TagEnhancement, "Enhancement" },
            { TagFilter, "Filter" }
        };

        public CasioType2MakernoteDirectory()
        {
            SetDescriptor(new CasioType2MakernoteDescriptor(this));
        }

        public override string Name => "Casio Makernote";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
