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

using System.Text;
using JetBrains.Annotations;
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
        public void ProcessRiff([NotNull] SequentialReader reader, [NotNull] IRiffHandler handler)
        {
            // RIFF files are always little-endian
            reader = reader.WithByteOrder(isMotorolaByteOrder: false);

            // PROCESS FILE HEADER

            var fileFourCc = reader.GetString(4, Encoding.UTF8);
            if (fileFourCc != "RIFF")
                throw new RiffProcessingException("Invalid RIFF header: " + fileFourCc);

            // The total size of the chunks that follow plus 4 bytes for the 'WEBP' FourCC
            var fileSize = reader.GetInt32();
            var sizeLeft = fileSize;
            var identifier = reader.GetString(4, Encoding.UTF8);
            sizeLeft -= 4;

            if (!handler.ShouldAcceptRiffIdentifier(identifier))
                return;

            // PROCESS CHUNKS

            while (sizeLeft != 0)
            {
                var chunkFourCc = reader.GetString(4, Encoding.UTF8);
                var chunkSize = reader.GetInt32();

                sizeLeft -= 8;

                // NOTE we fail a negative chunk size here (greater than 0x7FFFFFFF) as we cannot allocate arrays larger than this
                if (chunkSize < 0 || sizeLeft < chunkSize)
                    throw new RiffProcessingException("Invalid RIFF chunk size");

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
                    reader.GetSByte();
                    sizeLeft--;
                }
            }
        }
    }
}
