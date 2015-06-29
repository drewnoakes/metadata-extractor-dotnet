#region License
//
// Copyright 2002-2015 Drew Noakes
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

using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace MetadataExtractor.Util
{
    /// <summary>Examines the a file's first bytes and estimates the file's type.</summary>
    public static class FileTypeDetector
    {
        private static readonly ByteTrie<FileType?> _root;

        static FileTypeDetector()
        {
            _root = new ByteTrie<FileType?>();
            _root.SetDefaultValue(FileType.Unknown);
            // https://en.wikipedia.org/wiki/List_of_file_signatures
            _root.AddPath(FileType.Jpeg, new[] { (byte)0xff, (byte)0xd8 });
            _root.AddPath(FileType.Tiff, Encoding.UTF8.GetBytes("II"), new byte[] { 0x2a, 0x00 });
            _root.AddPath(FileType.Tiff, Encoding.UTF8.GetBytes("MM"), new byte[] { 0x00, 0x2a });
            _root.AddPath(FileType.Psd, Encoding.UTF8.GetBytes("8BPS"));
            _root.AddPath(FileType.Png, new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52 });
            _root.AddPath(FileType.Bmp, Encoding.UTF8.GetBytes("BM"));
            // TODO technically there are other very rare magic numbers for OS/2 BMP files...
            _root.AddPath(FileType.Gif, Encoding.UTF8.GetBytes("GIF87a"));
            _root.AddPath(FileType.Gif, Encoding.UTF8.GetBytes("GIF89a"));
            _root.AddPath(FileType.Ico, new byte[] { 0x00, 0x00, 0x01, 0x00 });
            _root.AddPath(FileType.Pcx, new byte[] { 0x0A, 0x00, 0x01 });
            // multiple PCX versions, explicitly listed
            _root.AddPath(FileType.Pcx, new byte[] { 0x0A, 0x02, 0x01 });
            _root.AddPath(FileType.Pcx, new byte[] { 0x0A, 0x03, 0x01 });
            _root.AddPath(FileType.Pcx, new byte[] { 0x0A, 0x05, 0x01 });
            _root.AddPath(FileType.Riff, Encoding.UTF8.GetBytes("RIFF"));
            _root.AddPath(FileType.Arw, Encoding.UTF8.GetBytes("II"), new byte[] { 0x2a, 0x00, 0x08, 0x00 });
            _root.AddPath(FileType.Crw, Encoding.UTF8.GetBytes("II"), new byte[] { 0x1a, 0x00, 0x00, 0x00 }, Encoding.UTF8.GetBytes("HEAPCCDR"));
            _root.AddPath(FileType.Cr2, Encoding.UTF8.GetBytes("II"), new byte[] { 0x2a, 0x00, 0x10, 0x00, 0x00, 0x00, 0x43, 0x52 });
            _root.AddPath(FileType.Nef, Encoding.UTF8.GetBytes("MM"), new byte[] { 0x00, 0x2a, 0x00, 0x00, 0x00, 0x80, 0x00 });
            _root.AddPath(FileType.Orf, Encoding.UTF8.GetBytes("IIRO"), new byte[] { 0x08, 0x00 });
            _root.AddPath(FileType.Orf, Encoding.UTF8.GetBytes("IIRS"), new byte[] { 0x08, 0x00 });
            _root.AddPath(FileType.Raf, Encoding.UTF8.GetBytes("FUJIFILMCCD-RAW"));
            _root.AddPath(FileType.Rw2, Encoding.UTF8.GetBytes("II"), new byte[] { 0x55, 0x00 });
        }

        /// <summary>Examines the a file's first bytes and estimates the file's type.</summary>
        /// <remarks>
        /// Stream must be seekable and contain enough bytes for the most complexat least eight bytes.
        /// </remarks>
        /// <exception cref="IOException">if an IO error occurred or the input stream ended unexpectedly.</exception>
        public static FileType? DetectFileType([NotNull] Stream stream)
        {
            var maxByteCount = _root.MaxDepth;
            var bytes = new byte[maxByteCount];
            var bytesRead = stream.Read(bytes, 0, bytes.Length);
            if (bytesRead == 0)
                return null;
            stream.Seek(-bytesRead, SeekOrigin.Current);
            //noinspection ConstantConditions
            return _root.Find(bytes);
        }
    }
}
