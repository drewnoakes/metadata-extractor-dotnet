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
        protected internal static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static BmpHeaderDirectory()
        {
            TagNameMap.Put(TagHeaderSize, "Header Size");
            TagNameMap.Put(TagImageHeight, "Image Height");
            TagNameMap.Put(TagImageWidth, "Image Width");
            TagNameMap.Put(TagColourPlanes, "Planes");
            TagNameMap.Put(TagBitsPerPixel, "Bits Per Pixel");
            TagNameMap.Put(TagCompression, "Compression");
            TagNameMap.Put(TagXPixelsPerMeter, "X Pixels per Meter");
            TagNameMap.Put(TagYPixelsPerMeter, "Y Pixels per Meter");
            TagNameMap.Put(TagPaletteColourCount, "Palette Colour Count");
            TagNameMap.Put(TagImportantColourCount, "Important Colour Count");
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
            return TagNameMap;
        }
    }
}
