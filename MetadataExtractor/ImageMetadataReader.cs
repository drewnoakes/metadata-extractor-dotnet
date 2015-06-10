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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Bmp;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.FileSystem;
using MetadataExtractor.Formats.Gif;
using MetadataExtractor.Formats.Ico.ico;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Pcx.pcx;
using MetadataExtractor.Formats.Photoshop.psd;
using MetadataExtractor.Formats.Png.png;
using MetadataExtractor.Formats.Tiff.tiff;
using MetadataExtractor.Formats.WebP.webp;
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
    ///   <item><see cref="PngMetadataReader"/> for BMP files</item>
    ///   <item><see cref="BmpMetadataReader"/> for BMP files</item>
    ///   <item><see cref="GifMetadataReader"/> for GIF files</item>
    ///   <item><see cref="IcoMetadataReader"/> for GIF files</item>
    ///   <item><see cref="PcxMetadataReader"/> for GIF files</item>
    ///   <item><see cref="WebpMetadataReader"/> for GIF files</item>
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
                    return WebpMetadataReader.ReadMetadata(stream);
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

        /// <summary>An application entry point.</summary>
        /// <remarks>
        /// An application entry point.  Takes the name of one or more files as arguments and prints the contents of all
        /// metadata directories to <c>System.out</c>.
        /// <para />
        /// If <c>-thumb</c> is passed, then any thumbnail data will be written to a file with name of the
        /// input file having <c>.thumb.jpg</c> appended.
        /// <para />
        /// If <c>-markdown</c> is passed, then output will be in markdown format.
        /// <para />
        /// If <c>-hex</c> is passed, then the ID of each tag will be displayed in hexadecimal.
        /// </remarks>
        /// <param name="args">the command line arguments</param>
        /// <exception cref="MetadataException"/>
        /// <exception cref="System.IO.IOException"/>
        public static void Main([NotNull] string[] args)
        {
            ICollection<string> argList = args.ToList();
            var thumbRequested = argList.Remove("-thumb");
            var markdownFormat = argList.Remove("-markdown");
            var showHex = argList.Remove("-hex");
            if (argList.Count < 1)
            {
                var version = typeof(ImageMetadataReader).Assembly.GetName().Version.ToString();
                Console.Out.WriteLine((object)("metadata-extractor version " + version));
                Console.Out.WriteLine();
                Console.Out.WriteLine((object)string.Format("Usage: java -jar metadata-extractor-{0}.jar <filename> [<filename>] [-thumb] [-markdown] [-hex]",
                    version ?? "a.b.c"));
                Environment.Exit(1);
            }
            foreach (var filePath in argList)
            {
                var stopwatch = Stopwatch.StartNew();
                if (!markdownFormat && argList.Count > 1)
                {
                    Console.Out.WriteLine("\n***** PROCESSING: {0}", filePath);
                }
                Metadata metadata = null;
                try
                {
                    metadata = ReadMetadata(filePath);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine (e);
                    Environment.Exit(1);
                }
                if (!markdownFormat)
                {
                    Console.Out.WriteLine("Processed {0:#,##0.##} MB file in {1:#,##0.##} ms\n", new FileInfo(filePath).Length / (1024d * 1024), stopwatch.Elapsed.TotalMilliseconds);
                }
                if (markdownFormat)
                {
                    var fileName = Path.GetFileName(filePath);
                    var urlName = StringUtil.UrlEncode(filePath);
                    var exifIfd0Directory = metadata.GetFirstDirectoryOfType<ExifIfd0Directory>();
                    var make = exifIfd0Directory == null ? string.Empty : exifIfd0Directory.GetString(ExifDirectoryBase.TagMake);
                    var model = exifIfd0Directory == null ? string.Empty : exifIfd0Directory.GetString(ExifDirectoryBase.TagModel);
                    Console.Out.WriteLine();
                    Console.Out.WriteLine("---");
                    Console.Out.WriteLine();
                    Console.Out.WriteLine("# {0} - {1}", make, model);
                    Console.Out.WriteLine();
                    Console.Out.WriteLine("<a href=\"https://raw.githubusercontent.com/drewnoakes/metadata-extractor-images/master/{0}\">", urlName);
                    Console.Out.WriteLine("<img src=\"https://raw.githubusercontent.com/drewnoakes/metadata-extractor-images/master/{0}\" width=\"300\"/><br/>", urlName);
                    Console.Out.WriteLine(fileName);
                    Console.Out.WriteLine("</a>");
                    Console.Out.WriteLine();
                    Console.Out.WriteLine("Directory | Tag Id | Tag Name | Extracted Value");
                    Console.Out.WriteLine(":--------:|-------:|----------|----------------");
                }
                // iterate over the metadata and print to System.out
                foreach (var directory in metadata.GetDirectories())
                {
                    var directoryName = directory.GetName();
                    foreach (var tag in directory.GetTags())
                    {
                        var tagName = tag.TagName;
                        var description = tag.Description;
                        // truncate the description if it's too long
                        if (description != null && description.Length > 1024)
                        {
                            description = description.Substring (0, 1024 - 0) + "...";
                        }
                        if (markdownFormat)
                        {
                            Console.Out.WriteLine("{0}|0x{1:X}|{2}|{3}", directoryName, tag.TagType, tagName, description);
                        }
                        else
                        {
                            // simple formatting
                            if (showHex)
                            {
                                Console.Out.WriteLine("[{0} - {1:X4}] {2} = {3}", directoryName, tag.TagType, tagName, description);
                            }
                            else
                            {
                                Console.Out.WriteLine("[{0}] {1} = {2}", directoryName, tagName, description);
                            }
                        }
                    }
                    // print out any errors
                    foreach (var error in directory.GetErrors())
                    {
                        Console.Error.WriteLine((object)("ERROR: " + error));
                    }
                }
                if (args.Length > 1 && thumbRequested)
                {
                    var directory1 = metadata.GetFirstDirectoryOfType<ExifThumbnailDirectory>();
                    if (directory1 != null && directory1.HasThumbnailData())
                    {
                        Console.Out.WriteLine("Writing thumbnail...");
                        directory1.WriteThumbnail(args[0].Trim() + ".thumb.jpg");
                    }
                    else
                    {
                        Console.Out.WriteLine("No thumbnail data exists in this image");
                    }
                }
            }
        }
    }
}
