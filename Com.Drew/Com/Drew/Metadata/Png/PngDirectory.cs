using System.Collections.Generic;
using Com.Drew.Metadata.Png;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Png
{
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class PngDirectory : Com.Drew.Metadata.Directory
	{
		public const int TagImageWidth = 1;

		public const int TagImageHeight = 2;

		public const int TagBitsPerSample = 3;

		public const int TagColorType = 4;

		public const int TagCompressionType = 5;

		public const int TagFilterMethod = 6;

		public const int TagInterlaceMethod = 7;

		public const int TagPaletteSize = 8;

		public const int TagPaletteHasTransparency = 9;

		public const int TagSrgbRenderingIntent = 10;

		public const int TagGamma = 11;

		public const int TagProfileName = 12;

		public const int TagTextualData = 13;

		public const int TagLastModificationTime = 14;

		public const int TagBackgroundColor = 15;

		[NotNull]
		protected internal static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>();

		static PngDirectory()
		{
			_tagNameMap.Put(TagImageHeight, "Image Height");
			_tagNameMap.Put(TagImageWidth, "Image Width");
			_tagNameMap.Put(TagBitsPerSample, "Bits Per Sample");
			_tagNameMap.Put(TagColorType, "Color Type");
			_tagNameMap.Put(TagCompressionType, "Compression Type");
			_tagNameMap.Put(TagFilterMethod, "Filter Method");
			_tagNameMap.Put(TagInterlaceMethod, "Interlace Method");
			_tagNameMap.Put(TagPaletteSize, "Palette Size");
			_tagNameMap.Put(TagPaletteHasTransparency, "Palette Has Transparency");
			_tagNameMap.Put(TagSrgbRenderingIntent, "sRGB Rendering Intent");
			_tagNameMap.Put(TagGamma, "Image Gamma");
			_tagNameMap.Put(TagProfileName, "ICC Profile Name");
			_tagNameMap.Put(TagTextualData, "Textual Data");
			_tagNameMap.Put(TagLastModificationTime, "Last Modification Time");
			_tagNameMap.Put(TagBackgroundColor, "Background Color");
		}

		public PngDirectory()
		{
			this.SetDescriptor(new PngDescriptor(this));
		}

		[NotNull]
		public override string GetName()
		{
			return "PNG";
		}

		[NotNull]
		protected internal override Dictionary<int, string> GetTagNameMap()
		{
			return _tagNameMap;
		}
	}
}
