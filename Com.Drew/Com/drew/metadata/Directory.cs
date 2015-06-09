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
using System.Text;
using Com.Drew.Lang;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata
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
        protected readonly IDictionary<int?, object> TagMap = new Dictionary<int?, object>();

        /// <summary>A convenient list holding tag values in the order in which they were stored.</summary>
        /// <remarks>
        /// A convenient list holding tag values in the order in which they were stored.
        /// This is used for creation of an iterator, and for counting the number of
        /// defined tags.
        /// </remarks>
        [NotNull]
        protected readonly ICollection<Tag> DefinedTagList = new List<Tag>();

        [NotNull]
        private readonly ICollection<string> _errorList = new List<string>(4);

        /// <summary>The descriptor used to interpret tag values.</summary>
        protected ITagDescriptor Descriptor;

        // ABSTRACT METHODS
        /// <summary>Provides the name of the directory, for display purposes.</summary>
        /// <remarks>Provides the name of the directory, for display purposes.  E.g. <c>Exif</c></remarks>
        /// <returns>the name of the directory</returns>
        [NotNull]
        public abstract string GetName();

        /// <summary>Provides the map of tag names, hashed by tag type identifier.</summary>
        /// <returns>the map of tag names</returns>
        [NotNull]
        protected abstract Dictionary<int?, string> GetTagNameMap();

        // VARIOUS METHODS
        /// <summary>Gets a value indicating whether the directory is empty, meaning it contains no errors and no tag values.</summary>
        public bool IsEmpty()
        {
            return _errorList.Count == 0 && DefinedTagList.Count == 0;
        }

        /// <summary>Indicates whether the specified tag type has been set.</summary>
        /// <param name="tagType">the tag type to check for</param>
        /// <returns>true if a value exists for the specified tag type, false if not</returns>
        public bool ContainsTag(int tagType)
        {
            return TagMap.ContainsKey(tagType);
        }

        /// <summary>Returns an Iterator of Tag instances that have been set in this Directory.</summary>
        /// <returns>an Iterator of Tag instances</returns>
        [NotNull]
        public ICollection<Tag> GetTags()
        {
            return Collections.UnmodifiableCollection(DefinedTagList);
        }

        /// <summary>Returns the number of tags set in this Directory.</summary>
        /// <returns>the number of tags set in this Directory</returns>
        public int GetTagCount()
        {
            return DefinedTagList.Count;
        }

        /// <summary>Sets the descriptor used to interpret tag values.</summary>
        /// <param name="descriptor">the descriptor used to interpret tag values</param>
        public void SetDescriptor([NotNull] ITagDescriptor descriptor)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException("cannot set a null descriptor");
            }
            Descriptor = descriptor;
        }

        /// <summary>Registers an error message with this directory.</summary>
        /// <param name="message">an error message.</param>
        public void AddError([NotNull] string message)
        {
            _errorList.Add(message);
        }

        /// <summary>Gets a value indicating whether this directory has any error messages.</summary>
        /// <returns>true if the directory contains errors, otherwise false</returns>
        public bool HasErrors()
        {
            return _errorList.Count > 0;
        }

        /// <summary>Used to iterate over any error messages contained in this directory.</summary>
        /// <returns>an iterable collection of error message strings.</returns>
        [NotNull]
        public IEnumerable<string> GetErrors()
        {
            return Collections.UnmodifiableCollection(_errorList);
        }

        /// <summary>Returns the count of error messages in this directory.</summary>
        public int GetErrorCount()
        {
            return _errorList.Count;
        }

        // TAG SETTERS
        /// <summary>Sets an <c>int</c> value for the specified tag.</summary>
        /// <param name="tagType">the tag's value as an int</param>
        /// <param name="value">the value for the specified tag as an int</param>
        public void SetInt(int tagType, int value)
        {
            SetObject(tagType, value);
        }

        /// <summary>Sets an <c>int[]</c> (array) for the specified tag.</summary>
        /// <param name="tagType">the tag identifier</param>
        /// <param name="ints">the int array to store</param>
        public void SetIntArray(int tagType, [NotNull] int[] ints)
        {
            SetObjectArray(tagType, ints);
        }

        /// <summary>Sets a <c>float</c> value for the specified tag.</summary>
        /// <param name="tagType">the tag's value as an int</param>
        /// <param name="value">the value for the specified tag as a float</param>
        public void SetFloat(int tagType, float value)
        {
            SetObject(tagType, value);
        }

        /// <summary>Sets a <c>float[]</c> (array) for the specified tag.</summary>
        /// <param name="tagType">the tag identifier</param>
        /// <param name="floats">the float array to store</param>
        public void SetFloatArray(int tagType, [NotNull] float[] floats)
        {
            SetObjectArray(tagType, floats);
        }

        /// <summary>Sets a <c>double</c> value for the specified tag.</summary>
        /// <param name="tagType">the tag's value as an int</param>
        /// <param name="value">the value for the specified tag as a double</param>
        public void SetDouble(int tagType, double value)
        {
            SetObject(tagType, value);
        }

        /// <summary>Sets a <c>double[]</c> (array) for the specified tag.</summary>
        /// <param name="tagType">the tag identifier</param>
        /// <param name="doubles">the double array to store</param>
        public void SetDoubleArray(int tagType, [NotNull] double[] doubles)
        {
            SetObjectArray(tagType, doubles);
        }

        /// <summary>Sets a <c>String</c> value for the specified tag.</summary>
        /// <param name="tagType">the tag's value as an int</param>
        /// <param name="value">the value for the specified tag as a String</param>
        public void SetString(int tagType, [NotNull] string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("cannot set a null String");
            }
            SetObject(tagType, value);
        }

        /// <summary>Sets a <c>String[]</c> (array) for the specified tag.</summary>
        /// <param name="tagType">the tag identifier</param>
        /// <param name="strings">the String array to store</param>
        public void SetStringArray(int tagType, [NotNull] string[] strings)
        {
            SetObjectArray(tagType, strings);
        }

        /// <summary>Sets a <c>boolean</c> value for the specified tag.</summary>
        /// <param name="tagType">the tag's value as an int</param>
        /// <param name="value">the value for the specified tag as a boolean</param>
        public void SetBoolean(int tagType, bool value)
        {
            SetObject(tagType, value);
        }

        /// <summary>Sets a <c>long</c> value for the specified tag.</summary>
        /// <param name="tagType">the tag's value as an int</param>
        /// <param name="value">the value for the specified tag as a long</param>
        public void SetLong(int tagType, long value)
        {
            SetObject(tagType, value);
        }

        /// <summary>Sets a <c>java.util.Date</c> value for the specified tag.</summary>
        /// <param name="tagType">the tag's value as an int</param>
        /// <param name="value">the value for the specified tag as a java.util.Date</param>
        public void SetDate(int tagType, DateTime value)
        {
            SetObject(tagType, value);
        }

        /// <summary>Sets a <c>Rational</c> value for the specified tag.</summary>
        /// <param name="tagType">the tag's value as an int</param>
        /// <param name="rational">rational number</param>
        public void SetRational(int tagType, [NotNull] Rational rational)
        {
            SetObject(tagType, rational);
        }

        /// <summary>Sets a <c>Rational[]</c> (array) for the specified tag.</summary>
        /// <param name="tagType">the tag identifier</param>
        /// <param name="rationals">the Rational array to store</param>
        public void SetRationalArray(int tagType, [NotNull] Rational[] rationals)
        {
            SetObjectArray(tagType, rationals);
        }

        /// <summary>Sets a <c>byte[]</c> (array) for the specified tag.</summary>
        /// <param name="tagType">the tag identifier</param>
        /// <param name="bytes">the byte array to store</param>
        public virtual void SetByteArray(int tagType, [NotNull] byte[] bytes)
        {
            SetObjectArray(tagType, bytes);
        }

        /// <summary>Sets a <c>sbyte[]</c> (array) for the specified tag.</summary>
        /// <param name="tagType">the tag identifier</param>
        /// <param name="bytes">the signed byte array to store</param>
        public void SetSByteArray(int tagType, [NotNull] sbyte[] bytes)
        {
            SetObjectArray(tagType, bytes);
        }

        /// <summary>Sets a <c>Object</c> for the specified tag.</summary>
        /// <param name="tagType">the tag's value as an int</param>
        /// <param name="value">the value for the specified tag</param>
        /// <exception cref="System.ArgumentNullException">if value is <c>null</c></exception>
        public void SetObject(int tagType, [NotNull] object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("cannot set a null object");
            }
            if (!TagMap.ContainsKey(tagType))
            {
                DefinedTagList.Add(new Tag(tagType, this));
            }
            //        else {
            //            final Object oldValue = _tagMap.get(tagType);
            //            if (!oldValue.equals(value))
            //                addError(String.format("Overwritten tag 0x%s (%s).  Old=%s, New=%s", Integer.toHexString(tagType), getTagName(tagType), oldValue, value));
            //        }
            TagMap[tagType] = value;
        }

        /// <summary>Sets an array <c>Object</c> for the specified tag.</summary>
        /// <param name="tagType">the tag's value as an int</param>
        /// <param name="array">the array of values for the specified tag</param>
        public virtual void SetObjectArray(int tagType, [NotNull] object array)
        {
            // for now, we don't do anything special -- this method might be a candidate for removal once the dust settles
            SetObject(tagType, array);
        }

        // TAG GETTERS
        /// <summary>Returns the specified tag's value as an int, if possible.</summary>
        /// <remarks>
        /// Returns the specified tag's value as an int, if possible.  Every attempt to represent the tag's value as an int
        /// is taken.  Here is a list of the action taken depending upon the tag's original type:
        /// <list type="bullet">
        /// <item> int - Return unchanged.
        /// <item> Number - Return an int value (real numbers are truncated).
        /// <item> Rational - Truncate any fractional part and returns remaining int.
        /// <item> String - Attempt to parse string as an int.  If this fails, convert the char[] to an int (using shifts and OR).
        /// <item> Rational[] - Return int value of first item in array.
        /// <item> byte[] - Return int value of first item in array.
        /// <item> int[] - Return int value of first item in array.
        /// </list>
        /// </remarks>
        /// <exception cref="MetadataException">if no value exists for tagType or if it cannot be converted to an int.</exception>
        /// <exception cref="Com.Drew.Metadata.MetadataException"/>
        public int GetInt(int tagType)
        {
            int? integer = GetInteger(tagType);
            if (integer != null)
            {
                return (int)integer;
            }
            object o = GetObject(tagType);
            if (o == null)
            {
                throw new MetadataException("Tag '" + GetTagName(tagType) + "' has not been set -- check using containsTag() first");
            }
            throw new MetadataException("Tag '" + tagType + "' cannot be converted to int.  It is of type '" + o.GetType() + "'.");
        }

        /// <summary>Returns the specified tag's value as an Integer, if possible.</summary>
        /// <remarks>
        /// Returns the specified tag's value as an Integer, if possible.  Every attempt to represent the tag's value as an
        /// Integer is taken.  Here is a list of the action taken depending upon the tag's original type:
        /// <list type="bullet">
        /// <item> int - Return unchanged
        /// <item> Number - Return an int value (real numbers are truncated)
        /// <item> Rational - Truncate any fractional part and returns remaining int
        /// <item> String - Attempt to parse string as an int.  If this fails, convert the char[] to an int (using shifts and OR)
        /// <item> Rational[] - Return int value of first item in array if length &gt; 0
        /// <item> byte[] - Return int value of first item in array if length &gt; 0
        /// <item> int[] - Return int value of first item in array if length &gt; 0
        /// </list>
        /// If the value is not found or cannot be converted to int, <c>null</c> is returned.
        /// </remarks>
        [CanBeNull]
        public int? GetInteger(int tagType)
        {
            object o = GetObject(tagType);
            if (o == null)
            {
                return null;
            }
            if (o.IsNumber())
            {
                return Number.GetInstance(o).IntValue();
            }
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
                    string s = value;
                    byte[] bytes = Encoding.UTF8.GetBytes(s);
                    long val = 0;
                    foreach (byte aByte in bytes)
                    {
                        val = val << 8;
                        val += (aByte & unchecked(0xff));
                    }
                    return (int)val;
                }
            }
            var rationals = o as Rational[];
            if (rationals != null)
            {
                if (rationals.Length == 1)
                {
                    return rationals[0].IntValue();
                }
            }
            else
            {
                if (o.GetType() == typeof(byte[]))
                {
                    var bytes = (byte[])o;
                    if (bytes.Length == 1)
                    {
                        return bytes[0];
                    }
                }
                else if (o.GetType() == typeof(sbyte[]))
                {
                    var bytes = (sbyte[])o;
                    if (bytes.Length == 1)
                    {
                        return bytes[0];
                    }
                }
                else
                {
                    var ints = o as int[];
                    if (ints != null)
                    {
                        if (ints.Length == 1)
                        {
                            return ints[0];
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>Gets the specified tag's value as a String array, if possible.</summary>
        /// <remarks>
        /// Gets the specified tag's value as a String array, if possible.  Only supported
        /// where the tag is set as String[], String, int[], byte[] or Rational[].
        /// </remarks>
        /// <param name="tagType">the tag identifier</param>
        /// <returns>the tag's value as an array of Strings. If the value is unset or cannot be converted, <c>null</c> is returned.</returns>
        [CanBeNull]
        public string[] GetStringArray(int tagType)
        {
            object o = GetObject(tagType);
            if (o == null)
            {
                return null;
            }
            var strings = o as string[];
            if (strings != null)
            {
                return strings;
            }
            var s = o as string;
            if (s != null)
            {
                return new[] { s };
            }
            var ints = o as int[];
            if (ints != null)
            {
                strings = new string[ints.Length];
                for (int i = 0; i < strings.Length; i++)
                {
                    strings[i] = ints[i].ToString();
                }
                return strings;
            }
            var bytes = o as byte[];
            if (bytes != null)
            {
                strings = new string[bytes.Length];
                for (int i = 0; i < strings.Length; i++)
                {
                    strings[i] = ((int)bytes[i]).ToString();
                }
                return strings;
            }
            var rationals = o as Rational[];
            if (rationals != null)
            {
                strings = new string[rationals.Length];
                for (int i = 0; i < strings.Length; i++)
                {
                    strings[i] = rationals[i].ToSimpleString(false);
                }
                return strings;
            }
            return null;
        }

        /// <summary>Gets the specified tag's value as an int array, if possible.</summary>
        /// <remarks>
        /// Gets the specified tag's value as an int array, if possible.  Only supported
        /// where the tag is set as String, Integer, int[], byte[] or Rational[].
        /// </remarks>
        /// <param name="tagType">the tag identifier</param>
        /// <returns>the tag's value as an int array</returns>
        [CanBeNull]
        public int[] GetIntArray(int tagType)
        {
            object o = GetObject(tagType);
            if (o == null)
            {
                return null;
            }
            var ints = o as int[];
            if (ints != null)
            {
                return ints;
            }
            var rationals = o as Rational[];
            if (rationals != null)
            {
                ints = new int[rationals.Length];
                for (int i = 0; i < ints.Length; i++)
                {
                    ints[i] = rationals[i].IntValue();
                }
                return ints;
            }
            var shorts = o as short[];
            if (shorts != null)
            {
                ints = new int[shorts.Length];
                for (int i = 0; i < shorts.Length; i++)
                {
                    ints[i] = shorts[i];
                }
                return ints;
            }
            if (o.GetType() == typeof(sbyte[]))
            {
                var bytes = (sbyte[])o;
                ints = new int[bytes.Length];
                for (int i = 0; i < bytes.Length; i++)
                {
                    ints[i] = bytes[i];
                }
                return ints;
            }
            if (o.GetType() == typeof(byte[]))
            {
                var bytes = (byte[])o;
                ints = new int[bytes.Length];
                for (int i = 0; i < bytes.Length; i++)
                {
                    ints[i] = bytes[i];
                }
                return ints;
            }
            var str = o as string;
            if (str != null)
            {
                ints = new int[str.Length];
                for (int i = 0; i < str.Length; i++)
                {
                    ints[i] = str[i];
                }
                return ints;
            }
            var nullableInt = o as int?;
            if (nullableInt != null)
            {
                return new[] { (int)o };
            }
            return null;
        }

        /// <summary>Gets the specified tag's value as an byte array, if possible.</summary>
        /// <remarks>
        /// Gets the specified tag's value as an byte array, if possible.  Only supported
        /// where the tag is set as String, Integer, int[], byte[] or Rational[].
        /// </remarks>
        /// <param name="tagType">the tag identifier</param>
        /// <returns>the tag's value as a byte array</returns>
        [CanBeNull]
        public byte[] GetByteArray(int tagType)
        {
            object o = GetObject(tagType);
            if (o == null)
            {
                return null;
            }
            byte[] bytes;

            var rationals = o as Rational[];
            if (rationals != null)
            {
                bytes = new byte[rationals.Length];
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = rationals[i].ByteValue();
                }
                return bytes;
            }
            bytes = o as byte[];
            if (bytes != null)
            {
                return bytes;
            }
            var ints = o as int[];
            if (ints != null)
            {
                bytes = new byte[ints.Length];
                for (int i = 0; i < ints.Length; i++)
                {
                    bytes[i] = unchecked((byte)ints[i]);
                }
                return bytes;
            }
            var shorts = o as short[];
            if (shorts != null)
            {
                bytes = new byte[shorts.Length];
                for (int i = 0; i < shorts.Length; i++)
                {
                    bytes[i] = unchecked((byte)shorts[i]);
                }
                return bytes;
            }
            var str = o as string;
            if (str != null)
            {
                bytes = new byte[str.Length];
                for (int i = 0; i < str.Length; i++)
                {
                    bytes[i] = unchecked((byte)str[i]);
                }
                return bytes;
            }
            var nullableInt = o as int?;
            if (nullableInt != null)
            {
                return new[] { (byte)nullableInt.Value };
            }
            return null;
        }

        /// <summary>Returns the specified tag's value as a double, if possible.</summary>
        /// <exception cref="Com.Drew.Metadata.MetadataException"/>
        public double GetDouble(int tagType)
        {
            double? value = GetDoubleObject(tagType);
            if (value != null)
            {
                return (double)value;
            }
            object o = GetObject(tagType);
            if (o == null)
            {
                throw new MetadataException("Tag '" + GetTagName(tagType) + "' has not been set -- check using containsTag() first");
            }
            throw new MetadataException("Tag '" + tagType + "' cannot be converted to a double.  It is of type '" + o.GetType() + "'.");
        }

        /// <summary>Returns the specified tag's value as a Double.</summary>
        /// <remarks>Returns the specified tag's value as a Double.  If the tag is not set or cannot be converted, <c>null</c> is returned.</remarks>
        [CanBeNull]
        public double? GetDoubleObject(int tagType)
        {
            object o = GetObject(tagType);
            if (o == null)
            {
                return null;
            }
            var s = o as string;
            if (s != null)
            {
                try
                {
                    return double.Parse(s);
                }
                catch (FormatException)
                {
                    return null;
                }
            }
            if (o.IsNumber())
            {
                return Number.GetInstance(o).DoubleValue();
            }
            return null;
        }

        /// <summary>Returns the specified tag's value as a float, if possible.</summary>
        /// <exception cref="Com.Drew.Metadata.MetadataException"/>
        public float GetFloat(int tagType)
        {
            float? value = GetFloatObject(tagType);
            if (value != null)
            {
                return (float)value;
            }
            object o = GetObject(tagType);
            if (o == null)
            {
                throw new MetadataException("Tag '" + GetTagName(tagType) + "' has not been set -- check using containsTag() first");
            }
            throw new MetadataException("Tag '" + tagType + "' cannot be converted to a float.  It is of type '" + o.GetType() + "'.");
        }

        /// <summary>Returns the specified tag's value as a float.</summary>
        /// <remarks>Returns the specified tag's value as a float.  If the tag is not set or cannot be converted, <c>null</c> is returned.</remarks>
        [CanBeNull]
        public float? GetFloatObject(int tagType)
        {
            object o = GetObject(tagType);
            if (o == null)
            {
                return null;
            }
            var s = o as string;
            if (s != null)
            {
                try
                {
                    return float.Parse(s);
                }
                catch (FormatException)
                {
                    return null;
                }
            }
            if (o.IsNumber())
            {
                return Number.GetInstance(o).FloatValue();
            }
            return null;
        }

        /// <summary>Returns the specified tag's value as a long, if possible.</summary>
        /// <exception cref="Com.Drew.Metadata.MetadataException"/>
        public long GetLong(int tagType)
        {
            long? value = GetLongObject(tagType);
            if (value != null)
            {
                return (long)value;
            }
            object o = GetObject(tagType);
            if (o == null)
            {
                throw new MetadataException("Tag '" + GetTagName(tagType) + "' has not been set -- check using containsTag() first");
            }
            throw new MetadataException("Tag '" + tagType + "' cannot be converted to a long.  It is of type '" + o.GetType() + "'.");
        }

        /// <summary>Returns the specified tag's value as a long.</summary>
        /// <remarks>Returns the specified tag's value as a long.  If the tag is not set or cannot be converted, <c>null</c> is returned.</remarks>
        [CanBeNull]
        public long? GetLongObject(int tagType)
        {
            object o = GetObject(tagType);
            if (o == null)
            {
                return null;
            }
            var s = o as string;
            if (s != null)
            {
                try
                {
                    return Convert.ToInt64(s);
                }
                catch (FormatException)
                {
                    return null;
                }
            }
            if (o.IsNumber())
            {
                return Number.GetInstance(o).LongValue();
            }
            return null;
        }

        /// <summary>Returns the specified tag's value as a boolean, if possible.</summary>
        /// <exception cref="Com.Drew.Metadata.MetadataException"/>
        public bool GetBoolean(int tagType)
        {
            bool? value = GetBooleanObject(tagType);
            if (value != null)
            {
                return (bool)value;
            }
            object o = GetObject(tagType);
            if (o == null)
            {
                throw new MetadataException("Tag '" + GetTagName(tagType) + "' has not been set -- check using containsTag() first");
            }
            throw new MetadataException("Tag '" + tagType + "' cannot be converted to a boolean.  It is of type '" + o.GetType() + "'.");
        }

        /// <summary>Returns the specified tag's value as a boolean.</summary>
        /// <remarks>Returns the specified tag's value as a boolean.  If the tag is not set or cannot be converted, <c>null</c> is returned.</remarks>
        [CanBeNull]
        public bool? GetBooleanObject(int tagType)
        {
            object o = GetObject(tagType);
            if (o == null)
            {
                return null;
            }
            var b = o as bool?;
            if (b != null)
            {
                return b;
            }
            var s = o as string;
            if (s != null)
            {
                try
                {
                    return bool.Parse(s);
                }
                catch (FormatException)
                {
                    return null;
                }
            }
            if (o.IsNumber())
            {
                return Number.GetInstance(o).DoubleValue() != 0;
            }
            return null;
        }

        /// <summary>Returns the specified tag's value as a java.util.Date.</summary>
        /// <remarks>
        /// Returns the specified tag's value as a java.util.Date.  If the value is unset or cannot be converted, <c>null</c> is returned.
        /// <para>
        /// If the underlying value is a <see cref="string"/>, then attempts will be made to parse the string as though it is in
        /// the <see cref="System.TimeZoneInfo"/> represented by the <paramref name="timeZone"/> parameter (if it is non-null).  Note that this parameter
        /// is only considered if the underlying value is a string and parsing occurs, otherwise it has no effect.
        /// </remarks>
        [CanBeNull]
        public DateTime? GetDate(int tagType, [CanBeNull] TimeZoneInfo timeZone = null)
        {
            object o = GetObject(tagType);
            if (o == null)
            {
                return null;
            }
            if (o is DateTime)
            {
                return (DateTime)o;
            }
            var s = o as string;
            if (s != null)
            {
                // This seems to cover all known Exif date strings
                // Note that "    :  :     :  :  " is a valid date string according to the Exif spec (which means 'unknown date'): http://www.awaresystems.be/imaging/tiff/tifftags/privateifd/exif/datetimeoriginal.html
                string[] datePatterns = new[] { "yyyy:MM:dd HH:mm:ss", "yyyy:MM:dd HH:mm", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy.MM.dd HH:mm:ss", "yyyy.MM.dd HH:mm" };
                string dateString = s;
                foreach (string datePattern in datePatterns)
                {
                    try
                    {
                        DateFormat parser = new SimpleDateFormat(datePattern);
                        if (timeZone != null)
                        {
                            parser.SetTimeZone(timeZone);
                        }
                        return parser.Parse(dateString);
                    }
                    catch (ParseException)
                    {
                    }
                }
            }
            // simply try the next pattern
            return null;
        }

        /// <summary>Returns the specified tag's value as a Rational.</summary>
        /// <remarks>Returns the specified tag's value as a Rational.  If the value is unset or cannot be converted, <c>null</c> is returned.</remarks>
        [CanBeNull]
        public Rational GetRational(int tagType)
        {
            object o = GetObject(tagType);
            if (o == null)
            {
                return null;
            }
            var rational = o as Rational;
            if (rational != null)
            {
                return rational;
            }
            if (o is int?)
            {
                return new Rational((int)o, 1);
            }
            if (o is long?)
            {
                return new Rational((long)o, 1);
            }
            // NOTE not doing conversions for real number types
            return null;
        }

        /// <summary>Returns the specified tag's value as an array of Rational.</summary>
        /// <remarks>Returns the specified tag's value as an array of Rational.  If the value is unset or cannot be converted, <c>null</c> is returned.</remarks>
        [CanBeNull]
        public Rational[] GetRationalArray(int tagType)
        {
            object o = GetObject(tagType);
            if (o == null)
            {
                return null;
            }
            var rationals = o as Rational[];
            if (rationals != null)
            {
                return rationals;
            }
            return null;
        }

        /// <summary>Returns the specified tag's value as a String.</summary>
        /// <remarks>
        /// Returns the specified tag's value as a String.  This value is the 'raw' value.  A more presentable decoding
        /// of this value may be obtained from the corresponding Descriptor.
        /// </remarks>
        /// <returns>
        /// the String representation of the tag's value, or
        /// <c>null</c> if the tag hasn't been defined.
        /// </returns>
        [CanBeNull]
        public string GetString(int tagType)
        {
            object o = GetObject(tagType);
            if (o == null)
                return null;

            var rational = o as Rational;
            if (rational != null)
                return rational.ToSimpleString(true);

            if (o is DateTime)
                return Extensions.ConvertToString((DateTime)o);

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
                    for (int i = 0; i < vals.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(vals[i]);
                    }
                }
                else if (componentType == typeof(double))
                {
                    var vals = (double[])array;
                    for (int i = 0; i < vals.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(vals[i]);
                    }
                }
                else if (componentType == typeof(int))
                {
                    var vals = (int[])array;
                    for (int i = 0; i < vals.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(vals[i]);
                    }
                }
                else if (componentType == typeof(uint))
                {
                    var vals = (uint[])array;
                    for (int i = 0; i < vals.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(vals[i]);
                    }
                }
                else if (componentType == typeof(short))
                {
                    var vals = (short[])array;
                    for (int i = 0; i < vals.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(vals[i]);
                    }
                }
                else if (componentType == typeof(ushort))
                {
                    var vals = (ushort[])array;
                    for (int i = 0; i < vals.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(vals[i]);
                    }
                }
                else if (componentType == typeof(byte))
                {
                    var vals = (byte[])array;
                    for (int i = 0; i < vals.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(vals[i]);
                    }
                }
                else if (componentType == typeof(sbyte))
                {
                    var vals = (sbyte[])array;
                    for (int i = 0; i < vals.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(vals[i]);
                    }
                }
                else if (componentType == typeof(string))
                {
                    var vals = (string[])array;
                    for (int i = 0; i < vals.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(vals[i]);
                    }
                }
                else if (componentType.IsByRef)
                {
                    var vals = (object[])array;
                    for (int i = 0; i < vals.Length; i++)
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
            byte[] bytes = GetByteArray(tagType);
            if (bytes == null)
            {
                return null;
            }
            return encoding.GetString(bytes);
        }

        /// <summary>Returns the object hashed for the particular tag type specified, if available.</summary>
        /// <param name="tagType">the tag type identifier</param>
        /// <returns>the tag's value as an Object if available, else <c>null</c></returns>
        [CanBeNull]
        public object GetObject(int tagType)
        {
            return TagMap.GetOrNull(tagType);
        }

        // OTHER METHODS
        /// <summary>Returns the name of a specified tag as a String.</summary>
        /// <param name="tagType">the tag type identifier</param>
        /// <returns>the tag's name as a String</returns>
        [NotNull]
        public string GetTagName(int tagType)
        {
            Dictionary<int?, string> nameMap = GetTagNameMap();
            string value;
            if (nameMap.TryGetValue(tagType, out value))
                return value;
            return string.Format("Unknown tag (0x{0:X4})", tagType);
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
            Debug.Assert((Descriptor != null));
            return Descriptor.GetDescription(tagType);
        }

        public override string ToString()
        {
            return string.Format("{0} Directory ({1} {2})", GetName(), TagMap.Count, TagMap.Count == 1 ? "tag" : "tags");
        }
    }
}
