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
using Com.Drew.Metadata;
using Com.Drew.Metadata.Adobe;
using Sharpen;

namespace Com.Drew.Metadata.Adobe
{
	/// <summary>Provides human-readable string versions of the tags stored in an AdobeJpegDirectory.</summary>
	public class AdobeJpegDescriptor : TagDescriptor<AdobeJpegDirectory>
	{
		public AdobeJpegDescriptor(AdobeJpegDirectory directory)
			: base(directory)
		{
		}

		public override string GetDescription(int tagType)
		{
			switch (tagType)
			{
				case AdobeJpegDirectory.TagColorTransform:
				{
					return GetColorTransformDescription();
				}

				case AdobeJpegDirectory.TagDctEncodeVersion:
				{
					return GetDctEncodeVersionDescription();
				}

				default:
				{
					return base.GetDescription(tagType);
				}
			}
		}

		private string GetDctEncodeVersionDescription()
		{
			int value = _directory.GetInteger(AdobeJpegDirectory.TagColorTransform);
			return value == null ? null : value == unchecked((int)(0x64)) ? "100" : Sharpen.Extensions.ToString(value);
		}

		private string GetColorTransformDescription()
		{
			int value = _directory.GetInteger(AdobeJpegDirectory.TagColorTransform);
			if (value == null)
			{
				return null;
			}
			switch (value)
			{
				case 0:
				{
					return "Unknown (RGB or CMYK)";
				}

				case 1:
				{
					return "YCbCr";
				}

				case 2:
				{
					return "YCCK";
				}

				default:
				{
					return Sharpen.Extensions.StringFormat("Unknown transform (%d)", value);
				}
			}
		}
	}
}
