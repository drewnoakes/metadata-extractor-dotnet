#region License
//
// Copyright 2002-2019 Drew Noakes
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

using System;
using System.Collections.Generic;
using MetadataExtractor.Formats.Riff;

namespace MetadataExtractor.Formats.Wav
{
    /// <summary>
    /// Implementation of <see cref="IRiffHandler"/> specialising in WAV support.
    /// </summary>
    /// <remarks>
    /// Extracts data from chunk/list types:
    /// <list type="bullet">
    ///   <item><c>"fmt "</c>: base format data</item>
    ///   <item><c>"fact"</c>: number of samples</item>
    /// </list>
    /// Source:
    /// http://www-mmsp.ece.mcgill.ca/Documents/AudioFormats/WAVE/WAVE.html
    /// </remarks>
    /// <author>Dmitry Shechtman</author>
    public sealed class WavRiffHandler : RiffHandler
    {
        public WavRiffHandler(List<Directory> directories)
            : base(directories, new Dictionary<string, Func<List<Directory>, IRiffChunkHandler>>
            {
                { "fmt ", d => new WavFormatHandler(d) },
                { "fact", d => new WavFactHandler(d) }
            })
        {
        }

        public override bool ShouldAcceptRiffIdentifier(string identifier) => identifier == "WAVE";

        public override bool ShouldAcceptList(string fourCc) => false;
    }
}
