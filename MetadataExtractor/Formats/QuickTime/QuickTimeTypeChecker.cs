// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Util;
using System.Text;

namespace MetadataExtractor.Formats.QuickTime
{
    /// <author>Dmitry Shechtman</author>
    internal sealed class QuickTimeTypeChecker : ITypeChecker
    {
        public int ByteCount => 12;

        public Util.FileType CheckType(byte[] bytes)
        {
            if (!bytes.RegionEquals(4, 4, Encoding.UTF8.GetBytes("ftyp")))
                return Util.FileType.Unknown;
            var fourCC = Encoding.UTF8.GetString(bytes, index: 8, count: 4);
            return fourCC switch
            {
                "crx " => Util.FileType.Crx,
                "heic" => Util.FileType.Heif,
                _ => Util.FileType.QuickTime,
            };
        }
    }
}
