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

using System.Collections.Generic;
using System.IO;
using System.Text;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Jfif
{
    /// <summary>Reader for JFIF data, found in the APP0 JPEG segment.</summary>
    /// <remarks>
    /// Reader for JFIF data, found in the APP0 JPEG segment.
    /// <para />
    /// More info at: http://en.wikipedia.org/wiki/JPEG_File_Interchange_Format
    /// </remarks>
    /// <author>Yuri Binev, Drew Noakes, Markus Meyer</author>
    public sealed class JfifReader : IJpegSegmentMetadataReader, IMetadataReader
    {
        public const string Preamble = "JFIF";

        public IEnumerable<JpegSegmentType> GetSegmentTypes()
        {
            yield return JpegSegmentType.App0;
        }

        public void ReadJpegSegments(IEnumerable<byte[]> segments, Metadata metadata, JpegSegmentType segmentType)
        {
            foreach (var segmentBytes in segments)
            {
                // Skip segments not starting with the required header
                if (segmentBytes.Length >= 4 && Preamble.Equals(Encoding.UTF8.GetString(segmentBytes, 0, Preamble.Length)))
                {
                    Extract(new ByteArrayReader(segmentBytes), metadata);
                }
            }
        }

        /// <summary>
        /// Performs the Jfif data extraction, adding found values to the specified instance of <see cref="Metadata"/>.
        /// </summary>
        public void Extract(IndexedReader reader, Metadata metadata)
        {
            var directory = new JfifDirectory();
            metadata.AddDirectory(directory);
            try
            {
                // For JFIF, the tag number is also the offset into the segment
                int ver = reader.GetUInt16(JfifDirectory.TagVersion);
                directory.Set(JfifDirectory.TagVersion, ver);
                int units = reader.GetUInt8(JfifDirectory.TagUnits);
                directory.Set(JfifDirectory.TagUnits, units);
                int height = reader.GetUInt16(JfifDirectory.TagResX);
                directory.Set(JfifDirectory.TagResX, height);
                int width = reader.GetUInt16(JfifDirectory.TagResY);
                directory.Set(JfifDirectory.TagResY, width);
            }
            catch (IOException me)
            {
                directory.AddError(me.Message);
            }
        }
    }
}
