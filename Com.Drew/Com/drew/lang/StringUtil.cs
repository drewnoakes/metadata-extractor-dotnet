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
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Lang
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class StringUtil
    {
        [NotNull]
        public static string Join<TT0>(IEnumerable<TT0> strings, [NotNull] string delimiter)
            where TT0 : CharSequence
        {
            return string.Join(delimiter, strings.Select(s => s.ToString()));
        }

        [NotNull]
        public static string Join<T>([NotNull] T[] strings, [NotNull] string delimiter)
            where T : CharSequence
        {
            return string.Join(delimiter, strings.Select(s => s.ToString()));
        }

        public static int Compare([CanBeNull] string s1, [CanBeNull] string s2)
        {
            bool null1 = s1 == null;
            bool null2 = s2 == null;
            if (null1 && null2)
            {
                return 0;
            }
            if (null1)
            {
                return -1;
            }
            if (null2)
            {
                return 1;
            }
            return string.CompareOrdinal(s1, s2);
        }

        [NotNull]
        public static string UrlEncode([NotNull] string name)
        {
            // Sufficient for now, it seems
            return name.Replace(" ", "%20");
        }
    }
}
