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
    public class FileTypeDetector
    {
        private static readonly ByteTrie<FileType?> _root;

        static FileTypeDetector()
        {
            _root = new ByteTrie<FileType?>();
            _root.SetDefaultValue(FileType.Unknown);
            // https://en.wikipedia.org/wiki/List_of_file_signatures
            _root.AddPath(FileType.Jpeg, new sbyte[] { unchecked((sbyte)0xff), unchecked((sbyte)0xd8) });
            _root.AddPath(FileType.Tiff, Sharpen.Runtime.GetBytesForString("II"), new sbyte[] { unchecked((int)(0x2a)), unchecked((int)(0x00)) });
            _root.AddPath(FileType.Tiff, Sharpen.Runtime.GetBytesForString("MM"), new sbyte[] { unchecked((int)(0x00)), unchecked((int)(0x2a)) });
            _root.AddPath(FileType.Psd, Sharpen.Runtime.GetBytesForString("8BPS"));
            _root.AddPath(FileType.Png, new sbyte[] { unchecked((sbyte)0x89), unchecked((int)(0x50)), unchecked((int)(0x4E)), unchecked((int)(0x47)), unchecked((int)(0x0D)), unchecked((int)(0x0A)), unchecked((int)(0x1A)), unchecked((int)(0x0A)), unchecked(
                (int)(0x00)), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((int)(0x0D)), unchecked((int)(0x49)), unchecked((int)(0x48)), unchecked((int)(0x44)), unchecked((int)(0x52)) });
            _root.AddPath(FileType.Bmp, Sharpen.Runtime.GetBytesForString("BM"));
            // TODO technically there are other very rare magic numbers for OS/2 BMP files...
            _root.AddPath(FileType.Gif, Sharpen.Runtime.GetBytesForString("GIF87a"));
            _root.AddPath(FileType.Gif, Sharpen.Runtime.GetBytesForString("GIF89a"));
            _root.AddPath(FileType.Ico, new sbyte[] { unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((int)(0x01)), unchecked((int)(0x00)) });
            _root.AddPath(FileType.Pcx, new sbyte[] { unchecked((int)(0x0A)), unchecked((int)(0x00)), unchecked((int)(0x01)) });
            // multiple PCX versions, explicitly listed
            _root.AddPath(FileType.Pcx, new sbyte[] { unchecked((int)(0x0A)), unchecked((int)(0x02)), unchecked((int)(0x01)) });
            _root.AddPath(FileType.Pcx, new sbyte[] { unchecked((int)(0x0A)), unchecked((int)(0x03)), unchecked((int)(0x01)) });
            _root.AddPath(FileType.Pcx, new sbyte[] { unchecked((int)(0x0A)), unchecked((int)(0x05)), unchecked((int)(0x01)) });
            _root.AddPath(FileType.Riff, Sharpen.Runtime.GetBytesForString("RIFF"));
            _root.AddPath(FileType.Arw, Sharpen.Runtime.GetBytesForString("II"), new sbyte[] { unchecked((int)(0x2a)), unchecked((int)(0x00)), unchecked((int)(0x08)), unchecked((int)(0x00)) });
            _root.AddPath(FileType.Crw, Sharpen.Runtime.GetBytesForString("II"), new sbyte[] { unchecked((int)(0x1a)), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((int)(0x00)) }, Sharpen.Runtime.GetBytesForString("HEAPCCDR"));
            _root.AddPath(FileType.Cr2, Sharpen.Runtime.GetBytesForString("II"), new sbyte[] { unchecked((int)(0x2a)), unchecked((int)(0x00)), unchecked((int)(0x10)), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((int
                )(0x43)), unchecked((int)(0x52)) });
            _root.AddPath(FileType.Nef, Sharpen.Runtime.GetBytesForString("MM"), new sbyte[] { unchecked((int)(0x00)), unchecked((int)(0x2a)), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((sbyte)0x80), unchecked((int
                )(0x00)) });
            _root.AddPath(FileType.Orf, Sharpen.Runtime.GetBytesForString("IIRO"), new sbyte[] { unchecked((sbyte)0x08), unchecked((int)(0x00)) });
            _root.AddPath(FileType.Orf, Sharpen.Runtime.GetBytesForString("IIRS"), new sbyte[] { unchecked((sbyte)0x08), unchecked((int)(0x00)) });
            _root.AddPath(FileType.Raf, Sharpen.Runtime.GetBytesForString("FUJIFILMCCD-RAW"));
            _root.AddPath(FileType.Rw2, Sharpen.Runtime.GetBytesForString("II"), new sbyte[] { unchecked((int)(0x55)), unchecked((int)(0x00)) });
        }

        /// <exception cref="System.Exception"/>
        private FileTypeDetector()
        {
            throw new Exception("Not intended for instantiation");
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
            int maxByteCount = _root.GetMaxDepth();
            inputStream.Mark(maxByteCount);
            sbyte[] bytes = new sbyte[maxByteCount];
            int bytesRead = inputStream.Read(bytes);
            if (bytesRead == -1)
            {
                throw new IOException("Stream ended before file's magic number could be determined.");
            }
            inputStream.Reset();
            //noinspection ConstantConditions
            return _root.Find(bytes);
        }
    }
}
