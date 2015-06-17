using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Directory = System.IO.Directory;

namespace FileLabeller
{
    /// <summary>
    /// Writes a text file containing the extracted metadata for each input file.
    /// </summary>
    internal class TextFileOutputHandler : FileHandlerBase
    {
        public override void OnStartingDirectory(string directoryPath)
        {
            base.OnStartingDirectory(directoryPath);
            Directory.Delete(Path.Combine(directoryPath, "metadata"), recursive: true);
        }

        public override void OnBeforeExtraction(string filePath, string relativePath, TextWriter log)
        {
            base.OnBeforeExtraction(filePath, relativePath, log);
            log.Write(filePath);
            log.Write('\n');
        }

        public override void OnExtractionSuccess(string filePath, IReadOnlyList<MetadataExtractor.Directory> directories, string relativePath, TextWriter log)
        {
            base.OnExtractionSuccess(filePath, directories, relativePath, log);

            try
            {
                using (var writer = OpenWriter(filePath))
                {
                    try
                    {
                        // Build a list of all directories
                        var directoryList = directories.ToList();

                        // Sort them by name
                        directoryList.Sort((o1, o2) => string.Compare(o1.Name, o2.Name, StringComparison.Ordinal));

                        // Write any errors
                        if (directoryList.Any(d => d.HasErrors))
                        {
                            foreach (var directory in directoryList)
                            {
                                if (!directory.HasErrors)
                                    continue;
                                foreach (var error in directory.Errors)
                                    writer.Write("[ERROR: {0}] {1}\n", directory.Name, error);
                            }
                            writer.Write('\n');
                        }

                        // Write tag values for each directory
                        foreach (var directory in directoryList)
                        {
                            var directoryName = directory.Name;
                            foreach (var tag in directory.Tags)
                            {
                                var tagName = tag.TagName;
                                var description = tag.Description;
                                writer.Write("[{0} - 0x{1:x4}] {2} = {3}\n",
                                    directoryName, tag.TagType, tagName, description);
                            }

                            if (directory.TagCount != 0)
                                writer.Write('\n');
                        }
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
            var metadataPath = Path.Combine(directoryPath, "metadata");
            var fileName = Path.GetFileName(filePath);

            // Create the output directory if it doesn't exist
            if (!Directory.Exists(metadataPath))
                Directory.CreateDirectory(metadataPath);

            var outputPath = string.Format("{0}/metadata/{1}.txt", directoryPath, fileName);

            var writer = new StreamWriter(outputPath, false);
            writer.Write("FILE: {0}\n\n", fileName);

            return writer;
        }
    }
}