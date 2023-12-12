// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Riff;

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
    public void ProcessRiff(SequentialReader reader, IRiffHandler handler)
    {
        try
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

            var maxPosition = reader.Position + sizeLeft;
            ProcessChunks(reader, maxPosition, handler);
        }
        catch (Exception e) when (e is ImageProcessingException or IOException)
        {
            handler.AddError(e.Message);
        }
    }

    // PROCESS CHUNKS
    private static void ProcessChunks(SequentialReader reader, long maxPosition, IRiffHandler handler)
    {
        // Processing chunks. Each chunk is 8 bytes header (4 bytes CC code + 4 bytes length of chunk) + data of the chunk

        while (reader.Position < maxPosition - 8)
        {
            string chunkFourCc = reader.GetString(4, Encoding.ASCII);
            int chunkSize = reader.GetInt32();

            // NOTE we fail a negative chunk size here (greater than 0x7FFFFFFF) as we cannot allocate arrays larger than this
            if (chunkSize < 0 || chunkSize + reader.Position > maxPosition)
                throw new RiffProcessingException("Invalid RIFF chunk size");

            if (chunkFourCc == "LIST" || chunkFourCc == "RIFF")
            {
                if (chunkSize < 4)
                    break;
                string listName = reader.GetString(4, Encoding.ASCII);
                if (handler.ShouldAcceptList(listName))
                    ProcessChunks(reader, reader.Position + chunkSize - 4, handler);
                else
                    reader.Skip(chunkSize - 4);
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

                // Skip any padding byte added to keep chunks aligned to even numbers of bytes
                if (chunkSize % 2 == 1)
                {
                    reader.Skip(1);
                }
            }
        }
    }
}
