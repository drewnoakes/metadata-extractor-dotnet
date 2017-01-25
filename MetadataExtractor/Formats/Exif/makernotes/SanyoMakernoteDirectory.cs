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
    /// <summary>Describes tags specific to Sanyo cameras.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class SanyoMakernoteDirectory : Directory
    {
        public const int TagMakernoteOffset = 0x00ff;
        public const int TagSanyoThumbnail = 0x0100;
        public const int TagSpecialMode = 0x0200;
        public const int TagSanyoQuality = 0x0201;
        public const int TagMacro = 0x0202;
        public const int TagDigitalZoom = 0x0204;
        public const int TagSoftwareVersion = 0x0207;
        public const int TagPictInfo = 0x0208;
        public const int TagCameraId = 0x0209;
        public const int TagSequentialShot = 0x020e;
        public const int TagWideRange = 0x020f;
        public const int TagColorAdjustmentMode = 0x0210;
        public const int TagQuickShot = 0x0213;
        public const int TagSelfTimer = 0x0214;
        public const int TagVoiceMemo = 0x0216;
        public const int TagRecordShutterRelease = 0x0217;
        public const int TagFlickerReduce = 0x0218;
        public const int TagOpticalZoomOn = 0x0219;
        public const int TagDigitalZoomOn = 0x021b;
        public const int TagLightSourceSpecial = 0x021d;
        public const int TagResaved = 0x021e;
        public const int TagSceneSelect = 0x021f;
        public const int TagManualFocusDistanceOrFaceInfo = 0x0223;
        public const int TagSequenceShotInterval = 0x0224;
        public const int TagFlashMode = 0x0225;
        public const int TagPrintImageMatchingInfo = 0x0E00;
        public const int TagDataDump = 0x0f00;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagMakernoteOffset, "Makernote Offset" },
            { TagSanyoThumbnail, "Sanyo Thumbnail" },
            { TagSpecialMode, "Special Mode" },
            { TagSanyoQuality, "Sanyo Quality" },
            { TagMacro, "Macro" },
            { TagDigitalZoom, "Digital Zoom" },
            { TagSoftwareVersion, "Software Version" },
            { TagPictInfo, "Pict Info" },
            { TagCameraId, "Camera ID" },
            { TagSequentialShot, "Sequential Shot" },
            { TagWideRange, "Wide Range" },
            { TagColorAdjustmentMode, "Color Adjustment Node" },
            { TagQuickShot, "Quick Shot" },
            { TagSelfTimer, "Self Timer" },
            { TagVoiceMemo, "Voice Memo" },
            { TagRecordShutterRelease, "Record Shutter Release" },
            { TagFlickerReduce, "Flicker Reduce" },
            { TagOpticalZoomOn, "Optical Zoom On" },
            { TagDigitalZoomOn, "Digital Zoom On" },
            { TagLightSourceSpecial, "Light Source Special" },
            { TagResaved, "Resaved" },
            { TagSceneSelect, "Scene Select" },
            { TagManualFocusDistanceOrFaceInfo, "Manual Focus Distance or Face Info" },
            { TagSequenceShotInterval, "Sequence Shot Interval" },
            { TagFlashMode, "Flash Mode" },
            { TagPrintImageMatchingInfo, "Print Image Matching (PIM) Info" },
            { TagDataDump, "Data Dump" }
        };

        public SanyoMakernoteDirectory()
        {
            SetDescriptor(new SanyoMakernoteDescriptor(this));
        }

        public override string Name => "Sanyo Makernote";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
