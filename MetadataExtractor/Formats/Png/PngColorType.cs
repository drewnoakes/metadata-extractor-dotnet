// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Linq;

namespace MetadataExtractor.Formats.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [Serializable]
    public sealed class PngColorType
    {
        /// <summary>Each pixel is a greyscale sample.</summary>
        public static readonly PngColorType Greyscale = new(0, "Greyscale", 1, 2, 4, 8, 16);

        /// <summary>Each pixel is an R,G,B triple.</summary>
        public static readonly PngColorType TrueColor = new(2, "True Color", 8, 16);

        /// <summary>Each pixel is a palette index.</summary>
        /// <remarks>Each pixel is a palette index. Seeing this value indicates that a <c>PLTE</c> chunk shall appear.</remarks>
        public static readonly PngColorType IndexedColor = new(3, "Indexed Color", 1, 2, 4, 8);

        /// <summary>Each pixel is a greyscale sample followed by an alpha sample.</summary>
        public static readonly PngColorType GreyscaleWithAlpha = new(4, "Greyscale with Alpha", 8, 16);

        /// <summary>Each pixel is an R,G,B triple followed by an alpha sample.</summary>
        public static readonly PngColorType TrueColorWithAlpha = new(6, "True Color with Alpha", 8, 16);

        public static PngColorType FromNumericValue(int numericValue)
        {
            var colorTypes = new[] { Greyscale, TrueColor, IndexedColor, GreyscaleWithAlpha, TrueColorWithAlpha };
            return colorTypes.FirstOrDefault(colorType => colorType.NumericValue == numericValue)
                ?? new PngColorType(numericValue, $"Unknown ({numericValue})");
        }

        public int NumericValue { get; }

        public string Description { get; }

        public int[] AllowedBitDepths { get; }

        private PngColorType(int numericValue, string description, params int[] allowedBitDepths)
        {
            NumericValue = numericValue;
            Description = description;
            AllowedBitDepths = allowedBitDepths;
        }
    }
}
