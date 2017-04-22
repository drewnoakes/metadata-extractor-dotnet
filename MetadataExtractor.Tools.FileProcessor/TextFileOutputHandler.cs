#region License
//
// Copyright 2002-2017 Drew Noakes
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
using System.Text;
using JetBrains.Annotations;
using MetadataExtractor.Formats.FileSystem;
using MetadataExtractor.Formats.Xmp;
using MetadataExtractor.Util;

namespace MetadataExtractor.Tools.FileProcessor
{
    /// <summary>
    /// Writes a text file containing the extracted metadata for each input file.
    /// </summary>
    internal class TextFileOutputHandler : FileHandlerBase
    {
        public override void OnStartingDirectory(string directoryPath)
        {
            base.OnStartingDirectory(directoryPath);
            System.IO.Directory.Delete(Path.Combine(directoryPath, "metadata"), recursive: true);
        }

        public override void OnBeforeExtraction(string filePath, string relativePath, TextWriter log)
        {
            base.OnBeforeExtraction(filePath, relativePath, log);
            log.Write(filePath);
            log.Write('\n');
        }

        public override void OnExtractionSuccess(string filePath, IList<Directory> directories, string relativePath, TextWriter log)
        {
            base.OnExtractionSuccess(filePath, directories, relativePath, log);

            try
            {
                using (var writer = OpenWriter(filePath))
                {
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
                            writer.Write('\n');
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

                                writer.Write("[{0} - 0x{1:x4}] {2} = {3}\n",
                                    directoryName, tag.Type, tagName, description);
                            }

                            if (directory.TagCount != 0)
                                writer.Write('\n');

                            var xmpDirectory = directory as XmpDirectory;
                            if (xmpDirectory?.XmpMeta != null)
                            {
                                var wrote = false;
                                foreach (var prop in xmpDirectory.XmpMeta.Properties)
                                {
                                    var value = prop.Value;
                                    if (value?.Length > 512)
                                        value = value.Substring(0, 512) + $" <truncated from {value.Length} characters>";
                                    writer.WriteLine($"[XMPMeta - {prop.Namespace}] {prop.Path} = {value}");
                                    wrote = true;
                                }
                                if (wrote)
                                    writer.Write('\n');
                            }
                        }

                        // Write file structure
                        var tree = directories.ToLookup(d => d.Parent);

                        void WriteLevel(Directory parent, int level)
                        {
                            const int indent = 4;

                            foreach (var child in tree[parent])
                            {
                                writer.Write(new string(' ', level*indent));
                                writer.Write($"- {child.Name}\n");
                                WriteLevel(child, level + 1);
                            }
                        }

                        WriteLevel(null, 0);

                        writer.Write('\n');
                    }
                    finally
                    {
                        writer.Write("Generated using metadata-extractor\n");
                        writer.Write("https://drewnoakes.com/code/exif/\n");
                    }
                }
            }
            catch (Exception e)
            {
                log.Write("Exception after extraction: {0}\n", e.Message);
            }
        }

        public override void OnExtractionError(string filePath, Exception exception, TextWriter log)
        {
            base.OnExtractionError(filePath, exception, log);

            try
            {
                using (var writer = OpenWriter(filePath))
                {
                    writer.Write("EXCEPTION: {0}\n", exception.Message);
                    writer.Write('\n');
                    writer.Write("Generated using metadata-extractor\n");
                    writer.Write("https://drewnoakes.com/code/exif/\n");
                }
            }
            catch (Exception e)
            {
                Console.Error.Write("Error writing exception details to metadata file: {0}\n", e);
            }
        }

        [NotNull]
        private static TextWriter OpenWriter(string filePath)
        {
            var directoryPath = Path.GetDirectoryName(filePath);
            Debug.Assert(directoryPath != null);
            var metadataPath = Path.Combine(directoryPath, "metadata");
            var fileName = Path.GetFileName(filePath);

            // Create the output directory if it doesn't exist
            if (!System.IO.Directory.Exists(metadataPath))
                System.IO.Directory.CreateDirectory(metadataPath);

            var outputPath = $"{directoryPath}/metadata/{fileName}.txt";

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