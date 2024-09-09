// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.QuickTime
{
    /// <author>Dmitry Shechtman</author>
    internal sealed class QuickTimeTypeChecker : ITypeChecker
    {
        private static readonly ByteTrie<Util.FileType> _ftypTrie = new(defaultValue: Util.FileType.QuickTime)
        {
            // http://www.ftyps.com

            // QuickTime Mov
            { Util.FileType.QuickTime, "moov"u8 },
            { Util.FileType.QuickTime, "wide"u8 },
            { Util.FileType.QuickTime, "mdat"u8 },
            { Util.FileType.QuickTime, "free"u8 },
            { Util.FileType.QuickTime, "qt  "u8 },
            { Util.FileType.QuickTime, "3g2a"u8 },

            // MP4
            { Util.FileType.Mp4, "3gp5"u8 },
            { Util.FileType.Mp4, "avc1"u8 },
            { Util.FileType.Mp4, "iso2"u8 },
            { Util.FileType.Mp4, "isom"u8 },
            { Util.FileType.Mp4, "M4A "u8 },
            { Util.FileType.Mp4, "M4B "u8 },
            { Util.FileType.Mp4, "M4P "u8 },
            { Util.FileType.Mp4, "M4V "u8 },
            { Util.FileType.Mp4, "M4VH"u8 },
            { Util.FileType.Mp4, "M4VP"u8 },
            { Util.FileType.Mp4, "mmp4"u8 },
            { Util.FileType.Mp4, "mp41"u8 },
            { Util.FileType.Mp4, "mp42"u8 },
            { Util.FileType.Mp4, "mp71"u8 },
            { Util.FileType.Mp4, "MSNV"u8 },
            { Util.FileType.Mp4, "NDAS"u8 },
            { Util.FileType.Mp4, "NDSC"u8 },
            { Util.FileType.Mp4, "NDSH"u8 },
            { Util.FileType.Mp4, "NDSM"u8 },
            { Util.FileType.Mp4, "NDSP"u8 },
            { Util.FileType.Mp4, "NDSS"u8 },
            { Util.FileType.Mp4, "NDXC"u8 },
            { Util.FileType.Mp4, "NDXH"u8 },
            { Util.FileType.Mp4, "NDXM"u8 },
            { Util.FileType.Mp4, "NDXP"u8 },
            { Util.FileType.Mp4, "NDXS"u8 },
            { Util.FileType.Mp4, "nvr1"u8 },

            // HEIF
            { Util.FileType.Heif, "mif1"u8 },
            { Util.FileType.Heif, "msf1"u8 },
            { Util.FileType.Heif, "heic"u8 },
            { Util.FileType.Heif, "heix"u8 },
            { Util.FileType.Heif, "hevc"u8 },
            { Util.FileType.Heif, "hevx"u8 },

            // AVIF
            { Util.FileType.Avif, "avif"u8 },

            // CRX
            { Util.FileType.Crx, "crx "u8 }
        };

        private static ReadOnlySpan<byte> FtypBytes => "ftyp"u8;

        public int ByteCount => 16;

        public Util.FileType CheckTypeOld(byte[] bytes)
        {
            return bytes.AsSpan(4, 4).SequenceEqual(FtypBytes)
                ? _ftypTrie.Find(bytes.AsSpan(8, 4))
                : Util.FileType.Unknown;
        }

        public Util.FileType CheckType(byte[] bytes)
        {
            // for standard apple quicktime format
            // 00000000: 0000 0014 6674 7970 7174 2020 0000 0000  ....ftypqt  ....
            // 00000010: 7174 2020 0000 0008 7769 6465 003e f32a  qt  ....wide.>.*
            // 00000020: 6d64 6174 cefe f2fe 09ff 09ff 0dff 31ff  mdat..........1.
            if (bytes.AsSpan(4, 4).SequenceEqual(FtypBytes))
            {
                return _ftypTrie.Find(bytes.AsSpan(8, 4));
            }

            // for live photo which is export from  native protocols with afcclient/afcservice
            // example 1
            // 00000000: 0000 0008 7769 6465 0033 a50e 6d64 6174  ....wide.3..mdat
            // 00000010: 0000 001f 4e01 051a 4756 4adc 5c4c 433f  ....N...GVJ.\LC?
            // 00000020: 94ef c511 3cd1 43a8 03ee 1111 ee02 00c3  ....<.C.........
            // example 2
            // 00000000: 0000 0008 7769 6465 004f 854d 6d64 6174  ....wide.O.Mmdat
            // 00000010: 0000 002a 4e01 0523 4756 4adc 5c4c 433f  ...*N..#GVJ.\LC?
            // 00000020: 94ef c511 3cd1 43a8 0000 0300 0003 0000  ....<.C.........
            if (bytes.AsSpan(4, 4).SequenceEqual("wide"u8) && bytes.AsSpan(12, 4).SequenceEqual("mdat"u8))
            {
                return Util.FileType.QuickTime;
            }
            return Util.FileType.Unknown;
        }
    }
}
