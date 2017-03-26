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
            if (obj is Array array && array.Length > 16)
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
        [Pure]
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

        [Pure]
        [CanBeNull]
        protected string GetVersionBytesDescription(int tagType, int majorDigits)
        {
            var values = Directory.GetInt32Array(tagType);
            return values == null ? null : ConvertBytesToVersionString(values, majorDigits);
        }

        [Pure]
        [CanBeNull]
        protected string GetIndexedDescription(int tagType, [NotNull] params string[] descriptions)
        {
            return GetIndexedDescription(tagType, 0, descriptions);
        }

        [Pure]
        [CanBeNull]
        protected string GetIndexedDescription(int tagType, int baseIndex, [NotNull] params string[] descriptions)
        {
            if (!Directory.TryGetInt32(tagType, out int index))
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

        [Pure]
        [CanBeNull]
        protected string GetByteLengthDescription(int tagType)
        {
            var bytes = Directory.GetByteArray(tagType);
            if (bytes == null)
                return null;
            return $"({bytes.Length} byte{(bytes.Length == 1 ? string.Empty : "s")})";
        }

        [Pure]
        [CanBeNull]
        protected string GetSimpleRational(int tagType)
        {
            if (!Directory.TryGetRational(tagType, out Rational value))
                return null;
            return value.ToSimpleString();
        }

        [Pure]
        [CanBeNull]
        protected string GetDecimalRational(int tagType, int decimalPlaces)
        {
            if (!Directory.TryGetRational(tagType, out Rational value))
                return null;
            return string.Format("{0:F" + decimalPlaces + "}", value.ToDouble());
        }

        [Pure]
        [CanBeNull]
        protected string GetFormattedInt(int tagType, [NotNull] string format)
        {
            if (!Directory.TryGetInt32(tagType, out int value))
                return null;
            return string.Format(format, value);
        }

        [Pure]
        [CanBeNull]
        protected string GetFormattedString(int tagType, [NotNull] string format)
        {
            var value = Directory.GetString(tagType);
            if (value == null)
                return null;
            return string.Format(format, value);
        }

        [Pure]
        [CanBeNull]
        protected string GetEpochTimeDescription(int tagType)
        {
            // TODO have observed a byte[8] here which is likely some kind of date (ticks as long?)
            return Directory.TryGetInt64(tagType, out long value)
                ? DateUtil.FromUnixTime(value).ToString("ddd MMM dd HH:mm:ss zzz yyyy")
                : null;
        }

        /// <remarks>LSB first. Labels may be null, a String, or a String[2] with (low label,high label) values.</remarks>
        [Pure]
        [CanBeNull]
        protected string GetBitFlagDescription(int tagType, [NotNull] params object[] labels)
        {
            if (!Directory.TryGetInt32(tagType, out int value))
                return null;
            var parts = new List<string>();
            var bitIndex = 0;
            while (labels.Length > bitIndex)
            {
                var labelObj = labels[bitIndex];
                if (labelObj != null)
                {
                    var isBitSet = (value & 1) == 1;
                    if (labelObj is string[] obj)
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

        [Pure]
        [CanBeNull]
        protected string GetStringFrom7BitBytes(int tagType)
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

        [Pure]
        [CanBeNull]
        protected string GetStringFromUtf8Bytes(int tag)
        {
            var values = Directory.GetByteArray(tag);
            if (values == null)
                return null;

            try
            {
                return Encoding.UTF8
                    .GetString(values, 0, values.Length)
                    .Trim('\0', ' ', '\r', '\n', '\t');
            }
            catch
            {
                return null;
            }
        }

        [Pure]
        [CanBeNull]
        protected string GetRationalOrDoubleString(int tagType)
        {
            if (Directory.TryGetRational(tagType, out Rational rational))
                return rational.ToSimpleString();

            if (Directory.TryGetDouble(tagType, out double d))
                return d.ToString("0.###");

            return null;
        }

        [Pure]
        [NotNull]
        protected static string GetFStopDescription(double fStop) => $"f/{Math.Round(fStop, 1, MidpointRounding.AwayFromZero):0.0}";

        [Pure]
        [NotNull]
        protected static string GetFocalLengthDescription(double mm) => $"{mm:0.#} mm";

        [Pure]
        [CanBeNull]
        protected string GetLensSpecificationDescription(int tagId)
        {
            var values = Directory.GetRationalArray(tagId);

            if (values == null || values.Length != 4 || values[0].IsZero && values[2].IsZero)
                return null;

            var sb = new StringBuilder();

            if (values[0] == values[1])
                sb.Append(values[0].ToSimpleString()).Append("mm");
            else
                sb.Append(values[0].ToSimpleString()).Append("-").Append(values[1].ToSimpleString()).Append("mm");

            if (!values[2].IsZero)
            {
                sb.Append(' ');

                if (values[2] == values[3])
                    sb.Append(GetFStopDescription(values[2].ToDouble()));
                else
                    sb.Append("f/")
#if !NETSTANDARD1_3
                      .Append(Math.Round(values[2].ToDouble(), 1, MidpointRounding.AwayFromZero).ToString("0.0"))
#else
                      .Append(Math.Round(values[2].ToDouble(), 1).ToString("0.0"))
#endif
                      .Append("-")
#if !NETSTANDARD1_3
                      .Append(Math.Round(values[3].ToDouble(), 1, MidpointRounding.AwayFromZero).ToString("0.0"));
#else
                      .Append(Math.Round(values[3].ToDouble(), 1).ToString("0.0"));
#endif
            }

            return sb.ToString();
        }

        [CanBeNull]
        protected string GetOrientationDescription(int tag)
        {
            return GetIndexedDescription(tag, 1,
                "Top, left side (Horizontal / normal)",
                "Top, right side (Mirror horizontal)",
                "Bottom, right side (Rotate 180)", "Bottom, left side (Mirror vertical)",
                "Left side, top (Mirror horizontal and rotate 270 CW)",
                "Right side, top (Rotate 90 CW)",
                "Right side, bottom (Mirror horizontal and rotate 90 CW)",
                "Left side, bottom (Rotate 270 CW)");
        }

        [CanBeNull]
        protected string GetShutterSpeedDescription(int tagId)
        {
            // I believe this method to now be stable, but am leaving some alternative snippets of
            // code in here, to assist anyone who's looking into this (given that I don't have a public CVS).
            //        float apexValue = _directory.getFloat(ExifSubIFDDirectory.TAG_SHUTTER_SPEED);
            //        int apexPower = (int)Math.pow(2.0, apexValue);
            //        return "1/" + apexPower + " sec";
            // TODO test this method
            // thanks to Mark Edwards for spotting and patching a bug in the calculation of this
            // description (spotted bug using a Canon EOS 300D)
            // thanks also to Gli Blr for spotting this bug
            if (!Directory.TryGetSingle(tagId, out float apexValue))
                return null;

            if (apexValue <= 1)
            {
                var apexPower = (float)(1 / Math.Exp(apexValue * Math.Log(2)));
                var apexPower10 = (long)Math.Round(apexPower * 10.0);
                var fApexPower = apexPower10 / 10.0f;
                return fApexPower + " sec";
            }
            else
            {
                var apexPower = (int)Math.Exp(apexValue * Math.Log(2));
                return "1/" + apexPower + " sec";
            }
        }

        // EXIF LightSource
        [CanBeNull]
        protected string GetLightSourceDescription(ushort wbtype)
        {
            switch (wbtype)
            {
                case 0:
                    return "Unknown";
                case 1:
                    return "Daylight";
                case 2:
                    return "Fluorescent";
                case 3:
                    return "Tungsten (Incandescent)";
                case 4:
                    return "Flash";
                case 9:
                    return "Fine Weather";
                case 10:
                    return "Cloudy";
                case 11:
                    return "Shade";
                case 12:
                    return "Daylight Fluorescent";    // (D 5700 - 7100K)
                case 13:
                    return "Day White Fluorescent";   // (N 4600 - 5500K)
                case 14:
                    return "Cool White Fluorescent";  // (W 3800 - 4500K)
                case 15:
                    return "White Fluorescent";       // (WW 3250 - 3800K)
                case 16:
                    return "Warm White Fluorescent";  // (L 2600 - 3250K)
                case 17:
                    return "Standard Light A";
                case 18:
                    return "Standard Light B";
                case 19:
                    return "Standard Light C";
                case 20:
                    return "D55";
                case 21:
                    return "D65";
                case 22:
                    return "D75";
                case 23:
                    return "D50";
                case 24:
                    return "ISO Studio Tungsten";
                case 255:
                    return "Other";
            }

            return GetDescription(wbtype);
        }
    }
}
