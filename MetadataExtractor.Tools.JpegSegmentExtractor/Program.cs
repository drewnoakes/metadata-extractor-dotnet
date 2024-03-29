﻿// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Jpeg;

namespace MetadataExtractor.Tools.JpegSegmentExtractor
{
    /// <summary>Extracts JPEG segments and writes them to individual files.</summary>
    /// <remarks>
    /// Extracting only the required segment(s) for use in unit testing has several benefits:
    ///
    /// <list type="bullet">
    ///   <item>Helps reduce the repository size. For example a small JPEG image may still be 20kB+ in size, yet its
    ///   APPD (IPTC) segment may be as small as 200 bytes.</item>
    ///   <item>Makes unit tests run more rapidly.</item>
    ///   <item>Partially anonymises user-contributed data by removing image portions.</item>
    /// </list>
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                PrintUsage();
                Environment.Exit(1);
            }
            var filePath = args[0];
            if (!File.Exists(filePath))
            {
                Console.Error.WriteLine("File does not exist");
                PrintUsage();
                Environment.Exit(1);
            }
            ICollection<JpegSegmentType> segmentTypes = new HashSet<JpegSegmentType>();
            for (var i = 1; i < args.Length; i++)
            {
                var segmentType = (JpegSegmentType)Enum.Parse(typeof(JpegSegmentType), args[i], ignoreCase: true);
                if (!segmentType.CanContainMetadata())
                {
                    Console.Error.WriteLine("WARNING: Segment type {0} cannot contain metadata so it may not be necessary to extract it", segmentType);
                }
                segmentTypes.Add(segmentType);
            }
            if (segmentTypes.Count == 0)
            {
                // If none specified, use all that could reasonably contain metadata
                foreach (var segmentType in JpegSegmentTypeExtensions.CanContainMetadataTypes)
                    segmentTypes.Add(segmentType);
            }
            Console.Out.WriteLine("Reading: {0}", filePath);
            using var stream = File.OpenRead(filePath);
            var segmentData = JpegSegmentReader.ReadSegments(new SequentialStreamReader(stream), segmentTypes);
            SaveSegmentFiles(filePath, segmentData);
        }

        private static void SaveSegmentFiles(string jpegFilePath, IEnumerable<JpegSegment> segments)
        {
            var segmentsByType = segments.ToLookup(s => s.Type);

            foreach (var segmentGroup in segmentsByType)
            {
                var segmentType = segmentGroup.Key;
                var segmentsOfType = segmentGroup.ToList();

                if (segmentsOfType.Count == 0)
                    continue;

                var format = segmentsOfType.Count > 1 ? "{0}.{1}.{2}" : "{0}.{1}";

                var i = 0;
                foreach (var segment in segmentsOfType)
                {
                    var outputFilePath = string.Format(format, jpegFilePath, segmentType.ToString().ToLower(), i++);

                    Console.Out.WriteLine($"Writing: {outputFilePath} (offset {segment.Offset}, length {segment.Bytes.Length})");
                    File.WriteAllBytes(outputFilePath, segment.Bytes);
                }
            }
        }

        private static void PrintUsage()
        {
            Console.Out.WriteLine("USAGE:");
            Console.Out.WriteLine();
            Console.Out.WriteLine("    {0} <filename> [<segment> ...]", Path.GetFileName(Process.GetCurrentProcess().MainModule?.FileName));
            Console.Out.WriteLine();
            Console.Out.Write("Where <segment> is zero or more of:");
            foreach (var segmentType in JpegSegmentTypeExtensions.CanContainMetadataTypes)
                Console.Out.Write(" " + segmentType.ToString().ToUpper());
            Console.Out.WriteLine();
        }
    }
}
