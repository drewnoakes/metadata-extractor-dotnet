// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Riff;
using MetadataExtractor.IO;
using System.Collections.Generic;

using static MetadataExtractor.Formats.Wav.WavFormatDirectory;

namespace MetadataExtractor.Formats.Wav
{
    /// <summary>
    /// Implementation of <see cref="IRiffChunkHandler"/> for WAV "fmt " chunk.
    /// </summary>
    /// <remarks>
    /// Source:
    /// http://www-mmsp.ece.mcgill.ca/Documents/AudioFormats/WAVE/WAVE.html
    /// </remarks>
    /// <author>Dmitry Shechtman</author>
    public sealed class WavFormatHandler : RiffChunkHandler<WavFormatDirectory>
    {
        public WavFormatHandler(List<Directory> directories)
            : base(directories)
        {
        }

        protected override int MinSize => 16;

        protected override void Populate(WavFormatDirectory directory, byte[] payload)
        {
            var reader = new SequentialByteArrayReader(payload, isMotorolaByteOrder: false);
            Populate(directory, reader, payload.Length);
        }

        public static void Populate(WavFormatDirectory directory, SequentialReader reader, int chunkSize)
        {
            directory.Set(TagFormat, reader.GetUInt16());
            directory.Set(TagChannels, reader.GetUInt16());
            directory.Set(TagSamplesPerSec, reader.GetUInt32());
            directory.Set(TagBytesPerSec, reader.GetUInt32());
            directory.Set(TagBlockAlign, reader.GetUInt16());
            directory.Set(TagBitsPerSample, reader.GetUInt16());

            if (chunkSize > 16)
            {
                var exSize = reader.GetUInt16();
                if (exSize > 0)
                {
                    if (exSize > chunkSize - 16)
                        directory.AddError("Invalid chunk 'fmt ' extension size");
                    else
                        PopulateEx(directory, reader, exSize);
                }
            }
        }

        private static void PopulateEx(WavFormatDirectory directory, SequentialReader reader, int exSize)
        {
            if (exSize < 2)
                return;
            directory.Set(TagValidBitsPerSample, reader.GetUInt16());

            if (exSize < 6)
                return;
            directory.Set(TagChannelMask, reader.GetUInt32());

            if (exSize < 22)
                return;
            directory.Set(TagSubformat, reader.GetBytes(16));
        }
    }
}
