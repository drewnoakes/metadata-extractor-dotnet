// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Text;

namespace MetadataExtractor
{
    /// <remarks>
    /// Adapted from http://developer.classpath.org/doc/java/lang/StringBuilder-source.html
    /// </remarks>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Finds the first instance of a substring in this StringBuilder.
        /// </summary>
        /// <param name="str">String to find</param>
        /// <returns>location (base 0) of the String, or -1 if not found</returns>
        public static int IndexOf(this StringBuilder sb, string str)
        {
          return IndexOf(sb, str, 0);
        }

        /// <summary>
        /// Finds the first instance of a String in this StringBuilder, starting at
        /// a given index.If starting index is less than 0, the search starts at
        /// the beginning of this String.If the starting index is greater than the
        /// length of this String, or the substring is not found, -1 is returned.
        /// </summary>
        /// <param name="str">String to find</param>
        /// <param name="fromIndex">index to start the search</param>
        /// <returns>location (base 0) of the String, or -1 if not found</returns>
        public static int IndexOf(this StringBuilder sb, string str, int fromIndex)
        {
            if (fromIndex < 0)
                fromIndex = 0;
            int limit = sb.Length - str.Length;
            for ( ; fromIndex <= limit; fromIndex++)
                if (RegionMatches(sb, fromIndex, str))
                    return fromIndex;

            return -1;
        }

        /// <summary>
        /// Predicate which determines if a substring of this matches another String
        /// starting at a specified offset for each String and continuing for a
        /// specified length.This is more efficient than creating a String to call
        /// indexOf on.
        /// </summary>
        /// <param name="toffset">toffset index to start comparison at for this String</param>
        /// <param name="other">other non-null String to compare to region of this</param>
        /// <returns>true if regions match, false otherwise</returns>
        private static bool RegionMatches(StringBuilder sb, int toffset, string other)
        {
            int len = other.Length;
            int index = 0; // other.offset;
            while (--len >= 0)
                if (sb[toffset++] != other[index++])
                    return false;
            return true;
        }

    }
}
