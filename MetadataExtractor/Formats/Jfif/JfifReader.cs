#region License
//
// Copyright 2002-2015 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Jfif
{
    /// <summary>Reads JFIF (JPEG File Interchange Format) data.</summary>
    /// <remarks>
    /// JFIF is found in JPEG APP0 segments.
    /// <para />
    /// More info at: http://en.wikipedia.org/wiki/JPEG_File_Interchange_Format
    /// </remarks>
    /// <author>Yuri Binev, Drew Noakes, Markus Meyer</author>
    public sealed class JfifReader : IJpegSegmentMetadataReader
    {
        private const string Preamble = "JFIF";

        public IEnumerable<JpegSegmentType> GetSegmentTypes()
        {
            yield return JpegSegmentType.App0;
        }

        public IReadOnlyList<Directory> ReadJpegSegments(IEnumerable<byte[]> segments, JpegSegmentType segmentType)
        {
            // Skip segments not starting with the required header
            return segments
                .Where(segment => segment.Length >= 4 && Preamble == Encoding.ASCII.GetString(segment, 0, Preamble.Length))
                .Select(segment => Extract(new ByteArrayReader(segment)))
                .ToList();
        }

        /// <summary>Reads JFIF values and returns them in an <see cref="JfifDirectory"/>.</summary>
        public JfifDirectory Extract(IndexedReader reader)
        {
            var directory = new JfifDirectory();

            try
            {
                // For JFIF the tag number is the value's byte offset
                int ver = reader.GetUInt16(JfifDirectory.TagVersion);
                directory.Set(JfifDirectory.TagVersion, ver);
                int units = reader.GetByte(JfifDirectory.TagUnits);
                directory.Set(JfifDirectory.TagUnits, units);
                int height = reader.GetUInt16(JfifDirectory.TagResX);
                directory.Set(JfifDirectory.TagResX, height);
                int width = reader.GetUInt16(JfifDirectory.TagResY);
                directory.Set(JfifDirectory.TagResY, width);
            }
            catch (IOException e)
            {
                directory.AddError(e.Message);
            }

            return directory;
        }
    }
}
