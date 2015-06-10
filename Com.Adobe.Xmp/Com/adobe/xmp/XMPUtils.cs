// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

using System;
using Com.Adobe.Xmp.Impl;
using Com.Adobe.Xmp.Options;

namespace Com.Adobe.Xmp
{
    /// <summary>Utility methods for XMP.</summary>
    /// <remarks>
    /// Utility methods for XMP. I included only those that are different from the
    /// Java default conversion utilities.
    /// </remarks>
    /// <since>21.02.2006</since>
    public static class XmpUtils
    {
        /// <summary>Create a single edit string from an array of strings.</summary>
        /// <param name="xmp">The XMP object containing the array to be catenated.</param>
        /// <param name="schemaNs">
        /// The schema namespace URI for the array. Must not be null or
        /// the empty string.
        /// </param>
        /// <param name="arrayName">
        /// The name of the array. May be a general path expression, must
        /// not be null or the empty string. Each item in the array must
        /// be a simple string value.
        /// </param>
        /// <param name="separator">
        /// The string to be used to separate the items in the catenated
        /// string. Defaults to &quot;; &quot;, ASCII semicolon and space
        /// (U+003B, U+0020).
        /// </param>
        /// <param name="quotes">
        /// The characters to be used as quotes around array items that
        /// contain a separator. Defaults to &apos;&quot;&apos;
        /// </param>
        /// <param name="allowCommas">Option flag to control the catenation.</param>
        /// <returns>Returns the string containing the catenated array items.</returns>
        /// <exception cref="XmpException">Forwards the Exceptions from the metadata processing</exception>
        /// <exception cref="XmpException"/>
        public static string CatenateArrayItems(IXmpMeta xmp, string schemaNs, string arrayName, string separator, string quotes, bool allowCommas)
        {
            return Impl.XmpUtils.CatenateArrayItems(xmp, schemaNs, arrayName, separator, quotes, allowCommas);
        }

        /// <summary>Separate a single edit string into an array of strings.</summary>
        /// <param name="xmp">The XMP object containing the array to be updated.</param>
        /// <param name="schemaNs">
        /// The schema namespace URI for the array. Must not be null or
        /// the empty string.
        /// </param>
        /// <param name="arrayName">
        /// The name of the array. May be a general path expression, must
        /// not be null or the empty string. Each item in the array must
        /// be a simple string value.
        /// </param>
        /// <param name="catedStr">The string to be separated into the array items.</param>
        /// <param name="arrayOptions">Option flags to control the separation.</param>
        /// <param name="preserveCommas">Flag if commas shall be preserved</param>
        /// <exception cref="XmpException">Forwards the Exceptions from the metadata processing</exception>
        /// <exception cref="XmpException"/>
        public static void SeparateArrayItems(IXmpMeta xmp, string schemaNs, string arrayName, string catedStr, PropertyOptions arrayOptions, bool preserveCommas)
        {
            Impl.XmpUtils.SeparateArrayItems(xmp, schemaNs, arrayName, catedStr, arrayOptions, preserveCommas);
        }

