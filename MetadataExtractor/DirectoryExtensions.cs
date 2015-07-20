#region License
//
// Copyright 2002-2015 Drew Noakes
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
using System.Globalization;
using System.Text;
using JetBrains.Annotations;

namespace MetadataExtractor
{
    public static class DirectoryExtensions
    {
        #region Int32

        /// <summary>Returns a tag's value as an <see cref="int"/>, or throws if conversion is not possible.</summary>
        /// <remarks>
        /// If the value is <see cref="IConvertible"/>, then that interface is used for conversion of the value.
        /// If the value is an array of <see cref="IConvertible"/> having length one, then the single item is converted.
        /// </remarks>
        /// <exception cref="MetadataException">No value exists for <paramref name="tagType"/>, or the value is not convertible to the requested type.</exception>
        public static int GetInt32(this Directory directory, int tagType)
        {
            int value;
            if (directory.TryGetInt32(tagType, out value))
                return value;

            return ThrowValueNotPossible<int>(directory, tagType);
        }

        [CanBeNull]
        public static int? GetInt32Nullable(this Directory directory, int tagType)
        {
            int value;
            if (directory.TryGetInt32(tagType, out value))
                return value;
            return null;
        }

        public static bool TryGetInt32(this Directory directory, int tagType, out int value)
        {
            var convertible = GetConvertibleObject(directory, tagType);

            if (convertible != null)
            {
                try
                {
                    value = convertible.ToInt32(null);
                    return true;
                }
                catch
                { }
            }

            value = default(int);
            return false;
        }

        #endregion

        #region Int64

        /// <summary>Returns a tag's value as an <see cref="int"/>, or throws if conversion is not possible.</summary>
        /// <remarks>
        /// If the value is <see cref="IConvertible"/>, then that interface is used for conversion of the value.
        /// If the value is an array of <see cref="IConvertible"/> having length one, then the single item is converted.
        /// </remarks>
        /// <exception cref="MetadataException">No value exists for <paramref name="tagType"/>, or the value is not convertible to the requested type.</exception>
        public static long GetInt64(this Directory directory, int tagType)
        {
            int value;
            if (directory.TryGetInt32(tagType, out value))
                return value;

            return ThrowValueNotPossible<long>(directory, tagType);
        }

        [CanBeNull]
        public static long? GetInt64Nullable(this Directory directory, int tagType)
        {
            long value;
            if (directory.TryGetInt64(tagType, out value))
                return value;
            return null;
        }

        public static bool TryGetInt64(this Directory directory, int tagType, out long value)
        {
            var convertible = GetConvertibleObject(directory, tagType);

            if (convertible != null)
            {
                try
                {
                    value = convertible.ToInt64(null);
                    return true;
                }
                catch
                {
                }
            }

            value = default(long);
            return false;
        }

        #endregion

        #region Single

        /// <summary>Returns a tag's value as an <see cref="float"/>, or throws if conversion is not possible.</summary>
        /// <remarks>
        /// If the value is <see cref="IConvertible"/>, then that interface is used for conversion of the value.
        /// If the value is an array of <see cref="IConvertible"/> having length one, then the single item is converted.
        /// </remarks>
        /// <exception cref="MetadataException">No value exists for <paramref name="tagType"/>, or the value is not convertible to the requested type.</exception>
        public static float GetSingle(this Directory directory, int tagType)
        {
            float value;
            if (directory.TryGetSingle(tagType, out value))
                return value;

            return ThrowValueNotPossible<float>(directory, tagType);
        }

        [CanBeNull]
        public static float? GetSingleNullable(this Directory directory, int tagType)
        {
            float value;
            if (directory.TryGetSingle(tagType, out value))
                return value;
            return null;
        }

        public static bool TryGetSingle(this Directory directory, int tagType, out float value)
        {
            var convertible = GetConvertibleObject(directory, tagType);

            if (convertible != null)
            {
                try
                {
                    value = convertible.ToSingle(null);
                    return true;
                }
                catch
                { }
            }

            value = default(float);
            return false;
        }

        #endregion

        #region Double

        /// <summary>Returns a tag's value as an <see cref="double"/>, or throws if conversion is not possible.</summary>
        /// <remarks>
        /// If the value is <see cref="IConvertible"/>, then that interface is used for conversion of the value.
        /// If the value is an array of <see cref="IConvertible"/> having length one, then the single item is converted.
        /// </remarks>
        /// <exception cref="MetadataException">No value exists for <paramref name="tagType"/>, or the value is not convertible to the requested type.</exception>
        public static double GetDouble(this Directory directory, int tagType)
        {
            double value;
            if (directory.TryGetDouble(tagType, out value))
                return value;

            return ThrowValueNotPossible<double>(directory, tagType);
        }

        [CanBeNull]
        public static double? GetDoubleNullable(this Directory directory, int tagType)
        {
            double value;
            if (directory.TryGetDouble(tagType, out value))
                return value;
            return null;
        }

        public static bool TryGetDouble(this Directory directory, int tagType, out double value)
        {
            var convertible = GetConvertibleObject(directory, tagType);

            if (convertible != null)
            {
                try
                {
                    value = convertible.ToSingle(null);
                    return true;
                }
                catch
                { }
            }

            value = default(double);
            return false;
        }

        #endregion

        #region Boolean

