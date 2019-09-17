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