        /// <summary>Remove multiple properties from an XMP object.</summary>
        /// <remarks>
        /// Remove multiple properties from an XMP object.
        /// RemoveProperties was created to support the File Info dialog's Delete
        /// button, and has been been generalized somewhat from those specific needs.
        /// It operates in one of three main modes depending on the schemaNS and
        /// propName parameters:
        /// <list type="bullet">
        /// <item> Non-empty <c>schemaNS</c> and <c>propName</c> - The named property is
        /// removed if it is an external property, or if the
        /// flag <c>doAllProperties</c> option is true. It does not matter whether the
        /// named property is an actual property or an alias.</item>
        /// <item> Non-empty <c>schemaNS</c> and empty <c>propName</c> - The all external
        /// properties in the named schema are removed. Internal properties are also
        /// removed if the flag <c>doAllProperties</c> option is set. In addition,
        /// aliases from the named schema will be removed if the flag <c>includeAliases</c>
        /// option is set.</item>
        /// <item> Empty <c>schemaNS</c> and empty <c>propName</c> - All external properties in
        /// all schema are removed. Internal properties are also removed if the
        /// flag <c>doAllProperties</c> option is passed. Aliases are implicitly handled
        /// because the associated actuals are internal if the alias is.</item>
        /// </list>
        /// It is an error to pass an empty <c>schemaNS</c> and non-empty <c>propName</c>.
        /// </remarks>
        /// <param name="xmp">The XMP object containing the properties to be removed.</param>
        /// <param name="schemaNs">
        /// Optional schema namespace URI for the properties to be
        /// removed.
        /// </param>
        /// <param name="propName">Optional path expression for the property to be removed.</param>
        /// <param name="doAllProperties">
        /// Option flag to control the deletion: do internal properties in
        /// addition to external properties.
        /// </param>
        /// <param name="includeAliases">
        /// Option flag to control the deletion:
        /// Include aliases in the "named schema" case above.
        /// <em>Note:</em> Currently not supported.
        /// </param>
        /// <exception cref="XmpException">Forwards the Exceptions from the metadata processing</exception>
        /// <exception cref="XmpException"/>
        public static void RemoveProperties(IXmpMeta xmp, string schemaNs, string propName, bool doAllProperties, bool includeAliases)
        {
            Impl.XmpUtils.RemoveProperties(xmp, schemaNs, propName, doAllProperties, includeAliases);
        }

        /// <summary>Append properties from one XMP object to another.</summary>
        /// <remarks>
        /// Append properties from one XMP object to another.
        /// <para />XMPUtils#appendProperties was created to support the File Info dialog's Append button, and
        /// has been been generalized somewhat from those specific needs. It appends information from one
        /// XMP object (source) to another (dest). The default operation is to append only external
        /// properties that do not already exist in the destination. The flag
        /// <c>doAllProperties</c> can be used to operate on all properties, external and internal.
        /// The flag <c>replaceOldValues</c> option can be used to replace the values
        /// of existing properties. The notion of external
        /// versus internal applies only to top level properties. The keep-or-replace-old notion applies
        /// within structs and arrays as described below.
        /// <list type="bullet">
        /// <item>If <c>replaceOldValues</c> is true then the processing is restricted to the top
        /// level properties. The processed properties from the source (according to
        /// <c>doAllProperties</c>) are propagated to the destination,
        /// replacing any existing values.Properties in the destination that are not in the source
        /// are left alone.</item>
        /// <item>If <c>replaceOldValues</c> is not passed then the processing is more complicated.
        /// Top level properties are added to the destination if they do not already exist.
        /// If they do exist but differ in form (simple/struct/array) then the destination is left alone.
        /// If the forms match, simple properties are left unchanged while structs and arrays are merged.</item>
        /// <item>If <c>deleteEmptyValues</c> is passed then an empty value in the source XMP causes
        /// the corresponding destination XMP property to be deleted. The default is to treat empty
        /// values the same as non-empty values. An empty value is any of a simple empty string, an array
        /// with no items, or a struct with no fields. Qualifiers are ignored.</item>
        /// </list>
        /// <para />The detailed behavior is defined by the following pseudo-code:
        /// <blockquote>
        /// <pre>
        /// appendProperties ( sourceXMP, destXMP, doAllProperties,
        /// replaceOldValues, deleteEmptyValues ):
        /// for all source schema (top level namespaces):
        /// for all top level properties in sourceSchema:
        /// if doAllProperties or prop is external:
        /// appendSubtree ( sourceNode, destSchema, replaceOldValues, deleteEmptyValues )
        /// appendSubtree ( sourceNode, destParent, replaceOldValues, deleteEmptyValues ):
        /// if deleteEmptyValues and source value is empty:
        /// delete the corresponding child from destParent
        /// else if sourceNode not in destParent (by name):
        /// copy sourceNode's subtree to destParent
        /// else if replaceOld:
        /// delete subtree from destParent
        /// copy sourceNode's subtree to destParent
        /// else:
        /// // Already exists in dest and not replacing, merge structs and arrays
        /// if sourceNode and destNode forms differ:
        /// return, leave the destNode alone
        /// else if form is a struct:
        /// for each field in sourceNode:
        /// AppendSubtree ( sourceNode.field, destNode, replaceOldValues )
        /// else if form is an alt-text array:
        /// copy new items by "xml:lang" value into the destination
        /// else if form is an array:
        /// copy new items by value into the destination, ignoring order and duplicates
        /// </pre>
        /// </blockquote>
        /// <para /><em>Note:</em> appendProperties can be expensive if replaceOldValues is not passed and
        /// the XMP contains large arrays. The array item checking described above is n-squared.
        /// Each source item is checked to see if it already exists in the destination,
        /// without regard to order or duplicates.
        /// <para />Simple items are compared by value and "xml:lang" qualifier, other qualifiers are ignored.
        /// Structs are recursively compared by field names, without regard to field order. Arrays are
        /// compared by recursively comparing all items.
        /// </remarks>
        /// <param name="source">The source XMP object.</param>
        /// <param name="dest">The destination XMP object.</param>
        /// <param name="doAllProperties">Do internal properties in addition to external properties.</param>
        /// <param name="replaceOldValues">Replace the values of existing properties.</param>
        /// <param name="deleteEmptyValues">Delete destination values if source property is empty.</param>
        /// <exception cref="XmpException">Forwards the Exceptions from the metadata processing</exception>
        /// <exception cref="XmpException"/>
        public static void AppendProperties(IXmpMeta source, IXmpMeta dest, bool doAllProperties, bool replaceOldValues, bool deleteEmptyValues = false)
        {
            Impl.XmpUtils.AppendProperties(source, dest, doAllProperties, replaceOldValues, deleteEmptyValues);
        }

