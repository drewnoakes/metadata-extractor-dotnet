using System;
using System.Linq;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Png
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
            var colorTypes = new[] { Greyscale, TrueColor, IndexedColor, GreyscaleWithAlpha, TrueColorWithAlpha };
            return colorTypes.FirstOrDefault(colorType => colorType.NumericValue == numericValue);
        }

        public int NumericValue { get; private set; }

        [NotNull]
        public string Description { get; private set; }

        [NotNull]
        public int[] AllowedBitDepths { get; private set; }

        private PngColorType(int numericValue, [NotNull] string description, [NotNull] params int[] allowedBitDepths)
        {
            NumericValue = numericValue;
            Description = description;
            AllowedBitDepths = allowedBitDepths;
        }
    }
}
