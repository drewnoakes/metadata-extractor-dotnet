// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
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
        protected T Directory { get; }

        public TagDescriptor(T directory)
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
        /// <see langword="null" /> if the tag hasn't been defined.
        /// </returns>
        public virtual string? GetDescription(int tagType)
        {
            var obj = Directory.GetObject(tagType);
            if (obj is null)
                return null;

            // special presentation for long arrays
            if (obj is ICollection { Count: > 16 } collection)
                return $"[{collection.Count} values]";

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
        /// <item>[0x30 0x32 0x31 0x30] ⇒ 2.10</item>
        /// <item>[0 1 0 0] ⇒ 1.00</item>
        /// </list>
        /// </remarks>
        /// <param name="components">the four version values</param>
        /// <param name="majorDigits">the number of components to be</param>
        /// <returns>the version as a string of form "2.10" or null if the argument cannot be converted</returns>
        [Pure]
        public static string? ConvertBytesToVersionString(int[]? components, int majorDigits)
        {
            if (components is null)
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

            if (version.Length == 0)
                return null;

            return version.ToString();
        }

        [Pure]
        protected string? GetVersionBytesDescription(int tagType, int majorDigits)
        {
            var values = Directory.GetInt32Array(tagType);
            return values is null ? null : ConvertBytesToVersionString(values, majorDigits);
        }

        [Pure]
        protected string? GetIndexedDescription(int tagType, params string?[] descriptions)
        {
            return GetIndexedDescription(tagType, 0, descriptions);
        }

        [Pure]
        protected string? GetIndexedDescription(int tagType, int baseIndex, params string?[] descriptions)
        {
            if (!Directory.TryGetUInt32(tagType, out uint index))
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
        protected string? GetBooleanDescription(int tagType, string trueValue, string falseValue)
        {
            if (!Directory.TryGetBoolean(tagType, out var value))
                return null;

            return value
                ? trueValue
                : falseValue;
        }

        [Pure]
        protected string? GetByteLengthDescription(int tagType)
        {
            var bytes = Directory.GetByteArray(tagType);
            if (bytes is null)
                return null;
            return $"({bytes.Length} byte{(bytes.Length == 1 ? string.Empty : "s")})";
        }

        [Pure]
        protected string? GetSimpleRational(int tagType)
        {
            if (!Directory.TryGetRational(tagType, out Rational value))
                return null;
            return value.ToSimpleString();
        }

        [Pure]
        protected string? GetDecimalRational(int tagType, int decimalPlaces)
        {
            if (!Directory.TryGetRational(tagType, out Rational value))
                return null;
            return string.Format("{0:F" + decimalPlaces + "}", value.ToDouble());
        }

        [Pure]
        protected string? GetFormattedInt(int tagType, string format)
        {
            if (!Directory.TryGetInt32(tagType, out int value))
                return null;
            return string.Format(format, value);
        }

        [Pure]
        protected string? GetFormattedString(int tagType, string format)
        {
            var value = Directory.GetString(tagType);
            if (value is null)
                return null;
            return string.Format(format, value);
        }

        [Pure]
        protected string? GetEpochTimeDescription(int tagType)
        {
            // TODO have observed a byte[8] here which is likely some kind of date (ticks as long?)
            return Directory.TryGetInt64(tagType, out long value)
                ? DateUtil.FromUnixTime(value).ToString("ddd MMM dd HH:mm:ss zzz yyyy")
                : null;
        }

        /// <remarks>LSB first. Labels may be null, a String, or a String[2] with (low label,high label) values.</remarks>
        [Pure]
        protected string? GetBitFlagDescription(int tagType, params object?[] labels)
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
                    if (labelObj is string[] { Length: 2 } labelPair)
                    {
                        parts.Add(labelPair[isBitSet ? 1 : 0]);
                    }
                    else if (isBitSet && labelObj is string label)
                    {
                        parts.Add(label);
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
        protected string? GetStringFrom7BitBytes(int tagType)
        {
            var bytes = Directory.GetByteArray(tagType);
            if (bytes is null)
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
        protected string? GetStringFromUtf8Bytes(int tag)
        {
            var values = Directory.GetByteArray(tag);
            if (values is null)
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
        protected string? GetRationalOrDoubleString(int tagType)
        {
            if (Directory.TryGetRational(tagType, out Rational rational))
                return rational.ToSimpleString();

            if (Directory.TryGetDouble(tagType, out double d))
                return d.ToString("0.###");

            return null;
        }

        [Pure]
        protected static string GetFStopDescription(double fStop) => $"f/{Math.Round(fStop, 1, MidpointRounding.AwayFromZero):0.0}";

        [Pure]
        protected static string GetFocalLengthDescription(double mm) => $"{mm:0.#} mm";

        [Pure]
        protected string? GetLensSpecificationDescription(int tagId)
        {
            var values = Directory.GetRationalArray(tagId);

            if (values is null || values.Length != 4 || values[0].IsZero && values[2].IsZero)
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

        protected string? GetOrientationDescription(int tag)
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

        protected string? GetShutterSpeedDescription(int tagId)
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

        protected string? GetEncodedTextDescription(int tagType)
        {
            var commentBytes = Directory.GetByteArray(tagType);

            if (commentBytes is null)
                return null;

            if (commentBytes.Length == 0)
                return string.Empty;

            // TODO use ByteTrie here
            // Someone suggested "ISO-8859-1".
            var encodingMap = new Dictionary<string, Encoding>
            {
                ["ASCII"] = Encoding.ASCII,
                ["UTF8"] = Encoding.UTF8,
#pragma warning disable SYSLIB0001 // Type or member is obsolete
                ["UTF7"] = Encoding.UTF7,
#pragma warning restore SYSLIB0001 // Type or member is obsolete
                ["UTF32"] = Encoding.UTF32,
                ["UNICODE"] = Encoding.BigEndianUnicode,
            };

            try
            {
                encodingMap["JIS"] = Encoding.GetEncoding("Shift-JIS");
            }
            catch (ArgumentException)
            {
                // On some platforms, 'Shift-JIS' is not a supported encoding name
            }

            try
            {
                if (commentBytes.Length >= 8)
                {
                    // TODO no guarantee bytes after the UTF8 name are valid UTF8 -- only read as many as needed
                    var idCode = Encoding.UTF8.GetString(commentBytes, 0, 8).TrimEnd('\0', ' ');
                    if (encodingMap.TryGetValue(idCode, out var encoding))
                    {
                        var text = encoding.GetString(commentBytes, 8, commentBytes.Length - 8);
                        if (encoding == Encoding.ASCII)
                            text = text.Trim('\0', ' ');
                        return text;
                    }
                }

                // special handling fell through, return a plain string representation
                return Encoding.UTF8.GetString(commentBytes, 0, commentBytes.Length).Trim('\0', ' ');
            }
            catch
            {
                return null;
            }
        }
    }
}
