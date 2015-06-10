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
using System.Linq;
using System.Reflection;
using Com.Drew.Imaging.Jpeg;
using Sharpen;

namespace MetadataExtractor.Tools.ExtractJpegSegments
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
                var segmentType = JpegSegmentType.ValueOf(args[i].ToUpper());
                if (!segmentType.CanContainMetadata)
                {
                    Console.Error.WriteLine("WARNING: Segment type {0} cannot contain metadata so it may not be necessary to extract it", segmentType);
                }
                segmentTypes.Add(segmentType);
            }
            if (segmentTypes.Count == 0)
            {
                // If none specified, use all that could reasonably contain metadata
                Collections.AddAll(segmentTypes, JpegSegmentType.CanContainMetadataTypes);
            }
            Console.Out.WriteLine("Reading: {0}", filePath);
            var segmentData = JpegSegmentReader.ReadSegments(filePath, segmentTypes);
            SaveSegmentFiles(filePath, segmentData);
        }

        /// <exception cref="System.IO.IOException"/>
        public static void SaveSegmentFiles(string jpegFilePath, JpegSegmentData segmentData)
        {
            foreach (var segmentType in segmentData.GetSegmentTypes())
            {
                IList<byte[]> segments = segmentData.GetSegments(segmentType).ToList();
                if (segments.Count == 0)
                {
                    continue;
                }
                if (segments.Count > 1)
                {
                    for (var i = 0; i < segments.Count; i++)
                    {
                        var outputFilePath = string.Format("{0}.{1}.{2}", jpegFilePath, segmentType.ToString().ToLower(), i);
                        Console.Out.WriteLine((object)("Writing: " + outputFilePath));
                        File.WriteAllBytes(outputFilePath, segments[i]);
                    }
                }
                else
                {
                    var outputFilePath = string.Format("{0}.{1}", jpegFilePath, segmentType.ToString().ToLower());
                    Console.Out.WriteLine((object)("Writing: " + outputFilePath));
                    File.WriteAllBytes(outputFilePath, segments[0]);
                }
            }
        }

        private static void PrintUsage()
        {
            Console.Out.WriteLine("USAGE:\n");
            Console.Out.WriteLine("\t{0} <filename> [<segment> ...]\n", Assembly.GetExecutingAssembly().GetName().Name);
            Console.Out.Write("Where <segment> is zero or more of:");
            foreach (var segmentType in typeof(JpegSegmentType).GetEnumConstants<JpegSegmentType>())
            {
                if (segmentType.CanContainMetadata)
                {
                    Console.Out.Write(" " + segmentType.ToString());
                }
            }
            Console.Out.WriteLine();
        }
    }
}
