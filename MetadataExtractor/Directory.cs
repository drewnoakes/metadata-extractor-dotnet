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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using JetBrains.Annotations;
using Sharpen;

namespace MetadataExtractor
{
    /// <summary>
    /// Abstract base class for all directory implementations, having methods for getting and setting tag values of various
    /// data types.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public abstract class Directory
    {
        /// <summary>Map of values hashed by type identifiers.</summary>
        [NotNull]
        private readonly Dictionary<int?, object> _tagMap = new Dictionary<int?, object>();

        /// <summary>Holds tags in the order in which they were stored.</summary>
        [NotNull]
        private readonly List<Tag> _definedTagList = new List<Tag>();

        [NotNull]
        private readonly List<string> _errorList = new List<string>(4);

        /// <summary>The descriptor used to interpret tag values.</summary>
        private ITagDescriptor _descriptor;

        /// <summary>Provides the name of the directory, for display purposes.</summary>
        /// <value>the name of the directory</value>
        [NotNull]
        public abstract string Name { get; }

        /// <summary>Provides the map of tag names, hashed by tag type identifier.</summary>
        /// <returns>the map of tag names</returns>
        [NotNull]
        protected abstract IReadOnlyDictionary<int?, string> GetTagNameMap();

        /// <summary>Gets a value indicating whether the directory is empty, meaning it contains no errors and no tag values.</summary>
        public bool IsEmpty
        {
            get { return _errorList.Count == 0 && _definedTagList.Count == 0; }
        }

        /// <summary>Indicates whether the specified tag type has been set.</summary>
        /// <param name="tagType">the tag type to check for</param>
        /// <returns>true if a value exists for the specified tag type, false if not</returns>
        public bool ContainsTag(int tagType)
        {
            return _tagMap.ContainsKey(tagType);
        }

        /// <summary>Returns an Iterator of Tag instances that have been set in this Directory.</summary>
        /// <value>The list of <see cref="Tag"/> instances</value>
        [NotNull]
        public IReadOnlyList<Tag> Tags
        {
            get { return _definedTagList; }
        }

        /// <summary>Returns the number of tags set in this Directory.</summary>
        /// <value>the number of tags set in this Directory</value>
        public int TagCount
        {
            get { return _definedTagList.Count; }
        }

        /// <summary>Sets the descriptor used to interpret tag values.</summary>
        /// <param name="descriptor">the descriptor used to interpret tag values</param>
        public void SetDescriptor([NotNull] ITagDescriptor descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException("descriptor");

            _descriptor = descriptor;
        }

        /// <summary>Registers an error message with this directory.</summary>
        /// <param name="message">an error message.</param>
        public void AddError([NotNull] string message)
        {
            _errorList.Add(message);
        }

        /// <summary>Gets a value indicating whether this directory has any error messages.</summary>
        /// <value>true if the directory contains errors, otherwise false</value>
        public bool HasErrors
        {
            get { return _errorList.Count > 0; }
        }

        /// <summary>Used to iterate over any error messages contained in this directory.</summary>
        /// <value>The collection of error message strings.</value>
        [NotNull]
        public IReadOnlyCollection<string> Errors
        {
            get { return _errorList; }
        }

        /// <summary>Returns the count of error messages in this directory.</summary>
        public int ErrorCount
        {
            get { return _errorList.Count; }
        }

        #region Tag Setters

        /// <summary>Sets a <c>Object</c> for the specified tag.</summary>
        /// <remarks>Any previous value for this tag is overwritten.</remarks>
        /// <param name="tagType">the tag's value as an int</param>
        /// <param name="value">the value for the specified tag</param>
        /// <exception cref="ArgumentNullException">if value is <c>null</c></exception>
        public virtual void Set(int tagType, [NotNull] object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (!_tagMap.ContainsKey(tagType))
                _definedTagList.Add(new Tag(tagType, this));

            _tagMap[tagType] = value;
        }

        #endregion

        #region Tag Getters

        /// <summary>Returns the specified tag's value as an int, if possible.</summary>
        /// <remarks>
        /// Every attempt to represent the tag's value as an int is taken.
        /// Here is a list of the action taken depending upon the tag's original type:
        /// <list type="bullet">
        /// <item> int - Return unchanged.</item>
        /// <item> Number - Return an int value (real numbers are truncated).</item>
        /// <item> Rational - Truncate any fractional part and returns remaining int.</item>
        /// <item> String - Attempt to parse string as an int.  If this fails, convert the char[] to an int (using shifts and OR).</item>
        /// <item> Rational[] - Return int value of first item in array.</item>
        /// <item> byte[] - Return int value of first item in array.</item>
        /// <item> int[] - Return int value of first item in array.</item>
        /// </list>
        /// </remarks>
        /// <exception cref="MetadataException">if no value exists for tagType or if it cannot be converted to an int.</exception>
        public int GetInt(int tagType)
        {
            var integer = GetInteger(tagType);

            if (integer != null)
                return (int)integer;

            var o = GetObject(tagType);
            if (o == null)
                throw new MetadataException("Tag '" + GetTagName(tagType) + "' has not been set -- check using containsTag() first");

            throw new MetadataException("Tag '" + tagType + "' cannot be converted to int.  It is of type '" + o.GetType() + "'.");
        }

        /// <summary>Returns the specified tag's value as an integer, if possible.</summary>
        /// <remarks>
        /// Every attempt to represent the tag's value as an integer is taken.
        /// Here is a list of the action taken depending upon the tag's original type:
        /// <list type="bullet">
        /// <item> int - Return unchanged</item>
        /// <item> Number - Return an int value (real numbers are truncated)</item>
        /// <item> Rational - Truncate any fractional part and returns remaining int</item>
        /// <item> String - Attempt to parse string as an int.  If this fails, convert the char[] to an int (using shifts and OR)</item>
        /// <item> Rational[] - Return int value of first item in array if length &gt; 0</item>
        /// <item> byte[] - Return int value of first item in array if length &gt; 0</item>
        /// <item> int[] - Return int value of first item in array if length &gt; 0</item>
        /// </list>
        /// If the value is not found or cannot be converted to int, <c>null</c> is returned.
        /// </remarks>
        [CanBeNull]
        public int? GetInteger(int tagType)
        {
            var o = GetObject(tagType);
            if (o == null)
                return null;
            if (o.IsNumber())
                return Number.GetInstance(o).IntValue();
            var value = o as string;
            if (value != null)
            {
                try
                {
                    return Convert.ToInt32(value);
                }
                catch (FormatException)
                {
                    // convert the char array to an int
                    var bytes = Encoding.UTF8.GetBytes(value);
                    long val = 0;
                    foreach (var aByte in bytes)
                    {
                        val = val << 8;
                        val += aByte;
                    }
                    return (int)val;
                }
            }
            var rationals = o as Rational[];
            if (rationals != null)
            {
                if (rationals.Length == 1)
                    return rationals[0].ToInt32();
            }
            else if (o.GetType() == typeof(byte[]))
            {
                var bytes = (byte[])o;
                if (bytes.Length == 1)
                    return bytes[0];
            }
            else if (o.GetType() == typeof(sbyte[]))
            {
                var bytes = (sbyte[])o;
                if (bytes.Length == 1)
                    return bytes[0];
            }
            else
            {
                var ints = o as int[];
                if (ints != null && ints.Length == 1)
                    return ints[0];
            }
            return null;
        }

        /// <summary>Gets the specified tag's value as a String array, if possible.</summary>
        /// <remarks>Only supported where the tag is set as String[], String, int[], byte[] or Rational[].</remarks>
        /// <param name="tagType">the tag identifier</param>
        /// <returns>the tag's value as an array of Strings. If the value is unset or cannot be converted, <c>null</c> is returned.</returns>
        [CanBeNull]
        public string[] GetStringArray(int tagType)
        {
            var o = GetObject(tagType);

            if (o == null)
                return null;

            var strings = o as string[];
            if (strings != null)
                return strings;

            var s = o as string;
            if (s != null)
                return new[] { s };

            var ints = o as int[];
            if (ints != null)
            {
                strings = new string[ints.Length];
                for (var i = 0; i < strings.Length; i++)
                    strings[i] = ints[i].ToString();
                return strings;
            }

            var bytes = o as byte[];
            if (bytes != null)
            {
                strings = new string[bytes.Length];
                for (var i = 0; i < strings.Length; i++)
                    strings[i] = ((int)bytes[i]).ToString();
                return strings;
            }

            var rationals = o as Rational[];
            if (rationals != null)
            {
                strings = new string[rationals.Length];
                for (var i = 0; i < strings.Length; i++)
                    strings[i] = rationals[i].ToSimpleString(false);
                return strings;
            }

            return null;
        }

        /// <summary>Gets the specified tag's value as an int array, if possible.</summary>
        /// <remarks>Only supported where the tag is set as String, Integer, int[], byte[] or Rational[].</remarks>
        /// <param name="tagType">the tag identifier</param>
        /// <returns>the tag's value as an int array</returns>
        [CanBeNull]
        public int[] GetIntArray(int tagType)
        {
            var o = GetObject(tagType);

            if (o == null)
                return null;

            var ints = o as int[];
            if (ints != null)
                return ints;

            var rationals = o as Rational[];
            if (rationals != null)
            {
                ints = new int[rationals.Length];
                for (var i = 0; i < ints.Length; i++)
                    ints[i] = rationals[i].ToInt32();
                return ints;
            }

            var shorts = o as short[];
            if (shorts != null)
            {
                ints = new int[shorts.Length];
                for (var i = 0; i < shorts.Length; i++)
                    ints[i] = shorts[i];
                return ints;
            }

            if (o.GetType() == typeof(sbyte[]))
            {
                var bytes = (sbyte[])o;
                ints = new int[bytes.Length];
                for (var i = 0; i < bytes.Length; i++)
                    ints[i] = bytes[i];
                return ints;
            }

            if (o.GetType() == typeof(byte[]))
            {
                var bytes = (byte[])o;
                ints = new int[bytes.Length];
                for (var i = 0; i < bytes.Length; i++)
                    ints[i] = bytes[i];
                return ints;
            }

            var str = o as string;
            if (str != null)
            {
                ints = new int[str.Length];
                for (var i = 0; i < str.Length; i++)
                    ints[i] = str[i];
                return ints;
            }

            var nullableInt = o as int?;
            if (nullableInt != null)
                return new[] { (int)o };

            return null;
        }

        /// <summary>Gets the specified tag's value as an byte array, if possible.</summary>
        /// <remarks>Only supported where the tag is set as String, Integer, int[], byte[] or Rational[].</remarks>
        /// <param name="tagType">the tag identifier</param>
        /// <returns>the tag's value as a byte array</returns>
        [CanBeNull]
        public byte[] GetByteArray(int tagType)
        {
            var o = GetObject(tagType);

            if (o == null)
                return null;

            byte[] bytes;

            var rationals = o as Rational[];
            if (rationals != null)
            {
                bytes = new byte[rationals.Length];
                for (var i = 0; i < bytes.Length; i++)
                    bytes[i] = rationals[i].ToByte();
                return bytes;
            }

            bytes = o as byte[];
            if (bytes != null)
                return bytes;

            var ints = o as int[];
            if (ints != null)
            {
                bytes = new byte[ints.Length];
                for (var i = 0; i < ints.Length; i++)
                    bytes[i] = unchecked((byte)ints[i]);
                return bytes;
            }

            var shorts = o as short[];
            if (shorts != null)
            {
                bytes = new byte[shorts.Length];
                for (var i = 0; i < shorts.Length; i++)
                    bytes[i] = unchecked((byte)shorts[i]);
                return bytes;
            }

            var str = o as string;
            if (str != null)
            {
                bytes = new byte[str.Length];
                for (var i = 0; i < str.Length; i++)
                    bytes[i] = unchecked((byte)str[i]);
                return bytes;
            }

            var nullableInt = o as int?;
            if (nullableInt != null)
                return new[] { (byte)nullableInt.Value };

            return null;
        }

        /// <summary>Returns the specified tag's value as a double, if possible.</summary>
        /// <exception cref="MetadataException"/>
        public double GetDouble(int tagType)
        {
            var value = GetDoubleObject(tagType);

            if (value != null)
                return (double)value;

            var o = GetObject(tagType);
            if (o == null)
                throw new MetadataException("Tag '" + GetTagName(tagType) + "' has not been set -- check using containsTag() first");

            throw new MetadataException("Tag '" + tagType + "' cannot be converted to a double.  It is of type '" + o.GetType() + "'.");
        }

        /// <summary>Returns the specified tag's value as a Double.</summary>
        /// <remarks>If the tag is not set or cannot be converted, <c>null</c> is returned.</remarks>
        [CanBeNull]
        public double? GetDoubleObject(int tagType)
        {
            var o = GetObject(tagType);

            if (o == null)
                return null;

            var s = o as string;
            if (s != null)
            {
                double d;
                return double.TryParse(s, out d) ? (double?)d : null;
            }

            if (o.IsNumber())
                return Number.GetInstance(o).DoubleValue();

            return null;
        }

        /// <summary>Returns the specified tag's value as a float, if possible.</summary>
        /// <exception cref="MetadataException"/>
        public float GetFloat(int tagType)
        {
            var value = GetFloatObject(tagType);
            if (value != null)
                return (float)value;

            var o = GetObject(tagType);
            if (o == null)
                throw new MetadataException("Tag '" + GetTagName(tagType) + "' has not been set -- check using containsTag() first");

            throw new MetadataException("Tag '" + tagType + "' cannot be converted to a float.  It is of type '" + o.GetType() + "'.");
        }

        /// <summary>Returns the specified tag's value as a float.</summary>
        /// <remarks>If the tag is not set or cannot be converted, <c>null</c> is returned.</remarks>
        [CanBeNull]
        public float? GetFloatObject(int tagType)
        {
            var o = GetObject(tagType);

            if (o == null)
                return null;

            var s = o as string;
            if (s != null)
            {
                float f;
                return float.TryParse(s, out f) ? (float?)f : null;
            }

            if (o.IsNumber())
                return Number.GetInstance(o).FloatValue();

            return null;
        }

        /// <summary>Returns the specified tag's value as a long, if possible.</summary>
        /// <exception cref="MetadataException"/>
        public long GetLong(int tagType)
        {
            var value = GetLongObject(tagType);

            if (value != null)
                return (long)value;

            var o = GetObject(tagType);
            if (o == null)
                throw new MetadataException("Tag '" + GetTagName(tagType) + "' has not been set -- check using containsTag() first");

            throw new MetadataException("Tag '" + tagType + "' cannot be converted to a long.  It is of type '" + o.GetType() + "'.");
        }

        /// <summary>Returns the specified tag's value as a long.</summary>
        /// <remarks>If the tag is not set or cannot be converted, <c>null</c> is returned.</remarks>
        [CanBeNull]
        public long? GetLongObject(int tagType)
        {
            var o = GetObject(tagType);

            if (o == null)
                return null;

            var s = o as string;
            if (s != null)
            {
                long l;
                return long.TryParse(s, out l) ? (long?)l : null;
            }

            if (o.IsNumber())
                return Number.GetInstance(o).LongValue();

            return null;
        }

        /// <summary>Returns the specified tag's value as a boolean, if possible.</summary>
        /// <exception cref="MetadataException"/>
        public bool GetBoolean(int tagType)
        {
            var value = GetBooleanObject(tagType);

            if (value != null)
                return (bool)value;

            var o = GetObject(tagType);
            if (o == null)
                throw new MetadataException("Tag '" + GetTagName(tagType) + "' has not been set -- check using containsTag() first");

            throw new MetadataException("Tag '" + tagType + "' cannot be converted to a boolean.  It is of type '" + o.GetType() + "'.");
        }

        /// <summary>Returns the specified tag's value as a boolean.</summary>
        /// <remarks>If the tag is not set or cannot be converted, <c>null</c> is returned.</remarks>
        [CanBeNull]
        public bool? GetBooleanObject(int tagType)
        {
            var o = GetObject(tagType);

            if (o == null)
                return null;

            var b = o as bool?;
            if (b != null)
                return b;

            var s = o as string;
            if (s != null)
            {
                bool result;
                return bool.TryParse(s, out result) ? (bool?)result : null;
            }

            if (o.IsNumber())
                return Number.GetInstance(o).DoubleValue() != 0;

            return null;
        }

        /// <summary>Returns the specified tag's value as a java.util.Date.</summary>
        /// <remarks>If the underlying value is a <see cref="string"/>, then attempts will be made to parse it.</remarks>
        /// <returns>The specified tag's value as a DateTime.  If the value is unset or cannot be converted, <c>null</c> is returned.</returns>
        [CanBeNull]
        public DateTime? GetDate(int tagType/*, [CanBeNull] TimeZoneInfo timeZone = null*/)
        {
            var o = GetObject(tagType);

            if (o == null)
                return null;

            if (o is DateTime)
                return (DateTime)o;

            var s = o as string;

            if (s == null)
                return null;

            // This seems to cover all known Exif date strings
            // Note that "    :  :     :  :  " is a valid date string according to the Exif spec (which means 'unknown date'): http://www.awaresystems.be/imaging/tiff/tifftags/privateifd/exif/datetimeoriginal.html
            var datePatterns = new[] { "yyyy:MM:dd HH:mm:ss", "yyyy:MM:dd HH:mm", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy.MM.dd HH:mm:ss", "yyyy.MM.dd HH:mm" };
            foreach (var datePattern in datePatterns)
            {
                DateTime result;
                if (DateTime.TryParseExact(s, datePattern, null, DateTimeStyles.AllowWhiteSpaces, out result))
                    return result;
            }

            return null;
        }

        /// <summary>Returns the specified tag's value as a Rational.</summary>
        /// <remarks>If the value is unset or cannot be converted, <c>null</c> is returned.</remarks>
        [CanBeNull]
        public Rational GetRational(int tagType)
        {
            var o = GetObject(tagType);

            if (o == null)
                return null;

            var rational = o as Rational;
            if (rational != null)
                return rational;

            if (o is int?)
                return new Rational((int)o, 1);

            if (o is long?)
                return new Rational((long)o, 1);

            // NOTE not doing conversions for real number types
            return null;
        }

        /// <summary>Returns the specified tag's value as an array of Rational.</summary>
        /// <remarks>If the value is unset or cannot be converted, <c>null</c> is returned.</remarks>
        [CanBeNull]
        public Rational[] GetRationalArray(int tagType)
        {
            return GetObject(tagType) as Rational[];
        }

        /// <summary>Returns the specified tag's value as a String.</summary>
        /// <remarks>
        /// This value is the 'raw' value.  A more presentable decoding
        /// of this value may be obtained from the corresponding Descriptor.
        /// </remarks>
        /// <returns>
        /// the String representation of the tag's value, or
        /// <c>null</c> if the tag hasn't been defined.
        /// </returns>
        [CanBeNull]
        public string GetString(int tagType)
        {
            var o = GetObject(tagType);
            if (o == null)
                return null;

            var rational = o as Rational;
            if (rational != null)
                return rational.ToSimpleString(true);

            if (o is DateTime)
                return ((DateTime)o).ToString("ddd MMM dd HH:mm:ss zzz yyyy");

            if (o is bool)
                return (bool)o ? "true" : "false";

            // handle arrays of objects and primitives
            var array = o as Array;
            if (array != null)
            {
                var componentType = array.GetType().GetElementType();
                var str = new StringBuilder();

                if (componentType == typeof(float))
                {
                    var vals = (float[])array;
                    for (var i = 0; i < vals.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(vals[i]);
                    }
                }
                else if (componentType == typeof(double))
                {
                    var vals = (double[])array;
                    for (var i = 0; i < vals.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(vals[i]);
                    }
                }
                else if (componentType == typeof(int))
                {
                    var vals = (int[])array;
                    for (var i = 0; i < vals.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(vals[i]);
                    }
                }
                else if (componentType == typeof(uint))
                {
                    var vals = (uint[])array;
                    for (var i = 0; i < vals.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(vals[i]);
                    }
                }
                else if (componentType == typeof(short))
                {
                    var vals = (short[])array;
                    for (var i = 0; i < vals.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(vals[i]);
                    }
                }
                else if (componentType == typeof(ushort))
                {
                    var vals = (ushort[])array;
                    for (var i = 0; i < vals.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(vals[i]);
                    }
                }
                else if (componentType == typeof(byte))
                {
                    var vals = (byte[])array;
                    for (var i = 0; i < vals.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(vals[i]);
                    }
                }
                else if (componentType == typeof(sbyte))
                {
                    var vals = (sbyte[])array;
                    for (var i = 0; i < vals.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(vals[i]);
                    }
                }
                else if (componentType == typeof(string))
                {
                    var vals = (string[])array;
                    for (var i = 0; i < vals.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(vals[i]);
                    }
                }
                else if (componentType.IsByRef)
                {
                    var vals = (object[])array;
                    for (var i = 0; i < vals.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(vals[i]);
                    }
                }
                else
                {
                    AddError("Unexpected array component type: " + componentType.FullName);
                }

                return str.ToString();
            }
            // Note that several cameras leave trailing spaces (Olympus, Nikon) but this library is intended to show
            // the actual data within the file.  It is not inconceivable that whitespace may be significant here, so we
            // do not trim.  Also, if support is added for writing data back to files, this may cause issues.
            // We leave trimming to the presentation layer.
            return o.ToString();
        }

        [CanBeNull]
        public string GetString(int tagType, Encoding encoding)
        {
            var bytes = GetByteArray(tagType);
            return bytes == null
                ? null
                : encoding.GetString(bytes);
        }

        /// <summary>Returns the object hashed for the particular tag type specified, if available.</summary>
        /// <param name="tagType">the tag type identifier</param>
        /// <returns>the tag's value as an Object if available, else <c>null</c></returns>
        [CanBeNull]
        public object GetObject(int tagType)
        {
            return _tagMap.GetOrNull(tagType);
        }

        #endregion

        /// <summary>Returns the name of a specified tag as a String.</summary>
        /// <param name="tagType">the tag type identifier</param>
        /// <returns>the tag's name as a String</returns>
        [NotNull]
        public string GetTagName(int tagType)
        {
            var nameMap = GetTagNameMap();
            string value;
            return nameMap.TryGetValue(tagType, out value)
                ? value
                : string.Format("Unknown tag (0x{0:X4})", tagType);
        }

        /// <summary>Gets whether the specified tag is known by the directory and has a name.</summary>
        /// <param name="tagType">the tag type identifier</param>
        /// <returns>whether this directory has a name for the specified tag</returns>
        public bool HasTagName(int tagType)
        {
            return GetTagNameMap().ContainsKey(tagType);
        }

        /// <summary>
        /// Provides a description of a tag's value using the descriptor set by
        /// <c>setDescriptor(Descriptor)</c>.
        /// </summary>
        /// <param name="tagType">the tag type identifier</param>
        /// <returns>the tag value's description as a String</returns>
        [CanBeNull]
        public string GetDescription(int tagType)
        {
            Debug.Assert((_descriptor != null));
            return _descriptor.GetDescription(tagType);
        }

        public override string ToString()
        {
            return string.Format("{0} Directory ({1} {2})", Name, _tagMap.Count, _tagMap.Count == 1 ? "tag" : "tags");
        }
    }
}
