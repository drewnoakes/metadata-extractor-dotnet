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
using Com.Drew.Metadata;
using Com.Drew.Metadata.Exif.Makernotes;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif.Makernotes
{
	/// <summary>
	/// Provides human-readable string representations of tag values stored in a
	/// <see cref="SigmaMakernoteDirectory"/>
	/// .
	/// </summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class SigmaMakernoteDescriptor : TagDescriptor<SigmaMakernoteDirectory>
	{
		public SigmaMakernoteDescriptor(SigmaMakernoteDirectory directory)
			: base(directory)
		{
		}

		public override string GetDescription(int tagType)
		{
			switch (tagType)
			{
                case SigmaMakernoteDirectory.TagExposureMode:
				{
					return GetExposureModeDescription();
				}

                case SigmaMakernoteDirectory.TagMeteringMode:
				{
					return GetMeteringModeDescription();
				}
			}
			return base.GetDescription(tagType);
		}

		[CanBeNull]
		private string GetMeteringModeDescription()
		{
            string value = _directory.GetString(SigmaMakernoteDirectory.TagMeteringMode);
			if (value == null || value.Length == 0)
			{
				return null;
			}
			switch (value[0])
			{
				case '8':
				{
					return "Multi Segment";
				}

				case 'A':
				{
					return "Average";
				}

				case 'C':
				{
					return "Center Weighted Average";
				}

				default:
				{
					return value;
				}
			}
		}

		[CanBeNull]
		private string GetExposureModeDescription()
		{
            string value = _directory.GetString(SigmaMakernoteDirectory.TagExposureMode);
			if (value == null || value.Length == 0)
			{
				return null;
			}
			switch (value[0])
			{
				case 'A':
				{
					return "Aperture Priority AE";
				}

				case 'M':
				{
					return "Manual";
				}

				case 'P':
				{
					return "Program AE";
				}

				case 'S':
				{
					return "Shutter Speed Priority AE";
				}

				default:
				{
					return value;
				}
			}
		}
	}
}
