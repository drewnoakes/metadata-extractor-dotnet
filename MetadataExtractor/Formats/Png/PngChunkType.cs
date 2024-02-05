// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PngChunkType
    {
        private static readonly HashSet<string> _identifiersAllowingMultiples
            = new(StringComparer.Ordinal) { "IDAT", "sPLT", "iTXt", "tEXt", "zTXt" };

#pragma warning disable IDE1006 // Naming Styles

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
        public static readonly PngChunkType IHDR = new("IHDR");

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
        public static readonly PngChunkType PLTE = new("PLTE");

        public static readonly PngChunkType IDAT = new("IDAT", true);

        public static readonly PngChunkType IEND = new("IEND");

        #endregion

        #region Standard ancillary chunks

        public static readonly PngChunkType cHRM = new("cHRM");

        public static readonly PngChunkType gAMA = new("gAMA");

        public static readonly PngChunkType iCCP = new("iCCP");

        public static readonly PngChunkType sBIT = new("sBIT");

        public static readonly PngChunkType sRGB = new("sRGB");

        public static readonly PngChunkType bKGD = new("bKGD");

        public static readonly PngChunkType hIST = new("hIST");

        public static readonly PngChunkType tRNS = new("tRNS");

        public static readonly PngChunkType pHYs = new("pHYs");

        public static readonly PngChunkType sPLT = new("sPLT", true);

        public static readonly PngChunkType tIME = new("tIME");

        public static readonly PngChunkType iTXt = new("iTXt", true);

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
        public static readonly PngChunkType tEXt = new("tEXt", true);

        public static readonly PngChunkType zTXt = new("zTXt", true);

        public static readonly PngChunkType eXIf = new("eXIf");

        #endregion

#pragma warning restore IDE1006 // Naming Styles

        private readonly byte[] _bytes;

        public PngChunkType(string identifier, bool multipleAllowed = false)
        {
            AreMultipleAllowed = multipleAllowed;
            var bytes = Encoding.UTF8.GetBytes(identifier);
            ValidateBytes(bytes);
            _bytes = bytes;
        }

        public PngChunkType(byte[] bytes)
        {
            ValidateBytes(bytes);
            _bytes = bytes;
            AreMultipleAllowed = _identifiersAllowingMultiples.Contains(Identifier);
        }

        private static void ValidateBytes(byte[] bytes)
        {
            if (bytes.Length != 4)
                throw new ArgumentException("PNG chunk type identifier must be four bytes in length");
            if (!bytes.All(IsValidByte))
                throw new ArgumentException("PNG chunk type identifier may only contain alphabet characters");
        }

        public bool IsCritical => IsUpperCase(_bytes[0]);

        public bool IsAncillary => !IsCritical;

        public bool IsPrivate => IsUpperCase(_bytes[1]);

        public bool IsSafeToCopy => IsLowerCase(_bytes[3]);

        public bool AreMultipleAllowed { get; }

        private static bool IsLowerCase(byte b) => (b & (1 << 5)) != 0;

        private static bool IsUpperCase(byte b) => (b & (1 << 5)) == 0;

        private static bool IsValidByte(byte b) => b is >= 65 and <= 90 or >= 97 and <= 122;

        public string Identifier => Encoding.UTF8.GetString(_bytes);

        public override string ToString() => Identifier;

        #region Equality and Hashing

        private bool Equals(PngChunkType other) => _bytes.SequenceEqual(other._bytes);

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj is PngChunkType t && Equals(t);
        }

        public override int GetHashCode()
        {
#if NET8_0_OR_GREATER
            HashCode hash = new();
            hash.AddBytes(_bytes);
            return hash.GetHashCode();
#else
            return _bytes[0] << 24 | _bytes[1] << 16 << _bytes[2] << 8 | _bytes[3];
#endif
        }

        public static bool operator ==(PngChunkType left, PngChunkType right) => Equals(left, right);
        public static bool operator !=(PngChunkType left, PngChunkType right) => !Equals(left, right);

        #endregion
    }
}
