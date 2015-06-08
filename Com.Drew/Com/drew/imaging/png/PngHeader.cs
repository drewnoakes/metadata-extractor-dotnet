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

        private readonly sbyte _bitsPerSample;

        [NotNull]
        private readonly PngColorType _colorType;

        private readonly sbyte _compressionType;

        private readonly sbyte _filterMethod;

        private readonly sbyte _interlaceMethod;

        /// <exception cref="Com.Drew.Imaging.Png.PngProcessingException"/>
        public PngHeader([NotNull] sbyte[] bytes)
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
                sbyte colorTypeNumber = reader.GetInt8();
                PngColorType colorType = PngColorType.FromNumericValue(colorTypeNumber);
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

        public sbyte GetBitsPerSample()
        {
            return _bitsPerSample;
        }

        [NotNull]
        public PngColorType GetColorType()
        {
            return _colorType;
        }

        public sbyte GetCompressionType()
        {
            return _compressionType;
        }

        public sbyte GetFilterMethod()
        {
            return _filterMethod;
        }

        public sbyte GetInterlaceMethod()
        {
            return _interlaceMethod;
        }
    }
}
