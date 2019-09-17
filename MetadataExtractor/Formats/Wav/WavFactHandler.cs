// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Riff;
using MetadataExtractor.IO;
using System.Collections.Generic;

using static MetadataExtractor.Formats.Wav.WavFactDirectory;

namespace MetadataExtractor.Formats.Wav
{
    /// <summary>
    /// Implementation of <see cref="IRiffChunkHandler"/> for WAV "fact" chunk.
    /// </summary>
    /// <remarks>
    /// Source:
    /// http://www-mmsp.ece.mcgill.ca/Documents/AudioFormats/WAVE/WAVE.html
    /// </remarks>
    /// <author>Dmitry Shechtman</author>
    public sealed class WavFactHandler : RiffChunkHandler<WavFactDirectory>
    {
        public WavFactHandler(List<Directory> directories)
            : base(directories)
        {
        }

        protected override int MinSize => 4;

        protected override void Populate(WavFactDirectory directory, byte[] payload)
        {
            var reader = new SequentialByteArrayReader(payload, isMotorolaByteOrder: false);
            directory.Set(TagSampleLength, reader.GetUInt32());
        }
    }
}
