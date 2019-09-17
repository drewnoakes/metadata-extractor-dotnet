// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Text;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.QuickTime
{
    /// <summary>
    /// Extension methods for reading QuickTime specific encodings from a <see cref="SequentialReader"/>.
    /// </summary>
    public static class QuickTimeReaderExtensions
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static string Get4ccString(this SequentialReader reader)
        {
            var sb = new StringBuilder(4);
            sb.Append((char) reader.GetByte());
            sb.Append((char) reader.GetByte());
            sb.Append((char) reader.GetByte());
            sb.Append((char) reader.GetByte());
            return sb.ToString();
        }

        public static decimal Get16BitFixedPoint(this SequentialReader reader)
        {
            return decimal.Add(
                reader.GetByte(),
                decimal.Divide(reader.GetByte(), byte.MaxValue));
        }

        public static decimal Get32BitFixedPoint(this SequentialReader reader)
        {
            return decimal.Add(
                reader.GetUInt16(),
                decimal.Divide(reader.GetUInt16(), ushort.MaxValue));
        }

        private static decimal GetS32BitFixedPoint(this SequentialReader reader)
        {
            return decimal.Add(
                reader.GetInt16(),
                decimal.Divide(reader.GetUInt16(), ushort.MaxValue));
        }

        /// <summary>
        /// Returns a matrix as float[9].
        /// See <a href="https://developer.apple.com/library/archive/documentation/QuickTime/QTFF/QTFFChap4/qtff4.html#//apple_ref/doc/uid/TP40000939-CH206-18737">QuickTime File Format Specification</a>.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>System.Single[].</returns>
        public static float[] GetMatrix(this SequentialReader reader)
        {
            var matrix = new float[9];
            for (var i = 0; i < matrix.Length; i++)
            {
                var val = reader.GetS32BitFixedPoint();
                // the right column is fixed 2.30 instead of 16.16
                if (i == 2 || i == 5 || i == 8)
                {
                    val /= 0x4000;
                }
                matrix[i] = (float)val;
            }
            return matrix;
        }
    }
}
