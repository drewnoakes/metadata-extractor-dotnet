// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;

using static MetadataExtractor.Formats.Wav.WavFormatDirectory;

namespace MetadataExtractor.Formats.Wav
{
    /// <author>Dmitry Shechtman</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class WavFormatDescriptor : TagDescriptor<WavFormatDirectory>
    {
        public WavFormatDescriptor(WavFormatDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tag)
        {
            return tag switch
            {
                TagFormat => GetFormatDescription(),
                TagSamplesPerSec => Directory.GetUInt32(tag).ToString("0 bps"),
                TagBytesPerSec => Directory.GetUInt32(tag).ToString("0 bps"),
                TagSubformat =>
                    BitConverter.ToString(Directory.GetByteArray(tag)).Replace("-", ""),
                _ => base.GetDescription(tag),
            };
        }

        private string? GetFormatDescription()
        {
            var value = Directory.GetUInt16(TagFormat);
            return value switch
            {
                1 => "PCM",
                3 => "IEEE float",
                6 => "8-bit ITU-T G.711 A-law",
                7 => "8-bit ITU-T G.711 µ-law",
                0xFFFE => "Determined by Subformat",
                _ => $"Unknown ({value})"
            };
        }
    }
}
