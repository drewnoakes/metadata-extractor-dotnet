// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Jpeg;

/// <summary>Reader for JPEG Huffman tables, found in the DHT JPEG segment.</summary>
/// <seealso cref="JpegSegment"/>
/// <author>Nadahar</author>
/// <author>Kevin Mott https://github.com/kwhopper</author>
public sealed class JpegDhtReader : IJpegSegmentMetadataReader
{
    ICollection<JpegSegmentType> IJpegSegmentMetadataReader.SegmentTypes { get; } = new[] { JpegSegmentType.Dht };

    public IEnumerable<Directory> ReadJpegSegments(IEnumerable<JpegSegment> segments)
    {
        // This Extract structure is a little different since we only want
        // to return one HuffmanTablesDirectory for one-to-many segments
        HuffmanTablesDirectory? directory = null;

        foreach (var segment in segments)
        {
            directory ??= new HuffmanTablesDirectory();

            Extract(new SequentialByteArrayReader(segment.Bytes), directory);
        }

        if (directory is not null)
            return new List<Directory>() { directory };

        return Directory.EmptyList;
    }

    public void Extract(SequentialReader reader, HuffmanTablesDirectory directory)
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

    private static byte[] GetBytes(SequentialReader reader, int count)
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
