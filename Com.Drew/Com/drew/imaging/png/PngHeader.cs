using System.IO;
using Com.Drew.Lang;
using JetBrains.Annotations;

namespace Com.Drew.Imaging.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PngHeader
    {
        private readonly int _imageWidth;

        private readonly int _imageHeight;

        private readonly byte _bitsPerSample;

        [NotNull]
        private readonly PngColorType _colorType;

        private readonly byte _compressionType;

        private readonly byte _filterMethod;

        private readonly byte _interlaceMethod;

        /// <exception cref="Com.Drew.Imaging.Png.PngProcessingException"/>
        public PngHeader([NotNull] byte[] bytes)
        {
            if (bytes.Length != 13)
            {
                throw new PngProcessingException("PNG header chunk must have 13 data bytes");
            }
            SequentialReader reader = new SequentialByteArrayReader(bytes);
            try
            {
                _imageWidth = reader.GetInt32();
                _imageHeight = reader.GetInt32();
                _bitsPerSample = reader.GetInt8();
                var colorTypeNumber = reader.GetInt8();
                var colorType = PngColorType.FromNumericValue(colorTypeNumber);
                if (colorType == null)
                {
                    throw new PngProcessingException("Unexpected PNG color type: " + colorTypeNumber);
                }
                _colorType = colorType;
                _compressionType = reader.GetInt8();
                _filterMethod = reader.GetInt8();
                _interlaceMethod = reader.GetInt8();
            }
            catch (IOException e)
            {
                // Should never happen
                throw new PngProcessingException(e);
            }
        }

        public int GetImageWidth()
        {
            return _imageWidth;
        }

        public int GetImageHeight()
        {
            return _imageHeight;
        }

        public byte GetBitsPerSample()
        {
            return _bitsPerSample;
        }

        [NotNull]
        public PngColorType GetColorType()
        {
            return _colorType;
        }

        public byte GetCompressionType()
        {
            return _compressionType;
        }

        public byte GetFilterMethod()
        {
            return _filterMethod;
        }

        public byte GetInterlaceMethod()
        {
            return _interlaceMethod;
        }
    }
}
