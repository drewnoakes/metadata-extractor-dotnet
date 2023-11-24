// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Globalization;

using JetBrains.Annotations;

namespace MetadataExtractor
{
    public static class DirectoryExtensions
    {
        #region Byte

        /// <summary>Returns a tag's value as a <see cref="byte"/>, or throws if conversion is not possible.</summary>
        /// <remarks>
        /// If the value is <see cref="IConvertible"/>, then that interface is used for conversion of the value.
        /// If the value is an array of <see cref="IConvertible"/> having length one, then the single item is converted.
        /// </remarks>
        /// <exception cref="MetadataException">No value exists for <paramref name="tagType"/>, or the value is not convertible to the requested type.</exception>
        [Pure]
        public static byte GetByte(this Directory directory, int tagType)
        {
            if (directory.TryGetByte(tagType, out byte value))
                return value;

            return ThrowValueNotPossible<byte>(directory, tagType);
        }

        [Pure]
        public static bool TryGetByte(this Directory directory, int tagType, out byte value)
        {
            var convertible = GetConvertibleObject(directory, tagType);

            if (convertible is not null)
            {
                try
                {
                    value = convertible.ToByte(null);
                    return true;
                }
                catch
                {
                    // ignored
                }
            }

            value = default;
            return false;
        }

        #endregion

        #region Int16

        /// <summary>Returns a tag's value as a <see cref="short"/>, or throws if conversion is not possible.</summary>
        /// <remarks>
        /// If the value is <see cref="IConvertible"/>, then that interface is used for conversion of the value.
        /// If the value is an array of <see cref="IConvertible"/> having length one, then the single item is converted.
        /// </remarks>
        /// <exception cref="MetadataException">No value exists for <paramref name="tagType"/>, or the value is not convertible to the requested type.</exception>
        [Pure]
        public static short GetInt16(this Directory directory, int tagType)
        {
            if (directory.TryGetInt16(tagType, out short value))
                return value;

            return ThrowValueNotPossible<short>(directory, tagType);
        }

        [Pure]
        public static bool TryGetInt16(this Directory directory, int tagType, out short value)
        {
            var convertible = GetConvertibleObject(directory, tagType);

            if (convertible is not null)
            {
                try
                {
                    value = convertible.ToInt16(null);
                    return true;
                }
                catch
                {
                    // ignored
                }
            }

            value = default;
            return false;
        }

        #endregion

        #region UInt16

        /// <summary>Returns a tag's value as a <see cref="ushort"/>, or throws if conversion is not possible.</summary>
        /// <remarks>
        /// If the value is <see cref="IConvertible"/>, then that interface is used for conversion of the value.
        /// If the value is an array of <see cref="IConvertible"/> having length one, then the single item is converted.
        /// </remarks>
        /// <exception cref="MetadataException">No value exists for <paramref name="tagType"/>, or the value is not convertible to the requested type.</exception>
        [Pure]
        public static ushort GetUInt16(this Directory directory, int tagType)
        {
            if (directory.TryGetUInt16(tagType, out ushort value))
                return value;

            return ThrowValueNotPossible<ushort>(directory, tagType);
        }

        [Pure]
        public static bool TryGetUInt16(this Directory directory, int tagType, out ushort value)
        {
            var convertible = GetConvertibleObject(directory, tagType);

            if (convertible is not null)
            {
                try
                {
                    value = convertible.ToUInt16(null);
                    return true;
                }
                catch
                {
                    // ignored
                }
            }

            value = default;
            return false;
        }

        #endregion

        #region Int32

        /// <summary>Returns a tag's value as an <see cref="int"/>, or throws if conversion is not possible.</summary>
        /// <remarks>
        /// If the value is <see cref="IConvertible"/>, then that interface is used for conversion of the value.
        /// If the value is an array of <see cref="IConvertible"/> having length one, then the single item is converted.
        /// </remarks>
        /// <exception cref="MetadataException">No value exists for <paramref name="tagType"/>, or the value is not convertible to the requested type.</exception>
        [Pure]
        public static int GetInt32(this Directory directory, int tagType)
        {
            if (directory.TryGetInt32(tagType, out int value))
                return value;

            return ThrowValueNotPossible<int>(directory, tagType);
        }

