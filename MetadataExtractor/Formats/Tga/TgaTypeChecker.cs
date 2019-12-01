// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Util;

namespace MetadataExtractor.Formats.Tga
{
    /// <author>Dmitry Shechtman</author>
    internal sealed class TgaTypeChecker : ITypeChecker
    {
        public int ByteCount => TgaHeaderReader.HeaderSize;

        public Util.FileType CheckType(byte[] bytes)
        {
            return TgaHeaderReader.TryExtract(bytes, out _)
                ? Util.FileType.Tga
                : Util.FileType.Unknown;
        }
    }
}
