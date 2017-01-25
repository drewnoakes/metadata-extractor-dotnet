#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

#if !NETSTANDARD1_3
using System;
#endif
using System.Linq;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
#if !NETSTANDARD1_3
    [Serializable]
#endif
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

        [NotNull]
        public static PngColorType FromNumericValue(int numericValue)
        {
            var colorTypes = new[] { Greyscale, TrueColor, IndexedColor, GreyscaleWithAlpha, TrueColorWithAlpha };
            return colorTypes.FirstOrDefault(colorType => colorType.NumericValue == numericValue)
                ?? new PngColorType(numericValue, $"Unknown ({numericValue})");
        }

        public int NumericValue { get; }

        [NotNull]
        public string Description { get; }

        [NotNull]
        public int[] AllowedBitDepths { get; }

        private PngColorType(int numericValue, [NotNull] string description, [NotNull] params int[] allowedBitDepths)
        {
            NumericValue = numericValue;
            Description = description;
            AllowedBitDepths = allowedBitDepths;
        }
    }
}
