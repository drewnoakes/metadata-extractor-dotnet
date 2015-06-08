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
        protected static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static GifHeaderDirectory()
        {
            TagNameMap.Put(TagGifFormatVersion, "GIF Format Version");
            TagNameMap.Put(TagImageHeight, "Image Height");
            TagNameMap.Put(TagImageWidth, "Image Width");
            TagNameMap.Put(TagColorTableSize, "Color Table Size");
            TagNameMap.Put(TagIsColorTableSorted, "Is Color Table Sorted");
            TagNameMap.Put(TagBitsPerPixel, "Bits per Pixel");
            TagNameMap.Put(TagHasGlobalColorTable, "Has Global Color Table");
            TagNameMap.Put(TagTransparentColorIndex, "Transparent Color Index");
            TagNameMap.Put(TagPixelAspectRatio, "Pixel Aspect Ratio");
        }

        public GifHeaderDirectory()
        {
            SetDescriptor(new GifHeaderDescriptor(this));
        }

        [NotNull]
        public override string GetName()
        {
            return "GIF Header";
        }

        [NotNull]
        protected override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
