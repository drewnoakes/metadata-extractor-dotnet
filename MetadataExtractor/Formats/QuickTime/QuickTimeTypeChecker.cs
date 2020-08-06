// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Util;
using System.Text;

namespace MetadataExtractor.Formats.QuickTime
{
    /// <author>Dmitry Shechtman</author>
    internal sealed class QuickTimeTypeChecker : ITypeChecker
    {
        private static readonly ByteTrie<Util.FileType> _ftypTrie = new ByteTrie<Util.FileType>(Util.FileType.QuickTime)
        {
            // http://www.ftyps.com

            // QuickTime Mov
            { Util.FileType.QuickTime, Encoding.UTF8.GetBytes("moov") },
            { Util.FileType.QuickTime, Encoding.UTF8.GetBytes("wide") },
            { Util.FileType.QuickTime, Encoding.UTF8.GetBytes("mdat") },
            { Util.FileType.QuickTime, Encoding.UTF8.GetBytes("free") },
            { Util.FileType.QuickTime, Encoding.UTF8.GetBytes("qt  ") },
            { Util.FileType.QuickTime, Encoding.UTF8.GetBytes("3g2a") },

            // MP4
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("avc1") },
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("iso2") },
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("isom") },
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("M4A ") },
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("M4B ") },
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("M4P ") },
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("M4V ") },
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("M4VH") },
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("M4VP") },
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("mmp4") },
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("mp41") },
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("mp42") },
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("mp71") },
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("MSNV") },
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("NDAS") },
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("NDSC") },
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("NDSH") },
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("NDSM") },
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("NDSP") },
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("NDSS") },
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("NDXC") },
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("NDXH") },
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("NDXM") },
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("NDXP") },
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("NDXS") },
            { Util.FileType.Mp4, Encoding.UTF8.GetBytes("nvr1") },

            // HEIF
            { Util.FileType.Heif, Encoding.UTF8.GetBytes("mif1") },
            { Util.FileType.Heif, Encoding.UTF8.GetBytes("msf1") },
            { Util.FileType.Heif, Encoding.UTF8.GetBytes("heic") },
            { Util.FileType.Heif, Encoding.UTF8.GetBytes("heix") },
            { Util.FileType.Heif, Encoding.UTF8.GetBytes("hevc") },
            { Util.FileType.Heif, Encoding.UTF8.GetBytes("hevx") },

            // CRX
            { Util.FileType.Crx, Encoding.UTF8.GetBytes("crx ") }
        };

        private static readonly byte[] _ftypBytes = Encoding.UTF8.GetBytes("ftyp");

        public int ByteCount => 12;

        public Util.FileType CheckType(byte[] bytes)
        {
            return bytes.RegionEquals(4, 4, _ftypBytes)
                ? _ftypTrie.Find(bytes, 8, 4)
                : Util.FileType.Unknown;
        }
    }
}
