using System.Collections.Generic;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Gif
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class GifHeaderDirectory : Directory
    {
        public const int TagGifFormatVersion = 1;

        public const int TagImageWidth = 2;

        public const int TagImageHeight = 3;

        public const int TagColorTableSize = 4;

        public const int TagIsColorTableSorted = 5;

        public const int TagBitsPerPixel = 6;

        public const int TagHasGlobalColorTable = 7;

        public const int TagTransparentColorIndex = 8;

        public const int TagPixelAspectRatio = 9;

        [NotNull]
        protected internal static readonly Dictionary<int?, string> _tagNameMap = new Dictionary<int?, string>();

        static GifHeaderDirectory()
        {
            _tagNameMap.Put(TagGifFormatVersion, "GIF Format Version");
            _tagNameMap.Put(TagImageHeight, "Image Height");
            _tagNameMap.Put(TagImageWidth, "Image Width");
            _tagNameMap.Put(TagColorTableSize, "Color Table Size");
            _tagNameMap.Put(TagIsColorTableSorted, "Is Color Table Sorted");
            _tagNameMap.Put(TagBitsPerPixel, "Bits per Pixel");
            _tagNameMap.Put(TagHasGlobalColorTable, "Has Global Color Table");
            _tagNameMap.Put(TagTransparentColorIndex, "Transparent Color Index");
            _tagNameMap.Put(TagPixelAspectRatio, "Pixel Aspect Ratio");
        }

        public GifHeaderDirectory()
        {
            this.SetDescriptor(new GifHeaderDescriptor(this));
        }

        [NotNull]
        public override string GetName()
        {
            return "GIF Header";
        }

        [NotNull]
        protected internal override Dictionary<int?, string> GetTagNameMap()
        {
            return _tagNameMap;
        }
    }
}
