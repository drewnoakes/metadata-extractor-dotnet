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
using Com.Drew.Metadata.Exif.Makernotes;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif.Makernotes
{
	/// <summary>
	/// Provides human-readable string representations of tag values stored in a
	/// <see cref="KodakMakernoteDirectory"/>
	/// .
	/// </summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class KodakMakernoteDescriptor : TagDescriptor<KodakMakernoteDirectory>
	{
		public KodakMakernoteDescriptor(KodakMakernoteDirectory directory)
			: base(directory)
		{
		}

		[CanBeNull]
		public override string GetDescription(int tagType)
		{
			switch (tagType)
			{
				case TagQuality:
				{
					return GetQualityDescription();
				}

				case TagBurstMode:
				{
					return GetBurstModeDescription();
				}

				case TagShutterMode:
				{
					return GetShutterModeDescription();
				}

				case TagFocusMode:
				{
					return GetFocusModeDescription();
				}

				case TagWhiteBalance:
				{
					return GetWhiteBalanceDescription();
				}

				case TagFlashMode:
				{
					return GetFlashModeDescription();
				}

				case TagFlashFired:
				{
					return GetFlashFiredDescription();
				}

				case TagColorMode:
				{
					return GetColorModeDescription();
				}

				case TagSharpness:
				{
					return GetSharpnessDescription();
				}

				default:
				{
					return base.GetDescription(tagType);
				}
			}
		}

		[CanBeNull]
		public virtual string GetSharpnessDescription()
		{
			return GetIndexedDescription(TagSharpness, "Normal");
		}

		[CanBeNull]
		public virtual string GetColorModeDescription()
		{
			int value = _directory.GetInteger(TagColorMode);
			if (value == null)
			{
				return null;
			}
			switch (value)
			{
				case unchecked((int)(0x001)):
				case unchecked((int)(0x2000)):
				{
					return "B&W";
				}

				case unchecked((int)(0x002)):
				case unchecked((int)(0x4000)):
				{
					return "Sepia";
				}

				case unchecked((int)(0x003)):
				{
					return "B&W Yellow Filter";
				}

				case unchecked((int)(0x004)):
				{
					return "B&W Red Filter";
				}

				case unchecked((int)(0x020)):
				{
					return "Saturated Color";
				}

				case unchecked((int)(0x040)):
				case unchecked((int)(0x200)):
				{
					return "Neutral Color";
				}

				case unchecked((int)(0x100)):
				{
					return "Saturated Color";
				}

				default:
				{
					return "Unknown (" + value + ")";
				}
			}
		}

		[CanBeNull]
		public virtual string GetFlashFiredDescription()
		{
			return GetIndexedDescription(TagFlashFired, "No", "Yes");
		}

		[CanBeNull]
		public virtual string GetFlashModeDescription()
		{
			int value = _directory.GetInteger(TagFlashMode);
			if (value == null)
			{
				return null;
			}
			switch (value)
			{
				case unchecked((int)(0x00)):
				{
					return "Auto";
				}

				case unchecked((int)(0x10)):
				case unchecked((int)(0x01)):
				{
					return "Fill Flash";
				}

				case unchecked((int)(0x20)):
				case unchecked((int)(0x02)):
				{
					return "Off";
				}

				case unchecked((int)(0x40)):
				case unchecked((int)(0x03)):
				{
					return "Red Eye";
				}

				default:
				{
					return "Unknown (" + value + ")";
				}
			}
		}

		[CanBeNull]
		public virtual string GetWhiteBalanceDescription()
		{
			return GetIndexedDescription(TagWhiteBalance, "Auto", "Flash", "Tungsten", "Daylight");
		}

		[CanBeNull]
		public virtual string GetFocusModeDescription()
		{
			return GetIndexedDescription(TagFocusMode, "Normal", null, "Macro");
		}

		[CanBeNull]
		public virtual string GetShutterModeDescription()
		{
			int value = _directory.GetInteger(TagShutterMode);
			if (value == null)
			{
				return null;
			}
			switch (value)
			{
				case 0:
				{
					return "Auto";
				}

				case 8:
				{
					return "Aperture Priority";
				}

				case 32:
				{
					return "Manual";
				}

				default:
				{
					return "Unknown (" + value + ")";
				}
			}
		}

		[CanBeNull]
		public virtual string GetBurstModeDescription()
		{
			return GetIndexedDescription(TagBurstMode, "Off", "On");
		}

		[CanBeNull]
		public virtual string GetQualityDescription()
		{
			return GetIndexedDescription(TagQuality, 1, "Fine", "Normal");
		}
	}
}
