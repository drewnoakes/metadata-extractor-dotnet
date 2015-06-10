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
using JetBrains.Annotations;
using MetadataExtractor.Formats.Bmp;
using MetadataExtractor.Formats.FileSystem;
using MetadataExtractor.Formats.Gif;
using MetadataExtractor.Formats.Ico;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Pcx;
using MetadataExtractor.Formats.Photoshop;
using MetadataExtractor.Formats.Png;
using MetadataExtractor.Formats.Tiff;
using MetadataExtractor.Formats.WebP;
using MetadataExtractor.Util;

namespace MetadataExtractor
{
    /// <summary>
    /// Obtains <see cref="Metadata"/> from all supported file formats.
    /// </summary>
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
    ///   <item><see cref="PcxMetadataReader"/> for PCX files</item>
    ///   <item><see cref="WebPMetadataReader"/> for WebP files</item>
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
        /// <summary>
        /// Reads metadata from an <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">a stream from which the file data may be read.  The stream must be positioned at the beginning of the file's data.</param>
        /// <returns>a populated <see cref="Metadata"/> object containing directories of tags with values and any processing errors.</returns>
        /// <exception cref="ImageProcessingException">if the file type is unknown, or for general processing errors.</exception>
        /// <exception cref="ImageProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static Metadata ReadMetadata([NotNull] Stream stream)
        {
            var fileType = FileTypeDetector.DetectFileType(stream) ?? FileType.Unknown;
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
                    return BmpMetadataReader.ReadMetadata(stream);
                case FileType.Gif:
                    return GifMetadataReader.ReadMetadata(stream);
                case FileType.Ico:
                    return IcoMetadataReader.ReadMetadata(stream);
                case FileType.Pcx:
                    return PcxMetadataReader.ReadMetadata(stream);
                case FileType.Riff:
                    return WebPMetadataReader.ReadMetadata(stream);
            }
            throw new ImageProcessingException("File format is not supported");
        }

        /// <summary>
        /// Reads <see cref="Metadata"/> from a file.
        /// </summary>
        /// <param name="filePath">a file from which the image data may be read.</param>
        /// <returns>a populated <see cref="Metadata"/> object containing directories of tags with values and any processing errors.</returns>
        /// <exception cref="ImageProcessingException">for general processing errors.</exception>
        /// <exception cref="ImageProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static Metadata ReadMetadata([NotNull] string filePath)
        {
            Metadata metadata;
            using (Stream inputStream = new FileStream(filePath, FileMode.Open))
                metadata = ReadMetadata(inputStream);
            new FileMetadataReader().Read(filePath, metadata);
            return metadata;
        }
    }
}
