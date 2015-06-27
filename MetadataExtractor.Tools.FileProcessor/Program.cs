#region License
//
// Copyright 2002-2015 Drew Noakes
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Exif;

/*
 * metadata-extractor foo.jpg
 * metadata-extractor foo.jpg --extract-thumb foo.thumb.jpg
 * metadata-extractor --recursive /foo --format
 *
 */

namespace MetadataExtractor.Tools.FileProcessor
{
    // TODO port MarkdownTableOutputHandler
    // TODO port UnknownTagHandler

    internal static class Program
    {
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
        public static void Main2([NotNull] string[] args)
        {
            ICollection<string> argList = args.ToList();
            var thumbRequested = argList.Remove("-thumb");
            var markdownFormat = argList.Remove("-markdown");
            var showHex = argList.Remove("-hex");
            if (argList.Count < 1)
            {
                var version = typeof(ImageMetadataReader).Assembly.GetName().Version.ToString();
                Console.Out.WriteLine("metadata-extractor version " + version);
                Console.Out.WriteLine();
                Console.Out.WriteLine("Usage: java -jar metadata-extractor-{0}.jar <filename> [<filename>] [-thumb] [-markdown] [-hex]", version ?? "a.b.c");
                if (Debugger.IsAttached)
                    Console.ReadLine();
                Environment.Exit(1);
            }
            foreach (var filePath in argList)
            {
                var stopwatch = Stopwatch.StartNew();
                if (!markdownFormat && argList.Count > 1)
                {
                    Console.Out.WriteLine("\n***** PROCESSING: {0}", filePath);
                }
                IEnumerable<Directory> directories = null;
                try
                {
                    directories = ImageMetadataReader.ReadMetadata(filePath);
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
                    var urlName = UrlEncode(filePath);
                    var exifIfd0Directory = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
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
                foreach (var directory in directories)
                {
                    var directoryName = directory.Name;
                    foreach (var tag in directory.Tags)
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
                    foreach (var error in directory.Errors)
                    {
                        Console.Error.WriteLine((object)("ERROR: " + error));
                    }
                }
                if (args.Length > 1 && thumbRequested)
                {
                    var thumbnailDirectory = directories.OfType<ExifThumbnailDirectory>().FirstOrDefault();
                    if (thumbnailDirectory != null && thumbnailDirectory.HasThumbnailData())
                    {
                        Console.Out.WriteLine("Writing thumbnail...");
                        thumbnailDirectory.WriteThumbnail(args[0].Trim() + ".thumb.jpg");
                    }
                    else
                    {
                        Console.Out.WriteLine("No thumbnail data exists in this image");
                    }
                }
            }

            if (Debugger.IsAttached)
                Console.ReadLine();
        }

        [NotNull]
        public static string UrlEncode([NotNull] string name)
        {
            // Sufficient for now, it seems
            // TODO review http://stackoverflow.com/questions/3840762/how-do-you-urlencode-without-using-system-web
            return name.Replace(" ", "%20");
        }

        private static int Main(string[] args)
        {
            var directories = new List<string>();

            var fileHandler = (IFileHandler)null;
            var log = Console.Out;

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg == "--text")
                {
                    // If "--text" is specified, write the discovered metadata into a sub-folder relative to the image
                    fileHandler = new TextFileOutputHandler();
                }
//                else if (arg == "--markdown")
//                {
//                    // If "--markdown" is specified, write a summary table in markdown format to standard out
//                    fileHandler = new MarkdownTableOutputHandler();
//                }
//                else if (arg == "--unknown")
//                {
//                    // If "--unknown" is specified, write CSV tallying unknown tag counts
//                    fileHandler = new UnknownTagHandler();
//                }
                else if (arg == "--log-file")
                {
                    if (i == args.Length - 1)
                    {
                        PrintUsage();
                        if (Debugger.IsAttached)
                            Console.ReadLine();
                        return 1;
                    }
                    log = new StreamWriter(args[++i], append: false);
                }
                else
                {
                    // Treat this argument as a directory
                    directories.Add(arg);
                }
            }

            if (directories.Count == 0)
            {
                Console.Error.WriteLine("Expects one or more directories as arguments.");
                PrintUsage();
                if (Debugger.IsAttached)
                    Console.ReadLine();
                return 1;
            }

            if (fileHandler == null)
                fileHandler = new BasicFileHandler();

            var stopwatch = Stopwatch.StartNew();

            foreach (var directory in directories)
                ProcessDirectory(directory, fileHandler, "", log);

            fileHandler.OnScanCompleted(log);

            Console.Out.WriteLine("Completed in {0:#,##0.##} ms", stopwatch.Elapsed.TotalMilliseconds);

            if (!ReferenceEquals(Console.Out, log))
                log.Dispose();

            if (Debugger.IsAttached)
                Console.ReadLine();

            return 0;
        }

        private static void PrintUsage()
        {
            Console.Out.WriteLine("Usage:");
            Console.Out.WriteLine();
            Console.Out.WriteLine("  {0}.exe [--text|--markdown|--unknown] [--log-file <file-name>]",
                Assembly.GetExecutingAssembly().GetName().Name);
        }

        private static void ProcessDirectory([NotNull] string path, [NotNull] IFileHandler handler, [NotNull] string relativePath, [NotNull] TextWriter log)
        {
            var entries = System.IO.Directory.GetFileSystemEntries(path);

            // Order alphabetically so that output is stable across invocations
            Array.Sort(entries, string.CompareOrdinal);

            foreach (var entry in entries)
            {
                var file = Path.Combine(path, entry);

                if (System.IO.Directory.Exists(file))
                {
                    ProcessDirectory(file, handler, relativePath.Length == 0 ? entry : relativePath + "/" + entry, log);
                }
                else if (handler.ShouldProcess(file))
                {
                    handler.OnBeforeExtraction(file, relativePath, log);

                    // Read metadata
                    try
                    {
                        var directories = ImageMetadataReader.ReadMetadata(file);
                        handler.OnExtractionSuccess(file, directories, relativePath, log);
                    }
                    catch (Exception e)
                    {
                        handler.OnExtractionError(file, e, log);
                    }
                }
            }
        }
    }
}