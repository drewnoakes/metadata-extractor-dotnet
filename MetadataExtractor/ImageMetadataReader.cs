// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Avi;
using MetadataExtractor.Formats.Bmp;
using MetadataExtractor.Formats.Eps;
using MetadataExtractor.Formats.FileSystem;
using MetadataExtractor.Formats.FileType;
using MetadataExtractor.Formats.Gif;
using MetadataExtractor.Formats.Heif;
using MetadataExtractor.Formats.Ico;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Mpeg;
using MetadataExtractor.Formats.Netpbm;
using MetadataExtractor.Formats.Pcx;
using MetadataExtractor.Formats.Photoshop;
using MetadataExtractor.Formats.Png;
using MetadataExtractor.Formats.QuickTime;
using MetadataExtractor.Formats.Raf;
using MetadataExtractor.Formats.Tga;
using MetadataExtractor.Formats.Tiff;
using MetadataExtractor.Formats.Wav;
using MetadataExtractor.Formats.WebP;

// ReSharper disable RedundantCaseLabel

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
        /// <param name="fileName">The file name, if available. May contain path information if that's more convenient. The method will only inspect the extension of the path (i.e. <c>EndsWith</c>).</param>
        /// <returns>A list of <see cref="Directory"/> instances containing the various types of metadata found within the file's data.</returns>
        /// <exception cref="ImageProcessingException">The file type is unknown, or processing errors occurred.</exception>
        /// <exception cref="IOException"/>
        public static IReadOnlyList<Directory> ReadMetadata(Stream stream, string? fileName = null)
        {
            var fileType = FileTypeDetector.DetectFileType(stream, fileName);

            var directories = new List<Directory>();

#pragma warning disable format

            directories.AddRange(fileType switch
            {
                FileType.Arw       => TiffMetadataReader.ReadMetadata(stream),
                FileType.Avi       => AviMetadataReader.ReadMetadata(stream),
                FileType.Avif      => HeifMetadataReader.ReadMetadata(stream),
                FileType.Bmp       => BmpMetadataReader.ReadMetadata(stream),
                FileType.Crx       => QuickTimeMetadataReader.ReadMetadata(stream),
                FileType.Cr2       => TiffMetadataReader.ReadMetadata(stream),
                FileType.Dng       => TiffMetadataReader.ReadMetadata(stream),
                FileType.Eps       => EpsMetadataReader.ReadMetadata(stream),
                FileType.Gif       => GifMetadataReader.ReadMetadata(stream),
                FileType.GoPro     => TiffMetadataReader.ReadMetadata(stream),
                FileType.Heif      => HeifMetadataReader.ReadMetadata(stream),
                FileType.Ico       => IcoMetadataReader.ReadMetadata(stream),
                FileType.Jpeg      => JpegMetadataReader.ReadMetadata(stream),
                FileType.Kdc       => TiffMetadataReader.ReadMetadata(stream),
                FileType.Mp3       => Mp3MetadataReader.ReadMetadata(stream),
                FileType.Mp4       => QuickTimeMetadataReader.ReadMetadata(stream),
                FileType.Nef       => TiffMetadataReader.ReadMetadata(stream),
                FileType.Netpbm    => [NetpbmMetadataReader.ReadMetadata(stream)],
                FileType.Orf       => TiffMetadataReader.ReadMetadata(stream),
                FileType.Pef       => TiffMetadataReader.ReadMetadata(stream),
                FileType.Pcx       => [PcxMetadataReader.ReadMetadata(stream)],
                FileType.Png       => PngMetadataReader.ReadMetadata(stream),
                FileType.Psd       => PsdMetadataReader.ReadMetadata(stream),
                FileType.QuickTime => QuickTimeMetadataReader.ReadMetadata(stream),
                FileType.Raf       => RafMetadataReader.ReadMetadata(stream),
                FileType.Rw2       => TiffMetadataReader.ReadMetadata(stream),
                FileType.Srw       => TiffMetadataReader.ReadMetadata(stream),
                FileType.Tga       => TgaMetadataReader.ReadMetadata(stream),
                FileType.ThreeFR   => TiffMetadataReader.ReadMetadata(stream),
                FileType.Tiff      => TiffMetadataReader.ReadMetadata(stream),
                FileType.Wav       => WavMetadataReader.ReadMetadata(stream),
                FileType.WebP      => WebPMetadataReader.ReadMetadata(stream),

                FileType.Unknown   => throw new ImageProcessingException("File format could not be determined"),
                _                  => Enumerable.Empty<Directory>()
            });

#pragma warning restore format

            directories.Add(new FileTypeDirectory(fileType));

            return directories;
        }

        /// <summary>Reads metadata from a file.</summary>
        /// <remarks>Unlike <see cref="ReadMetadata(Stream, string)"/>, this overload includes a <see cref="FileMetadataDirectory"/> in the output.</remarks>
        /// <param name="filePath">Location of a file from which data should be read.</param>
        /// <returns>A list of <see cref="Directory"/> instances containing the various types of metadata found within the file's data.</returns>
        /// <exception cref="ImageProcessingException">The file type is unknown, or processing errors occurred.</exception>
        /// <exception cref="IOException"/>
        public static IReadOnlyList<Directory> ReadMetadata(string filePath)
        {
            var directories = new List<Directory>();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                directories.AddRange(ReadMetadata(stream, filePath));

            directories.Add(new FileMetadataReader().Read(filePath));

            return directories;
        }
    }
}
