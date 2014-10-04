using System.Collections.Generic;
using Com.Drew.Metadata;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Png
{
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class PngChromaticitiesDirectory : Com.Drew.Metadata.Directory
	{
		public const int TagWhitePointX = 1;

		public const int TagWhitePointY = 2;

		public const int TagRedX = 3;

		public const int TagRedY = 4;

		public const int TagGreenX = 5;

		public const int TagGreenY = 6;

		public const int TagBlueX = 7;

		public const int TagBlueY = 8;

		[NotNull]
		protected internal static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>();

		static PngChromaticitiesDirectory()
		{
			_tagNameMap.Put(TagWhitePointX, "White Point X");
			_tagNameMap.Put(TagWhitePointY, "White Point Y");
			_tagNameMap.Put(TagRedX, "Red X");
			_tagNameMap.Put(TagRedY, "Red Y");
			_tagNameMap.Put(TagGreenX, "Green X");
			_tagNameMap.Put(TagGreenY, "Green Y");
			_tagNameMap.Put(TagBlueX, "Blue X");
			_tagNameMap.Put(TagBlueY, "Blue Y");
		}

		public PngChromaticitiesDirectory()
		{
			this.SetDescriptor(new TagDescriptor<Com.Drew.Metadata.Png.PngChromaticitiesDirectory>(this));
		}

		[NotNull]
		public override string GetName()
		{
			return "PNG Chromaticities";
		}

		[NotNull]
		protected internal override Dictionary<int, string> GetTagNameMap()
		{
			return _tagNameMap;
		}
	}
}
