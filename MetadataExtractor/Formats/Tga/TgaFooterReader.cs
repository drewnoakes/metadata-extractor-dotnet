// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;
using MetadataExtractor.Util;
using System.IO;
using System.Text;

namespace MetadataExtractor.Formats.Tga
{
    internal struct TgaFooter
    {
        public int ExtOffset;
        public int DevOffset;
        public byte[] Signature;
    }

    /// <summary>Reads TGA image file footer.</summary>
    /// <author>Dmitry Shechtman</author>
    internal sealed class TgaFooterReader : TgaReader<TgaFooter, SequentialReader>
    {
        private const int FooterSize = 26;

        private static readonly byte[] FooterSignature = Encoding.ASCII.GetBytes("TRUEVISION-XFILE.\0");

        public bool TryGetOffsets(Stream stream, out int extOffset, out int devOffset)
        {
            var footer = Extract(stream, -FooterSize, SeekOrigin.End);
            if (footer.Signature.RegionEquals(0, FooterSignature.Length, FooterSignature))
            {
                extOffset = footer.ExtOffset;
                devOffset = footer.DevOffset;
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
                ExtOffset = reader.GetInt32(),
                DevOffset = reader.GetInt32(),
                Signature = reader.GetBytes(FooterSignature.Length)
            };
        }
    }
}
