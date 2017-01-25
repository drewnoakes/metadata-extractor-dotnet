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

using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Riff
{
    /// <summary>
    /// Interface of an class capable of handling events raised during the reading of a RIFF file
    /// via <see cref="RiffReader"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public interface IRiffHandler
    {
        /// <summary>Gets whether the specified RIFF identifier is of interest to this handler.</summary>
        /// <remarks>Returning <c>false</c> causes processing to stop after reading only the first twelve bytes of data.</remarks>
        /// <param name="identifier">The four character code identifying the type of RIFF data</param>
        /// <returns>true if processing should continue, otherwise false</returns>
        bool ShouldAcceptRiffIdentifier([NotNull] string identifier);

        /// <summary>Gets whether this handler is interested in the specific chunk type.</summary>
        /// <remarks>
        /// Returns <c>true</c> if the data should be copied into an array and passed
        /// to <see cref="ProcessChunk(string, byte[])"/>, or <c>false</c> to avoid
        /// the copy and skip to the next chunk in the file, if any.
        /// </remarks>
        /// <param name="fourCc">the four character code of this chunk</param>
        /// <returns><c>true</c> if <see cref="ProcessChunk(string, byte[])"/> should be called, otherwise <c>false</c>.</returns>
        bool ShouldAcceptChunk([NotNull] string fourCc);

        /// <summary>Perform whatever processing is necessary for the type of chunk with its payload.</summary>
        /// <remarks>This is only called if a previous call to <see cref="ShouldAcceptChunk(string)"/> with the same <c>fourCC</c> returned <c>true</c>.</remarks>
        /// <param name="fourCc">the four character code of the chunk</param>
        /// <param name="payload">they payload of the chunk as a byte array</param>
        void ProcessChunk([NotNull] string fourCc, [NotNull] byte[] payload);
    }
}
