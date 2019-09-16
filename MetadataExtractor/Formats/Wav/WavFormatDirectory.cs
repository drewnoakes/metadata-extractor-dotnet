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

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Wav
{
    /// <author>Dmitry Shechtman</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class WavFormatDirectory : Directory
    {
        public const int TagFormat = 1;
        public const int TagChannels = 2;
        public const int TagSamplesPerSec = 3;
        public const int TagBytesPerSec = 4;
        public const int TagBlockAlign = 5;
        public const int TagBitsPerSample = 6;
        public const int TagValidBitsPerSample = 7;
        public const int TagChannelMask = 8;
        public const int TagSubformat = 9;

        private readonly string[] _tagNames =
        {
            "Format",
            "Channels",
            "Sampling Rate",
            "Data Rate",
            "Data Block Size",
            "Bits Per Sample",
            "Valid Bits Per Sample",
            "Speaker Position Mask",
            "Subformat"
        };

        public WavFormatDirectory()
        {
            SetDescriptor(new WavFormatDescriptor(this));
        }

        public override string Name => "WAVE Format";

        protected override bool TryGetTagName(int tagType, [NotNullWhen(true)] out string? tagName)
        {
            tagName = tagType > 0 && tagType <= _tagNames.Length
                ? _tagNames[tagType - 1]
                : null;
            return tagName != null;
        }
    }
}
