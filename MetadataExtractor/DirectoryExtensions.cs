using System;
using System.Globalization;
using System.Text;
using JetBrains.Annotations;
using Sharpen;

namespace MetadataExtractor
{
    public static class DirectoryExtensions
    {
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
        public static int GetInt(this Directory directory, int tagType)
        {
            var integer = directory.GetInteger(tagType);

            if (integer != null)
                return (int)integer;

            var o = directory.GetObject(tagType);
            if (o == null)
                throw new MetadataException("Tag '" + directory.GetTagName(tagType) + "' has not been set -- check using containsTag() first");

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
        public static int? GetInteger(this Directory directory, int tagType)
        {
            var o = directory.GetObject(tagType);
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
        /// <param name="tagType">the tag identifier</param>
        /// <returns>the tag's value as an int array</returns>
        [CanBeNull]
        public static int[] GetIntArray(this Directory directory, int tagType)
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
        /// <param name="tagType">the tag identifier</param>
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

        /// <summary>Returns the specified tag's value as a double, if possible.</summary>
        /// <exception cref="MetadataException"/>
        public static double GetDouble(this Directory directory, int tagType)
        {
            var value = directory.GetDoubleObject(tagType);

            if (value != null)
                return (double)value;

            var o = directory.GetObject(tagType);
            if (o == null)
                throw new MetadataException("Tag '" + directory.GetTagName(tagType) + "' has not been set -- check using containsTag() first");

            throw new MetadataException("Tag '" + tagType + "' cannot be converted to a double.  It is of type '" + o.GetType() + "'.");
        }

        /// <summary>Returns the specified tag's value as a Double.</summary>
        /// <remarks>If the tag is not set or cannot be converted, <c>null</c> is returned.</remarks>
        [CanBeNull]
        public static double? GetDoubleObject(this Directory directory, int tagType)
        {
            var o = directory.GetObject(tagType);

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
        public static float GetFloat(this Directory directory, int tagType)
        {
            var value = directory.GetFloatObject(tagType);
            if (value != null)
                return (float)value;

            var o = directory.GetObject(tagType);
            if (o == null)
                throw new MetadataException("Tag '" + directory.GetTagName(tagType) + "' has not been set -- check using containsTag() first");

            throw new MetadataException("Tag '" + tagType + "' cannot be converted to a float.  It is of type '" + o.GetType() + "'.");
        }

        /// <summary>Returns the specified tag's value as a float.</summary>
        /// <remarks>If the tag is not set or cannot be converted, <c>null</c> is returned.</remarks>
        [CanBeNull]
        public static float? GetFloatObject(this Directory directory, int tagType)
        {
            var o = directory.GetObject(tagType);

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
        public static long GetLong(this Directory directory, int tagType)
        {
            var value = directory.GetLongObject(tagType);

            if (value != null)
                return (long)value;

            var o = directory.GetObject(tagType);
            if (o == null)
                throw new MetadataException("Tag '" + directory.GetTagName(tagType) + "' has not been set -- check using containsTag() first");

            throw new MetadataException("Tag '" + tagType + "' cannot be converted to a long.  It is of type '" + o.GetType() + "'.");
        }

        /// <summary>Returns the specified tag's value as a long.</summary>
        /// <remarks>If the tag is not set or cannot be converted, <c>null</c> is returned.</remarks>
        [CanBeNull]
        public static long? GetLongObject(this Directory directory, int tagType)
        {
            var o = directory.GetObject(tagType);

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
        public static bool GetBoolean(this Directory directory, int tagType)
        {
            var value = directory.GetBooleanObject(tagType);

            if (value != null)
                return (bool)value;

            var o = directory.GetObject(tagType);
            if (o == null)
                throw new MetadataException("Tag '" + directory.GetTagName(tagType) + "' has not been set -- check using containsTag() first");

            throw new MetadataException("Tag '" + tagType + "' cannot be converted to a boolean.  It is of type '" + o.GetType() + "'.");
        }

        /// <summary>Returns the specified tag's value as a boolean.</summary>
        /// <remarks>If the tag is not set or cannot be converted, <c>null</c> is returned.</remarks>
        [CanBeNull]
        public static bool? GetBooleanObject(this Directory directory, int tagType)
        {
            var o = directory.GetObject(tagType);

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
        public static DateTime? GetDate(this Directory directory, int tagType/*, [CanBeNull] TimeZoneInfo timeZone = null*/)
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
                    directory.AddError("Unexpected array component type: " + componentType.FullName);
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
        public static string GetString(this Directory directory, int tagType, Encoding encoding)
        {
            var bytes = directory.GetByteArray(tagType);
            return bytes == null
                ? null
                : encoding.GetString(bytes);
        }
    }
}