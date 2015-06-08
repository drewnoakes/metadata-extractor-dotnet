using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Imaging.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class PngChunkType
    {
        private static readonly ICollection<string> IdentifiersAllowingMultiples = new HashSet<string>(Arrays.AsList("IDAT", "sPLT", "iTXt", "tEXt", "zTXt"));

        /// <summary>
        /// Denotes a critical
        /// <see cref="PngChunk"/>
        /// that contains basic information about the PNG image.
        /// This must be the first chunk in the data sequence, and may only occur once.
        /// <p>
        /// The format is:
        /// <ul>
        /// <li><b>pixel width</b> 4 bytes, unsigned and greater than zero</li>
        /// <li><b>pixel height</b> 4 bytes, unsigned and greater than zero</li>
        /// <li><b>bit depth</b> 1 byte, number of bits per sample or per palette index (not per pixel)</li>
        /// <li><b>color type</b> 1 byte, maps to
        /// <see cref="PngColorType"/>
        /// enum</li>
        /// <li><b>compression method</b> 1 byte, currently only a value of zero (deflate/inflate) is in the standard</li>
        /// <li><b>filter method</b> 1 byte, currently only a value of zero (adaptive filtering with five basic filter types) is in the standard</li>
        /// <li><b>interlace method</b> 1 byte, indicates the transmission order of image data, currently only 0 (no interlace) and 1 (Adam7 interlace) are in the standard</li>
        /// </ul>
        /// </summary>
        public static readonly PngChunkType Ihdr = new PngChunkType("IHDR");

        /// <summary>
        /// Denotes a critical
        /// <see cref="PngChunk"/>
        /// that contains palette entries.
        /// This chunk should only appear for a
        /// <see cref="PngColorType"/>
        /// of <code>IndexedColor</code>,
        /// and may only occur once in the PNG data sequence.
        /// <p>
        /// The chunk contains between one and 256 entries, each of three bytes:
        /// <ul>
        /// <li><b>red</b> 1 byte</li>
        /// <li><b>green</b> 1 byte</li>
        /// <li><b>blue</b> 1 byte</li>
        /// </ul>
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
        /// <p>
        /// The format is:
        /// <ul>
        /// <li><b>keyword</b> 1-79 bytes</li>
        /// <li><b>null separator</b> 1 byte (\0)</li>
        /// <li><b>text string</b> 0 or more bytes</li>
        /// </ul>
        /// Text is interpreted according to the Latin-1 character set [ISO-8859-1].
        /// Newlines should be represented by a single linefeed character (0x9).
        /// </summary>
        public static readonly PngChunkType TEXt = new PngChunkType("tEXt", true);

        public static readonly PngChunkType ZTXt = new PngChunkType("zTXt", true);

        private readonly sbyte[] _bytes;

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
            try
            {
                sbyte[] bytes = Runtime.GetBytesForString(identifier, "ASCII");
                ValidateBytes(bytes);
                _bytes = bytes;
            }
            catch (UnsupportedEncodingException)
            {
                throw new ArgumentException("Unable to convert string code to bytes.");
            }
        }

        public PngChunkType([NotNull] sbyte[] bytes)
        {
            ValidateBytes(bytes);
            _bytes = bytes;
            _multipleAllowed = IdentifiersAllowingMultiples.Contains(GetIdentifier());
        }

        private static void ValidateBytes(sbyte[] bytes)
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

        public virtual bool IsCritical()
        {
            return IsUpperCase(_bytes[0]);
        }

        public virtual bool IsAncillary()
        {
            return !IsCritical();
        }

        public virtual bool IsPrivate()
        {
            return IsUpperCase(_bytes[1]);
        }

        public virtual bool IsSafeToCopy()
        {
            return IsLowerCase(_bytes[3]);
        }

        public virtual bool AreMultipleAllowed()
        {
            return _multipleAllowed;
        }

        private static bool IsLowerCase(sbyte b)
        {
            return (b & (1 << 5)) != 0;
        }

        private static bool IsUpperCase(sbyte b)
        {
            return (b & (1 << 5)) == 0;
        }

        private static bool IsValidByte(sbyte b)
        {
            return (b >= 65 && ((sbyte)b) <= 90) || (b >= 97 && ((sbyte)b) <= 122);
        }

        public virtual string GetIdentifier()
        {
            try
            {
                return Runtime.GetStringForBytes(_bytes, "ASCII");
            }
            catch (UnsupportedEncodingException)
            {
                // The constructor should ensure that we're always able to encode the bytes in ASCII.
                // noinspection ConstantConditions
                Debug.Assert((false));
                return "Invalid object instance";
            }
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
            PngChunkType that = (PngChunkType)o;
            return Arrays.Equals(_bytes, that._bytes);
        }

        public override int GetHashCode()
        {
            return Arrays.HashCode(_bytes);
        }
    }
}
