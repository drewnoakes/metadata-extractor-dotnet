using System.IO;
using JetBrains.Annotations;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PngChromaticities
    {
        public int WhitePointX { get; private set; }
        public int WhitePointY { get; private set; }
        public int RedX { get; private set; }
        public int RedY { get; private set; }
        public int GreenX { get; private set; }
        public int GreenY { get; private set; }
        public int BlueX { get; private set; }
        public int BlueY { get; private set; }

        /// <exception cref="PngProcessingException"/>
        public PngChromaticities([NotNull] byte[] bytes)
        {
            if (bytes.Length != 8*4)
                throw new PngProcessingException("Invalid number of bytes");

            var reader = new SequentialByteArrayReader(bytes);

            try
            {
                WhitePointX = reader.GetInt32();
                WhitePointY = reader.GetInt32();
                RedX = reader.GetInt32();
                RedY = reader.GetInt32();
                GreenX = reader.GetInt32();
                GreenY = reader.GetInt32();
                BlueX = reader.GetInt32();
                BlueY = reader.GetInt32();
            }
            catch (IOException ex)
            {
                throw new PngProcessingException(ex);
            }
        }
    }
}
