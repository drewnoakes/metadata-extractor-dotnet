/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using Com.Drew.Lang;
using JetBrains.Annotations;

namespace Com.Drew.Imaging.Riff
{
    /// <summary>
    /// Processes RIFF-formatted data, calling into client code via that
    /// <see cref="RiffHandler"/>
    /// interface.
    /// <p></p>
    /// For information on this file format, see:
    /// <ul>
    /// <li>http://en.wikipedia.org/wiki/Resource_Interchange_File_Format</li>
    /// <li>https://developers.google.com/speed/webp/docs/riff_container</li>
    /// <li>https://www.daubnet.com/en/file-format-riff</li>
    /// </ul>
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class RiffReader
    {
        /// <summary>Processes a RIFF data sequence.</summary>
        /// <param name="reader">
        /// the
        /// <see cref="Com.Drew.Lang.SequentialReader"/>
        /// from which the data should be read
        /// </param>
        /// <param name="handler">
        /// the
        /// <see cref="RiffHandler"/>
        /// that will coordinate processing and accept read values
        /// </param>
        /// <exception cref="RiffProcessingException">
        /// if an error occurred during the processing of RIFF data that could not be
        /// ignored or recovered from
        /// </exception>
        /// <exception cref="System.IO.IOException">an error occurred while accessing the required data</exception>
        /// <exception cref="Com.Drew.Imaging.Riff.RiffProcessingException"/>
        public virtual void ProcessRiff([NotNull] SequentialReader reader, [NotNull] RiffHandler handler)
        {
            // RIFF files are always little-endian
            reader.SetMotorolaByteOrder(false);
            // PROCESS FILE HEADER
            string fileFourCC = reader.GetString(4);
            if (!fileFourCC.Equals("RIFF"))
            {
                throw new RiffProcessingException("Invalid RIFF header: " + fileFourCC);
            }
            // The total size of the chunks that follow plus 4 bytes for the 'WEBP' FourCC
            int fileSize = reader.GetInt32();
            int sizeLeft = fileSize;
            string identifier = reader.GetString(4);
            sizeLeft -= 4;
            if (!handler.ShouldAcceptRiffIdentifier(identifier))
            {
                return;
            }
            // PROCESS CHUNKS
            while (sizeLeft != 0)
            {
                string chunkFourCC = reader.GetString(4);
                int chunkSize = reader.GetInt32();
                sizeLeft -= 8;
                // NOTE we fail a negative chunk size here (greater than 0x7FFFFFFF) as Java cannot
                // allocate arrays larger than this.
                if (chunkSize < 0 || sizeLeft < chunkSize)
                {
                    throw new RiffProcessingException("Invalid RIFF chunk size");
                }
                if (handler.ShouldAcceptChunk(chunkFourCC))
                {
                    // TODO is it feasible to avoid copying the chunk here, and to pass the sequential reader to the handler?
                    handler.ProcessChunk(chunkFourCC, reader.GetBytes(chunkSize));
                }
                else
                {
                    reader.Skip(chunkSize);
                }
                sizeLeft -= chunkSize;
                // Skip any padding byte added to keep chunks aligned to even numbers of bytes
                if (chunkSize % 2 == 1)
                {
                    reader.GetInt8();
                    sizeLeft--;
                }
            }
        }
    }
}
