// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Apple;

/// <summary>
/// A limited-functionality binary property list (BPLIST) reader.
/// </summary>
public sealed class BplistReader
{
    // https://opensource.apple.com/source/CF/CF-550/ForFoundationOnly.h
    // https://opensource.apple.com/source/CF/CF-550/CFBinaryPList.c
    // https://synalysis.com/how-to-decode-apple-binary-property-list-files/

    /// <summary>
    /// Gets whether <paramref name="bplist"/> starts with the expected header bytes.
    /// </summary>
    public static bool IsValid(ReadOnlySpan<byte> bplist)
    {
        return bplist.StartsWith("bplist00"u8);
    }

    /// <summary>Parses a binary property list with one- or two-byte offsets and one-byte object references.</summary>
    /// <exception cref="IOException">The trailer or object offsets are invalid or use unsupported widths.</exception>
    public static PropertyListResults Parse(ReadOnlySpan<byte> bplist)
    {
        if (!IsValid(bplist))
        {
            throw new ArgumentException("Input is not a bplist.", nameof(bplist));
        }

        if (bplist.Length < 8 + Trailer.SizeBytes)
            throw new IOException("Binary plist trailer is missing.");

        Trailer trailer = ReadTrailer(bplist);
        var trailerOffset = bplist.Length - Trailer.SizeBytes;

        if (trailer.OffsetIntSize is not (1 or 2) || trailer.ObjectRefSize != 1)
            throw new IOException("Unsupported binary plist offset or object reference size.");
        if (trailer.OffsetTableOffset < 8 || trailer.OffsetTableOffset > trailerOffset)
            throw new IOException("Binary plist offset table is outside the object data.");
        if (trailer.NumObjects <= 0 ||
            trailer.NumObjects > (trailerOffset - trailer.OffsetTableOffset) / trailer.OffsetIntSize)
            throw new IOException("Binary plist object count exceeds the offset table.");
        if (trailer.TopObject < 0 || trailer.TopObject >= trailer.NumObjects)
            throw new IOException("Binary plist top object is outside the object table.");

        int offset = (int)trailer.OffsetTableOffset;
        var reader = new BufferReader(bplist.Slice(offset, trailerOffset - offset), isBigEndian: true);

        var offsets = new int[(int)trailer.NumObjects];

        for (int i = 0; i < offsets.Length; i++)
        {
            if (trailer.OffsetIntSize == 1)
            {
                offsets[i] = reader.GetByte();
            }
            else if (trailer.OffsetIntSize == 2)
            {
                offsets[i] = reader.GetUInt16();
            }

            if (offsets[i] < 8 || offsets[i] >= offset)
                throw new IOException("Binary plist object offset is outside the object data.");
        }

        List<object> objects = [];

        for (int i = 0; i < offsets.Length; i++)
        {
            reader = new BufferReader(bplist.Slice(offsets[i], offset - offsets[i]), isBigEndian: true);

            byte b = reader.GetByte();

            byte objectFormat = (byte)((b >> 4) & 0x0F);
            byte marker = (byte)(b & 0x0F);

            object obj = objectFormat switch
            {
                // dict
                0x0D => HandleDict(ref reader, marker),
                // string (ASCII)
                0x05 => reader.GetString(bytesRequested: marker & 0x0F, Encoding.ASCII),
                // data
                0x04 => HandleData(ref reader, marker),
                // int
                0x01 => HandleInt(ref reader, marker),
                // unknown
                _ => throw new NotSupportedException($"Unsupported object format {objectFormat:X2}.")
            };

            objects.Add(obj);
        }

        return new PropertyListResults(objects, trailer);

        static Trailer ReadTrailer(ReadOnlySpan<byte> bplist)
        {
            var reader = new BufferReader(bplist.Slice(bplist.Length - Trailer.SizeBytes), isBigEndian: true);

            // Skip 5-byte unused values, 1-byte sort version.
            reader.Skip(5 + 1);

            return new Trailer
            {
                OffsetIntSize = reader.GetByte(),
                ObjectRefSize = reader.GetByte(),
                NumObjects = reader.GetInt64(),
                TopObject = reader.GetInt64(),
                OffsetTableOffset = reader.GetInt64()
            };
        }

        static object HandleInt(ref BufferReader reader, byte marker)
        {
            return marker switch
            {
                0 => (object)reader.GetByte(),
                1 => reader.GetInt16(),
                2 => reader.GetInt32(),
                3 => reader.GetInt64(),
                _ => throw new NotSupportedException($"Unsupported int size {marker}.")
            };
        }

        static Dictionary<byte, byte> HandleDict(ref BufferReader reader, byte count)
        {
            Span<byte> keyRefs = stackalloc byte[count]; // count is a byte (0-255)

            reader.GetBytes(keyRefs);

            Dictionary<byte, byte> map = [];

            for (int j = 0; j < count; j++)
            {
                map.Add(keyRefs[j], reader.GetByte());
            }

            return map;
        }

        object HandleData(ref BufferReader reader, byte marker)
        {
            int byteCount = marker;

            if (marker == 0x0F)
            {
                byte sizeMarker = reader.GetByte();

                if (((sizeMarker >> 4) & 0x0F) != 1)
                {
                    throw new NotSupportedException($"Invalid size marker {sizeMarker:X2}.");
                }

                int sizeType = sizeMarker & 0x0F;

                if (sizeType == 0)
                {
                    byteCount = reader.GetByte();
                }
                else if (sizeType == 1)
                {
                    byteCount = reader.GetUInt16();
                }
            }

            return reader.GetBytes(byteCount);
        }
    }

    public sealed class PropertyListResults
    {
        private readonly List<object> _objects;
        private readonly Trailer _trailer;

        internal PropertyListResults(List<object> objects, Trailer trailer)
        {
            _objects = objects;
            _trailer = trailer;
        }

        public Dictionary<byte, byte>? GetTopObject()
        {
            return _objects[checked((int)_trailer.TopObject)] as Dictionary<byte, byte>;
        }

        public object Get(byte key)
        {
            return _objects[key];
        }
    }

    internal class Trailer
    {
        public const int SizeBytes = 32;
        public byte OffsetIntSize { get; init; }
        public byte ObjectRefSize { get; init; }
        public long NumObjects { get; init; }
        public long TopObject { get; init; }
        public long OffsetTableOffset { get; init; }
    }
}
