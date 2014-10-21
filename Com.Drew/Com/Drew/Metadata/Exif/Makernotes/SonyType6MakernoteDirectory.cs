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
using System.Collections.Generic;
using Com.Drew.Metadata.Exif.Makernotes;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif.Makernotes
{
	/// <summary>Describes tags specific to Sony cameras that use the Sony Type 6 makernote tags.</summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class SonyType6MakernoteDirectory : Com.Drew.Metadata.Directory
	{
		public const int TagMakernoteThumbOffset = unchecked((int)(0x0513));

		public const int TagMakernoteThumbLength = unchecked((int)(0x0514));

		public const int TagMakernoteThumbVersion = unchecked((int)(0x2000));

		[NotNull]
		protected internal static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>();

		static SonyType6MakernoteDirectory()
		{
			//    public static final int TAG_UNKNOWN_1 = 0x0515;
			_tagNameMap.Put(TagMakernoteThumbOffset, "Makernote Thumb Offset");
			_tagNameMap.Put(TagMakernoteThumbLength, "Makernote Thumb Length");
			//        _tagNameMap.put(TAG_UNKNOWN_1, "Sony-6-0x0203");
			_tagNameMap.Put(TagMakernoteThumbVersion, "Makernote Thumb Version");
		}

		public SonyType6MakernoteDirectory()
		{
			this.SetDescriptor(new SonyType6MakernoteDescriptor(this));
		}

		[NotNull]
		public override string GetName()
		{
			return "Sony Makernote";
		}

		[NotNull]
		protected internal override Dictionary<int, string> GetTagNameMap()
		{
			return _tagNameMap;
		}
	}
}