        [Pure]
        public static bool TryGetInt32(this Directory directory, int tagType, out int value)
        {
            var convertible = GetConvertibleObject(directory, tagType);

            if (convertible is not null)
            {
                try
                {
                    value = convertible.ToInt32(null);
                    return true;
                }
                catch
                {
                    // ignored
                }
            }

            value = default;
            return false;
        }

        #endregion

        #region UInt32

        /// <summary>Returns a tag's value as a <see cref="uint"/>, or throws if conversion is not possible.</summary>
        /// <remarks>
        /// If the value is <see cref="IConvertible"/>, then that interface is used for conversion of the value.
        /// If the value is an array of <see cref="IConvertible"/> having length one, then the single item is converted.
        /// </remarks>
        /// <exception cref="MetadataException">No value exists for <paramref name="tagType"/>, or the value is not convertible to the requested type.</exception>
        public static uint GetUInt32(this Directory directory, int tagType)
        {
            if (directory.TryGetUInt32(tagType, out uint value))
                return value;

            return ThrowValueNotPossible<ushort>(directory, tagType);
        }

        public static bool TryGetUInt32(this Directory directory, int tagType, out uint value)
        {
            var convertible = GetConvertibleObject(directory, tagType);

            if (convertible is not null)
            {
                try
                {
                    value = convertible.ToUInt32(null);
                    return true;
                }
                catch
                {
                    // ignored
                }
            }

            value = default;
            return false;
        }

        #endregion

        #region Int64

        /// <summary>Returns a tag's value as a <see cref="long"/>, or throws if conversion is not possible.</summary>
        /// <remarks>
        /// If the value is <see cref="IConvertible"/>, then that interface is used for conversion of the value.
        /// If the value is an array of <see cref="IConvertible"/> having length one, then the single item is converted.
        /// </remarks>
        /// <exception cref="MetadataException">No value exists for <paramref name="tagType"/>, or the value is not convertible to the requested type.</exception>
        [Pure]
        public static long GetInt64(this Directory directory, int tagType)
        {
            if (directory.TryGetInt64(tagType, out long value))
                return value;

            return ThrowValueNotPossible<long>(directory, tagType);
        }

        [Pure]
        public static bool TryGetInt64(this Directory directory, int tagType, out long value)
        {
            var convertible = GetConvertibleObject(directory, tagType);

            if (convertible is not null)
            {
                try
                {
                    value = convertible.ToInt64(null);
                    return true;
                }
                catch
                {
                    // ignored
                }
            }

            value = default;
            return false;
        }

        #endregion

        #region UInt64

        /// <summary>Returns a tag's value as an <see cref="ulong"/>, or throws if conversion is not possible.</summary>
        /// <remarks>
        /// If the value is <see cref="IConvertible"/>, then that interface is used for conversion of the value.
        /// If the value is an array of <see cref="IConvertible"/> having length one, then the single item is converted.
        /// </remarks>
        /// <exception cref="MetadataException">No value exists for <paramref name="tagType"/>, or the value is not convertible to the requested type.</exception>
        [Pure]
        public static ulong GetUInt64(this Directory directory, int tagType)
        {
            if (directory.TryGetUInt64(tagType, out ulong value))
                return value;

            return ThrowValueNotPossible<ulong>(directory, tagType);
        }

        [Pure]
        public static bool TryGetUInt64(this Directory directory, int tagType, out ulong value)
        {
            var convertible = GetConvertibleObject(directory, tagType);

            if (convertible is not null)
            {
                try
                {
                    value = convertible.ToUInt64(null);
                    return true;
                }
                catch
                {
                    // ignored
                }
            }

            value = default;
            return false;
        }

        #endregion

        #region Single

        /// <summary>Returns a tag's value as a <see cref="float"/>, or throws if conversion is not possible.</summary>
        /// <remarks>
        /// If the value is <see cref="IConvertible"/>, then that interface is used for conversion of the value.
        /// If the value is an array of <see cref="IConvertible"/> having length one, then the single item is converted.
        /// </remarks>
        /// <exception cref="MetadataException">No value exists for <paramref name="tagType"/>, or the value is not convertible to the requested type.</exception>
        [Pure]
        public static float GetSingle(this Directory directory, int tagType)
        {
            if (directory.TryGetSingle(tagType, out float value))
                return value;

            return ThrowValueNotPossible<float>(directory, tagType);
        }