        /// <summary>Returns a tag's value as an <see cref="bool"/>, or throws if conversion is not possible.</summary>
        /// <remarks>
        /// If the value is <see cref="IConvertible"/>, then that interface is used for conversion of the value.
        /// If the value is an array of <see cref="IConvertible"/> having length one, then the single item is converted.
        /// </remarks>
        /// <exception cref="MetadataException">No value exists for <paramref name="tagType"/>, or the value is not convertible to the requested type.</exception>
        public static bool GetBoolean(this Directory directory, int tagType)
        {
            bool value;
            if (directory.TryGetBoolean(tagType, out value))
                return value;

            return ThrowValueNotPossible<bool>(directory, tagType);
        }

        [CanBeNull]
        public static bool? GetBooleanNullable(this Directory directory, int tagType)
        {
            bool value;
            if (directory.TryGetBoolean(tagType, out value))
                return value;
            return null;
        }

        public static bool TryGetBoolean(this Directory directory, int tagType, out bool value)
        {
            var convertible = GetConvertibleObject(directory, tagType);

            if (convertible != null)
            {
                try
                {
                    value = convertible.ToBoolean(null);
                    return true;
                }
                catch
                { }
            }

            value = default(bool);
            return false;
        }

        #endregion

        /// <summary>Gets the specified tag's value as a String array, if possible.</summary>
        /// <remarks>Only supported where the tag is set as String[], String, int[], byte[] or Rational[].</remarks>
        /// <returns>the tag's value as an array of Strings. If the value is unset or cannot be converted, <c>null</c> is returned.</returns>
        [CanBeNull]
        public static string[] GetStringArray(this Directory directory, int tagType)
        {
            var o = directory.GetObject(tagType);

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
        /// <returns>the tag's value as an int array</returns>
        [CanBeNull]
        public static int[] GetInt32Array(this Directory directory, int tagType)
        {
            var o = directory.GetObject(tagType);

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
        /// <returns>the tag's value as a byte array</returns>
        [CanBeNull]
        public static byte[] GetByteArray(this Directory directory, int tagType)
        {
            var o = directory.GetObject(tagType);

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

        /// <summary>Returns the specified tag's value as a java.util.Date.</summary>
        /// <remarks>If the underlying value is a <see cref="string"/>, then attempts will be made to parse it.</remarks>
        /// <returns>The specified tag's value as a DateTime.  If the value is unset or cannot be converted, <c>null</c> is returned.</returns>
        [CanBeNull]
        public static DateTime? GetDateTimeNullable(this Directory directory, int tagType/*, [CanBeNull] TimeZoneInfo timeZone = null*/)
        {
            var o = directory.GetObject(tagType);

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
        public static Rational GetRational(this Directory directory, int tagType)
        {
            var o = directory.GetObject(tagType);

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
        public static Rational[] GetRationalArray(this Directory directory, int tagType)
        {
            return directory.GetObject(tagType) as Rational[];
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
        public static string GetString(this Directory directory, int tagType)
        {
            var o = directory.GetObject(tagType);
            if (o == null)
                return null;

            var rational = o as Rational;
            if (rational != null)
                return rational.ToSimpleString(true);

            if (o is DateTime)
            {
                var dateTime = (DateTime)o;
                return dateTime.ToString(
                    dateTime.Kind == DateTimeKind.Utc
                        ? "ddd MMM dd HH:mm:ss yyyy"
                        : "ddd MMM dd HH:mm:ss zzz yyyy");
            }

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
                        str.AppendFormat("{0:0.###}", vals[i]);
                    }
                }
                else if (componentType == typeof(double))
                {
                    var vals = (double[])array;
                    for (var i = 0; i < vals.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.AppendFormat("{0:0.###}", vals[i]);
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
                else
                {
                    var vals = (object[])array;
                    for (var i = 0; i < vals.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(vals[i]);
                    }
                }

                return str.ToString();
            }

            if (o is double)
                return ((double)o).ToString("0.###");

            if (o is float)
                return ((float)o).ToString("0.###");

            // Note that several cameras leave trailing spaces (Olympus, Nikon) but this library is intended to show
            // the actual data within the file.  It is not inconceivable that whitespace may be significant here, so we
            // do not trim.  Also, if support is added for writing data back to files, this may cause issues.
            // We leave trimming to the presentation layer.
            return o.ToString();
        }

        [CanBeNull]
        public static string GetString(this Directory directory, int tagType, Encoding encoding)
        {
            var bytes = directory.GetByteArray(tagType);
            return bytes == null
                ? null
                : encoding.GetString(bytes);
        }

        [CanBeNull]
        private static IConvertible GetConvertibleObject([NotNull] this Directory directory, int tagType)
        {
            var o = directory.GetObject(tagType);

            if (o == null)
                return null;

            var convertible = o as IConvertible;

            if (convertible != null)
                return convertible;

            var array = o as Array;
            if (array != null && array.Length == 1 && array.Rank == 1)
                return array.GetValue(0) as IConvertible;

            return null;
        }

        private static T ThrowValueNotPossible<T>(Directory directory, int tagType)
        {
            var o = directory.GetObject(tagType);

            if (o == null)
                throw new MetadataException($"No value exists for tag {directory.GetTagName(tagType)}.");

            throw new MetadataException($"Tag {tagType} cannot be converted to {typeof(T).Name}.  It is of type {o.GetType()} with value: {o}");
        }
    }
}