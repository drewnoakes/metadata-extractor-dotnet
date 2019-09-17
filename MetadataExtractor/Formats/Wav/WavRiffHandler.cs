// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
