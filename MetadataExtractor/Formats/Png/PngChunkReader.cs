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
using System.Linq;
using JetBrains.Annotations;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PngChunkReader
    {
        private static readonly byte[] _pngSignatureBytes = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

        /// <exception cref="PngProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public IEnumerable<PngChunk> Extract([NotNull] SequentialReader reader, [CanBeNull] ICollection<PngChunkType> desiredChunkTypes)
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
