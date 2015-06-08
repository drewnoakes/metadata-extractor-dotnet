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
using System.IO;
using Com.Drew.Imaging;
using Com.Drew.Lang;
using Com.Drew.Metadata;
using Com.Drew.Metadata.Exif;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Tools
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class ProcessAllImagesInFolderUtility
    {
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Com.Drew.Imaging.Jpeg.JpegProcessingException"/>
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                System.Console.Error.Println("Expects one or more directories as arguments.");
                System.Environment.Exit(1);
            }
            IList<string> directories = new AList<string>();
            ProcessAllImagesInFolderUtility.FileHandler handler = null;
            foreach (string arg in args)
            {
                if (Sharpen.Runtime.EqualsIgnoreCase(arg, "-text"))
                {
                    // If "-text" is specified, write the discovered metadata into a sub-folder relative to the image
                    handler = new ProcessAllImagesInFolderUtility.TextFileOutputHandler();
                }
                else
                {
                    if (Sharpen.Runtime.EqualsIgnoreCase(arg, "-markdown"))
                    {
                        // If "-markdown" is specified, write a summary table in markdown format to standard out
                        handler = new ProcessAllImagesInFolderUtility.MarkdownTableOutputHandler();
                    }
                    else
                    {
                        if (Sharpen.Runtime.EqualsIgnoreCase(arg, "-unknown"))
                        {
                            // If "-unknown" is specified, write CSV tallying unknown tag counts
                            handler = new ProcessAllImagesInFolderUtility.UnknownTagHandler();
                        }
                        else
                        {
                            // Treat this argument as a directory
                            directories.Add(arg);
                        }
                    }
                }
            }
            if (handler == null)
            {
                handler = new ProcessAllImagesInFolderUtility.BasicFileHandler();
            }
            long start = Runtime.NanoTime();
            // Order alphabetically so that output is stable across invocations
            directories.Sort();
            foreach (string directory in directories)
            {
                ProcessDirectory(new FilePath(directory), handler, string.Empty);
            }
            handler.OnCompleted();
            System.Console.Out.Println(Sharpen.Extensions.StringFormat("Completed in %d ms", (Runtime.NanoTime() - start) / 1000000));
        }

        private static void ProcessDirectory([NotNull] FilePath path, [NotNull] ProcessAllImagesInFolderUtility.FileHandler handler, [NotNull] string relativePath)
        {
            string[] pathItems = path.List();
            if (pathItems == null)
            {
                return;
            }
            // Order alphabetically so that output is stable across invocations
            Arrays.Sort(pathItems);
            foreach (string pathItem in pathItems)
            {
                FilePath file = new FilePath(path, pathItem);
                if (file.IsDirectory())
                {
                    ProcessDirectory(file, handler, relativePath.Length == 0 ? pathItem : relativePath + "/" + pathItem);
                }
                else
                {
                    if (handler.ShouldProcess(file))
                    {
                        handler.OnProcessingStarting(file);
                        // Read metadata
                        Com.Drew.Metadata.Metadata metadata;
                        try
                        {
                            metadata = ImageMetadataReader.ReadMetadata(file);
                        }
                        catch (Exception t)
                        {
                            handler.OnException(file, t);
                            continue;
                        }
                        handler.OnExtracted(file, metadata, relativePath);
                    }
                }
            }
        }

        internal interface FileHandler
        {
            bool ShouldProcess([NotNull] FilePath file);

            void OnException([NotNull] FilePath file, [NotNull] Exception throwable);

            void OnExtracted([NotNull] FilePath file, [NotNull] Com.Drew.Metadata.Metadata metadata, [NotNull] string relativePath);

            void OnCompleted();

            void OnProcessingStarting([NotNull] FilePath file);
        }

        internal abstract class FileHandlerBase : ProcessAllImagesInFolderUtility.FileHandler
        {
            private readonly ICollection<string> _supportedExtensions = new HashSet<string>(Arrays.AsList("jpg", "jpeg", "png", "gif", "bmp", "ico", "webp", "pcx", "ai", "eps", "nef", "crw", "cr2", "orf", "arw", "raf", "srw", "x3f", "rw2", "rwl", "tif", 
                "tiff", "psd", "dng"));

            private int _processedFileCount = 0;

            private int _exceptionCount = 0;

            private int _errorCount = 0;

            private long _processedByteCount = 0;

            public virtual bool ShouldProcess([NotNull] FilePath file)
            {
                string extension = GetExtension(file);
                return extension != null && _supportedExtensions.Contains(extension.ToLower());
            }

            public virtual void OnProcessingStarting([NotNull] FilePath file)
            {
                _processedFileCount++;
                _processedByteCount += file.Length();
            }

            public virtual void OnException([NotNull] FilePath file, [NotNull] Exception throwable)
            {
                _exceptionCount++;
                if (throwable is ImageProcessingException)
                {
                    // this is an error in the Jpeg segment structure.  we're looking for bad handling of
                    // metadata segments.  in this case, we didn't even get a segment.
                    System.Console.Error.Printf("%s: %s [Error Extracting Metadata]\n\t%s%n", throwable.GetType().FullName, file, throwable.Message);
                }
                else
                {
                    // general, uncaught exception during processing of jpeg segments
                    System.Console.Error.Printf("%s: %s [Error Extracting Metadata]%n", throwable.GetType().FullName, file);
                    Sharpen.Runtime.PrintStackTrace(throwable, System.Console.Error);
                }
            }

            public virtual void OnExtracted([NotNull] FilePath file, [NotNull] Com.Drew.Metadata.Metadata metadata, [NotNull] string relativePath)
            {
                if (metadata.HasErrors())
                {
                    System.Console.Error.Println(file);
                    foreach (Com.Drew.Metadata.Directory directory in metadata.GetDirectories())
                    {
                        if (!directory.HasErrors())
                        {
                            continue;
                        }
                        foreach (string error in directory.GetErrors())
                        {
                            System.Console.Error.Printf("\t[%s] %s%n", directory.GetName(), error);
                            _errorCount++;
                        }
                    }
                }
            }

            public virtual void OnCompleted()
            {
                if (_processedFileCount > 0)
                {
                    System.Console.Out.Println(Sharpen.Extensions.StringFormat("Processed %,d files (%,d bytes) with %,d exceptions and %,d file errors", _processedFileCount, _processedByteCount, _exceptionCount, _errorCount));
                }
            }

            [CanBeNull]
            protected internal virtual string GetExtension([NotNull] FilePath file)
            {
                string fileName = file.GetName();
                int i = fileName.LastIndexOf('.');
                if (i == -1)
                {
                    return null;
                }
                if (i == fileName.Length - 1)
                {
                    return null;
                }
                return Sharpen.Runtime.Substring(fileName, i + 1);
            }
        }

        /// <summary>Writes a text file containing the extracted metadata for each input file.</summary>
        internal class TextFileOutputHandler : ProcessAllImagesInFolderUtility.FileHandlerBase
        {
            public override void OnExtracted([NotNull] FilePath file, [NotNull] Com.Drew.Metadata.Metadata metadata, [NotNull] string relativePath)
            {
                base.OnExtracted(file, metadata, relativePath);
                try
                {
                    PrintWriter writer = null;
                    try
                    {
                        writer = OpenWriter(file);
                        // Build a list of all directories
                        IList<Com.Drew.Metadata.Directory> directories = new AList<Com.Drew.Metadata.Directory>();
                        foreach (Com.Drew.Metadata.Directory directory in metadata.GetDirectories())
                        {
                            directories.Add(directory);
                        }
                        // Sort them by name
                        directories.Sort(new _IComparer_235());
                        // Write any errors
                        if (metadata.HasErrors())
                        {
                            foreach (Com.Drew.Metadata.Directory directory_1 in directories)
                            {
                                if (!directory_1.HasErrors())
                                {
                                    continue;
                                }
                                foreach (string error in directory_1.GetErrors())
                                {
                                    writer.Format("[ERROR: %s] %s\n", directory_1.GetName(), error);
                                }
                            }
                            writer.Write("\n");
                        }
                        // Write tag values for each directory
                        foreach (Com.Drew.Metadata.Directory directory_2 in directories)
                        {
                            string directoryName = directory_2.GetName();
                            foreach (Tag tag in directory_2.GetTags())
                            {
                                string tagName = tag.GetTagName();
                                string description = tag.GetDescription();
                                writer.Format("[%s - %s] %s = %s%n", directoryName, tag.GetTagTypeHex(), tagName, description);
                            }
                            if (directory_2.GetTagCount() != 0)
                            {
                                writer.Write('\n');
                            }
                        }
                    }
                    finally
                    {
                        CloseWriter(writer);
                    }
                }
                catch (IOException e)
                {
                    Sharpen.Runtime.PrintStackTrace(e);
                }
            }

            private sealed class _IComparer_235 : IComparer<Com.Drew.Metadata.Directory>
            {
                public _IComparer_235()
                {
                }

                public int Compare(Com.Drew.Metadata.Directory o1, Com.Drew.Metadata.Directory o2)
                {
                    return string.CompareOrdinal(o1.GetName(), o2.GetName());
                }
            }

            public override void OnException([NotNull] FilePath file, [NotNull] Exception throwable)
            {
                base.OnException(file, throwable);
                try
                {
                    PrintWriter writer = null;
                    try
                    {
                        writer = OpenWriter(file);
                        Sharpen.Runtime.PrintStackTrace(throwable, writer);
                        writer.Write('\n');
                    }
                    finally
                    {
                        CloseWriter(writer);
                    }
                }
                catch (IOException e)
                {
                    System.Console.Error.Printf("IO exception writing metadata file: %s%n", e.Message);
                }
            }

            /// <exception cref="System.IO.IOException"/>
            [NotNull]
            private static PrintWriter OpenWriter([NotNull] FilePath file)
            {
                // Create the output directory if it doesn't exist
                FilePath metadataDir = new FilePath(Sharpen.Extensions.StringFormat("%s/metadata", file.GetParent()));
                if (!metadataDir.Exists())
                {
                    metadataDir.Mkdir();
                }
                string outputPath = Sharpen.Extensions.StringFormat("%s/metadata/%s.txt", file.GetParent(), file.GetName().ToLower());
                FileWriter writer = new FileWriter(outputPath, false);
                writer.Write("FILE: " + file.GetName() + "\n");
                writer.Write('\n');
                return new PrintWriter(writer);
            }

            /// <exception cref="System.IO.IOException"/>
            private static void CloseWriter([CanBeNull] TextWriter writer)
            {
                if (writer != null)
                {
                    writer.Write("Generated using metadata-extractor\n");
                    writer.Write("https://drewnoakes.com/code/exif/\n");
                    writer.Flush();
                    writer.Close();
                }
            }
        }

        /// <summary>Creates a table describing sample images using Wiki markdown.</summary>
        internal class MarkdownTableOutputHandler : ProcessAllImagesInFolderUtility.FileHandlerBase
        {
            private readonly IDictionary<string, string> _extensionEquivalence = new Dictionary<string, string>();

            private readonly IDictionary<string, IList<ProcessAllImagesInFolderUtility.MarkdownTableOutputHandler.Row>> _rowListByExtension = new Dictionary<string, IList<ProcessAllImagesInFolderUtility.MarkdownTableOutputHandler.Row>>();

            internal class Row
            {
                internal readonly FilePath file;

                internal readonly Com.Drew.Metadata.Metadata metadata;

                [NotNull]
                internal readonly string relativePath;

                [CanBeNull] internal string manufacturer;

                [CanBeNull] internal string model;

                [CanBeNull] internal string exifVersion;

                [CanBeNull] internal string thumbnail;

                [CanBeNull] internal string makernote;

                internal Row(MarkdownTableOutputHandler _enclosing, [NotNull] FilePath file, [NotNull] Com.Drew.Metadata.Metadata metadata, [NotNull] string relativePath)
                {
                    this._enclosing = _enclosing;
                    this.file = file;
                    this.metadata = metadata;
                    this.relativePath = relativePath;
                    ExifIFD0Directory ifd0Dir = metadata.GetFirstDirectoryOfType<ExifIFD0Directory>();
                    ExifSubIFDDirectory subIfdDir = metadata.GetFirstDirectoryOfType<ExifSubIFDDirectory>();
                    ExifThumbnailDirectory thumbDir = metadata.GetFirstDirectoryOfType<ExifThumbnailDirectory>();
                    if (ifd0Dir != null)
                    {
                        this.manufacturer = ifd0Dir.GetDescription(ExifIFD0Directory.TagMake);
                        this.model = ifd0Dir.GetDescription(ExifIFD0Directory.TagModel);
                    }
                    bool hasMakernoteData = false;
                    if (subIfdDir != null)
                    {
                        this.exifVersion = subIfdDir.GetDescription(ExifSubIFDDirectory.TagExifVersion);
                        hasMakernoteData = subIfdDir.ContainsTag(ExifSubIFDDirectory.TagMakernote);
                    }
                    if (thumbDir != null)
                    {
                        int? width = thumbDir.GetInteger(ExifThumbnailDirectory.TagImageWidth);
                        int? height = thumbDir.GetInteger(ExifThumbnailDirectory.TagImageHeight);
                        this.thumbnail = width != null && height != null ? Sharpen.Extensions.StringFormat("Yes (%s x %s)", width, height) : "Yes";
                    }
                    foreach (Com.Drew.Metadata.Directory directory in metadata.GetDirectories())
                    {
                        if (directory.GetType().FullName.Contains("Makernote"))
                        {
                            this.makernote = Sharpen.Extensions.Trim(directory.GetName().Replace("Makernote", string.Empty));
                        }
                    }
                    if (this.makernote == null)
                    {
                        this.makernote = hasMakernoteData ? "(Unknown)" : "N/A";
                    }
                }

                private readonly MarkdownTableOutputHandler _enclosing;
            }

            public MarkdownTableOutputHandler()
            {
                _extensionEquivalence.Put("jpeg", "jpg");
            }

            public override void OnExtracted([NotNull] FilePath file, [NotNull] Com.Drew.Metadata.Metadata metadata, [NotNull] string relativePath)
            {
                base.OnExtracted(file, metadata, relativePath);
                string extension = GetExtension(file);
                if (extension == null)
                {
                    return;
                }
                // Sanitise the extension
                extension = extension.ToLower();
                if (_extensionEquivalence.ContainsKey(extension))
                {
                    extension = _extensionEquivalence.Get(extension);
                }
                IList<ProcessAllImagesInFolderUtility.MarkdownTableOutputHandler.Row> list = _rowListByExtension.Get(extension);
                if (list == null)
                {
                    list = new AList<ProcessAllImagesInFolderUtility.MarkdownTableOutputHandler.Row>();
                    _rowListByExtension.Put(extension, list);
                }
                list.Add(new ProcessAllImagesInFolderUtility.MarkdownTableOutputHandler.Row(this, file, metadata, relativePath));
            }

            public override void OnCompleted()
            {
                base.OnCompleted();
                OutputStream outputStream = null;
                PrintStream stream = null;
                try
                {
                    outputStream = new FileOutputStream("../wiki/ImageDatabaseSummary.md", false);
                    stream = new PrintStream(outputStream, false);
                    WriteOutput(stream);
                    stream.Flush();
                }
                catch (IOException e)
                {
                    Sharpen.Runtime.PrintStackTrace(e);
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }
                    if (outputStream != null)
                    {
                        try
                        {
                            outputStream.Close();
                        }
                        catch (IOException e)
                        {
                            Sharpen.Runtime.PrintStackTrace(e);
                        }
                    }
                }
            }

            /// <exception cref="System.IO.IOException"/>
            private void WriteOutput([NotNull] PrintStream stream)
            {
                TextWriter writer = new OutputStreamWriter(stream);
                writer.Write("# Image Database Summary\n\n");
                foreach (string extension in _rowListByExtension.Keys)
                {
                    writer.Write("## " + extension.ToUpper() + " Files\n\n");
                    writer.Write("File|Manufacturer|Model|Dir Count|Exif?|Makernote|Thumbnail|All Data\n");
                    writer.Write("----|------------|-----|---------|-----|---------|---------|--------\n");
                    IList<ProcessAllImagesInFolderUtility.MarkdownTableOutputHandler.Row> rows = _rowListByExtension.Get(extension);
                    // Order by manufacturer, then model
                    rows.Sort(new _IComparer_441());
                    foreach (ProcessAllImagesInFolderUtility.MarkdownTableOutputHandler.Row row in rows)
                    {
                        writer.Write(Sharpen.Extensions.StringFormat("[%s](https://raw.githubusercontent.com/drewnoakes/metadata-extractor-images/master/%s/%s)|%s|%s|%d|%s|%s|%s|[metadata](https://raw.githubusercontent.com/drewnoakes/metadata-extractor-images/master/%s/metadata/%s.txt)%n"
                            , row.file.GetName(), row.relativePath, StringUtil.UrlEncode(row.file.GetName()), row.manufacturer == null ? string.Empty : row.manufacturer, row.model == null ? string.Empty : row.model, row.metadata.GetDirectoryCount(), row.exifVersion ==
                             null ? string.Empty : row.exifVersion, row.makernote == null ? string.Empty : row.makernote, row.thumbnail == null ? string.Empty : row.thumbnail, row.relativePath, StringUtil.UrlEncode(row.file.GetName()).ToLower()));
                    }
                    writer.Write('\n');
                }
                writer.Flush();
            }

            private sealed class _IComparer_441 : IComparer<ProcessAllImagesInFolderUtility.MarkdownTableOutputHandler.Row>
            {
                public _IComparer_441()
                {
                }

                public int Compare(ProcessAllImagesInFolderUtility.MarkdownTableOutputHandler.Row o1, ProcessAllImagesInFolderUtility.MarkdownTableOutputHandler.Row o2)
                {
                    int c1 = StringUtil.Compare(o1.manufacturer, o2.manufacturer);
                    return c1 != 0 ? c1 : StringUtil.Compare(o1.model, o2.model);
                }
            }
        }

        /// <summary>Keeps track of unknown tags.</summary>
        internal class UnknownTagHandler : ProcessAllImagesInFolderUtility.FileHandlerBase
        {
            private Dictionary<string, Dictionary<int?, int?>> _occurrenceCountByTagByDirectory = new Dictionary<string, Dictionary<int?, int?>>();

            public override void OnExtracted([NotNull] FilePath file, [NotNull] Com.Drew.Metadata.Metadata metadata, [NotNull] string relativePath)
            {
                base.OnExtracted(file, metadata, relativePath);
                foreach (Com.Drew.Metadata.Directory directory in metadata.GetDirectories())
                {
                    foreach (Tag tag in directory.GetTags())
                    {
                        // Only interested in unknown tags (those without names)
                        if (tag.HasTagName())
                        {
                            continue;
                        }
                        Dictionary<int?, int?> occurrenceCountByTag = _occurrenceCountByTagByDirectory.Get(directory.GetName());
                        if (occurrenceCountByTag == null)
                        {
                            occurrenceCountByTag = new Dictionary<int?, int?>();
                            _occurrenceCountByTagByDirectory.Put(directory.GetName(), occurrenceCountByTag);
                        }
                        int? count = occurrenceCountByTag.Get(tag.GetTagType());
                        if (count == null)
                        {
                            count = 0;
                            occurrenceCountByTag.Put(tag.GetTagType(), 0);
                        }
                        occurrenceCountByTag.Put(tag.GetTagType(), (int)count + 1);
                    }
                }
            }

            public override void OnCompleted()
            {
                base.OnCompleted();
                foreach (KeyValuePair<string, Dictionary<int?, int?>> pair1 in _occurrenceCountByTagByDirectory.EntrySet())
                {
                    string directoryName = pair1.Key;
                    IList<KeyValuePair<int?, int?>> counts = new AList<KeyValuePair<int?, int?>>(pair1.Value.EntrySet());
                    counts.Sort(new _IComparer_516());
                    foreach (KeyValuePair<int?, int?> pair2 in counts)
                    {
                        int? tagType = pair2.Key;
                        int? count = pair2.Value;
                        System.Console.Out.Format("%s, 0x%04X, %d\n", directoryName, tagType, count);
                    }
                }
            }

            private sealed class _IComparer_516 : IComparer<KeyValuePair<int?, int?>>
            {
                public _IComparer_516()
                {
                }

                public int Compare(KeyValuePair<int?, int?> o1, KeyValuePair<int?, int?> o2)
                {
                    return o2.Value.CompareTo(o1.Value);
                }
            }
        }

        /// <summary>Does nothing with the output except enumerate it in memory and format descriptions.</summary>
        /// <remarks>
        /// Does nothing with the output except enumerate it in memory and format descriptions. This is useful in order to
        /// flush out any potential exceptions raised during the formatting of extracted value descriptions.
        /// </remarks>
        internal class BasicFileHandler : ProcessAllImagesInFolderUtility.FileHandlerBase
        {
            public override void OnExtracted([NotNull] FilePath file, [NotNull] Com.Drew.Metadata.Metadata metadata, [NotNull] string relativePath)
            {
                base.OnExtracted(file, metadata, relativePath);
                // Iterate through all values, calling toString to flush out any formatting exceptions
                foreach (Com.Drew.Metadata.Directory directory in metadata.GetDirectories())
                {
                    directory.GetName();
                    foreach (Tag tag in directory.GetTags())
                    {
                        tag.GetTagName();
                        tag.GetDescription();
                    }
                }
            }
        }
    }
}
