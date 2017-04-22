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

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Bmp
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class BmpHeaderDescriptor : TagDescriptor<BmpHeaderDirectory>
    {
        public BmpHeaderDescriptor([NotNull] BmpHeaderDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case BmpHeaderDirectory.TagCompression:
                    return GetCompressionDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetCompressionDescription()
        {
            // 0 = None
            // 1 = RLE 8-bit/pixel
            // 2 = RLE 4-bit/pixel
            // 3 = Bit field (or Huffman 1D if BITMAPCOREHEADER2 (size 64))
            // 4 = JPEG (or RLE-24 if BITMAPCOREHEADER2 (size 64))
            // 5 = PNG
            // 6 = Bit field

            if (!Directory.TryGetInt32(BmpHeaderDirectory.TagCompression, out int value) ||
                !Directory.TryGetInt32(BmpHeaderDirectory.TagHeaderSize, out int headerSize))
                return null;

            switch (value)
            {
                case 0:
                    return "None";
                case 1:
                    return "RLE 8-bit/pixel";
                case 2:
                    return "RLE 4-bit/pixel";
                case 3:
                    return headerSize == 64 ? "Bit field" : "Huffman 1D";
                case 4:
                    return headerSize == 64 ? "JPEG" : "RLE-24";
                case 5:
                    return "PNG";
                case 6:
                    return "Bit field";
            }

            return base.GetDescription(BmpHeaderDirectory.TagCompression);
        }
    }
}
