// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;
using MetadataExtractor.Util;
using System.IO;
using System.Text;

namespace MetadataExtractor.Formats.Tga
{
    struct TgaFooter
    {
        public int extOffset;
        public int devOffset;
        public byte[] signature;
    }

    /// <summary>Reads TGA image file footer.</summary>
    /// <author>Dmitry Shechtman</author>
    sealed class TgaFooterReader : TgaReader<TgaFooter, SequentialReader>
    {
        private const int FooterSize = 26;

        private static readonly byte[] FooterMagic = Encoding.ASCII.GetBytes("TRUEVISION-XFILE.\0");

        public static readonly TgaFooterReader Instance = new TgaFooterReader();

        private TgaFooterReader()
        {
        }

        public static bool TryGetOffsets(Stream stream, out int extOffset, out int devOffset)
        {
            var footer = Instance.Extract(stream, -FooterSize, SeekOrigin.End);
            if (footer.signature.RegionEquals(0, FooterMagic.Length, FooterMagic))
            {
                extOffset = footer.extOffset;
                devOffset = footer.devOffset;

                if (extOffset >= 0 && extOffset < stream.Length && devOffset >= 0 && devOffset < stream.Length)
                    return true;
            }
            extOffset = devOffset = 0;
            return false;
        }

        protected override SequentialReader CreateReader(Stream stream)
        {
            return new SequentialStreamReader(stream, isMotorolaByteOrder: false);
        }

        protected override TgaFooter Extract(Stream stream, int offset, SequentialReader reader)
        {
            return new TgaFooter
            {
                extOffset = reader.GetInt32(),
                devOffset = reader.GetInt32(),
                signature = reader.GetBytes(FooterMagic.Length)
            };
        }
    }
}
