#region License
//
// Copyright 2002-2019 Drew Noakes
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
using JetBrains.Annotations;
using MetadataExtractor.IO;
#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Reader for JPEG Huffman tables, found in the DHT JPEG segment.</summary>
    /// <seealso cref="JpegSegment"/>
    /// <author>Nadahar</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public sealed class JpegDhtReader : IJpegSegmentMetadataReader
    {
        ICollection<JpegSegmentType> IJpegSegmentMetadataReader.SegmentTypes => new[] { JpegSegmentType.Dht };

        public DirectoryList ReadJpegSegments(IEnumerable<JpegSegment> segments)
        {
            // This Extract structure is a little different since we only want
            // to return one HuffmanTablesDirectory for one-to-many segments
            HuffmanTablesDirectory directory = null;

            foreach (var segment in segments)
            {
                if(directory == null)
                    directory = new HuffmanTablesDirectory();

                Extract(new SequentialByteArrayReader(segment.Bytes), directory);
            }

            if (directory != null)
                return new List<Directory>() { directory };

            return Directory.EmptyList;
        }

        public void Extract([NotNull] SequentialReader reader, HuffmanTablesDirectory directory)
        {
            try
            {
                while (reader.Available() > 0)
                {
                    byte header = reader.GetByte();
                    HuffmanTableClass tableClass = HuffmanTable.TypeOf((header & 0xF0) >> 4);
                    int tableDestinationId = header & 0xF;

                    byte[] lBytes = GetBytes(reader, 16);
                    int vCount = 0;
                    foreach (byte b in lBytes)
                    {
                        vCount += (b & 0xFF);
                    }
                    byte[] vBytes = GetBytes(reader, vCount);
                    directory.AddTable(new HuffmanTable(tableClass, tableDestinationId, lBytes, vBytes));
                }
            }
            catch (IOException me)
            {
                directory.AddError(me.ToString());
            }
        }

        private byte[] GetBytes([NotNull] SequentialReader reader, int count)
        {
            byte[] bytes = new byte[count];
            for (int i = 0; i < count; i++)
            {
                byte b = reader.GetByte();
                if (b == 0xFF)
                {
                    byte stuffing = reader.GetByte();
                    if (stuffing != 0x00)
                    {
                        throw new MetadataException("Marker " + (JpegSegmentType)stuffing + " found inside DHT segment");
                    }
                }
                bytes[i] = b;
            }
            return bytes;
        }
    }
}
