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
using Com.Drew.Metadata.Exif;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif
{
	/// <summary>Describes Exif interoperability tags.</summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class ExifInteropDirectory : Com.Drew.Metadata.Directory
	{
		public const int TagInteropIndex = unchecked((int)(0x0001));

		public const int TagInteropVersion = unchecked((int)(0x0002));

		public const int TagRelatedImageFileFormat = unchecked((int)(0x1000));

		public const int TagRelatedImageWidth = unchecked((int)(0x1001));

		public const int TagRelatedImageLength = unchecked((int)(0x1002));

		[NotNull]
		protected internal static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>();

		static ExifInteropDirectory()
		{
			_tagNameMap.Put(TagInteropIndex, "Interoperability Index");
			_tagNameMap.Put(TagInteropVersion, "Interoperability Version");
			_tagNameMap.Put(TagRelatedImageFileFormat, "Related Image File Format");
			_tagNameMap.Put(TagRelatedImageWidth, "Related Image Width");
			_tagNameMap.Put(TagRelatedImageLength, "Related Image Length");
		}

		public ExifInteropDirectory()
		{
			this.SetDescriptor(new ExifInteropDescriptor(this));
		}

		[NotNull]
		public override string GetName()
		{
			return "Interoperability";
		}

		[NotNull]
		protected internal override Dictionary<int, string> GetTagNameMap()
		{
			return _tagNameMap;
		}
	}
}
