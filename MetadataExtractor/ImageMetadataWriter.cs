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
#if !PORTABLE
using MetadataExtractor.Formats.FileSystem;
#endif
using MetadataExtractor.Formats.Gif;
using MetadataExtractor.Formats.Ico;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Pcx;
using MetadataExtractor.Formats.Photoshop;
using MetadataExtractor.Formats.Png;
using MetadataExtractor.Formats.QuickTime;
using MetadataExtractor.Formats.Raf;
using MetadataExtractor.Formats.Tiff;
using MetadataExtractor.Formats.WebP;
using MetadataExtractor.Util;
using System.Xml.Linq;

namespace MetadataExtractor
{
    /// <summary>Writes metadata to any supported file format.</summary>
    /// <remarks>
    /// This class a lightweight wrapper around other, specific metadata processors.
    /// During saving, the file type is determined from the first few bytes of the existing file.
    /// Writing is then delegated to one of:
    ///
    /// <list type="bullet">
    ///   <item><see cref="JpegMetadataWriter"/> for JPEG files</item>
    ///   <item>Writing to other file types is currently not implemented. Feel free to contribute!</item>
    /// </list>
    ///
    /// If you know the file type you're working with, you may use one of the above processors directly.
    /// For most scenarios it is simpler, more convenient and more robust to use this class.
    /// <para />
    /// <see cref="FileTypeDetector"/> is used to determine the provided image's file type, and therefore
    /// the appropriate metadata reader to use.
    /// </remarks>
    /// <author>Michael Osthege</author>
    /// <exception cref="ImageProcessingException">on unsupported file types</exception>
    public static class ImageMetadataWriter
    {
        /// <summary>Writes metadata to a <see cref="Stream"/>.</summary>
        /// <param name="stream">A stream to which the file data may be written.  The stream must be positioned at the beginning of the file's data.</param>
        /// <param name="metadata">Collection of metadata objects.</param>
        /// <exception cref="ImageProcessingException">The file type is unknown, or processing errors occurred.</exception>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static MemoryStream WriteMetadata([NotNull] Stream stream, IEnumerable<object> metadata)
        {
            var fileType = FileTypeDetector.DetectFileType(stream);
            switch (fileType)
            {
                case FileType.Jpeg: return JpegMetadataWriter.WriteMetadata(stream, metadata);
                //case FileType.Tiff:
                //case FileType.Arw:
                //case FileType.Cr2:
                //case FileType.Nef:
                //case FileType.Orf:
                //case FileType.Rw2:
                //    return TiffMetadataReader.ReadMetadata(stream);
                //case FileType.Psd:
                //    return PsdMetadataReader.ReadMetadata(stream);
                //case FileType.Png:
                //    return PngMetadataReader.ReadMetadata(stream);
                //case FileType.Bmp:
                //    return new[] { BmpMetadataReader.ReadMetadata(stream) };
                //case FileType.Gif:
                //    return new[] { GifMetadataReader.ReadMetadata(stream) };
                //case FileType.Ico:
                //    return IcoMetadataReader.ReadMetadata(stream);
                //case FileType.Pcx:
                //    return new[] { PcxMetadataReader.ReadMetadata(stream) };
                //case FileType.Riff:
                //    return WebPMetadataReader.ReadMetadata(stream);
                //case FileType.Raf:
                //    return RafMetadataReader.ReadMetadata(stream);
                //case FileType.QuickTime:
                //    return QuicktimeMetadataReader.ReadMetadata(stream);
            }

            throw new ImageProcessingException("File format is not supported");
        }
    }
}
