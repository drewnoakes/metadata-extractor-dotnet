// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496
{
    public class BoxLocation
    {
        public uint Type { get; }
        public ulong Origin { get; }
        public ulong Length { get; }
        public ulong NextPosition => Origin + Length;

        public ulong BytesLeft(SequentialReader sr) => NextPosition - (ulong)sr.Position;

        public bool DoneReading(SequentialReader sr) => (ulong)sr.Position >= NextPosition;

        public BoxLocation(SequentialReader reader)
        {
            Origin = (ulong)reader.Position;
            Length = reader.GetUInt32();
            Type = reader.GetUInt32();
            Length = Length switch
            {
                1 => reader.GetUInt64(),
                0 => (ulong)(reader.Available() + 8), // 8 is for the two words already consumed.
                _ => Length
            };
        }
    }
}
