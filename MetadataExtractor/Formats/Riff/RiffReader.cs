// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Riff
{
    /// <summary>
    /// Processes RIFF-formatted data, calling into client code via that <see cref="IRiffHandler"/> interface.
    /// </summary>
    /// <remarks>
    /// For information on this file format, see:
    /// <list type="bullet">
    ///   <item>http://en.wikipedia.org/wiki/Resource_Interchange_File_Format</item>
    ///   <item>https://developers.google.com/speed/webp/docs/riff_container</item>
    ///   <item>https://www.daubnet.com/en/file-format-riff</item>
    /// </list>
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class RiffReader
    {
        /// <summary>Processes a RIFF data sequence.</summary>
        /// <param name="reader">The <see cref="SequentialReader"/> from which the data should be read.</param>
        /// <param name="handler">The <see cref="IRiffHandler"/> that will coordinate processing and accept read values.</param>
        /// <exception cref="RiffProcessingException">An error occurred during the processing of RIFF data that could not be ignored or recovered from.</exception>
        /// <exception cref="System.IO.IOException">an error occurred while accessing the required data</exception>
        public void ProcessRiff(SequentialReader reader, IRiffHandler handler)
        {
            // RIFF files are always little-endian
            reader = reader.WithByteOrder(isMotorolaByteOrder: false);

            // PROCESS FILE HEADER

            var fileFourCc = reader.GetString(4, Encoding.ASCII);
            if (fileFourCc != "RIFF")
                throw new RiffProcessingException("Invalid RIFF header: " + fileFourCc);

            // The total size of the chunks that follow plus 4 bytes for the 'WEBP' or 'AVI ' FourCC
            int fileSize = reader.GetInt32();
            int sizeLeft = fileSize;
            string identifier = reader.GetString(4, Encoding.ASCII);
            sizeLeft -= 4;

            if (!handler.ShouldAcceptRiffIdentifier(identifier))
                return;

            ProcessChunks(reader, sizeLeft, handler);
        }

        // PROCESS CHUNKS
        public void ProcessChunks(SequentialReader reader, int sizeLeft, IRiffHandler handler)
        {
            // Processing chunks. Each chunk is 8 bytes header (4 bytes CC code + 4 bytes length of chunk) + data of the chunk

            while (reader.Position < sizeLeft)
            {
                // Check if end of the file is closer then 8 bytes
                if (reader.IsCloserToEnd(8)) return;

                string chunkFourCc = reader.GetString(4, Encoding.ASCII);
                int chunkSize = reader.GetInt32();

                sizeLeft -= 8;

                // NOTE we fail a negative chunk size here (greater than 0x7FFFFFFF) as we cannot allocate arrays larger than this
                if (chunkSize < 0 || sizeLeft < chunkSize)
                    throw new RiffProcessingException("Invalid RIFF chunk size");

                // Check if end of the file is closer then chunkSize bytes
                if (reader.IsCloserToEnd(chunkSize)) return;

                if (chunkFourCc == "LIST" || chunkFourCc == "RIFF")
                {
                    string listName = reader.GetString(4, Encoding.ASCII);
                    if (handler.ShouldAcceptList(listName))
                        ProcessChunks(reader, sizeLeft - 4, handler);
                    else
                        reader.Skip(sizeLeft - 4);
                    sizeLeft -= chunkSize;
                }
                else
                {
                    if (handler.ShouldAcceptChunk(chunkFourCc))
                    {
                        // TODO is it feasible to avoid copying the chunk here, and to pass the sequential reader to the handler?
                        handler.ProcessChunk(chunkFourCc, reader.GetBytes(chunkSize));
                    }
                    else
                    {
                        reader.Skip(chunkSize);
                    }

                    sizeLeft -= chunkSize;

                    // Skip any padding byte added to keep chunks aligned to even numbers of bytes
                    if (chunkSize % 2 == 1)
                    {
                        reader.Skip(1);
                        sizeLeft--;
                    }
                }
            }
        }
    }
}
