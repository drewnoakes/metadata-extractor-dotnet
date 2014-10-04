/*
 * Copyright 2002-2013 Drew Noakes
 *
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
 *    http://drewnoakes.com/code/exif/
 *    http://code.google.com/p/metadata-extractor/
 */
using System.Collections.Generic;
using Com.Drew.Metadata.Exif.Makernotes;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif.Makernotes
{
	/// <summary>Describes tags specific to Sanyo cameras.</summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class SanyoMakernoteDirectory : Com.Drew.Metadata.Directory
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
		protected internal static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>();

		static SanyoMakernoteDirectory()
		{
			_tagNameMap.Put(TagMakernoteOffset, "Makernote Offset");
			_tagNameMap.Put(TagSanyoThumbnail, "Sanyo Thumbnail");
			_tagNameMap.Put(TagSpecialMode, "Special Mode");
			_tagNameMap.Put(TagSanyoQuality, "Sanyo Quality");
			_tagNameMap.Put(TagMacro, "Macro");
			_tagNameMap.Put(TagDigitalZoom, "Digital Zoom");
			_tagNameMap.Put(TagSoftwareVersion, "Software Version");
			_tagNameMap.Put(TagPictInfo, "Pict Info");
			_tagNameMap.Put(TagCameraId, "Camera ID");
			_tagNameMap.Put(TagSequentialShot, "Sequential Shot");
			_tagNameMap.Put(TagWideRange, "Wide Range");
			_tagNameMap.Put(TagColorAdjustmentMode, "Color Adjustment Node");
			_tagNameMap.Put(TagQuickShot, "Quick Shot");
			_tagNameMap.Put(TagSelfTimer, "Self Timer");
			_tagNameMap.Put(TagVoiceMemo, "Voice Memo");
			_tagNameMap.Put(TagRecordShutterRelease, "Record Shutter Release");
			_tagNameMap.Put(TagFlickerReduce, "Flicker Reduce");
			_tagNameMap.Put(TagOpticalZoomOn, "Optical Zoom On");
			_tagNameMap.Put(TagDigitalZoomOn, "Digital Zoom On");
			_tagNameMap.Put(TagLightSourceSpecial, "Light Source Special");
			_tagNameMap.Put(TagResaved, "Resaved");
			_tagNameMap.Put(TagSceneSelect, "Scene Select");
			_tagNameMap.Put(TagManualFocusDistanceOrFaceInfo, "Manual Focus Distance or Face Info");
			_tagNameMap.Put(TagSequenceShotInterval, "Sequence Shot Interval");
			_tagNameMap.Put(TagFlashMode, "Flash Mode");
			_tagNameMap.Put(TagPrintIm, "Print IM");
			_tagNameMap.Put(TagDataDump, "Data Dump");
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
		protected internal override Dictionary<int, string> GetTagNameMap()
		{
			return _tagNameMap;
		}
	}
}