        /// <summary>Convert from string to Boolean.</summary>
        /// <param name="value">The string representation of the Boolean.</param>
        /// <returns>
        /// The appropriate boolean value for the string. The checked values
        /// for <c>true</c> and <c>false</c> are:
        /// <list type="bullet">
        /// <item><see cref="XmpConstConstants.TrueString"/> and <see cref="XmpConstConstants.FalseString"/></item>
        /// <item>&quot;t&quot; and &quot;f&quot;</item>
        /// <item>&quot;on&quot; and &quot;off&quot;</item>
        /// <item>&quot;yes&quot; and &quot;no&quot;</item>
        /// <item>&quot;value &lt;&gt; 0&quot; and &quot;value == 0&quot;</item>
        /// </list>
        /// </returns>
        /// <exception cref="XmpException">If an empty string is passed.</exception>
        /// <exception cref="XmpException"/>
        public static bool ConvertToBoolean(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new XmpException("Empty convert-string", XmpErrorCode.BadValue);
            }
            value = value.ToLower();
            try
            {
                // First try interpretation as Integer (anything not 0 is true)
                return Convert.ToInt32(value) != 0;
            }
            catch (FormatException)
            {
                return "true".Equals(value) || "t".Equals(value) || "on".Equals(value) || "yes".Equals(value);
            }
        }

        /// <summary>Convert from boolean to string.</summary>
        /// <param name="value">a boolean value</param>
        /// <returns>
        /// The XMP string representation of the boolean. The values used are
        /// given by the constants <see cref="XmpConstConstants.TrueString"/> and
        /// <see cref="XmpConstConstants.FalseString"/>.
        /// </returns>
        public static string ConvertFromBoolean(bool value)
        {
            return value ? XmpConstConstants.TrueString : XmpConstConstants.FalseString;
        }

        /// <summary>Converts a string value to an <c>int</c>.</summary>
        /// <param name="rawValue">the string value</param>
        /// <returns>Returns an int.</returns>
        /// <exception cref="XmpException">
        /// If the <c>rawValue</c> is <c>null</c> or empty or the
        /// conversion fails.
        /// </exception>
        /// <exception cref="XmpException"/>
        public static int ConvertToInteger(string rawValue)
        {
            try
            {
                if (string.IsNullOrEmpty(rawValue))
                {
                    throw new XmpException("Empty convert-string", XmpErrorCode.BadValue);
                }
                if (rawValue.StartsWith("0x"))
                {
                    return Convert.ToInt32(rawValue.Substring (2), 16);
                }
                return Convert.ToInt32(rawValue);
            }
            catch (FormatException)
            {
                throw new XmpException("Invalid integer string", XmpErrorCode.BadValue);
            }
        }

