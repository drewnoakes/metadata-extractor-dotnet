// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using MetadataExtractor.Formats.FileSystem;
using MetadataExtractor.Formats.Xmp;
using MetadataExtractor.Util;

using XmpCore;
using XmpCore.Impl;
using XmpCore.Options;

namespace MetadataExtractor.Tools.FileProcessor
{
    /// <summary>
    /// Writes a text file containing the extracted metadata for each input file.
    /// </summary>
    internal class TextFileOutputHandler : FileHandlerBase
    {
        private static readonly string NEW_LINE = "\n";

        public override void OnStartingDirectory(string directoryPath)
        {
            base.OnStartingDirectory(directoryPath);
            System.IO.Directory.Delete(Path.Combine(directoryPath, @"metadata\dotnet"), recursive: true);
        }

        public override void OnBeforeExtraction(string filePath, string relativePath, TextWriter log)
        {
            base.OnBeforeExtraction(filePath, relativePath, log);
            log.Write(filePath);
            log.Write(NEW_LINE);
        }

        public override void OnExtractionSuccess(string filePath, IList<Directory> directories, string relativePath, TextWriter log, long streamPosition)
        {
            base.OnExtractionSuccess(filePath, directories, relativePath, log, streamPosition);

            try
            {
                using var writer = OpenWriter(filePath);
                try
                {
                    // Write any errors
                    if (directories.Any(d => d.HasError))
                    {
                        foreach (var directory in directories)
                        {
                            if (!directory.HasError)
                                continue;
                            foreach (var error in directory.Errors)
                                writer.Write("[ERROR: {0}] {1}\n", directory.Name, error);
                        }
                        writer.Write(NEW_LINE);
                    }

                    // Write tag values for each directory
                    foreach (var directory in directories)
                    {
                        var directoryName = directory.Name;
                        foreach (var tag in directory.Tags)
                        {
                            var tagName = tag.Name;
                            var description = tag.Description;

                            if (directory is FileMetadataDirectory && tag.Type == FileMetadataDirectory.TagFileModifiedDate)
                                description = "<omitted for regression testing as checkout dependent>";

                            writer.Write("[{0} - 0x{1:x4}] {2} = {3}{4}",
                                directoryName, tag.Type, tagName, description, NEW_LINE);
                        }

                        if (directory.TagCount != 0)
                            writer.Write(NEW_LINE);

                        // Special handling for XMP directory data
                        var xmpDirectory = directory as XmpDirectory;
                        if (xmpDirectory?.XmpMeta != null)
                        {
                            var wrote = false;

                            XmpIterator iterator = new XmpIterator((XmpMeta)xmpDirectory.XmpMeta, null, null, new IteratorOptions() { IsJustLeafNodes = true });

                            while (iterator.HasNext())
                            {
                                var prop = (IXmpPropertyInfo)iterator.Next();
                                var path = prop.Path;

                                if (path == null)
                                    continue;

                                var ns = prop.Namespace ?? "";
                                var value = prop.Value ?? "";

                                if (value.Length > 512)
                                    value = value.Substring(0, 512) + $" <truncated from {value.Length} characters>";
                                writer.Write($"[XMPMeta - {ns}] {path} = {value}{NEW_LINE}");
                                wrote = true;
                            }
                            if (wrote)
                                writer.Write(NEW_LINE);
                        }
                    }

                    // Write file structure
                    var tree = directories.ToLookup(d => d.Parent);

                    void WriteLevel(Directory? parent, int level)
                    {
                        const int indent = 4;

                        foreach (var child in tree[parent])
                        {
                            writer.Write(new string(' ', level * indent));
                            writer.Write($"- {child.Name}\n");
                            WriteLevel(child, level + 1);
                        }
                    }

                    WriteLevel(null, 0);

                    writer.Write(NEW_LINE);
                }
                finally
                {
                    writer.Write("Generated using metadata-extractor\n");
                    writer.Write("https://drewnoakes.com/code/exif/\n");
                }
            }
            catch (Exception e)
            {
                log.Write("Exception after extraction: {0}\n", e.Message);
            }
        }

        public override void OnExtractionError(string filePath, Exception exception, TextWriter log, long streamPosition)
        {
            base.OnExtractionError(filePath, exception, log, streamPosition);

            try
            {
                using var writer = OpenWriter(filePath);
                writer.Write("EXCEPTION: {0}\n", exception.Message);
                writer.Write('\n');
                writer.Write("Generated using metadata-extractor\n");
                writer.Write("https://drewnoakes.com/code/exif/\n");
            }
            catch (Exception e)
            {
                Console.Error.Write("Error writing exception details to metadata file: {0}\n", e);
            }
        }

        private static TextWriter OpenWriter(string filePath)
        {
            var directoryPath = Path.GetDirectoryName(filePath);
            Debug.Assert(directoryPath != null);
            var metadataPath = Path.Combine(
                Path.Combine(directoryPath, "metadata"),
                "dotnet");
            var fileName = Path.GetFileName(filePath);

            // Create the output directory if it doesn't exist
            if (!System.IO.Directory.Exists(metadataPath))
                System.IO.Directory.CreateDirectory(metadataPath);

            var outputPath = Path.Combine(metadataPath, $"{fileName}.txt");

            var stream = File.Open(outputPath, FileMode.Create);
            var writer = new StreamWriter(stream, new UTF8Encoding(false));
            writer.Write("FILE: {0}\n", fileName);

            // Detect file type
            using (var fileTypeDetectStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var fileType = FileTypeDetector.DetectFileType(fileTypeDetectStream);
                writer.Write("TYPE: {0}\n\n", fileType.ToString().ToUpper());
            }

            return writer;
        }
    }
}
