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
using Com.Drew.Imaging.Bmp;
using Com.Drew.Imaging.Gif;
using Com.Drew.Imaging.Ico;
using Com.Drew.Imaging.Jpeg;
using Com.Drew.Imaging.Pcx;
using Com.Drew.Imaging.Png;
using Com.Drew.Imaging.Psd;
using Com.Drew.Imaging.Tiff;
using Com.Drew.Imaging.Webp;
using Com.Drew.Lang;
using Com.Drew.Metadata;
using Com.Drew.Metadata.Exif;
using Com.Drew.Metadata.File;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Imaging
{
    /// <summary>
    /// Obtains
    /// <see cref="Com.Drew.Metadata.Metadata"/>
    /// from all supported file formats.
    /// <para>
    /// This class a lightweight wrapper around specific file type processors:
    /// <list type="bullet">
    /// <item>
    /// <see cref="Com.Drew.Imaging.Jpeg.JpegMetadataReader"/>
    /// for JPEG files</item>
    /// <item>
    /// <see cref="Com.Drew.Imaging.Tiff.TiffMetadataReader"/>
    /// for TIFF and (most) RAW files</item>
    /// <item>
    /// <see cref="Com.Drew.Imaging.Psd.PsdMetadataReader"/>
    /// for Photoshop files</item>
    /// <item>
    /// <see cref="Com.Drew.Imaging.Png.PngMetadataReader"/>
    /// for BMP files</item>
    /// <item>
    /// <see cref="Com.Drew.Imaging.Bmp.BmpMetadataReader"/>
    /// for BMP files</item>
    /// <item>
    /// <see cref="Com.Drew.Imaging.Gif.GifMetadataReader"/>
    /// for GIF files</item>
    /// </list>
    /// If you know the file type you're working with, you may use one of the above processors directly.
    /// For most scenarios it is simpler, more convenient and more robust to use this class.
    /// <para>
    /// <see cref="FileTypeDetector"/>
    /// is used to determine the provided image's file type, and therefore
    /// the appropriate metadata reader to use.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class ImageMetadataReader
    {
        /// <summary>
        /// Reads metadata from an <see cref="InputStream"/>.
        /// <para>
        /// The file type is determined by inspecting the leading bytes of the stream, and parsing of the file
        /// is delegated to one of:
        /// <list type="bullet">
        /// <item>
        /// <see cref="Com.Drew.Imaging.Jpeg.JpegMetadataReader"/>
        /// for JPEG files</item>
        /// <item>
        /// <see cref="Com.Drew.Imaging.Tiff.TiffMetadataReader"/>
        /// for TIFF and (most) RAW files</item>
        /// <item>
        /// <see cref="Com.Drew.Imaging.Psd.PsdMetadataReader"/>
        /// for Photoshop files</item>
        /// <item>
        /// <see cref="Com.Drew.Imaging.Png.PngMetadataReader"/>
        /// for PNG files</item>
        /// <item>
        /// <see cref="Com.Drew.Imaging.Bmp.BmpMetadataReader"/>
        /// for BMP files</item>
        /// <item>
        /// <see cref="Com.Drew.Imaging.Gif.GifMetadataReader"/>
        /// for GIF files</item>
        /// </list>
        /// </summary>
        /// <param name="inputStream">
        /// a stream from which the file data may be read.  The stream must be positioned at the
        /// beginning of the file's data.
        /// </param>
        /// <returns>
        /// a populated
        /// <see cref="Com.Drew.Metadata.Metadata"/>
        /// object containing directories of tags with values and any processing errors.
        /// </returns>
        /// <exception cref="ImageProcessingException">if the file type is unknown, or for general processing errors.</exception>
        /// <exception cref="Com.Drew.Imaging.ImageProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static Metadata.Metadata ReadMetadata([NotNull] InputStream inputStream)
        {
            var stream = inputStream as BufferedInputStream;
            BufferedInputStream bufferedInputStream = stream ?? new BufferedInputStream(inputStream);
            FileType fileType = FileTypeDetector.DetectFileType(bufferedInputStream) ?? FileType.Unknown;
            if (fileType == FileType.Jpeg)
            {
                return JpegMetadataReader.ReadMetadata(bufferedInputStream);
            }
            if (fileType == FileType.Tiff || fileType == FileType.Arw || fileType == FileType.Cr2 || fileType == FileType.Nef || fileType == FileType.Orf || fileType == FileType.Rw2)
            {
                return TiffMetadataReader.ReadMetadata(bufferedInputStream);
            }
            if (fileType == FileType.Psd)
            {
                return PsdMetadataReader.ReadMetadata(bufferedInputStream);
            }
            if (fileType == FileType.Png)
            {
                return PngMetadataReader.ReadMetadata(bufferedInputStream);
            }
            if (fileType == FileType.Bmp)
            {
                return BmpMetadataReader.ReadMetadata(bufferedInputStream);
            }
            if (fileType == FileType.Gif)
            {
                return GifMetadataReader.ReadMetadata(bufferedInputStream);
            }
            if (fileType == FileType.Ico)
            {
                return IcoMetadataReader.ReadMetadata(bufferedInputStream);
            }
            if (fileType == FileType.Pcx)
            {
                return PcxMetadataReader.ReadMetadata(bufferedInputStream);
            }
            if (fileType == FileType.Riff)
            {
                return WebpMetadataReader.ReadMetadata(bufferedInputStream);
            }
            throw new ImageProcessingException("File format is not supported");
        }

        /// <summary>
        /// Reads
        /// <see cref="Com.Drew.Metadata.Metadata"/>
        /// from a
        /// <see cref="Sharpen.FilePath"/>
        /// object.
        /// <para>
        /// The file type is determined by inspecting the leading bytes of the stream, and parsing of the file
        /// is delegated to one of:
        /// <list type="bullet">
        /// <item>
        /// <see cref="Com.Drew.Imaging.Jpeg.JpegMetadataReader"/>
        /// for JPEG files</item>
        /// <item>
        /// <see cref="Com.Drew.Imaging.Tiff.TiffMetadataReader"/>
        /// for TIFF and (most) RAW files</item>
        /// <item>
        /// <see cref="Com.Drew.Imaging.Psd.PsdMetadataReader"/>
        /// for Photoshop files</item>
        /// <item>
        /// <see cref="Com.Drew.Imaging.Png.PngMetadataReader"/>
        /// for PNG files</item>
        /// <item>
        /// <see cref="Com.Drew.Imaging.Bmp.BmpMetadataReader"/>
        /// for BMP files</item>
        /// <item>
        /// <see cref="Com.Drew.Imaging.Gif.GifMetadataReader"/>
        /// for GIF files</item>
        /// </list>
        /// </summary>
        /// <param name="file">a file from which the image data may be read.</param>
        /// <returns>
        /// a populated
        /// <see cref="Com.Drew.Metadata.Metadata"/>
        /// object containing directories of tags with values and any processing errors.
        /// </returns>
        /// <exception cref="ImageProcessingException">for general processing errors.</exception>
        /// <exception cref="Com.Drew.Imaging.ImageProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static Metadata.Metadata ReadMetadata([NotNull] FilePath file)
        {
            InputStream inputStream = new FileInputStream(file);
            Metadata.Metadata metadata;
            try
            {
                metadata = ReadMetadata(inputStream);
            }
            finally
            {
                inputStream.Close();
            }
            new FileMetadataReader().Read(file, metadata);
            return metadata;
        }

        /// <exception cref="System.Exception"/>
        private ImageMetadataReader()
        {
            throw new Exception("Not intended for instantiation");
        }

        /// <summary>An application entry point.</summary>
        /// <remarks>
        /// An application entry point.  Takes the name of one or more files as arguments and prints the contents of all
        /// metadata directories to <c>System.out</c>.
        /// <para>
        /// If <c>-thumb</c> is passed, then any thumbnail data will be written to a file with name of the
        /// input file having <c>.thumb.jpg</c> appended.
        /// <para>
        /// If <c>-markdown</c> is passed, then output will be in markdown format.
        /// <para>
        /// If <c>-hex</c> is passed, then the ID of each tag will be displayed in hexadecimal.
        /// </remarks>
        /// <param name="args">the command line arguments</param>
        /// <exception cref="Com.Drew.Metadata.MetadataException"/>
        /// <exception cref="System.IO.IOException"/>
        public static void Main([NotNull] string[] args)
        {
            ICollection<string> argList = new AList<string>(Arrays.AsList(args));
            bool thumbRequested = argList.Remove("-thumb");
            bool markdownFormat = argList.Remove("-markdown");
            bool showHex = argList.Remove("-hex");
            if (argList.Count < 1)
            {
                string version = typeof(ImageMetadataReader).Assembly.GetImplementationVersion();
                Console.Out.Println("metadata-extractor version " + version);
                Console.Out.Println();
                Console.Out.Println(Extensions.StringFormat("Usage: java -jar metadata-extractor-%s.jar <filename> [<filename>] [-thumb] [-markdown] [-hex]",
                    version ?? "a.b.c"));
                Environment.Exit(1);
            }
            foreach (string filePath in argList)
            {
                long startTime = Runtime.NanoTime();
                FilePath file = new FilePath(filePath);
                if (!markdownFormat && argList.Count > 1)
                {
                    Console.Out.Printf("\n***** PROCESSING: %s\n%n", filePath);
                }
                Metadata.Metadata metadata = null;
                try
                {
                    metadata = ReadMetadata(file);
                }
                catch (Exception e)
                {
                    Runtime.PrintStackTrace(e, Console.Error);
                    Environment.Exit(1);
                }
                long took = Runtime.NanoTime() - startTime;
                if (!markdownFormat)
                {
                    Console.Out.Printf("Processed %.3f MB file in %.2f ms%n%n", file.Length() / (1024d * 1024), took / 1000000d);
                }
                if (markdownFormat)
                {
                    string fileName = file.GetName();
                    string urlName = StringUtil.UrlEncode(filePath);
                    ExifIfd0Directory exifIfd0Directory = metadata.GetFirstDirectoryOfType<ExifIfd0Directory>();
                    string make = exifIfd0Directory == null ? string.Empty : exifIfd0Directory.GetString(ExifDirectoryBase.TagMake);
                    string model = exifIfd0Directory == null ? string.Empty : exifIfd0Directory.GetString(ExifDirectoryBase.TagModel);
                    Console.Out.Println();
                    Console.Out.Println("---");
                    Console.Out.Println();
                    Console.Out.Printf("# %s - %s%n", make, model);
                    Console.Out.Println();
                    Console.Out.Printf("<a href=\"https://raw.githubusercontent.com/drewnoakes/metadata-extractor-images/master/%s\">%n", urlName);
                    Console.Out.Printf("<img src=\"https://raw.githubusercontent.com/drewnoakes/metadata-extractor-images/master/%s\" width=\"300\"/><br/>%n", urlName);
                    Console.Out.Println(fileName);
                    Console.Out.Println("</a>");
                    Console.Out.Println();
                    Console.Out.Println("Directory | Tag Id | Tag Name | Extracted Value");
                    Console.Out.Println(":--------:|-------:|----------|----------------");
                }
                // iterate over the metadata and print to System.out
                foreach (Directory directory in metadata.GetDirectories())
                {
                    string directoryName = directory.GetName();
                    foreach (Tag tag in directory.GetTags())
                    {
                        string tagName = tag.GetTagName();
                        string description = tag.GetDescription();
                        // truncate the description if it's too long
                        if (description != null && description.Length > 1024)
                        {
                            description = Runtime.Substring(description, 0, 1024) + "...";
                        }
                        if (markdownFormat)
                        {
                            Console.Out.Printf("%s|0x%s|%s|%s%n", directoryName, Extensions.ToHexString(tag.GetTagType()), tagName, description);
                        }
                        else
                        {
                            // simple formatting
                            if (showHex)
                            {
                                Console.Out.Printf("[%s - %s] %s = %s%n", directoryName, tag.GetTagTypeHex(), tagName, description);
                            }
                            else
                            {
                                Console.Out.Printf("[%s] %s = %s%n", directoryName, tagName, description);
                            }
                        }
                    }
                    // print out any errors
                    foreach (string error in directory.GetErrors())
                    {
                        Console.Error.Println("ERROR: " + error);
                    }
                }
                if (args.Length > 1 && thumbRequested)
                {
                    ExifThumbnailDirectory directory1 = metadata.GetFirstDirectoryOfType<ExifThumbnailDirectory>();
                    if (directory1 != null && directory1.HasThumbnailData())
                    {
                        Console.Out.Println("Writing thumbnail...");
                        directory1.WriteThumbnail(Extensions.Trim(args[0]) + ".thumb.jpg");
                    }
                    else
                    {
                        Console.Out.Println("No thumbnail data exists in this image");
                    }
                }
            }
        }
    }
}
