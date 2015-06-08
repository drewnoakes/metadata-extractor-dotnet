/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Com.Drew.Lang;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata
{
    /// <summary>Base class for all tag descriptor classes.</summary>
    /// <remarks>
    /// Base class for all tag descriptor classes.  Implementations are responsible for
    /// providing the human-readable string representation of tag values stored in a directory.
    /// The directory is provided to the tag descriptor via its constructor.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class TagDescriptor<T> : ITagDescriptor
        where T : Directory
    {
        [NotNull]
        protected internal readonly T Directory;

        public TagDescriptor([NotNull] T directory)
        {
            Directory = directory;
        }

        /// <summary>Returns a descriptive value of the specified tag for this image.</summary>
        /// <remarks>
        /// Returns a descriptive value of the specified tag for this image.
        /// Where possible, known values will be substituted here in place of the raw
        /// tokens actually kept in the metadata segment.  If no substitution is
        /// available, the value provided by <code>getString(tagType)</code> will be returned.
        /// </remarks>
        /// <param name="tagType">the tag to find a description for</param>
        /// <returns>
        /// a description of the image's value for the specified tag, or
        /// <code>null</code> if the tag hasn't been defined.
        /// </returns>
        [CanBeNull]
        public virtual string GetDescription(int tagType)
        {
            object @object = Directory.GetObject(tagType);
            if (@object == null)
            {
                return null;
            }
            // special presentation for long arrays
            if (@object.GetType().IsArray)
            {
                int length = Runtime.GetArrayLength(@object);
                if (length > 16)
                {
                    var componentType = @object.GetType().GetElementType();
                    var componentTypeName = componentType == typeof(sbyte)
                        ? "byte"
                        : componentType == typeof(short)
                            ? "short"
                            : componentType == typeof(int)
                                ? "int"
                                : componentType == typeof(long)
                                    ? "long"
                                    : componentType.Name;
                    return Extensions.StringFormat("[%d %s%s]", length, componentTypeName, length == 1 ? string.Empty : "s");
                }
            }
            // no special handling required, so use default conversion to a string
            return Directory.GetString(tagType);
        }

        /// <summary>
        /// Takes a series of 4 bytes from the specified offset, and converts these to a
        /// well-known version number, where possible.
        /// </summary>
        /// <remarks>
        /// Takes a series of 4 bytes from the specified offset, and converts these to a
        /// well-known version number, where possible.
        /// <p>
        /// Two different formats are processed:
        /// <ul>
        /// <li>[30 32 31 30] -&gt; 2.10</li>
        /// <li>[0 1 0 0] -&gt; 1.00</li>
        /// </ul>
        /// </remarks>
        /// <param name="components">the four version values</param>
        /// <param name="majorDigits">the number of components to be</param>
        /// <returns>the version as a string of form "2.10" or null if the argument cannot be converted</returns>
        [CanBeNull]
        public static string ConvertBytesToVersionString([CanBeNull] int[] components, int majorDigits)
        {
            if (components == null)
            {
                return null;
            }
            StringBuilder version = new StringBuilder();
            for (int i = 0; i < 4 && i < components.Length; i++)
            {
                if (i == majorDigits)
                {
                    version.Append('.');
                }
                char c = (char)components[i];
                if (c < '0')
                {
                    c += '0';
                }
                if (i == 0 && c == '0')
                {
                    continue;
                }
                version.Append(c);
            }
            return Extensions.ConvertToString(version);
        }

        [CanBeNull]
        protected internal virtual string GetVersionBytesDescription(int tagType, int majorDigits)
        {
            int[] values = Directory.GetIntArray(tagType);
            return values == null ? null : ConvertBytesToVersionString(values, majorDigits);
        }

        [CanBeNull]
        protected internal virtual string GetIndexedDescription(int tagType, [NotNull] params string[] descriptions)
        {
            return GetIndexedDescription(tagType, 0, descriptions);
        }

        [CanBeNull]
        protected internal virtual string GetIndexedDescription(int tagType, int baseIndex, [NotNull] params string[] descriptions)
        {
            int? index = Directory.GetInteger(tagType);
            if (index == null)
            {
                return null;
            }
            int arrayIndex = (int)index - baseIndex;
            if (arrayIndex >= 0 && arrayIndex < descriptions.Length)
            {
                string description = descriptions[arrayIndex];
                if (description != null)
                {
                    return description;
                }
            }
            return "Unknown (" + index + ")";
        }

        [CanBeNull]
        protected internal virtual string GetByteLengthDescription(int tagType)
        {
            sbyte[] bytes = Directory.GetByteArray(tagType);
            if (bytes == null)
            {
                return null;
            }
            return Extensions.StringFormat("(%d byte%s)", bytes.Length, bytes.Length == 1 ? string.Empty : "s");
        }

        [CanBeNull]
        protected internal virtual string GetSimpleRational(int tagType)
        {
            Rational value = Directory.GetRational(tagType);
            if (value == null)
            {
                return null;
            }
            return value.ToSimpleString(true);
        }

        [CanBeNull]
        protected internal virtual string GetDecimalRational(int tagType, int decimalPlaces)
        {
            Rational value = Directory.GetRational(tagType);
            if (value == null)
            {
                return null;
            }
            return Extensions.StringFormat("%." + decimalPlaces + "f", value.DoubleValue());
        }

        [CanBeNull]
        protected internal virtual string GetFormattedInt(int tagType, [NotNull] string format)
        {
            int? value = Directory.GetInteger(tagType);
            if (value == null)
            {
                return null;
            }
            return Extensions.StringFormat(format, value);
        }

        [CanBeNull]
        protected internal virtual string GetFormattedFloat(int tagType, [NotNull] string format)
        {
            float? value = Directory.GetFloatObject(tagType);
            if (value == null)
            {
                return null;
            }
            return Extensions.StringFormat(format, value);
        }

        [CanBeNull]
        protected internal virtual string GetFormattedString(int tagType, [NotNull] string format)
        {
            string value = Directory.GetString(tagType);
            if (value == null)
            {
                return null;
            }
            return Extensions.StringFormat(format, value);
        }

        [CanBeNull]
        protected internal virtual string GetEpochTimeDescription(int tagType)
        {
            // TODO have observed a byte[8] here which is likely some kind of date (ticks as long?)
            long? value = Directory.GetLongObject(tagType);
            if (value == null)
            {
                return null;
            }
            return Extensions.ConvertToString(Extensions.CreateDate((long)value));
        }

        /// <summary>LSB first.</summary>
        /// <remarks>LSB first. Labels may be null, a String, or a String[2] with (low label,high label) values.</remarks>
        [CanBeNull]
        protected internal virtual string GetBitFlagDescription(int tagType, [NotNull] params object[] labels)
        {
            int? value = Directory.GetInteger(tagType);
            if (value == null)
            {
                return null;
            }
            IList<CharSequence> parts = new AList<CharSequence>();
            int bitIndex = 0;
            while (labels.Length > bitIndex)
            {
                object labelObj = labels[bitIndex];
                if (labelObj != null)
                {
                    bool isBitSet = ((int)value & 1) == 1;
                    if (labelObj is string[])
                    {
                        string[] labelPair = (string[])labelObj;
                        Debug.Assert((labelPair.Length == 2));
                        parts.Add(labelPair[isBitSet ? 1 : 0]);
                    }
                    else
                    {
                        if (isBitSet && labelObj is string)
                        {
                            parts.Add((string)labelObj);
                        }
                    }
                }
                value >>= 1;
                bitIndex++;
            }
            return StringUtil.Join(parts.AsIterable(), ", ");
        }

        [CanBeNull]
        protected internal virtual string Get7BitStringFromBytes(int tagType)
        {
            sbyte[] bytes = Directory.GetByteArray(tagType);
            if (bytes == null)
            {
                return null;
            }
            int length = bytes.Length;
            for (int index = 0; index < bytes.Length; index++)
            {
                int i = bytes[index] & unchecked((int)(0xFF));
                if (i == 0 || i > unchecked((int)(0x7F)))
                {
                    length = index;
                    break;
                }
            }
            return Runtime.GetStringForBytes(bytes, 0, length);
        }

        [CanBeNull]
        protected internal virtual string GetAsciiStringFromBytes(int tag)
        {
            sbyte[] values = Directory.GetByteArray(tag);
            if (values == null)
            {
                return null;
            }
            try
            {
                return Extensions.Trim(Runtime.GetStringForBytes(values, "ASCII"));
            }
            catch (UnsupportedEncodingException)
            {
                return null;
            }
        }
    }
}
