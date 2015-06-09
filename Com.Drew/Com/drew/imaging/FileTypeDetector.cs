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

using System.IO;
using System.Text;
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
            Root.AddPath(FileType.Jpeg, new byte[] { unchecked((byte)0xff), unchecked((byte)0xd8) });
            Root.AddPath(FileType.Tiff, Encoding.UTF8.GetBytes("II"), new byte[] { unchecked(0x2a), unchecked(0x00) });
            Root.AddPath(FileType.Tiff, Encoding.UTF8.GetBytes("MM"), new byte[] { unchecked(0x00), unchecked(0x2a) });
            Root.AddPath(FileType.Psd, Encoding.UTF8.GetBytes("8BPS"));
            Root.AddPath(FileType.Png, new byte[] { unchecked((byte)0x89), unchecked(0x50), unchecked(0x4E), unchecked(0x47), unchecked(0x0D), unchecked(0x0A), unchecked(0x1A), unchecked(0x0A), unchecked(
                0x00), unchecked(0x00), unchecked(0x00), unchecked(0x0D), unchecked(0x49), unchecked(0x48), unchecked(0x44), unchecked(0x52) });
            Root.AddPath(FileType.Bmp, Encoding.UTF8.GetBytes("BM"));
            // TODO technically there are other very rare magic numbers for OS/2 BMP files...
            Root.AddPath(FileType.Gif, Encoding.UTF8.GetBytes("GIF87a"));
            Root.AddPath(FileType.Gif, Encoding.UTF8.GetBytes("GIF89a"));
            Root.AddPath(FileType.Ico, new byte[] { unchecked(0x00), unchecked(0x00), unchecked(0x01), unchecked(0x00) });
            Root.AddPath(FileType.Pcx, new byte[] { unchecked(0x0A), unchecked(0x00), unchecked(0x01) });
            // multiple PCX versions, explicitly listed
            Root.AddPath(FileType.Pcx, new byte[] { unchecked(0x0A), unchecked(0x02), unchecked(0x01) });
            Root.AddPath(FileType.Pcx, new byte[] { unchecked(0x0A), unchecked(0x03), unchecked(0x01) });
            Root.AddPath(FileType.Pcx, new byte[] { unchecked(0x0A), unchecked(0x05), unchecked(0x01) });
            Root.AddPath(FileType.Riff, Encoding.UTF8.GetBytes("RIFF"));
            Root.AddPath(FileType.Arw, Encoding.UTF8.GetBytes("II"), new byte[] { unchecked(0x2a), unchecked(0x00), unchecked(0x08), unchecked(0x00) });
            Root.AddPath(FileType.Crw, Encoding.UTF8.GetBytes("II"), new byte[] { unchecked(0x1a), unchecked(0x00), unchecked(0x00), unchecked(0x00) }, Encoding.UTF8.GetBytes("HEAPCCDR"));
            Root.AddPath(FileType.Cr2, Encoding.UTF8.GetBytes("II"), new byte[] { unchecked(0x2a), unchecked(0x00), unchecked(0x10), unchecked(0x00), unchecked(0x00), unchecked(0x00), unchecked(0x43), unchecked(0x52) });
            Root.AddPath(FileType.Nef, Encoding.UTF8.GetBytes("MM"), new byte[] { unchecked(0x00), unchecked(0x2a), unchecked(0x00), unchecked(0x00), unchecked(0x00), unchecked((byte)0x80), unchecked(0x00) });
            Root.AddPath(FileType.Orf, Encoding.UTF8.GetBytes("IIRO"), new byte[] { unchecked(0x08), unchecked(0x00) });
            Root.AddPath(FileType.Orf, Encoding.UTF8.GetBytes("IIRS"), new byte[] { unchecked(0x08), unchecked(0x00) });
            Root.AddPath(FileType.Raf, Encoding.UTF8.GetBytes("FUJIFILMCCD-RAW"));
            Root.AddPath(FileType.Rw2, Encoding.UTF8.GetBytes("II"), new byte[] { unchecked(0x55), unchecked(0x00) });
        }

        /// <summary>Examines the a file's first bytes and estimates the file's type.</summary>
        /// <remarks>
        /// Examines the a file's first bytes and estimates the file's type.
        /// <para>
        /// Stream must be seekable and contain enough bytes for the most complexat least eight bytes.
        /// </remarks>
        /// <exception cref="System.IO.IOException">if an IO error occurred or the input stream ended unexpectedly.</exception>
        [NotNull]
        public static FileType? DetectFileType([NotNull] Stream stream)
        {
            int maxByteCount = Root.GetMaxDepth();
            byte[] bytes = new byte[maxByteCount];
            int bytesRead = stream.Read(bytes, 0, bytes.Length);
            if (bytesRead == 0)
                return null;
            stream.Seek(-bytesRead, SeekOrigin.Current);
            //noinspection ConstantConditions
            return Root.Find(bytes);
        }
    }
}
