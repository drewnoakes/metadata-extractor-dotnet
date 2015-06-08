using System.IO;
using Com.Drew.Lang;
using JetBrains.Annotations;

namespace Com.Drew.Imaging.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PngChromaticities
    {
        private readonly int _whitePointX;

        private readonly int _whitePointY;

        private readonly int _redX;

        private readonly int _redY;

        private readonly int _greenX;

        private readonly int _greenY;

        private readonly int _blueX;

        private readonly int _blueY;

        /// <exception cref="Com.Drew.Imaging.Png.PngProcessingException"/>
        public PngChromaticities([NotNull] sbyte[] bytes)
        {
            if (bytes.Length != 8 * 4)
            {
                throw new PngProcessingException("Invalid number of bytes");
            }
            SequentialByteArrayReader reader = new SequentialByteArrayReader(bytes);
            try
            {
                _whitePointX = reader.GetInt32();
                _whitePointY = reader.GetInt32();
                _redX = reader.GetInt32();
                _redY = reader.GetInt32();
                _greenX = reader.GetInt32();
                _greenY = reader.GetInt32();
                _blueX = reader.GetInt32();
                _blueY = reader.GetInt32();
            }
            catch (IOException ex)
            {
                throw new PngProcessingException(ex);
            }
        }

        public int GetWhitePointX()
        {
            return _whitePointX;
        }

        public int GetWhitePointY()
        {
            return _whitePointY;
        }

        public int GetRedX()
        {
            return _redX;
        }

        public int GetRedY()
        {
            return _redY;
        }

        public int GetGreenX()
        {
            return _greenX;
        }

        public int GetGreenY()
        {
            return _greenY;
        }

        public int GetBlueX()
        {
            return _blueX;
        }

        public int GetBlueY()
        {
            return _blueY;
        }
    }
}
