using System.Collections.Generic;

namespace MetadataExtractor.Formats.Bmp
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

        private static readonly Dictionary<int?, string> _tagNameMap = new Dictionary<int?, string>
        {
            { TagHeaderSize, "Header Size" },
            { TagImageHeight, "Image Height" },
            { TagImageWidth, "Image Width" },
            { TagColourPlanes, "Planes" },
            { TagBitsPerPixel, "Bits Per Pixel" },
            { TagCompression, "Compression" },
            { TagXPixelsPerMeter, "X Pixels per Meter" },
            { TagYPixelsPerMeter, "Y Pixels per Meter" },
            { TagPaletteColourCount, "Palette Colour Count" },
            { TagImportantColourCount, "Important Colour Count" }
        };

        public BmpHeaderDirectory()
        {
            SetDescriptor(new BmpHeaderDescriptor(this));
        }

        public override string Name
        {
            get { return "BMP Header"; }
        }

        protected override IReadOnlyDictionary<int?, string> GetTagNameMap()
        {
            return _tagNameMap;
        }
    }
}
