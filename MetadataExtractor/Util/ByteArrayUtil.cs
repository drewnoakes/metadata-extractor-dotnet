#region License
//
// Copyright 2002-2019 Drew Noakes
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

using JetBrains.Annotations;

namespace MetadataExtractor.Util
{
    public static class ByteArrayUtil
    {
        [Pure]
        public static bool StartsWith(this byte[] source, byte[] pattern)
        {
            if (source.Length < pattern.Length)
                return false;

            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < pattern.Length; i++)
                if (source[i] != pattern[i])
                    return false;

            return true;
        }

        [Pure]
        public static bool EqualTo(this byte[] source, byte[] compare)
        {
            // If not the same length, bail out
            if (source.Length != compare.Length)
                return false;

            // If they are the same object, bail out
            if (ReferenceEquals(source, compare))
                return true;

            // Loop all values and compare
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] != compare[i])
                    return false;
            }

            // If we got here, equal
            return true;
        }
    }
}