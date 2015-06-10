using JetBrains.Annotations;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Png.png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PngHeader
    {
        /// <exception cref="PngProcessingException"/>
        public PngHeader([NotNull] byte[] bytes)
        {
            if (bytes.Length != 13)
                throw new PngProcessingException("PNG header chunk must have exactly 13 data bytes");

            var reader = new SequentialByteArrayReader(bytes);

            ImageWidth = reader.GetInt32();
            ImageHeight = reader.GetInt32();
            BitsPerSample = reader.GetUInt8();

            var colorTypeNumber = reader.GetUInt8();
            var colorType = PngColorType.FromNumericValue(colorTypeNumber);
            if (colorType == null)
                throw new PngProcessingException("Unexpected PNG color type: " + colorTypeNumber);
            ColorType = colorType;

            CompressionType = reader.GetUInt8();
            FilterMethod = reader.GetUInt8();
            InterlaceMethod = reader.GetUInt8();
        }

        public int ImageWidth { get; private set; }

        public int ImageHeight { get; private set; }

        public byte BitsPerSample { get; private set; }

        [NotNull]
        public PngColorType ColorType { get; private set; }

        public byte CompressionType { get; private set; }

        public byte FilterMethod { get; private set; }

        public byte InterlaceMethod { get; private set; }
    }
}
