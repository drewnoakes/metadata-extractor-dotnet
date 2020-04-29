// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496
{
    internal sealed class BoxLocation
    {
        public uint Type { get; }

        /// <summary>
        /// Gets the offset within the file at which the box resides.
        /// </summary>
        public ulong Origin { get; }

        /// <summary>
        /// Gets the length of this box in bytes.
        /// </summary>
        public ulong Length { get; }

        /// <summary>
        /// Gets the offset within the file at which the next box resides.
        /// </summary>
        public ulong NextPosition => Origin + Length;

        /// <summary>
        /// Gets the four character code which identifies this box.
        /// </summary>
        public string TypeString => TypeStringConverter.ToTypeString(Type);

        public BoxLocation(SequentialReader reader)
        {
            Origin = (ulong)reader.Position;

            // Four bytes for the length
            Length = reader.GetUInt32();

            // Four bytes for the type (4CC, in ASCII)
            Type = reader.GetUInt32();

            // Handle special length values
            Length = Length switch
            {
                // A length of 0 means the box continues until the end of the stream
                0 => (ulong)reader.Available(), // Include 8 bytes for length/type

                // A length of 1 means the length is stored in 64 bits
                1 => reader.GetUInt64(),

                // The length is fine so keep it
                _ => Length
            };
        }
    }
}
