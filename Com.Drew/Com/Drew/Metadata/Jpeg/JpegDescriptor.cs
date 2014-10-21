/*
 * Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#) 
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
using System.Text;
using Com.Drew.Metadata;
using Com.Drew.Metadata.Jpeg;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Jpeg
{
	/// <summary>Provides human-readable string versions of the tags stored in a JpegDirectory.</summary>
	/// <remarks>
	/// Provides human-readable string versions of the tags stored in a JpegDirectory.
	/// Thanks to Darrell Silver (www.darrellsilver.com) for the initial version of this class.
	/// </remarks>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class JpegDescriptor : TagDescriptor<JpegDirectory>
	{
		public JpegDescriptor(JpegDirectory directory)
			: base(directory)
		{
		}

		[CanBeNull]
		public override string GetDescription(int tagType)
		{
			switch (tagType)
			{
				case JpegDirectory.TagCompressionType:
				{
					return GetImageCompressionTypeDescription();
				}

				case JpegDirectory.TagComponentData1:
				{
					return GetComponentDataDescription(0);
				}

				case JpegDirectory.TagComponentData2:
				{
					return GetComponentDataDescription(1);
				}

				case JpegDirectory.TagComponentData3:
				{
					return GetComponentDataDescription(2);
				}

				case JpegDirectory.TagComponentData4:
				{
					return GetComponentDataDescription(3);
				}

				case JpegDirectory.TagDataPrecision:
				{
					return GetDataPrecisionDescription();
				}

				case JpegDirectory.TagImageHeight:
				{
					return GetImageHeightDescription();
				}

				case JpegDirectory.TagImageWidth:
				{
					return GetImageWidthDescription();
				}

				default:
				{
					return base.GetDescription(tagType);
				}
			}
		}

		[CanBeNull]
		public virtual string GetImageCompressionTypeDescription()
		{
			int? value = _directory.GetInteger(JpegDirectory.TagCompressionType);
			if (value == null)
			{
				return null;
			}
			switch (value)
			{
				case 0:
				{
					// Note there is no 2 or 12
					return "Baseline";
				}

				case 1:
				{
					return "Extended sequential, Huffman";
				}

				case 2:
				{
					return "Progressive, Huffman";
				}

				case 3:
				{
					return "Lossless, Huffman";
				}

				case 5:
				{
					return "Differential sequential, Huffman";
				}

				case 6:
				{
					return "Differential progressive, Huffman";
				}

				case 7:
				{
					return "Differential lossless, Huffman";
				}

				case 8:
				{
					return "Reserved for JPEG extensions";
				}

				case 9:
				{
					return "Extended sequential, arithmetic";
				}

				case 10:
				{
					return "Progressive, arithmetic";
				}

				case 11:
				{
					return "Lossless, arithmetic";
				}

				case 13:
				{
					return "Differential sequential, arithmetic";
				}

				case 14:
				{
					return "Differential progressive, arithmetic";
				}

				case 15:
				{
					return "Differential lossless, arithmetic";
				}

				default:
				{
					return "Unknown type: " + value;
				}
			}
		}

		[CanBeNull]
		public virtual string GetImageWidthDescription()
		{
			string value = _directory.GetString(JpegDirectory.TagImageWidth);
			if (value == null)
			{
				return null;
			}
			return value + " pixels";
		}

		[CanBeNull]
		public virtual string GetImageHeightDescription()
		{
			string value = _directory.GetString(JpegDirectory.TagImageHeight);
			if (value == null)
			{
				return null;
			}
			return value + " pixels";
		}

		[CanBeNull]
		public virtual string GetDataPrecisionDescription()
		{
			string value = _directory.GetString(JpegDirectory.TagDataPrecision);
			if (value == null)
			{
				return null;
			}
			return value + " bits";
		}

		[CanBeNull]
		public virtual string GetComponentDataDescription(int componentNumber)
		{
			JpegComponent value = _directory.GetComponent(componentNumber);
			if (value == null)
			{
				return null;
			}
			StringBuilder sb = new StringBuilder();
			sb.Append(value.GetComponentName());
			sb.Append(" component: Quantization table ");
			sb.Append(value.GetQuantizationTableNumber());
			sb.Append(", Sampling factors ");
			sb.Append(value.GetHorizontalSamplingFactor());
			sb.Append(" horiz/");
			sb.Append(value.GetVerticalSamplingFactor());
			sb.Append(" vert");
			return sb.ToString();
		}
	}
}
