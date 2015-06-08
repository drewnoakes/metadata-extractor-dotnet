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
using System.IO;
using Com.Drew.Imaging.Jpeg;
using Com.Drew.Lang;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Jpeg
{
    /// <summary>
    /// Decodes JPEG SOFn data, populating a
    /// <see cref="Com.Drew.Metadata.Metadata"/>
    /// object with tag values in a
    /// <see cref="JpegDirectory"/>
    /// .
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Darrell Silver http://www.darrellsilver.com</author>
    public class JpegReader : JpegSegmentMetadataReader
    {
        [NotNull]
        public virtual Iterable<JpegSegmentType> GetSegmentTypes()
        {
            // NOTE that some SOFn values do not exist
            return Arrays.AsList(JpegSegmentType.Sof0, JpegSegmentType.Sof1, JpegSegmentType.Sof2, JpegSegmentType.Sof3, JpegSegmentType.Sof5, JpegSegmentType.Sof6, JpegSegmentType.Sof7, JpegSegmentType.Sof8, JpegSegmentType.Sof9, JpegSegmentType.Sof10, 
                JpegSegmentType.Sof11, JpegSegmentType.Sof13, JpegSegmentType.Sof14, JpegSegmentType.Sof15).AsIterable();
        }

        //            JpegSegmentType.SOF4,
        //            JpegSegmentType.SOF12,
        public virtual void ReadJpegSegments([NotNull] Iterable<sbyte[]> segments, [NotNull] Com.Drew.Metadata.Metadata metadata, [NotNull] JpegSegmentType segmentType)
        {
            foreach (sbyte[] segmentBytes in segments)
            {
                Extract(segmentBytes, metadata, segmentType);
            }
        }

        public virtual void Extract(sbyte[] segmentBytes, Com.Drew.Metadata.Metadata metadata, JpegSegmentType segmentType)
        {
            JpegDirectory directory = new JpegDirectory();
            metadata.AddDirectory(directory);
            // The value of TAG_COMPRESSION_TYPE is determined by the segment type found
            directory.SetInt(JpegDirectory.TagCompressionType, segmentType.byteValue - JpegSegmentType.Sof0.byteValue);
            SequentialReader reader = new SequentialByteArrayReader(segmentBytes);
            try
            {
                directory.SetInt(JpegDirectory.TagDataPrecision, reader.GetUInt8());
                directory.SetInt(JpegDirectory.TagImageHeight, reader.GetUInt16());
                directory.SetInt(JpegDirectory.TagImageWidth, reader.GetUInt16());
                short componentCount = reader.GetUInt8();
                directory.SetInt(JpegDirectory.TagNumberOfComponents, componentCount);
                // for each component, there are three bytes of data:
                // 1 - Component ID: 1 = Y, 2 = Cb, 3 = Cr, 4 = I, 5 = Q
                // 2 - Sampling factors: bit 0-3 vertical, 4-7 horizontal
                // 3 - Quantization table number
                for (int i = 0; i < (int)componentCount; i++)
                {
                    int componentId = reader.GetUInt8();
                    int samplingFactorByte = reader.GetUInt8();
                    int quantizationTableNumber = reader.GetUInt8();
                    JpegComponent component = new JpegComponent(componentId, samplingFactorByte, quantizationTableNumber);
                    directory.SetObject(JpegDirectory.TagComponentData1 + i, component);
                }
            }
            catch (IOException ex)
            {
                directory.AddError(ex.Message);
            }
        }
    }
}
