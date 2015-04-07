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
using Com.Drew.Metadata;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Pcx
{
	/// <author>Drew Noakes https://drewnoakes.com</author>
	public class PcxDescriptor : TagDescriptor<PcxDirectory>
	{
		public PcxDescriptor([NotNull] PcxDirectory directory)
			: base(directory)
		{
		}

		public override string GetDescription(int tagType)
		{
			switch (tagType)
			{
				case PcxDirectory.TagVersion:
				{
					return GetVersionDescription();
				}

				case PcxDirectory.TagColorPlanes:
				{
					return GetColorPlanesDescription();
				}

				case PcxDirectory.TagPaletteType:
				{
					return GetPaletteTypeDescription();
				}

				default:
				{
					return base.GetDescription(tagType);
				}
			}
		}

		[CanBeNull]
		public virtual string GetVersionDescription()
		{
			// Prior to v2.5 of PC Paintbrush, the PCX image file format was considered proprietary information
			// by ZSoft Corporation
			return GetIndexedDescription(PcxDirectory.TagVersion, "2.5 with fixed EGA palette information", null, "2.8 with modifiable EGA palette information", "2.8 without palette information (default palette)", "PC Paintbrush for Windows", "3.0 or better"
				);
		}

		[CanBeNull]
		public virtual string GetColorPlanesDescription()
		{
			return GetIndexedDescription(PcxDirectory.TagColorPlanes, 3, "24-bit color", "16 colors");
		}

		[CanBeNull]
		public virtual string GetPaletteTypeDescription()
		{
			return GetIndexedDescription(PcxDirectory.TagPaletteType, 1, "Color or B&W", "Grayscale");
		}
	}
}
