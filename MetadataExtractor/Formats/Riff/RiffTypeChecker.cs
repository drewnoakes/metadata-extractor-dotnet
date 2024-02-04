// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Riff
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    internal sealed class RiffTypeChecker : ITypeChecker
    {
        public int ByteCount => 12;

        public Util.FileType CheckType(byte[] bytes)
        {
            if (!bytes.AsSpan().StartsWith("RIFF"u8))
                return Util.FileType.Unknown;

            var fourCC = bytes.AsSpan(start: 8, length: 4);

            if (fourCC.SequenceEqual("WAVE"u8))
                return Util.FileType.Wav;

            if (fourCC.SequenceEqual("AVI "u8))
                return Util.FileType.Avi;

            if (fourCC.SequenceEqual("WEBP"u8))
                return Util.FileType.WebP;

            return Util.FileType.Riff;
        }
    }
}
