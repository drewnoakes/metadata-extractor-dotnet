#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;

// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo

namespace MetadataExtractor.Util
{
    /// <summary>Examines the a file's first bytes and estimates the file's type.</summary>
    public static class FileTypeDetector
    {
        // https://en.wikipedia.org/wiki/List_of_file_signatures
        private static readonly ByteTrie<FileType> _root = new ByteTrie<FileType>(defaultValue: FileType.Unknown)
        {
            { FileType.Jpeg, new[] { (byte)0xff, (byte)0xd8 } },
            { FileType.Tiff, Encoding.UTF8.GetBytes("II"), new byte[] { 0x2a, 0x00 } },
            { FileType.Tiff, Encoding.UTF8.GetBytes("MM"), new byte[] { 0x00, 0x2a } },
            { FileType.Psd, Encoding.UTF8.GetBytes("8BPS") },
            { FileType.Png, new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52 } },
            { FileType.Bmp, Encoding.UTF8.GetBytes("BM") },
            // TODO technically there are other very rare magic numbers for OS/2 BMP files
            { FileType.Gif, Encoding.UTF8.GetBytes("GIF87a") },
            { FileType.Gif, Encoding.UTF8.GetBytes("GIF89a") },
            { FileType.Ico, new byte[] { 0x00, 0x00, 0x01, 0x00 } },
            { FileType.Netpbm, Encoding.UTF8.GetBytes("P1") }, // ASCII B
            { FileType.Netpbm, Encoding.UTF8.GetBytes("P2") }, // ASCII greysca
            { FileType.Netpbm, Encoding.UTF8.GetBytes("P3") }, // ASCII R
            { FileType.Netpbm, Encoding.UTF8.GetBytes("P4") }, // RAW B
            { FileType.Netpbm, Encoding.UTF8.GetBytes("P5") }, // RAW greysca
            { FileType.Netpbm, Encoding.UTF8.GetBytes("P6") }, // RAW R
            { FileType.Netpbm, Encoding.UTF8.GetBytes("P7") }, // P
            { FileType.Pcx, new byte[] { 0x0A, 0x00, 0x01 } },
            // multiple PCX versions, explicitly list
            { FileType.Pcx, new byte[] { 0x0A, 0x02, 0x01 } },
            { FileType.Pcx, new byte[] { 0x0A, 0x03, 0x01 } },
            { FileType.Pcx, new byte[] { 0x0A, 0x05, 0x01 } },
            { FileType.Riff, Encoding.UTF8.GetBytes("RIFF") },
            { FileType.Arw, Encoding.UTF8.GetBytes("II"), new byte[] { 0x2a, 0x00, 0x08, 0x00 } },
            { FileType.Crw, Encoding.UTF8.GetBytes("II"), new byte[] { 0x1a, 0x00, 0x00, 0x00 }, Encoding.UTF8.GetBytes("HEAPCCDR") },
            { FileType.Cr2, Encoding.UTF8.GetBytes("II"), new byte[] { 0x2a, 0x00, 0x10, 0x00, 0x00, 0x00, 0x43, 0x52 } },
            { FileType.Nef, Encoding.UTF8.GetBytes("MM"), new byte[] { 0x00, 0x2a, 0x00, 0x00, 0x00, 0x80, 0x00 } },
            { FileType.Orf, Encoding.UTF8.GetBytes("IIRO"), new byte[] { 0x08, 0x00 } },
            { FileType.Orf, Encoding.UTF8.GetBytes("MMOR"), new byte[] { 0x00, 0x00 } },
            { FileType.Orf, Encoding.UTF8.GetBytes("IIRS"), new byte[] { 0x08, 0x00 } },
            { FileType.Raf, Encoding.UTF8.GetBytes("FUJIFILMCCD-RAW") },
            { FileType.Rw2, Encoding.UTF8.GetBytes("II"), new byte[] { 0x55, 0x00 } },
        };

        private static readonly IEnumerable<Func<byte[], FileType>> _fixedCheckers = new Func<byte[], FileType>[]
        {
            bytes => bytes.RegionEquals(4, 4, Encoding.UTF8.GetBytes("ftyp"))
                ? FileType.QuickTime
                : FileType.Unknown
        };

        /// <summary>Examines the a file's first bytes and estimates the file's type.</summary>
        /// <exception cref="ArgumentException">Stream does not support seeking.</exception>
        /// <exception cref="IOException">An IO error occurred, or the input stream ended unexpectedly.</exception>
        public static FileType DetectFileType([NotNull] Stream stream)
        {
            if (!stream.CanSeek)
                throw new ArgumentException("Must support seek", nameof(stream));

            var maxByteCount = _root.MaxDepth;

            var bytes = new byte[maxByteCount];
            var bytesRead = stream.Read(bytes, 0, bytes.Length);

            if (bytesRead == 0)
                return FileType.Unknown;

            stream.Seek(-bytesRead, SeekOrigin.Current);

            var fileType = _root.Find(bytes);

            if (fileType == FileType.Unknown)
            {
                foreach (var fixedChecker in _fixedCheckers)
                {
                    fileType = fixedChecker(bytes);
                    if (fileType != FileType.Unknown)
                        return fileType;
                }
            }

            return fileType;
        }
    }

    internal static class ByteArrayExtensions
    {
        public static bool RegionEquals([NotNull] this byte[] bytes, int offset, int count, [NotNull] byte[] comparand)
        {
            if (offset < 0 ||                   // invalid arg
                count < 0 ||                    // invalid arg
                bytes.Length < offset + count)  // extends beyond end
                return false;

            for (int i = 0, j = offset; i < count; i++, j++)
            {
                if (bytes[j] != comparand[i])
                    return false;
            }

            return true;
        }
    }
}