        /// <summary>Convert from int to string.</summary>
        /// <param name="value">an int value</param>
        /// <returns>The string representation of the int.</returns>
        public static string ConvertFromInteger(int value)
        {
            return value.ToString();
        }

        /// <summary>Converts a string value to a <c>long</c>.</summary>
        /// <param name="rawValue">the string value</param>
        /// <returns>Returns a long.</returns>
        /// <exception cref="XmpException">
        /// If the <c>rawValue</c> is <c>null</c> or empty or the
        /// conversion fails.
        /// </exception>
        /// <exception cref="XmpException"/>
        public static long ConvertToLong(string rawValue)
        {
            try
            {
                if (string.IsNullOrEmpty(rawValue))
                {
                    throw new XmpException("Empty convert-string", XmpErrorCode.BadValue);
                }
                if (rawValue.StartsWith("0x"))
                {
                    return Convert.ToInt64(rawValue.Substring (2), 16);
                }
                return Convert.ToInt64(rawValue);
            }
            catch (FormatException)
            {
                throw new XmpException("Invalid long string", XmpErrorCode.BadValue);
            }
        }

        /// <summary>Convert from long to string.</summary>
        /// <param name="value">a long value</param>
        /// <returns>The string representation of the long.</returns>
        public static string ConvertFromLong(long value)
        {
            return value.ToString();
        }

        /// <summary>Converts a string value to a <c>double</c>.</summary>
        /// <param name="rawValue">the string value</param>
        /// <returns>Returns a double.</returns>
        /// <exception cref="XmpException">
        /// If the <c>rawValue</c> is <c>null</c> or empty or the
        /// conversion fails.
        /// </exception>
        /// <exception cref="XmpException"/>
        public static double ConvertToDouble(string rawValue)
        {
            try
            {
                if (string.IsNullOrEmpty(rawValue))
                {
                    throw new XmpException("Empty convert-string", XmpErrorCode.BadValue);
                }
                return double.Parse(rawValue);
            }
            catch (FormatException)
            {
                throw new XmpException("Invalid double string", XmpErrorCode.BadValue);
            }
        }

        /// <summary>Convert from long to string.</summary>
        /// <param name="value">a long value</param>
        /// <returns>The string representation of the long.</returns>
        public static string ConvertFromDouble(double value)
        {
            return value.ToString();
        }

        /// <summary>Converts a string value to an <c>XMPDateTime</c>.</summary>
        /// <param name="rawValue">the string value</param>
        /// <returns>Returns an <c>XMPDateTime</c>-object.</returns>
        /// <exception cref="XmpException">
        /// If the <c>rawValue</c> is <c>null</c> or empty or the
        /// conversion fails.
        /// </exception>
        /// <exception cref="XmpException"/>
        public static IXmpDateTime ConvertToDate(string rawValue)
        {
            if (string.IsNullOrEmpty(rawValue))
            {
                throw new XmpException("Empty convert-string", XmpErrorCode.BadValue);
            }
            return Iso8601Converter.Parse(rawValue);
        }

        /// <summary>Convert from <c>XMPDateTime</c> to string.</summary>
        /// <param name="value">an <c>XMPDateTime</c></param>
        /// <returns>The string representation of the long.</returns>
        public static string ConvertFromDate(IXmpDateTime value)
        {
            return Iso8601Converter.Render(value);
        }

        /// <summary>Convert from a byte array to a base64 encoded string.</summary>
        /// <param name="buffer">the byte array to be converted</param>
        /// <returns>Returns the base64 string.</returns>
        public static string EncodeBase64(byte[] buffer)
        {
            return Convert.ToBase64String(buffer);
        }

        /// <summary>Decode from Base64 encoded string to raw data.</summary>
        /// <param name="base64String">a base64 encoded string</param>
        /// <returns>Returns a byte array containg the decoded string.</returns>
        /// <exception cref="XmpException">Thrown if the given string is not property base64 encoded</exception>
        /// <exception cref="XmpException"/>
        public static byte[] DecodeBase64(string base64String)
        {
            try
            {
                return Convert.FromBase64String(base64String);
            }
            catch (Exception e)
            {
                throw new XmpException("Invalid base64 string", XmpErrorCode.BadValue, e);
            }
        }
    }
}
