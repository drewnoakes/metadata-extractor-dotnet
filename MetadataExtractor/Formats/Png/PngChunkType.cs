#region License
//
// Copyright 2002-2015 Drew Noakes
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
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PngChunkType
    {
        private static readonly ICollection<string> IdentifiersAllowingMultiples = new HashSet<string> { "IDAT", "sPLT", "iTXt", "tEXt", "zTXt" };

        /// <summary>
        /// Denotes a critical
        /// <see cref="PngChunk"/>
        /// that contains basic information about the PNG image.
        /// This must be the first chunk in the data sequence, and may only occur once.
        /// <para />
        /// The format is:
        /// <list type="bullet">
        /// <item><b>pixel width</b> 4 bytes, unsigned and greater than zero</item>
        /// <item><b>pixel height</b> 4 bytes, unsigned and greater than zero</item>
        /// <item><b>bit depth</b> 1 byte, number of bits per sample or per palette index (not per pixel)</item>
        /// <item><b>color type</b> 1 byte, maps to
        /// <see cref="PngColorType"/>
        /// enum</item>
        /// <item><b>compression method</b> 1 byte, currently only a value of zero (deflate/inflate) is in the standard</item>
        /// <item><b>filter method</b> 1 byte, currently only a value of zero (adaptive filtering with five basic filter types) is in the standard</item>
        /// <item><b>interlace method</b> 1 byte, indicates the transmission order of image data, currently only 0 (no interlace) and 1 (Adam7 interlace) are in the standard</item>
        /// </list>
        /// </summary>
        public static readonly PngChunkType Ihdr = new PngChunkType("IHDR");

        /// <summary>
        /// Denotes a critical
        /// <see cref="PngChunk"/>
        /// that contains palette entries.
        /// This chunk should only appear for a
        /// <see cref="PngColorType"/>
        /// of <c>IndexedColor</c>,
        /// and may only occur once in the PNG data sequence.
        /// <para />
        /// The chunk contains between one and 256 entries, each of three bytes:
        /// <list type="bullet">
        /// <item><b>red</b> 1 byte</item>
        /// <item><b>green</b> 1 byte</item>
        /// <item><b>blue</b> 1 byte</item>
        /// </list>
        /// The number of entries is determined by the chunk length. A chunk length indivisible by three is an error.
        /// </summary>
        public static readonly PngChunkType Plte = new PngChunkType("PLTE");

        public static readonly PngChunkType Idat = new PngChunkType("IDAT", true);

        public static readonly PngChunkType Iend = new PngChunkType("IEND");

        public static readonly PngChunkType CHrm = new PngChunkType("cHRM");

        public static readonly PngChunkType GAma = new PngChunkType("gAMA");

        public static readonly PngChunkType ICcp = new PngChunkType("iCCP");

        public static readonly PngChunkType SBit = new PngChunkType("sBIT");

        public static readonly PngChunkType SRgb = new PngChunkType("sRGB");

        public static readonly PngChunkType BKgd = new PngChunkType("bKGD");

        public static readonly PngChunkType HIst = new PngChunkType("hIST");

        public static readonly PngChunkType TRns = new PngChunkType("tRNS");

        public static readonly PngChunkType PHYs = new PngChunkType("pHYs");

        public static readonly PngChunkType SPlt = new PngChunkType("sPLT", true);

        public static readonly PngChunkType TIme = new PngChunkType("tIME");

        public static readonly PngChunkType ITXt = new PngChunkType("iTXt", true);

        /// <summary>
        /// Denotes an ancillary
        /// <see cref="PngChunk"/>
        /// that contains textual data, having first a keyword and then a value.
        /// If multiple text data keywords are needed, then multiple chunks are included in the PNG data stream.
        /// <para />
        /// The format is:
        /// <list type="bullet">
        /// <item><b>keyword</b> 1-79 bytes</item>
        /// <item><b>null separator</b> 1 byte (\0)</item>
        /// <item><b>text string</b> 0 or more bytes</item>
        /// </list>
        /// Text is interpreted according to the Latin-1 character set [ISO-8859-1].
        /// Newlines should be represented by a single linefeed character (0x9).
        /// </summary>
        public static readonly PngChunkType TEXt = new PngChunkType("tEXt", true);

        public static readonly PngChunkType ZTXt = new PngChunkType("zTXt", true);

        private readonly byte[] _bytes;

        private readonly bool _multipleAllowed;

        public PngChunkType([NotNull] string identifier, bool multipleAllowed = false)
        {
            //
            // Standard critical chunks
            //
            //
            // Standard ancillary chunks
            //
            _multipleAllowed = multipleAllowed;
            var bytes = Encoding.ASCII.GetBytes(identifier);
            ValidateBytes(bytes);
            _bytes = bytes;
        }

        public PngChunkType([NotNull] byte[] bytes)
        {
            ValidateBytes(bytes);
            _bytes = bytes;
            _multipleAllowed = IdentifiersAllowingMultiples.Contains(GetIdentifier());
        }

        private static void ValidateBytes(byte[] bytes)
        {
            if (bytes.Length != 4)
            {
                throw new ArgumentException("PNG chunk type identifier must be four bytes in length");
            }
            if (bytes.Any(b => !IsValidByte(b)))
            {
                throw new ArgumentException("PNG chunk type identifier may only contain alphabet characters");
            }
        }

        public bool IsCritical()
        {
            return IsUpperCase(_bytes[0]);
        }

        public bool IsAncillary()
        {
            return !IsCritical();
        }

        public bool IsPrivate()
        {
            return IsUpperCase(_bytes[1]);
        }

        public bool IsSafeToCopy()
        {
            return IsLowerCase(_bytes[3]);
        }

        public bool AreMultipleAllowed()
        {
            return _multipleAllowed;
        }

        private static bool IsLowerCase(byte b)
        {
            return (b & (1 << 5)) != 0;
        }

        private static bool IsUpperCase(byte b)
        {
            return (b & (1 << 5)) == 0;
        }

        private static bool IsValidByte(byte b)
        {
            return (b >= 65 && b <= 90) || (b >= 97 && b <= 122);
        }

        public string GetIdentifier()
        {
            return Encoding.ASCII.GetString(_bytes);
        }

        public override string ToString()
        {
            return GetIdentifier();
        }

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }
            var that = (PngChunkType)o;
            return _bytes.SequenceEqual(that._bytes);
        }

        public override int GetHashCode()
        {
            return _bytes == null ? 0 : _bytes.Aggregate(1, (current, element) => 31*current + element);
        }
    }
}
