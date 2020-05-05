// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using MetadataExtractor.IO;

using static MetadataExtractor.Formats.Exif.ExifDirectoryBase;

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>Base class for several Exif format descriptor classes.</summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public abstract class ExifDescriptorBase<T> : TagDescriptor<T> where T : Directory
    {
        protected ExifDescriptorBase(T directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            // REMOVED
        }
        
        public string? GetYResolutionDescription()
        {
            var resolution = GetRationalOrDoubleString(TagYResolution);
            if (resolution == null)
                return null;
            var unit = GetResolutionDescription();
            return $"{resolution} dots per {unit?.ToLower() ?? "unit"}";
        }

        public string? GetXResolutionDescription()
        {
            var resolution = GetRationalOrDoubleString(TagXResolution);
            if (resolution == null)
                return null;
            var unit = GetResolutionDescription();
            return $"{resolution} dots per {unit?.ToLower() ?? "unit"}";
        }

        /// <summary>
        /// String description of CFA Pattern
        /// </summary>
        /// <remarks>
        /// Converted from Exiftool version 10.33 created by Phil Harvey
        /// http://www.sno.phy.queensu.ca/~phil/exiftool/
        /// lib\Image\ExifTool\Exif.pm
        ///
        /// Indicates the color filter array (CFA) geometric pattern of the image sensor when a one-chip color area sensor is used.
        /// It does not apply to all sensing methods.
        /// </remarks>
        public string? GetCfaPatternDescription()
        {
            return FormatCFAPattern(DecodeCFAPattern(TagCfaPattern));
        }

        /// <summary>
        /// String description of CFA Pattern
        /// </summary>
        /// <remarks>
        /// Indicates the color filter array (CFA) geometric pattern of the image sensor when a one-chip color area sensor is used.
        /// It does not apply to all sensing methods.
        ///
        /// <see cref="TagCfaPattern2"/> holds only the pixel pattern. <see cref="TagCfaRepeatPatternDim"/> is expected to exist and pass
        /// some conditional tests.
        /// </remarks>
        public string? GetCfaPattern2Description()
        {
            var values = Directory.GetByteArray(TagCfaPattern2);
            if (values == null)
                return null;

            if (!(Directory.GetObject(TagCfaRepeatPatternDim) is ushort[] repeatPattern))
                return $"Repeat Pattern not found for CFAPattern ({base.GetDescription(TagCfaPattern2)})";

            if (repeatPattern.Length == 2 && values.Length == (repeatPattern[0] * repeatPattern[1]))
            {
                var intpattern = new int[2 + values.Length];
                intpattern[0] = repeatPattern[0];
                intpattern[1] = repeatPattern[1];

                Array.Copy(values, 0, intpattern, 2, values.Length);

                return FormatCFAPattern(intpattern);
            }

            return $"Unknown Pattern ({base.GetDescription(TagCfaPattern2)})";
        }

        private static string? FormatCFAPattern(int[]? pattern)
=======
        private static string? FormatCFAPattern(int[] pattern)
>>>>>>> WIP
        {
            if (pattern == null)
                return null;
            if (pattern.Length < 2)
                return "<truncated data>";
            if (pattern[0] == 0 && pattern[1] == 0)
                return "<zero pattern size>";

            var end = 2 + pattern[0] * pattern[1];
            if (end > pattern.Length)
                return "<invalid pattern size>";

            string[] cfaColors = { "Red", "Green", "Blue", "Cyan", "Magenta", "Yellow", "White" };

            var ret = new StringBuilder();
            ret.Append("[");
            for (var pos = 2; pos < end; pos++)
            {
                if (pattern[pos] <= cfaColors.Length - 1)
                    ret.Append(cfaColors[pattern[pos]]);
                else
                    ret.Append("Unknown");  // indicated pattern position is outside the array bounds

                if ((pos - 2) % pattern[1] == 0)
                    ret.Append(",");
                else if (pos != end - 1)
                    ret.Append("][");
            }
            ret.Append("]");

            return ret.ToString();
        }

        /// <summary>
        /// Decode raw CFAPattern value
        /// </summary>
        /// <remarks>
        /// Converted from Exiftool version 10.33 created by Phil Harvey
        /// http://www.sno.phy.queensu.ca/~phil/exiftool/
        /// lib\Image\ExifTool\Exif.pm
        ///
        /// The value consists of:
        /// - Two short, being the grid width and height of the repeated pattern.
        /// - Next, for every pixel in that pattern, an identification code.
        /// </remarks>
        private int[]? DecodeCFAPattern(int tagType)
        {
            int[] ret;

            var values = Directory.GetByteArray(tagType);
            if (values == null)
                return null;

            if (values.Length < 4)
            {
                ret = new int[values.Length];
                for (var i = 0; i < values.Length; i++)
                    ret[i] = values[i];
                return ret;
            }

            IndexedReader reader = new ByteArrayReader(values);

            // first two values should be read as 16-bits (2 bytes)
            var item0 = reader.GetInt16(0);
            var item1 = reader.GetInt16(2);

            ret = new int[values.Length - 2];

            var copyArray = false;
            var end = 2 + item0 * item1;
            if (end > values.Length) // sanity check in case of byte order problems; calculated 'end' should be <= length of the values
            {
                // try swapping byte order (I have seen this order different than in EXIF)
                reader = reader.WithByteOrder(!reader.IsMotorolaByteOrder);
                item0 = reader.GetInt16(0);
                item1 = reader.GetInt16(2);

                if (values.Length >= 2 + item0 * item1)
                    copyArray = true;
            }
            else
            {
                copyArray = true;
            }

            if (copyArray)
            {
                ret[0] = item0;
                ret[1] = item1;

                for (var i = 4; i < values.Length; i++)
                    ret[i - 2] = reader.GetByte(i);
            }
            return ret;
        }

        public string? GetFocalPlaneXResolutionDescription()
        {
            if (!Directory.TryGetRational(TagFocalPlaneXResolution, out Rational value))
                return null;
            var unit = GetFocalPlaneResolutionUnitDescription();
            return value.Reciprocal.ToSimpleString() + (unit == null ? string.Empty : " " + unit.ToLower());
        }

        public string? GetFocalPlaneYResolutionDescription()
        {
            if (!Directory.TryGetRational(TagFocalPlaneYResolution, out Rational value))
                return null;
            var unit = GetFocalPlaneResolutionUnitDescription();
            return value.Reciprocal.ToSimpleString() + (unit == null ? string.Empty : " " + unit.ToLower());
        }
    }
}
