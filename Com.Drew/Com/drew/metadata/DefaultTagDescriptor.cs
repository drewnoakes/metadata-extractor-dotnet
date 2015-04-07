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
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata
{
	/// <summary>A default implementation of the abstract TagDescriptor.</summary>
	/// <remarks>
	/// A default implementation of the abstract TagDescriptor.  As this class is not coded with awareness of any metadata
	/// tags, it simply reports tag names using the format 'Unknown tag 0x00' (with the corresponding tag number in hex)
	/// and gives descriptions using the default string representation of the value.
	/// </remarks>
	/// <author>Drew Noakes https://drewnoakes.com</author>
	public class DefaultTagDescriptor : TagDescriptor<Com.Drew.Metadata.Directory>
	{
		public DefaultTagDescriptor([NotNull] Com.Drew.Metadata.Directory directory)
			: base(directory)
		{
		}

		/// <summary>Gets a best-effort tag name using the format 'Unknown tag 0x00' (with the corresponding tag type in hex).</summary>
		/// <param name="tagType">the tag type identifier.</param>
		/// <returns>a string representation of the tag name.</returns>
		[NotNull]
		public virtual string GetTagName(int tagType)
		{
			string hex = Sharpen.Extensions.ToHexString(tagType).ToUpper();
			while (hex.Length < 4)
			{
				hex = "0" + hex;
			}
			return "Unknown tag 0x" + hex;
		}
	}
}
