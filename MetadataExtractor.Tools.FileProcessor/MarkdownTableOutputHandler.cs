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
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Exif;

namespace MetadataExtractor.Tools.FileProcessor
{
    /// <summary>
    /// Creates a table describing sample images using Wiki markdown.
    /// <para/>
    /// Output hosted at: https://github.com/drewnoakes/metadata-extractor-images/wiki/ContentSummary
    /// </summary>
    internal class MarkdownTableOutputHandler : FileHandlerBase
    {
        // TODO this should be modelled centrally
        private readonly Dictionary<string, string> _extensionEquivalence = new Dictionary<string, string> { { "jpeg", "jpg" }, { "tiff", "tif" } };
        private readonly Dictionary<string, List<Row>> _rowsByExtension = new Dictionary<string, List<Row>>();

        private class Row
        {
            public string FilePath { get; }
            public string RelativePath { get; }
            public int DirectoryCount { get; }
            [CanBeNull] public string Manufacturer { get; }
            [CanBeNull] public string Model { get; }
            [CanBeNull] public string ExifVersion { get; }
            [CanBeNull] public string Thumbnail { get; }
            [CanBeNull] public string Makernote { get; }

            internal Row(string filePath, ICollection<Directory> directories, string relativePath)
            {
                FilePath = filePath;
                RelativePath = relativePath;
                DirectoryCount = directories.Count;

                var ifd0Dir = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
                var subIfdDir = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                var thumbDir = directories.OfType<ExifThumbnailDirectory>().FirstOrDefault();

                if (ifd0Dir != null)
                {
                    Manufacturer = ifd0Dir.GetDescription(ExifDirectoryBase.TagMake);
                    Model = ifd0Dir.GetDescription(ExifDirectoryBase.TagModel);
                }

                var hasMakernoteData = false;
                if (subIfdDir != null)
                {
                    ExifVersion = subIfdDir.GetDescription(ExifDirectoryBase.TagExifVersion);
                    hasMakernoteData = subIfdDir.ContainsTag(ExifDirectoryBase.TagMakernote);
                }

                if (thumbDir != null)
                {
                    Thumbnail = thumbDir.TryGetInt32(ExifDirectoryBase.TagImageWidth, out int width) &&
                                thumbDir.TryGetInt32(ExifDirectoryBase.TagImageHeight, out int height)
                        ? $"Yes ({width} x {height})"
                        : "Yes";
                }

                foreach (var directory in directories)
                {
                    if (directory.GetType().Name.Contains("Makernote"))
                    {
                        Makernote = directory.Name.Replace("Makernote", "").Trim();
                        break;
                    }
                }

                if (Makernote == null)
                    Makernote = hasMakernoteData ? "(Unknown)" : "N/A";
            }
        }

        public override void OnExtractionSuccess(string filePath, IList<Directory> directories, string relativePath, TextWriter log)
        {
            base.OnExtractionSuccess(filePath, directories, relativePath, log);

            var extension = Path.GetExtension(filePath);

            if (extension == string.Empty)
                return;

            // Sanitise the extension
            extension = extension.ToLower();
            if (_extensionEquivalence.ContainsKey(extension))
                extension = _extensionEquivalence[extension];

            if (!_rowsByExtension.TryGetValue(extension, out List<Row> rows))
            {
                rows = new List<Row>();
                _rowsByExtension[extension] = rows;
            }

            rows.Add(new Row(filePath, directories, relativePath));
        }

        public override void OnScanCompleted(TextWriter log)
        {
            base.OnScanCompleted(log);

            using (var stream = File.OpenWrite("ContentSummary.md"))
            using (var writer = new StreamWriter(stream))
                WriteOutput(writer);
        }

        private void WriteOutput(TextWriter writer)
        {
            writer.WriteLine("# Image Database Summary");
            writer.WriteLine();

            foreach (var extension in _rowsByExtension.Keys)
            {
                writer.WriteLine($"## {extension.ToUpper()} Files");
                writer.WriteLine();

                writer.Write("File|Manufacturer|Model|Dir Count|Exif?|Makernote|Thumbnail|All Data\n");
                writer.Write("----|------------|-----|---------|-----|---------|---------|--------\n");

                var rows = _rowsByExtension[extension];

                // Order by manufacturer, then model
                rows.Sort((o1, o2) =>
                {
                    var c1 = string.CompareOrdinal(o1.Manufacturer, o2.Manufacturer);
                    return c1 != 0
                        ? c1
                        : string.CompareOrdinal(o1.Model, o2.Model);
                });

                foreach (var row in rows)
                {
                    var fileName = Path.GetFileName(row.FilePath);
                    var urlEncodedFileName = Uri.EscapeDataString(fileName).Replace("%20", "+");

                    writer.WriteLine(
                        "[{0}](https://raw.githubusercontent.com/drewnoakes/metadata-extractor-images/master/{1}/{2})|{3}|{4}|{5}|{6}|{7}|{8}|[metadata](https://raw.githubusercontent.com/drewnoakes/metadata-extractor-images/master/{9}/metadata/{10}.txt)",
                        fileName,
                        row.RelativePath,
                        urlEncodedFileName,
                        row.Manufacturer,
                        row.Model,
                        row.DirectoryCount,
                        row.ExifVersion,
                        row.Makernote,
                        row.Thumbnail,
                        row.RelativePath,
                        urlEncodedFileName.ToLower());
                }

                writer.WriteLine();
            }
        }
    }
}