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

using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Bmp;
using MetadataExtractor.Formats.FileSystem;
using MetadataExtractor.Formats.Gif;
using MetadataExtractor.Formats.Ico;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Netpbm;
using MetadataExtractor.Formats.Pcx;
using MetadataExtractor.Formats.Photoshop;
using MetadataExtractor.Formats.Png;
using MetadataExtractor.Formats.QuickTime;
using MetadataExtractor.Formats.Raf;
using MetadataExtractor.Formats.Tiff;
using MetadataExtractor.Formats.WebP;
using MetadataExtractor.Util;

#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor
{
    /// <summary>Reads metadata from any supported file format.</summary>
    /// <remarks>
    /// This class a lightweight wrapper around other, specific metadata processors.
    /// During extraction, the file type is determined from the first few bytes of the file.
    /// Parsing is then delegated to one of:
    ///
    /// <list type="bullet">
    ///   <item><see cref="JpegMetadataReader"/> for JPEG files</item>
    ///   <item><see cref="TiffMetadataReader"/> for TIFF and (most) RAW files</item>
    ///   <item><see cref="PsdMetadataReader"/> for Photoshop files</item>
    ///   <item><see cref="PngMetadataReader"/> for PNG files</item>
    ///   <item><see cref="BmpMetadataReader"/> for BMP files</item>
    ///   <item><see cref="GifMetadataReader"/> for GIF files</item>
    ///   <item><see cref="IcoMetadataReader"/> for ICO files</item>
    ///   <item><see cref="NetpbmMetadataReader"/> for Netpbm files (PPM, PGM, PBM, PPM)</item>
    ///   <item><see cref="PcxMetadataReader"/> for PCX files</item>
    ///   <item><see cref="WebPMetadataReader"/> for WebP files</item>
    ///   <item><see cref="RafMetadataReader"/> for RAF files</item>
    ///   <item><see cref="QuickTimeMetadataReader"/> for QuickTime files</item>
    /// </list>
    ///
    /// If you know the file type you're working with, you may use one of the above processors directly.
    /// For most scenarios it is simpler, more convenient and more robust to use this class.
    /// <para />
    /// <see cref="FileTypeDetector"/> is used to determine the provided image's file type, and therefore
    /// the appropriate metadata reader to use.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class ImageMetadataReader
    {
        /// <summary>Reads metadata from an <see cref="Stream"/>.</summary>
        /// <param name="stream">A stream from which the file data may be read.  The stream must be positioned at the beginning of the file's data.</param>
        /// <returns>A list of <see cref="Directory"/> instances containing the various types of metadata found within the file's data.</returns>
        /// <exception cref="ImageProcessingException">The file type is unknown, or processing errors occurred.</exception>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static DirectoryList ReadMetadata([NotNull] Stream stream)
        {
            var fileType = FileTypeDetector.DetectFileType(stream);

            switch (fileType)
            {
                case FileType.Jpeg:
                    return JpegMetadataReader.ReadMetadata(stream);
                case FileType.Tiff:
                case FileType.Arw:
                case FileType.Cr2:
                case FileType.Nef:
                case FileType.Orf:
                case FileType.Rw2:
                    return TiffMetadataReader.ReadMetadata(stream);
                case FileType.Psd:
                    return PsdMetadataReader.ReadMetadata(stream);
                case FileType.Png:
                    return PngMetadataReader.ReadMetadata(stream);
                case FileType.Bmp:
                    return new Directory[] { BmpMetadataReader.ReadMetadata(stream) };
                case FileType.Gif:
                    return GifMetadataReader.ReadMetadata(stream);
                case FileType.Ico:
                    return IcoMetadataReader.ReadMetadata(stream);
                case FileType.Pcx:
                    return new Directory[] { PcxMetadataReader.ReadMetadata(stream) };
                case FileType.Riff:
                    return WebPMetadataReader.ReadMetadata(stream);
                case FileType.Raf:
                    return RafMetadataReader.ReadMetadata(stream);
                case FileType.QuickTime:
                    return QuickTimeMetadataReader.ReadMetadata(stream);
                case FileType.Netpbm:
                    return new Directory[] { NetpbmMetadataReader.ReadMetadata(stream) };
            }

            throw new ImageProcessingException("File format is not supported");
        }

        /// <summary>Reads metadata from a file.</summary>
        /// <remarks>Unlike <see cref="ReadMetadata(System.IO.Stream)"/>, this overload includes a <see cref="FileMetadataDirectory"/> in the output.</remarks>
        /// <param name="filePath">Location of a file from which data should be read.</param>
        /// <returns>A list of <see cref="Directory"/> instances containing the various types of metadata found within the file's data.</returns>
        /// <exception cref="ImageProcessingException">The file type is unknown, or processing errors occurred.</exception>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static DirectoryList ReadMetadata([NotNull] string filePath)
        {
            var directories = new List<Directory>();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                directories.AddRange(ReadMetadata(stream));

            directories.Add(new FileMetadataReader().Read(filePath));

            return directories;
        }
    }
}