        [Pure]
        public static bool TryGetSingle(this Directory directory, int tagType, out float value)
        {
            var convertible = GetConvertibleObject(directory, tagType);

            if (convertible is not null)
            {
                try
                {
                    value = convertible.ToSingle(null);
                    return true;
                }
                catch
                {
                    // ignored
                }
            }

            value = default;
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
        [Pure]
        public static double GetDouble(this Directory directory, int tagType)
        {
            if (directory.TryGetDouble(tagType, out double value))
                return value;

            return ThrowValueNotPossible<double>(directory, tagType);
        }

        [Pure]
        public static bool TryGetDouble(this Directory directory, int tagType, out double value)
        {
            var convertible = GetConvertibleObject(directory, tagType);

            if (convertible is not null)
            {
                try
                {
                    value = convertible.ToSingle(null);
                    return true;
                }
                catch
                {
                    // ignored
                }
            }

            value = default;
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
        [Pure]
        public static bool GetBoolean(this Directory directory, int tagType)
        {
            if (directory.TryGetBoolean(tagType, out bool value))
                return value;

            return ThrowValueNotPossible<bool>(directory, tagType);
        }

        [Pure]
        public static bool TryGetBoolean(this Directory directory, int tagType, out bool value)
        {
            var convertible = GetConvertibleObject(directory, tagType);

            if (convertible is not null)
            {
                try
                {
                    value = convertible.ToBoolean(null);
                    return true;
                }
                catch
                {
                    // ignored
                }
            }

            value = default;
            return false;
        }

        #endregion

        /// <summary>Gets the specified tag's value as a String array, if possible.</summary>
        /// <remarks>Only supported where the tag is set as String[], String, int[], byte[] or Rational[].</remarks>
        /// <returns>the tag's value as an array of Strings. If the value is unset or cannot be converted, <see langword="null" /> is returned.</returns>
        [Pure]
        public static string[]? GetStringArray(this Directory directory, int tagType)
        {
            var o = directory.GetObject(tagType);

            if (o is null)
                return null;

            if (o is string[] strings)
                return strings;

            if (o is string s)
                return [s];

            if (o is StringValue sv)
                return [sv.ToString()];

            if (o is StringValue[] stringValues)
            {
                var strs = new string[stringValues.Length];
                for (var i = 0; i < strs.Length; i++)
                    strs[i] = stringValues[i].ToString();
                return strs;
            }

            if (o is int[] ints)
            {
                strings = new string[ints.Length];
                for (var i = 0; i < strings.Length; i++)
                    strings[i] = ints[i].ToString();
                return strings;
            }

            if (o is byte[] bytes)
            {
                strings = new string[bytes.Length];
                for (var i = 0; i < strings.Length; i++)
                    strings[i] = ((int)bytes[i]).ToString();
                return strings;
            }

            if (o is Rational[] rationals)
            {
                strings = new string[rationals.Length];
                for (var i = 0; i < strings.Length; i++)
                    strings[i] = rationals[i].ToSimpleString(false);
                return strings;
            }

            return null;
        }

        /// <summary>Gets the specified tag's value as a StringValue array, if possible.</summary>
        /// <remarks>Only succeeds if the tag is set as StringValue[], or String.</remarks>
        /// <returns>the tag's value as an array of StringValues. If the value is unset or cannot be converted, <see langword="null" /> is returned.</returns>
        [Pure]
        public static StringValue[]? GetStringValueArray(this Directory directory, int tagType)
        {
            var o = directory.GetObject(tagType);

            if (o is null)
                return null;
            if (o is StringValue[] stringValues)
                return stringValues;
            if (o is StringValue sv)
                return [sv];

            return null;
        }

        /// <summary>Gets the specified tag's value as an int array, if possible.</summary>
        /// <remarks>Only supported where the tag is set as String, Integer, int[], byte[] or Rational[].</remarks>
        /// <returns>the tag's value as an int array</returns>
        [Pure]
        public static int[]? GetInt32Array(this Directory directory, int tagType)
        {
            var o = directory.GetObject(tagType);

            if (o is null)
                return null;

            if (o is int[] ints)
                return ints;

            if (o is Rational[] rationals)
            {
                ints = new int[rationals.Length];
                for (var i = 0; i < ints.Length; i++)
                    ints[i] = rationals[i].ToInt32();
                return ints;
            }

            if (o is short[] shorts)
            {
                ints = new int[shorts.Length];
                for (var i = 0; i < shorts.Length; i++)
                    ints[i] = shorts[i];
                return ints;
            }

            if (o is sbyte[] sbytes)
            {
                ints = new int[sbytes.Length];
                for (var i = 0; i < sbytes.Length; i++)
                    ints[i] = sbytes[i];
                return ints;
            }

            if (o is byte[] bytes)
            {
                ints = new int[bytes.Length];
                for (var i = 0; i < bytes.Length; i++)
                    ints[i] = bytes[i];
                return ints;
            }

            if (o is string str)
            {
                ints = new int[str.Length];
                for (var i = 0; i < str.Length; i++)
                    ints[i] = str[i];
                return ints;
            }

            var nullableInt = o as int?;
            if (nullableInt != null)
                return [(int)o];

            return null;
        }

        /// <summary>Gets the specified tag's value as an byte array, if possible.</summary>
        /// <remarks>Only supported where the tag is set as StringValue, String, Integer, int[], byte[] or Rational[].</remarks>
        /// <returns>the tag's value as a byte array</returns>
        [Pure]
        public static byte[]? GetByteArray(this Directory directory, int tagType)
        {
            var o = directory.GetObject(tagType);

            if (o is null)
                return null;

            if (o is StringValue value)
                return value.Bytes;

            byte[]? bytes;

            if (o is Rational[] rationals)
            {
                bytes = new byte[rationals.Length];
                for (var i = 0; i < bytes.Length; i++)
                    bytes[i] = rationals[i].ToByte();
                return bytes;
            }

            bytes = o as byte[];
            if (bytes is not null)
                return bytes;

            if (o is int[] ints)
            {
                bytes = new byte[ints.Length];
                for (var i = 0; i < ints.Length; i++)
                    bytes[i] = unchecked((byte)ints[i]);
                return bytes;
            }

            if (o is short[] shorts)
            {
                bytes = new byte[shorts.Length];
                for (var i = 0; i < shorts.Length; i++)
                    bytes[i] = unchecked((byte)shorts[i]);
                return bytes;
            }

            if (o is string str)
            {
                bytes = new byte[str.Length];
                for (var i = 0; i < str.Length; i++)
                    bytes[i] = unchecked((byte)str[i]);
                return bytes;
            }

            var nullableInt = o as int?;
            if (nullableInt != null)
                return [(byte)nullableInt.Value];

            return null;
        }

        #region DateTime

        /// <summary>Returns a tag's value as a <see cref="DateTime"/>, or throws if conversion is not possible.</summary>
        /// <remarks>
        /// If the value is <see cref="IConvertible"/>, then that interface is used for conversion of the value.
        /// If the value is an array of <see cref="IConvertible"/> having length one, then the single item is converted.
        /// </remarks>
        /// <exception cref="MetadataException">No value exists for <paramref name="tagType"/>, or the value is not convertible to the requested type.</exception>
        public static DateTime GetDateTime(this Directory directory, int tagType /*, TimeZoneInfo? timeZone = null*/)
        {
            if (directory.TryGetDateTime(tagType, out DateTime value))
                return value;

            return ThrowValueNotPossible<DateTime>(directory, tagType);
        }

        // This seems to cover all known Exif date strings
        // Note that "    :  :     :  :  " is a valid date string according to the Exif spec (which means 'unknown date'): http://www.awaresystems.be/imaging/tiff/tifftags/privateifd/exif/datetimeoriginal.html
        // Custom format reference: https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings
        private static readonly string[] _datePatterns =
        [
            "yyyy:MM:dd HH:mm:ss.fff",
            "yyyy:MM:dd HH:mm:ss.fffzzz",
            "yyyy:MM:dd HH:mm:ss",
            "yyyy:MM:dd HH:mm:sszzz",
            "yyyy:MM:dd HH:mm",
            "yyyy:MM:dd HH:mmzzz",
            "yyyy-MM-dd HH:mm:ss.fff",
            "yyyy-MM-dd HH:mm:ss.fffzzz",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd HH:mm:sszzz",
            "yyyy-MM-dd HH:mm",
            "yyyy-MM-dd HH:mmzzz",
            "yyyy.MM.dd HH:mm:ss",
            "yyyy.MM.dd HH:mm:sszzz",
            "yyyy.MM.dd HH:mm",
            "yyyy.MM.dd HH:mmzzz",
            "yyyy-MM-ddTHH:mm:ss.fff",
            "yyyy-MM-ddTHH:mm:ss.fffzzz",
            "yyyy-MM-ddTHH:mm:ss.ff",
            "yyyy-MM-ddTHH:mm:ss.f",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm:sszzz",
            "yyyy-MM-ddTHH:mm",
            "yyyy-MM-ddTHH:mmzzz",
            "yyyy:MM:dd",
            "yyyy-MM-dd",
            "yyyy-MM",
            "yyyyMMdd", // as used in IPTC data
            "yyyy"
        ];

        /// <summary>Attempts to return the specified tag's value as a DateTime.</summary>
        /// <remarks>
        /// <para>
        /// If the underlying value is a <see cref="string"/>, then attempts will be made to parse it.
        /// </para>
        /// <para>
        /// If that string contains a time-zone offset, the returned <see cref="DateTime"/> will have kind <see cref="DateTimeKind.Utc"/>,
        /// otherwise it will be <see cref="DateTimeKind.Unspecified"/>.
        /// </para>
        /// </remarks>
        /// <returns><c>true</c> if a DateTime was returned, otherwise <c>false</c>.</returns>
        [Pure]
        public static bool TryGetDateTime(this Directory directory, int tagType /*, TimeZoneInfo? timeZone = null*/, out DateTime dateTime)
        {
            var o = directory.GetObject(tagType);

            if (o is null)
            {
                dateTime = default;
                return false;
            }

            if (o is DateTime dt)
            {
                dateTime = dt;
                return true;
            }

            var s = o as string;

            if (o is StringValue sv)
                s = sv.ToString();

            if (s is not null)
            {
                if (DateTime.TryParseExact(s, _datePatterns, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AdjustToUniversal, out dateTime))
                {
                    return true;
                }

                dateTime = default;
                return false;
            }

            if (o is IConvertible convertible)
            {
                try
                {
                    dateTime = convertible.ToDateTime(null);
                    return true;
                }
                catch (FormatException)
                { }
            }

            dateTime = default;
            return false;
        }

        #endregion

        #region Rational

        [Pure]
        public static Rational GetRational(this Directory directory, int tagType)
        {
            if (directory.TryGetRational(tagType, out Rational value))
                return value;

            return ThrowValueNotPossible<Rational>(directory, tagType);
        }

        /// <summary>Returns the specified tag's value as a Rational.</summary>
        /// <remarks>If the value is unset or cannot be converted, <see langword="null" /> is returned.</remarks>
        [Pure]
        public static bool TryGetRational(this Directory directory, int tagType, out Rational value)
        {
            var o = directory.GetObject(tagType);

            if (o is null)
            {
                value = default;
                return false;
            }

            if (o is Rational r)
            {
                value = r;
                return true;
            }

            if (o is int i)
            {
                value = new Rational(i, 1);
                return true;
            }

            if (o is long l)
            {
                value = new Rational(l, 1);
                return true;
            }

            // NOTE not doing conversions for real number types

            value = default;
            return false;
        }

        #endregion

        /// <summary>Returns the specified tag's value as an array of Rational.</summary>
        /// <remarks>If the value is unset or cannot be converted, <see langword="null" /> is returned.</remarks>
        [Pure]
        public static Rational[]? GetRationalArray(this Directory directory, int tagType)
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
        /// <see langword="null" /> if the tag hasn't been defined.
        /// </returns>
        [Pure]
        public static string? GetString(this Directory directory, int tagType)
        {
            var o = directory.GetObject(tagType);

            if (o is null)
                return null;

            if (o is Rational r)
                return r.ToSimpleString();

            if (o is DateTime dt)
                return dt.ToString(
                    dt.Kind != DateTimeKind.Unspecified
                        ? "ddd MMM dd HH:mm:ss zzz yyyy"
                        : "ddd MMM dd HH:mm:ss yyyy");

            if (o is bool b)
                return b ? "true" : "false";

            // handle arrays of objects and primitives
            if (o is Array array)
            {
                var componentType = array.GetType().GetElementType();
                var str = new StringBuilder();

                if (componentType == typeof(float))
                {
                    var values = (float[])array;
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.AppendFormat("{0:0.###}", values[i]);
                    }
                }
                else if (componentType == typeof(double))
                {
                    var values = (double[])array;
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.AppendFormat("{0:0.###}", values[i]);
                    }
                }
                else if (componentType == typeof(int))
                {
                    var values = (int[])array;
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(values[i]);
                    }
                }
                else if (componentType == typeof(uint))
                {
                    var values = (uint[])array;
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(values[i]);
                    }
                }
                else if (componentType == typeof(short))
                {
                    var values = (short[])array;
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(values[i]);
                    }
                }
                else if (componentType == typeof(ushort))
                {
                    var values = (ushort[])array;
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(values[i]);
                    }
                }
                else if (componentType == typeof(byte))
                {
                    var values = (byte[])array;
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(values[i]);
                    }
                }
                else if (componentType == typeof(sbyte))
                {
                    var values = (sbyte[])array;
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(values[i]);
                    }
                }
                else if (componentType == typeof(Rational))
                {
                    var values = (Rational[])array;
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(values[i]);
                    }
                }
                else if (componentType == typeof(string))
                {
                    var values = (string[])array;
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(values[i]);
                    }
                }
                else if (componentType is { IsByRef: true })
                {
                    var values = (object[])array;
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(values[i]);
                    }
                }
                else
                {
                    for (var i = 0; i < array.Length; i++)
                    {
                        if (i != 0)
                            str.Append(' ');
                        str.Append(array.GetValue(i));
                    }
                }

                return str.ToString();
            }

            if (o is IEnumerable<string> strings)
            {
#if NET35
                var sb = new StringBuilder();
                foreach (var s in strings)
                {
                    if (sb.Length != 0)
                        sb.Append(' ');
                    sb.Append(s);
                }
                return sb.ToString();
#else
                return string.Join(" ", strings);
#endif
            }

            if (o is double d)
                return d.ToString("0.###");

            if (o is float f)
                return f.ToString("0.###");

            // Note that several cameras leave trailing spaces (Olympus, Nikon) but this library is intended to show
            // the actual data within the file.  It is not inconceivable that whitespace may be significant here, so we
            // do not trim.  Also, if support is added for writing data back to files, this may cause issues.
            // We leave trimming to the presentation layer.
            return o.ToString();
        }

        [Pure]
        public static string? GetString(this Directory directory, int tagType, Encoding encoding)
        {
            var bytes = directory.GetByteArray(tagType);
            return bytes is null ? null
                : encoding.GetString(bytes, 0, bytes.Length);
        }

        [Pure]
        public static StringValue GetStringValue(this Directory directory, int tagType)
        {
            var o = directory.GetObject(tagType);

            if (o is StringValue value)
                return value;

            return default;
        }

        [Pure]
        private static IConvertible? GetConvertibleObject(this Directory directory, int tagType)
        {
            var o = directory.GetObject(tagType);

            if (o is null)
                return null;

            if (o is IConvertible convertible)
                return convertible;

            if (o is Array { Length: 1, Rank: 1 } array)
                return array.GetValue(0) as IConvertible;

            return null;
        }

        private static T ThrowValueNotPossible<T>(Directory directory, int tagType)
        {
            var o = directory.GetObject(tagType);

            if (o is null)
                throw new MetadataException($"No value exists for tag {directory.GetTagName(tagType)}.");

            throw new MetadataException($"Tag {tagType} cannot be converted to {typeof(T).Name}. It is of type {o.GetType()} with value: {o}");
        }
    }
}
