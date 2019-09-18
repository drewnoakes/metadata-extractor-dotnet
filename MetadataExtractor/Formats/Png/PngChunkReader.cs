// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PngChunkReader
    {
        private static readonly byte[] _pngSignatureBytes = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

        /// <exception cref="PngProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        public IEnumerable<PngChunk> Extract(SequentialReader reader, ICollection<PngChunkType>? desiredChunkTypes)
        {
            //
            // PNG DATA STREAM
            //
            // Starts with a PNG SIGNATURE, followed by a sequence of CHUNKS.
            //
            // PNG SIGNATURE
            //
            //   Always composed of these bytes: 89 50 4E 47 0D 0A 1A 0A
            //
            // CHUNK
            //
            //   4 - length of the data field (unsigned, but always within 31 bytes), may be zero
            //   4 - chunk type, restricted to [65,90] and [97,122] (A-Za-z)
            //   * - data field
            //   4 - CRC calculated from chunk type and chunk data, but not length
            //
            // CHUNK TYPES
            //
            //   Critical Chunk Types:
            //
            //     IHDR - image header, always the first chunk in the data stream
            //     PLTE - palette table, associated with indexed PNG images
            //     IDAT - image data chunk, of which there may be many
            //     IEND - image trailer, always the last chunk in the data stream
            //
            //   Ancillary Chunk Types:
            //
            //     Transparency information:  tRNS
            //     Colour space information:  cHRM, gAMA, iCCP, sBIT, sRGB
            //     Textual information:       iTXt, tEXt, zTXt
            //     Miscellaneous information: bKGD, hIST, pHYs, sPLT
            //     Time information:          tIME
            //

            // network byte order
            reader = reader.WithByteOrder(isMotorolaByteOrder: true);

            if (!_pngSignatureBytes.SequenceEqual(reader.GetBytes(_pngSignatureBytes.Length)))
                throw new PngProcessingException("PNG signature mismatch");

            var seenImageHeader = false;
            var seenImageTrailer = false;
            var chunks = new List<PngChunk>();
            var seenChunkTypes = new HashSet<PngChunkType>();

            while (!seenImageTrailer)
            {
                // Process the next chunk.
                var chunkDataLength = reader.GetInt32();
                if (chunkDataLength < 0)
                    throw new PngProcessingException("PNG chunk length exceeds maximum");
                var chunkType = new PngChunkType(reader.GetBytes(4));
                var willStoreChunk = desiredChunkTypes == null || desiredChunkTypes.Contains(chunkType);
                var chunkData = reader.GetBytes(chunkDataLength);

                // Skip the CRC bytes at the end of the chunk
                // TODO consider verifying the CRC value to determine if we're processing bad data
                reader.Skip(4);

                if (willStoreChunk && seenChunkTypes.Contains(chunkType) && !chunkType.AreMultipleAllowed)
                    throw new PngProcessingException($"Observed multiple instances of PNG chunk '{chunkType}', for which multiples are not allowed");

                if (chunkType.Equals(PngChunkType.IHDR))
                    seenImageHeader = true;
                else if (!seenImageHeader)
                    throw new PngProcessingException($"First chunk should be '{PngChunkType.IHDR}', but '{chunkType}' was observed");

                if (chunkType.Equals(PngChunkType.IEND))
                    seenImageTrailer = true;

                if (willStoreChunk)
                    chunks.Add(new PngChunk(chunkType, chunkData));

                seenChunkTypes.Add(chunkType);
            }

            return chunks;
        }
    }
}
