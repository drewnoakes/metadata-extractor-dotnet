using System.Collections.Generic;
using JetBrains.Annotations;

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
            TagNameMap[TagGifFormatVersion] = "GIF Format Version";
            TagNameMap[TagImageHeight] = "Image Height";
            TagNameMap[TagImageWidth] = "Image Width";
            TagNameMap[TagColorTableSize] = "Color Table Size";
            TagNameMap[TagIsColorTableSorted] = "Is Color Table Sorted";
            TagNameMap[TagBitsPerPixel] = "Bits per Pixel";
            TagNameMap[TagHasGlobalColorTable] = "Has Global Color Table";
            TagNameMap[TagTransparentColorIndex] = "Transparent Color Index";
            TagNameMap[TagPixelAspectRatio] = "Pixel Aspect Ratio";
        }

        public GifHeaderDirectory()
        {
            SetDescriptor(new GifHeaderDescriptor(this));
        }

        public override string GetName()
        {
            return "GIF Header";
        }

        protected override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
