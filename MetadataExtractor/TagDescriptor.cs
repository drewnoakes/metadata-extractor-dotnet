#region License
//
// Copyright 2002-2016 Drew Noakes
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using JetBrains.Annotations;
using MetadataExtractor.Util;

namespace MetadataExtractor
{
    /// <summary>Base class for all tag descriptor classes.</summary>
    /// <remarks>
    /// Implementations are responsible for providing the human-readable string representation of tag values stored in a directory.
    /// The directory is provided to the tag descriptor via its constructor.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class TagDescriptor<T> : ITagDescriptor
        where T : Directory
    {
        [NotNull]
        protected readonly T Directory;

        public TagDescriptor([NotNull] T directory)
        {
            Directory = directory;
        }

        /// <summary>Returns a descriptive value of the specified tag for this image.</summary>
        /// <remarks>
        /// Where possible, known values will be substituted here in place of the raw
        /// tokens actually kept in the metadata segment.  If no substitution is
        /// available, the value provided by <c>getString(tagType)</c> will be returned.
        /// </remarks>
        /// <param name="tagType">the tag to find a description for</param>
        /// <returns>
        /// a description of the image's value for the specified tag, or
        /// <c>null</c> if the tag hasn't been defined.
        /// </returns>
        public virtual string GetDescription(int tagType)
        {
            var obj = Directory.GetObject(tagType);
            if (obj == null)
                return null;

            // special presentation for long arrays
            var array = obj as Array;
            if (array != null && array.Length > 16)
                return $"[{array.Length} {(array.Length == 1 ? "value" : "values")}]";

            // no special handling required, so use default conversion to a string
            return Directory.GetString(tagType);
        }

        /// <summary>
        /// Takes a series of 4 bytes from the specified offset, and converts these to a
        /// well-known version number, where possible.
        /// </summary>
        /// <remarks>
        /// Two different formats are processed:
        /// <list type="bullet">
        /// <item>[30 32 31 30] -&gt; 2.10</item>
        /// <item>[0 1 0 0] -&gt; 1.00</item>
        /// </list>
        /// </remarks>
        /// <param name="components">the four version values</param>
        /// <param name="majorDigits">the number of components to be</param>
        /// <returns>the version as a string of form "2.10" or null if the argument cannot be converted</returns>
        [CanBeNull]
        public static string ConvertBytesToVersionString([CanBeNull] int[] components, int majorDigits)
        {
            if (components == null)
                return null;

            var version = new StringBuilder();
            for (var i = 0; i < 4 && i < components.Length; i++)
            {
                if (i == majorDigits)
                    version.Append('.');
                var c = (char)components[i];
                if (c < '0')
                    c += '0';
                if (i == 0 && c == '0')
                    continue;
                version.Append(c);
            }
            return version.ToString();
        }

        [CanBeNull]
        protected string GetVersionBytesDescription(int tagType, int majorDigits)
        {
            var values = Directory.GetInt32Array(tagType);
            return values == null ? null : ConvertBytesToVersionString(values, majorDigits);
        }

        [CanBeNull]
        protected string GetIndexedDescription(int tagType, [NotNull] params string[] descriptions)
        {
            return GetIndexedDescription(tagType, 0, descriptions);
        }

        [CanBeNull]
        protected string GetIndexedDescription(int tagType, int baseIndex, [NotNull] params string[] descriptions)
        {
            int index;
            if (!Directory.TryGetInt32(tagType, out index))
                return null;

            var arrayIndex = index - baseIndex;

            if (arrayIndex >= 0 && arrayIndex < descriptions.Length)
            {
                var description = descriptions[arrayIndex];
                if (description != null)
                    return description;
            }

            return "Unknown (" + index + ")";
        }

        [CanBeNull]
        protected string GetByteLengthDescription(int tagType)
        {
            var bytes = Directory.GetByteArray(tagType);
            if (bytes == null)
                return null;
            return $"({bytes.Length} byte{(bytes.Length == 1 ? string.Empty : "s")})";
        }

        [CanBeNull]
        protected string GetSimpleRational(int tagType)
        {
            Rational value;
            if (!Directory.TryGetRational(tagType, out value))
                return null;
            return value.ToSimpleString();
        }

        [CanBeNull]
        protected string GetDecimalRational(int tagType, int decimalPlaces)
        {
            Rational value;
            if (!Directory.TryGetRational(tagType, out value))
                return null;
            return string.Format("{0:F" + decimalPlaces + "}", value.ToDouble());
        }

        [CanBeNull]
        protected string GetFormattedInt(int tagType, [NotNull] string format)
        {
            int value;
            if (!Directory.TryGetInt32(tagType, out value))
                return null;
            return string.Format(format, value);
        }

        [CanBeNull]
        protected string GetFormattedString(int tagType, [NotNull] string format)
        {
            var value = Directory.GetString(tagType);
            if (value == null)
                return null;
            return string.Format(format, value);
        }

        [CanBeNull]
        protected string GetEpochTimeDescription(int tagType)
        {
            // TODO have observed a byte[8] here which is likely some kind of date (ticks as long?)
            long value;
            return Directory.TryGetInt64(tagType, out value)
                ? DateUtil.FromUnixTime(value).ToString("ddd MMM dd HH:mm:ss zzz yyyy")
                : null;
        }

        /// <remarks>LSB first. Labels may be null, a String, or a String[2] with (low label,high label) values.</remarks>
        [CanBeNull]
        protected string GetBitFlagDescription(int tagType, [NotNull] params object[] labels)
        {
            int value;
            if (!Directory.TryGetInt32(tagType, out value))
                return null;
            var parts = new List<string>();
            var bitIndex = 0;
            while (labels.Length > bitIndex)
            {
                var labelObj = labels[bitIndex];
                if (labelObj != null)
                {
                    var isBitSet = (value & 1) == 1;
                    var obj = labelObj as string[];
                    if (obj != null)
                    {
                        var labelPair = obj;
                        Debug.Assert(labelPair.Length == 2);
                        parts.Add(labelPair[isBitSet ? 1 : 0]);
                    }
                    else if (isBitSet && labelObj is string)
                    {
                        parts.Add((string)labelObj);
                    }
                }
                value >>= 1;
                bitIndex++;
            }
#if NET35
            return string.Join(", ", parts.ToArray());
#else
            return string.Join(", ", parts);
#endif
        }

        [CanBeNull]
        protected string Get7BitStringFromBytes(int tagType)
        {
            var bytes = Directory.GetByteArray(tagType);
            if (bytes == null)
                return null;
            var length = bytes.Length;
            for (var index = 0; index < bytes.Length; index++)
            {
                var i = bytes[index] & 0xFF;
                if (i == 0 || i > 0x7F)
                {
                    length = index;
                    break;
                }
            }
            return Encoding.UTF8.GetString(bytes, 0, length);
        }

        [CanBeNull]
        protected string GetAsciiStringFromBytes(int tag)
        {
            var values = Directory.GetByteArray(tag);
            if (values == null)
                return null;

            try
            {
#if PORTABLE
                var encoding = Encoding.UTF8;
#else
                var encoding = Encoding.ASCII;
#endif
                return encoding
                    .GetString(values, 0, values.Length)
                    .Trim('\0', ' ', '\r', '\n', '\t');
            }
            catch
            {
                return null;
            }
        }

        [CanBeNull]
        protected string GetRationalOrDoubleString(int tagType)
        {
            Rational rational;
            if (Directory.TryGetRational(tagType, out rational))
                return rational.ToSimpleString();

            double d;
            if (Directory.TryGetDouble(tagType, out d))
                return d.ToString("0.###");

            return null;
        }

        protected static string GetFStopDescription(double fStop) => $"f/{fStop:0.0}";

        protected static string GetFocalLengthDescription(double mm) => $"{mm:0.#} mm";

        [CanBeNull]
        public string GetLensSpecificationDescription(int tagId)
        {
            var values = Directory.GetRationalArray(tagId);

            if (values == null || (values != null && (values.Length != 4 || (values[0].ToDouble() == 0 && values[2].ToDouble() == 0))))
                return null;

            StringBuilder sb = new StringBuilder();

            if (values[0].Equals(values[1]))
                sb.Append(values[0].ToSimpleString(true)).Append("mm");
            else
                sb.Append(values[0].ToSimpleString(true)).Append("-").Append(values[1].ToSimpleString(true) + "mm");

            if (values[2].ToDouble() != 0)
            {
                sb.Append(" ");

                if (values[2].Equals(values[3]))
                    sb.Append(GetFStopDescription(values[2].ToDouble()));
                else
                    sb.Append("f/")
#if !PORTABLE
                      .Append(Math.Round(values[2].ToDouble(), 1, MidpointRounding.AwayFromZero).ToString("0.0"))
#else
                      .Append(Math.Round(values[2].ToDouble(), 1).ToString("0.0"))
#endif
                      .Append("-")
#if !PORTABLE
                      .Append(Math.Round(values[3].ToDouble(), 1, MidpointRounding.AwayFromZero).ToString("0.0"));
#else
                      .Append(Math.Round(values[3].ToDouble(), 1).ToString("0.0"));
#endif
            }

            return sb.ToString();
        }
    }
}
