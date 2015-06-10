using System;
using System.Linq;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Imaging.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [Serializable]
    public sealed class PngColorType
    {
        /// <summary>Each pixel is a greyscale sample.</summary>
        public static readonly PngColorType Greyscale = new PngColorType(0, "Greyscale", 1, 2, 4, 8, 16);

        /// <summary>Each pixel is an R,G,B triple.</summary>
        public static readonly PngColorType TrueColor = new PngColorType(2, "True Color", 8, 16);

        /// <summary>Each pixel is a palette index.</summary>
        /// <remarks>Each pixel is a palette index. Seeing this value indicates that a <c>PLTE</c> chunk shall appear.</remarks>
        public static readonly PngColorType IndexedColor = new PngColorType(3, "Indexed Color", 1, 2, 4, 8);

        /// <summary>Each pixel is a greyscale sample followed by an alpha sample.</summary>
        public static readonly PngColorType GreyscaleWithAlpha = new PngColorType(4, "Greyscale with Alpha", 8, 16);

        /// <summary>Each pixel is an R,G,B triple followed by an alpha sample.</summary>
        public static readonly PngColorType TrueColorWithAlpha = new PngColorType(6, "True Color with Alpha", 8, 16);

        [CanBeNull]
        public static PngColorType FromNumericValue(int numericValue)
        {
            var colorTypes = typeof(PngColorType).GetEnumConstants<PngColorType>();
            return colorTypes.FirstOrDefault(colorType => colorType.GetNumericValue() == numericValue);
        }

        private readonly int _numericValue;

        [NotNull]
        private readonly string _description;

        [NotNull]
        private readonly int[] _allowedBitDepths;

        private PngColorType(int numericValue, [NotNull] string description, [NotNull] params int[] allowedBitDepths)
        {
            _numericValue = numericValue;
            _description = description;
            _allowedBitDepths = allowedBitDepths;
        }

        public int GetNumericValue()
        {
            return _numericValue;
        }

        [NotNull]
        public string GetDescription()
        {
            return _description;
        }

        [NotNull]
        public int[] GetAllowedBitDepths()
        {
            return _allowedBitDepths;
        }
    }
}
