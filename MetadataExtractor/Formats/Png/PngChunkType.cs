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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public sealed class PngChunkType
    {
        private static readonly ICollection<string> IdentifiersAllowingMultiples = new HashSet<string> { "IDAT", "sPLT", "iTXt", "tEXt", "zTXt" };

        #region Standard critical chunks

        /// <summary>
        /// Denotes a critical <see cref="PngChunk"/> that contains basic information about the PNG image.
        /// </summary>
        /// <remarks>
        /// This must be the first chunk in the data sequence, and may only occur once.
        /// <para />
        /// The format is:
        /// <list type="bullet">
        ///   <item><b>pixel width</b> 4 bytes, unsigned and greater than zero</item>
        ///   <item><b>pixel height</b> 4 bytes, unsigned and greater than zero</item>
        ///   <item><b>bit depth</b> 1 byte, number of bits per sample or per palette index (not per pixel)</item>
        ///   <item><b>color type</b> 1 byte, maps to <see cref="PngColorType"/> enum</item>
        ///   <item><b>compression method</b> 1 byte, currently only a value of zero (deflate/inflate) is in the standard</item>
        ///   <item><b>filter method</b> 1 byte, currently only a value of zero (adaptive filtering with five basic filter types) is in the standard</item>
        ///   <item><b>interlace method</b> 1 byte, indicates the transmission order of image data, currently only 0 (no interlace) and 1 (Adam7 interlace) are in the standard</item>
        /// </list>
        /// </remarks>
        public static readonly PngChunkType IHDR = new PngChunkType("IHDR");

        /// <summary>
        /// Denotes a critical <see cref="PngChunk"/> that contains palette entries.
        /// </summary>
        /// <remarks>
        /// This chunk should only appear for a <see cref="PngColorType"/> of <see cref="PngColorType.IndexedColor"/>,
        /// and may only occur once in the PNG data sequence.
        /// <para />
        /// The chunk contains between one and 256 entries, each of three bytes:
        /// <list type="bullet">
        ///   <item><b>red</b> 1 byte</item>
        ///   <item><b>green</b> 1 byte</item>
        ///   <item><b>blue</b> 1 byte</item>
        /// </list>
        /// The number of entries is determined by the chunk length. A chunk length indivisible by three is an error.
        /// </remarks>
        public static readonly PngChunkType PLTE = new PngChunkType("PLTE");

        public static readonly PngChunkType IDAT = new PngChunkType("IDAT", true);

        public static readonly PngChunkType IEND = new PngChunkType("IEND");

        #endregion

        #region Standard ancillary chunks

        public static readonly PngChunkType cHRM = new PngChunkType("cHRM");

        public static readonly PngChunkType gAMA = new PngChunkType("gAMA");

        public static readonly PngChunkType iCCP = new PngChunkType("iCCP");

        public static readonly PngChunkType sBIT = new PngChunkType("sBIT");

        public static readonly PngChunkType sRGB = new PngChunkType("sRGB");

        public static readonly PngChunkType bKGD = new PngChunkType("bKGD");

        public static readonly PngChunkType hIST = new PngChunkType("hIST");

        public static readonly PngChunkType tRNS = new PngChunkType("tRNS");

        public static readonly PngChunkType pHYs = new PngChunkType("pHYs");

        public static readonly PngChunkType sPLT = new PngChunkType("sPLT", true);

        public static readonly PngChunkType tIME = new PngChunkType("tIME");

        public static readonly PngChunkType iTXt = new PngChunkType("iTXt", true);

        /// <summary>
        /// Denotes an ancillary <see cref="PngChunk"/> that contains textual data, having first a keyword and then a value.
        /// </summary>
        /// <remarks>
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
        /// </remarks>
        public static readonly PngChunkType tEXt = new PngChunkType("tEXt", true);

        public static readonly PngChunkType zTXt = new PngChunkType("zTXt", true);

        #endregion

        private readonly byte[] _bytes;

        private readonly bool _multipleAllowed;

        public PngChunkType([NotNull] string identifier, bool multipleAllowed = false)
        {
            _multipleAllowed = multipleAllowed;
            var bytes = Encoding.ASCII.GetBytes(identifier);
            ValidateBytes(bytes);
            _bytes = bytes;
        }

        public PngChunkType([NotNull] byte[] bytes)
        {
            ValidateBytes(bytes);
            _bytes = bytes;
            _multipleAllowed = IdentifiersAllowingMultiples.Contains(Identifier);
        }

        private static void ValidateBytes(byte[] bytes)
        {
            if (bytes.Length != 4)
                throw new ArgumentException("PNG chunk type identifier must be four bytes in length");
            if (bytes.Any(b => !IsValidByte(b)))
                throw new ArgumentException("PNG chunk type identifier may only contain alphabet characters");
        }

        public bool IsCritical
        {
            get { return IsUpperCase(_bytes[0]); }
        }

        public bool IsAncillary
        {
            get { return !IsCritical; }
        }

        public bool IsPrivate
        {
            get { return IsUpperCase(_bytes[1]); }
        }

        public bool IsSafeToCopy
        {
            get { return IsLowerCase(_bytes[3]); }
        }

        public bool AreMultipleAllowed
        {
            get { return _multipleAllowed; }
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

        public string Identifier
        {
            get { return Encoding.ASCII.GetString(_bytes); }
        }

        public override string ToString()
        {
            return Identifier;
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
