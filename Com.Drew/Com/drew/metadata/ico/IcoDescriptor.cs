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

namespace Com.Drew.Metadata.Ico
{
	/// <author>Drew Noakes https://drewnoakes.com</author>
	public class IcoDescriptor : TagDescriptor<IcoDirectory>
	{
		public IcoDescriptor([NotNull] IcoDirectory directory)
			: base(directory)
		{
		}

		public override string GetDescription(int tagType)
		{
			switch (tagType)
			{
				case IcoDirectory.TagImageType:
				{
					return GetImageTypeDescription();
				}

				case IcoDirectory.TagImageWidth:
				{
					return GetImageWidthDescription();
				}

				case IcoDirectory.TagImageHeight:
				{
					return GetImageHeightDescription();
				}

				case IcoDirectory.TagColourPaletteSize:
				{
					return GetColourPaletteSizeDescription();
				}

				default:
				{
					return base.GetDescription(tagType);
				}
			}
		}

		[CanBeNull]
		public virtual string GetImageTypeDescription()
		{
			return GetIndexedDescription(IcoDirectory.TagImageType, 1, "Icon", "Cursor");
		}

		[CanBeNull]
		public virtual string GetImageWidthDescription()
		{
			int? width = _directory.GetInteger(IcoDirectory.TagImageWidth);
			if (width == null)
			{
				return null;
			}
			return (width == 0 ? 256 : width) + " pixels";
		}

		[CanBeNull]
		public virtual string GetImageHeightDescription()
		{
			int? width = _directory.GetInteger(IcoDirectory.TagImageHeight);
			if (width == null)
			{
				return null;
			}
			return (width == 0 ? 256 : width) + " pixels";
		}

		[CanBeNull]
		public virtual string GetColourPaletteSizeDescription()
		{
			int? size = _directory.GetInteger(IcoDirectory.TagColourPaletteSize);
			if (size == null)
			{
				return null;
			}
			return size == 0 ? "No palette" : size + " colour" + (size == 1 ? string.Empty : "s");
		}
	}
}
