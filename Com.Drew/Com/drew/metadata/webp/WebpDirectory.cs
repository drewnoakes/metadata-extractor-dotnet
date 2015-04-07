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
using System.Collections.Generic;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Webp
{
	/// <author>Drew Noakes https://drewnoakes.com</author>
	public class WebpDirectory : Com.Drew.Metadata.Directory
	{
		public const int TagImageHeight = 1;

		public const int TagImageWidth = 2;

		public const int TagHasAlpha = 3;

		public const int TagIsAnimation = 4;

		[NotNull]
		protected internal static readonly Dictionary<int?, string> _tagNameMap = new Dictionary<int?, string>();

		static WebpDirectory()
		{
			_tagNameMap.Put(TagImageHeight, "Image Height");
			_tagNameMap.Put(TagImageWidth, "Image Width");
			_tagNameMap.Put(TagHasAlpha, "Has Alpha");
			_tagNameMap.Put(TagIsAnimation, "Is Animation");
		}

		public WebpDirectory()
		{
			this.SetDescriptor(new WebpDescriptor(this));
		}

		[NotNull]
		public override string GetName()
		{
			return "WebP";
		}

		[NotNull]
		protected internal override Dictionary<int?, string> GetTagNameMap()
		{
			return _tagNameMap;
		}
	}
}
