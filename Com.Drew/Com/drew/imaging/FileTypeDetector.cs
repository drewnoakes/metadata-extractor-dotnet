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
using Com.Drew.Lang;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Imaging
{
    /// <summary>Examines the a file's first bytes and estimates the file's type.</summary>
    public static class FileTypeDetector
    {
        private static readonly ByteTrie<FileType?> Root;

        static FileTypeDetector()
        {
            Root = new ByteTrie<FileType?>();
            Root.SetDefaultValue(FileType.Unknown);
            // https://en.wikipedia.org/wiki/List_of_file_signatures
            Root.AddPath(FileType.Jpeg, new sbyte[] { unchecked((sbyte)0xff), unchecked((sbyte)0xd8) });
            Root.AddPath(FileType.Tiff, Runtime.GetBytesForString("II"), new sbyte[] { unchecked((int)(0x2a)), unchecked((int)(0x00)) });
            Root.AddPath(FileType.Tiff, Runtime.GetBytesForString("MM"), new sbyte[] { unchecked((int)(0x00)), unchecked((int)(0x2a)) });
            Root.AddPath(FileType.Psd, Runtime.GetBytesForString("8BPS"));
            Root.AddPath(FileType.Png, new sbyte[] { unchecked((sbyte)0x89), unchecked((int)(0x50)), unchecked((int)(0x4E)), unchecked((int)(0x47)), unchecked((int)(0x0D)), unchecked((int)(0x0A)), unchecked((int)(0x1A)), unchecked((int)(0x0A)), unchecked(
                (int)(0x00)), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((int)(0x0D)), unchecked((int)(0x49)), unchecked((int)(0x48)), unchecked((int)(0x44)), unchecked((int)(0x52)) });
            Root.AddPath(FileType.Bmp, Runtime.GetBytesForString("BM"));
            // TODO technically there are other very rare magic numbers for OS/2 BMP files...
            Root.AddPath(FileType.Gif, Runtime.GetBytesForString("GIF87a"));
            Root.AddPath(FileType.Gif, Runtime.GetBytesForString("GIF89a"));
            Root.AddPath(FileType.Ico, new sbyte[] { unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((int)(0x01)), unchecked((int)(0x00)) });
            Root.AddPath(FileType.Pcx, new sbyte[] { unchecked((int)(0x0A)), unchecked((int)(0x00)), unchecked((int)(0x01)) });
            // multiple PCX versions, explicitly listed
            Root.AddPath(FileType.Pcx, new sbyte[] { unchecked((int)(0x0A)), unchecked((int)(0x02)), unchecked((int)(0x01)) });
            Root.AddPath(FileType.Pcx, new sbyte[] { unchecked((int)(0x0A)), unchecked((int)(0x03)), unchecked((int)(0x01)) });
            Root.AddPath(FileType.Pcx, new sbyte[] { unchecked((int)(0x0A)), unchecked((int)(0x05)), unchecked((int)(0x01)) });
            Root.AddPath(FileType.Riff, Runtime.GetBytesForString("RIFF"));
            Root.AddPath(FileType.Arw, Runtime.GetBytesForString("II"), new sbyte[] { unchecked((int)(0x2a)), unchecked((int)(0x00)), unchecked((int)(0x08)), unchecked((int)(0x00)) });
            Root.AddPath(FileType.Crw, Runtime.GetBytesForString("II"), new sbyte[] { unchecked((int)(0x1a)), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((int)(0x00)) }, Runtime.GetBytesForString("HEAPCCDR"));
            Root.AddPath(FileType.Cr2, Runtime.GetBytesForString("II"), new sbyte[] { unchecked((int)(0x2a)), unchecked((int)(0x00)), unchecked((int)(0x10)), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((int
                )(0x43)), unchecked((int)(0x52)) });
            Root.AddPath(FileType.Nef, Runtime.GetBytesForString("MM"), new sbyte[] { unchecked((int)(0x00)), unchecked((int)(0x2a)), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((sbyte)0x80), unchecked((int
                )(0x00)) });
            Root.AddPath(FileType.Orf, Runtime.GetBytesForString("IIRO"), new sbyte[] { unchecked((sbyte)0x08), unchecked((int)(0x00)) });
            Root.AddPath(FileType.Orf, Runtime.GetBytesForString("IIRS"), new sbyte[] { unchecked((sbyte)0x08), unchecked((int)(0x00)) });
            Root.AddPath(FileType.Raf, Runtime.GetBytesForString("FUJIFILMCCD-RAW"));
            Root.AddPath(FileType.Rw2, Runtime.GetBytesForString("II"), new sbyte[] { unchecked((int)(0x55)), unchecked((int)(0x00)) });
        }

        /// <summary>Examines the a file's first bytes and estimates the file's type.</summary>
        /// <remarks>
        /// Examines the a file's first bytes and estimates the file's type.
        /// <p>
        /// Requires a
        /// <see cref="System.IO.BufferedInputStream"/>
        /// in order to mark and reset the stream to the position
        /// at which it was provided to this method once completed.
        /// <p>
        /// Requires the stream to contain at least eight bytes.
        /// </remarks>
        /// <exception cref="System.IO.IOException">if an IO error occurred or the input stream ended unexpectedly.</exception>
        [NotNull]
        public static FileType? DetectFileType([NotNull] BufferedInputStream inputStream)
        {
            int maxByteCount = Root.GetMaxDepth();
            inputStream.Mark(maxByteCount);
            sbyte[] bytes = new sbyte[maxByteCount];
            int bytesRead = inputStream.Read(bytes);
            if (bytesRead == -1)
            {
                throw new IOException("Stream ended before file's magic number could be determined.");
            }
            inputStream.Reset();
            //noinspection ConstantConditions
            return Root.Find(bytes);
        }
    }
}
