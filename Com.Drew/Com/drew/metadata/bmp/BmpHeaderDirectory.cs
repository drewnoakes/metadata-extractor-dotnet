using System.Collections.Generic;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Bmp
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class BmpHeaderDirectory : Directory
    {
        public const int TagHeaderSize = -1;

        public const int TagImageHeight = 1;

        public const int TagImageWidth = 2;

        public const int TagColourPlanes = 3;

        public const int TagBitsPerPixel = 4;

        public const int TagCompression = 5;

        public const int TagXPixelsPerMeter = 6;

        public const int TagYPixelsPerMeter = 7;

        public const int TagPaletteColourCount = 8;

        public const int TagImportantColourCount = 9;

        [NotNull]
        protected static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static BmpHeaderDirectory()
        {
            TagNameMap[TagHeaderSize] = "Header Size";
            TagNameMap[TagImageHeight] = "Image Height";
            TagNameMap[TagImageWidth] = "Image Width";
            TagNameMap[TagColourPlanes] = "Planes";
            TagNameMap[TagBitsPerPixel] = "Bits Per Pixel";
            TagNameMap[TagCompression] = "Compression";
            TagNameMap[TagXPixelsPerMeter] = "X Pixels per Meter";
            TagNameMap[TagYPixelsPerMeter] = "Y Pixels per Meter";
            TagNameMap[TagPaletteColourCount] = "Palette Colour Count";
            TagNameMap[TagImportantColourCount] = "Important Colour Count";
        }

        public BmpHeaderDirectory()
        {
            SetDescriptor(new BmpHeaderDescriptor(this));
        }

        public override string GetName()
        {
            return "BMP Header";
        }

        protected override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
