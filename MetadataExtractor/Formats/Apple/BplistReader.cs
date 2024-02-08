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

    private static ReadOnlySpan<byte> BplistHeader => "bplist00"u8;

    /// <summary>
    /// Gets whether <paramref name="bplist"/> starts with the expected header bytes.
    /// </summary>
    public static bool IsValid(ReadOnlySpan<byte> bplist)
    {
        if (bplist.Length < BplistHeader.Length)
        {
            return false;
        }

        for (int i = 0; i < BplistHeader.Length; i++)
        {
            if (bplist[i] != BplistHeader[i])
            {
                return false;
            }
        }

        return true;
    }

    public static PropertyListResults Parse(ReadOnlySpan<byte> bplist)
    {
        if (!IsValid(bplist))
        {
            throw new ArgumentException("Input is not a bplist.", nameof(bplist));
        }

        Trailer trailer = ReadTrailer(bplist);

        int offset = checked((int)(trailer.OffsetTableOffset + trailer.TopObject));
        var reader = new BufferReader(bplist.Slice(offset), isBigEndian: true);

        int[] offsets = new int[(int)trailer.NumObjects];

        for (int i = 0; i < (int)trailer.NumObjects; i++)
        {
            if (trailer.OffsetIntSize == 1)
            {
                offsets[i] = reader.GetByte();
            }
            else if (trailer.OffsetIntSize == 2)
            {
                offsets[i] = reader.GetUInt16();
            }
        }

        List<object> objects = [];

        for (int i = 0; i < offsets.Length; i++)
        {
            reader = new BufferReader(bplist.Slice(offsets[i]), isBigEndian: true);

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
            Span<byte> keyRefs = stackalloc byte[count]; // count is < 256

            for (int j = 0; j < count; j++)
            {
                keyRefs[j] = reader.GetByte();
            }

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
