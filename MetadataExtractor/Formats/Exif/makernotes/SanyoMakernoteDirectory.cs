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
    /// <summary>Describes tags specific to Sanyo cameras.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
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

        public const int TagPrintIm = 0x0e00;

        public const int TagDataDump = 0x0f00;

        [NotNull]
        protected static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static SanyoMakernoteDirectory()
        {
            TagNameMap[TagMakernoteOffset] = "Makernote Offset";
            TagNameMap[TagSanyoThumbnail] = "Sanyo Thumbnail";
            TagNameMap[TagSpecialMode] = "Special Mode";
            TagNameMap[TagSanyoQuality] = "Sanyo Quality";
            TagNameMap[TagMacro] = "Macro";
            TagNameMap[TagDigitalZoom] = "Digital Zoom";
            TagNameMap[TagSoftwareVersion] = "Software Version";
            TagNameMap[TagPictInfo] = "Pict Info";
            TagNameMap[TagCameraId] = "Camera ID";
            TagNameMap[TagSequentialShot] = "Sequential Shot";
            TagNameMap[TagWideRange] = "Wide Range";
            TagNameMap[TagColorAdjustmentMode] = "Color Adjustment Node";
            TagNameMap[TagQuickShot] = "Quick Shot";
            TagNameMap[TagSelfTimer] = "Self Timer";
            TagNameMap[TagVoiceMemo] = "Voice Memo";
            TagNameMap[TagRecordShutterRelease] = "Record Shutter Release";
            TagNameMap[TagFlickerReduce] = "Flicker Reduce";
            TagNameMap[TagOpticalZoomOn] = "Optical Zoom On";
            TagNameMap[TagDigitalZoomOn] = "Digital Zoom On";
            TagNameMap[TagLightSourceSpecial] = "Light Source Special";
            TagNameMap[TagResaved] = "Resaved";
            TagNameMap[TagSceneSelect] = "Scene Select";
            TagNameMap[TagManualFocusDistanceOrFaceInfo] = "Manual Focus Distance or Face Info";
            TagNameMap[TagSequenceShotInterval] = "Sequence Shot Interval";
            TagNameMap[TagFlashMode] = "Flash Mode";
            TagNameMap[TagPrintIm] = "Print IM";
            TagNameMap[TagDataDump] = "Data Dump";
        }

        public SanyoMakernoteDirectory()
        {
            SetDescriptor(new SanyoMakernoteDescriptor(this));
        }

        public override string Name
        {
            get { return "Sanyo Makernote"; }
        }

        protected override IReadOnlyDictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
