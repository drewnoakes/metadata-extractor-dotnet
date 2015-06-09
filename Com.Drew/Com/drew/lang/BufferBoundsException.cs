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
using System.IO;
using Sharpen;

namespace Com.Drew.Lang
{
    /// <summary>
    /// A checked replacement for <see cref="System.IndexOutOfRangeException"/>.  Used by
    /// <see cref="IndexedReader"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [Serializable]
    public sealed class BufferBoundsException : IOException
    {
        public BufferBoundsException(int index, int bytesRequested, long bufferLength)
            : base(GetMessage(index, bytesRequested, bufferLength))
        {
        }

        public BufferBoundsException(string message)
            : base(message)
        {
        }

        private static string GetMessage(int index, int bytesRequested, long bufferLength)
        {
            if (index < 0)
            {
                return string.Format("Attempt to read from buffer using a negative index ({0})", index);
            }
            if (bytesRequested < 0)
            {
                return string.Format("Number of requested bytes cannot be negative ({0})", bytesRequested);
            }
            if (index + (long)bytesRequested - 1L > int.MaxValue)
            {
                return string.Format("Number of requested bytes summed with starting index exceed maximum range of signed 32 bit integers (requested index: {0}, requested count: {1})", index, bytesRequested);
            }
            return string.Format("Attempt to read from beyond end of underlying data source (requested index: {0}, requested count: {1}, max index: {2})", index, bytesRequested, bufferLength - 1);
        }
    }
}
