// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor
{
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    internal static class StringBuilderExtensions
    {
        /// <summary>
        /// Returns the first index of character <paramref name="c"/> in <paramref name="sb"/>,
        /// or <c>-1</c> if it is not found.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to search within.</param>
        /// <param name="c">The character to find.</param>
        public static int IndexOf(this StringBuilder sb, char c)
        {
            for (var i = 0; i < sb.Length; i++)
            {
                if (sb[i] == c)
                    return i;
            }

            return -1;
        }
    }
}
