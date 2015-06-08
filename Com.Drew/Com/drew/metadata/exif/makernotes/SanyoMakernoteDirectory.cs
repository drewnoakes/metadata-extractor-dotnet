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
    /// <summary>Describes tags specific to Sanyo cameras.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class SanyoMakernoteDirectory : Directory
    {
        public const int TagMakernoteOffset = unchecked((int)(0x00ff));

        public const int TagSanyoThumbnail = unchecked((int)(0x0100));

        public const int TagSpecialMode = unchecked((int)(0x0200));

        public const int TagSanyoQuality = unchecked((int)(0x0201));

        public const int TagMacro = unchecked((int)(0x0202));

        public const int TagDigitalZoom = unchecked((int)(0x0204));

        public const int TagSoftwareVersion = unchecked((int)(0x0207));

        public const int TagPictInfo = unchecked((int)(0x0208));

        public const int TagCameraId = unchecked((int)(0x0209));

        public const int TagSequentialShot = unchecked((int)(0x020e));

        public const int TagWideRange = unchecked((int)(0x020f));

        public const int TagColorAdjustmentMode = unchecked((int)(0x0210));

        public const int TagQuickShot = unchecked((int)(0x0213));

        public const int TagSelfTimer = unchecked((int)(0x0214));

        public const int TagVoiceMemo = unchecked((int)(0x0216));

        public const int TagRecordShutterRelease = unchecked((int)(0x0217));

        public const int TagFlickerReduce = unchecked((int)(0x0218));

        public const int TagOpticalZoomOn = unchecked((int)(0x0219));

        public const int TagDigitalZoomOn = unchecked((int)(0x021b));

        public const int TagLightSourceSpecial = unchecked((int)(0x021d));

        public const int TagResaved = unchecked((int)(0x021e));

        public const int TagSceneSelect = unchecked((int)(0x021f));

        public const int TagManualFocusDistanceOrFaceInfo = unchecked((int)(0x0223));

        public const int TagSequenceShotInterval = unchecked((int)(0x0224));

        public const int TagFlashMode = unchecked((int)(0x0225));

        public const int TagPrintIm = unchecked((int)(0x0e00));

        public const int TagDataDump = unchecked((int)(0x0f00));

        [NotNull]
        protected internal static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static SanyoMakernoteDirectory()
        {
            TagNameMap.Put(TagMakernoteOffset, "Makernote Offset");
            TagNameMap.Put(TagSanyoThumbnail, "Sanyo Thumbnail");
            TagNameMap.Put(TagSpecialMode, "Special Mode");
            TagNameMap.Put(TagSanyoQuality, "Sanyo Quality");
            TagNameMap.Put(TagMacro, "Macro");
            TagNameMap.Put(TagDigitalZoom, "Digital Zoom");
            TagNameMap.Put(TagSoftwareVersion, "Software Version");
            TagNameMap.Put(TagPictInfo, "Pict Info");
            TagNameMap.Put(TagCameraId, "Camera ID");
            TagNameMap.Put(TagSequentialShot, "Sequential Shot");
            TagNameMap.Put(TagWideRange, "Wide Range");
            TagNameMap.Put(TagColorAdjustmentMode, "Color Adjustment Node");
            TagNameMap.Put(TagQuickShot, "Quick Shot");
            TagNameMap.Put(TagSelfTimer, "Self Timer");
            TagNameMap.Put(TagVoiceMemo, "Voice Memo");
            TagNameMap.Put(TagRecordShutterRelease, "Record Shutter Release");
            TagNameMap.Put(TagFlickerReduce, "Flicker Reduce");
            TagNameMap.Put(TagOpticalZoomOn, "Optical Zoom On");
            TagNameMap.Put(TagDigitalZoomOn, "Digital Zoom On");
            TagNameMap.Put(TagLightSourceSpecial, "Light Source Special");
            TagNameMap.Put(TagResaved, "Resaved");
            TagNameMap.Put(TagSceneSelect, "Scene Select");
            TagNameMap.Put(TagManualFocusDistanceOrFaceInfo, "Manual Focus Distance or Face Info");
            TagNameMap.Put(TagSequenceShotInterval, "Sequence Shot Interval");
            TagNameMap.Put(TagFlashMode, "Flash Mode");
            TagNameMap.Put(TagPrintIm, "Print IM");
            TagNameMap.Put(TagDataDump, "Data Dump");
        }

        public SanyoMakernoteDirectory()
        {
            this.SetDescriptor(new SanyoMakernoteDescriptor(this));
        }

        [NotNull]
        public override string GetName()
        {
            return "Sanyo Makernote";
        }

        [NotNull]
        protected internal override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
