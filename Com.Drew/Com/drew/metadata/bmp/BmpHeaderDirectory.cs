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
        protected internal static readonly Dictionary<int?, string> _tagNameMap = new Dictionary<int?, string>();

        static BmpHeaderDirectory()
        {
            _tagNameMap.Put(TagHeaderSize, "Header Size");
            _tagNameMap.Put(TagImageHeight, "Image Height");
            _tagNameMap.Put(TagImageWidth, "Image Width");
            _tagNameMap.Put(TagColourPlanes, "Planes");
            _tagNameMap.Put(TagBitsPerPixel, "Bits Per Pixel");
            _tagNameMap.Put(TagCompression, "Compression");
            _tagNameMap.Put(TagXPixelsPerMeter, "X Pixels per Meter");
            _tagNameMap.Put(TagYPixelsPerMeter, "Y Pixels per Meter");
            _tagNameMap.Put(TagPaletteColourCount, "Palette Colour Count");
            _tagNameMap.Put(TagImportantColourCount, "Important Colour Count");
        }

        public BmpHeaderDirectory()
        {
            this.SetDescriptor(new BmpHeaderDescriptor(this));
        }

        [NotNull]
        public override string GetName()
        {
            return "BMP Header";
        }

        [NotNull]
        protected internal override Dictionary<int?, string> GetTagNameMap()
        {
            return _tagNameMap;
        }
    }
}
