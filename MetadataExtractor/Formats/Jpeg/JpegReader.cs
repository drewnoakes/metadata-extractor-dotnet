#region License
//
// Copyright 2002-2017 Drew Noakes
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
using JetBrains.Annotations;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Reads SOF (Start of Frame) segment data.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Darrell Silver http://www.darrellsilver.com</author>
    public sealed class JpegReader : IJpegSegmentMetadataReader
    {
        ICollection<JpegSegmentType> IJpegSegmentMetadataReader.SegmentTypes => new HashSet<JpegSegmentType>
        {
            // NOTE that some SOFn values do not exist
            JpegSegmentType.Sof0, JpegSegmentType.Sof1, JpegSegmentType.Sof2, JpegSegmentType.Sof3,
            JpegSegmentType.Sof5, JpegSegmentType.Sof6, JpegSegmentType.Sof7, JpegSegmentType.Sof9,
            JpegSegmentType.Sof10, JpegSegmentType.Sof11, JpegSegmentType.Sof13, JpegSegmentType.Sof14,
            JpegSegmentType.Sof15
        };

#if NET35
        public IList<Directory> ReadJpegSegments(IEnumerable<JpegSegment> segments)
            => segments.Select(Extract).Cast<Directory>().ToList();
#else
        public IReadOnlyList<Directory> ReadJpegSegments(IEnumerable<JpegSegment> segments)
            => segments.Select(Extract).ToList();
#endif

        /// <summary>Reads JPEG SOF values and returns them in a <see cref="JpegDirectory"/>.</summary>
        [NotNull]
        public JpegDirectory Extract([NotNull] JpegSegment segment)
        {
            var directory = new JpegDirectory();

            // The value of TagCompressionType is determined by the segment type found
            directory.Set(JpegDirectory.TagCompressionType, (int)segment.Type - (int)JpegSegmentType.Sof0);

            SequentialReader reader = new SequentialByteArrayReader(segment.Bytes);

            try
            {
                directory.Set(JpegDirectory.TagDataPrecision, reader.GetByte());
                directory.Set(JpegDirectory.TagImageHeight, reader.GetUInt16());
                directory.Set(JpegDirectory.TagImageWidth, reader.GetUInt16());

                var componentCount = reader.GetByte();

                directory.Set(JpegDirectory.TagNumberOfComponents, componentCount);

                // For each component, there are three bytes of data:
                // 1 - Component ID: 1 = Y, 2 = Cb, 3 = Cr, 4 = I, 5 = Q
                // 2 - Sampling factors: bit 0-3 vertical, 4-7 horizontal
                // 3 - Quantization table number

                for (var i = 0; i < componentCount; i++)
                {
                    var componentId = reader.GetByte();
                    var samplingFactorByte = reader.GetByte();
                    var quantizationTableNumber = reader.GetByte();
                    var component = new JpegComponent(componentId, samplingFactorByte, quantizationTableNumber);
                    directory.Set(JpegDirectory.TagComponentData1 + i, component);
                }
            }
            catch (IOException ex)
            {
                directory.AddError(ex.Message);
            }

            return directory;
        }
    }
}
