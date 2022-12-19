// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Tga
{
    internal readonly struct TgaFooter
    {
        public int ExtOffset { get; }
        public int DevOffset { get; }
        public byte[] Signature { get; }

        public TgaFooter(int extOffset, int devOffset, byte[] signature)
        {
            ExtOffset = extOffset;
            DevOffset = devOffset;
            Signature = signature;
        }
    }

    /// <summary>Reads TGA image file footer.</summary>
    /// <author>Dmitry Shechtman</author>
    internal sealed class TgaFooterReader : TgaReader<TgaFooter>
    {
        private const int FooterSize = 26;

        private static readonly byte[] _footerSignature = Encoding.ASCII.GetBytes("TRUEVISION-XFILE.\0");

        public bool TryGetOffsets(Stream stream, out int extOffset, out int devOffset)
        {
            var footer = Extract(stream, -FooterSize, SeekOrigin.End);
            if (footer.Signature.RegionEquals(0, _footerSignature.Length, _footerSignature))
            {
                extOffset = footer.ExtOffset;
                devOffset = footer.DevOffset;
                if (extOffset >= 0 && extOffset < stream.Length && devOffset >= 0 && devOffset < stream.Length)
                    return true;
            }
            extOffset = devOffset = 0;
            return false;
        }

        protected override TgaFooter Extract(Stream stream, int offset)
        {
            var reader = new SequentialStreamReader(stream, isMotorolaByteOrder: false);

            return new TgaFooter(
                extOffset: reader.GetInt32(),
                devOffset: reader.GetInt32(),
                signature: reader.GetBytes(_footerSignature.Length));
        }
    }
}
