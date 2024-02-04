// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Mpeg
{
    internal sealed class MpegAudioTypeChecker : ITypeChecker
    {
        /*
            MPEG Audio http://www.mp3-tech.org/programmer/frame_header.html

            Bits
            11  Frame sync (all bits must be set)
            2   MPEG Audio version ID
                00 - MPEG Version 2.5 (later extension of MPEG 2)
                01 - Reserved
                10 - MPEG Version 2 (ISO/IEC 13818-3)
                11 - MPEG Version 1 (ISO/IEC 11172-3)
            2   Layer description
                00 - Reserved
                01 - Layer III
                10 - Layer II
                11 - Layer I

            Additional bits contain more information, but are not required for file type identification.

            MP3 with ID3V2-Tags https://id3.org/id3v2.4.0-structure
            MP3-File with ID3V2-Tagging at start
         */

        public int ByteCount => 3;

        public Util.FileType CheckType(byte[] bytes)
        {
            // MP3-File with "ID3" at start
            if (bytes.AsSpan(0, 3).SequenceEqual("ID3"u8))
                return Util.FileType.Mp3;

            // MPEG audio requires the first 11 bits to be set
            if (bytes[0] != 0xFF || (bytes[1] & 0xE0) != 0xE0)
                return Util.FileType.Unknown;

            // The MPEG Audio version ID value of 01 is reserved
            int version = (bytes[1] >> 3) & 3;
            if (version == 1)
                return Util.FileType.Unknown;

            // The layer description value of 00 is reserved
            int layerDescription = (bytes[1] >> 1) & 3;
            if (layerDescription == 0)
                return Util.FileType.Unknown;

            // The bitrate index value of 1111 is disallowed
            int bitrateIndex = bytes[2] >> 4;
            if (bitrateIndex == 0x0F)
                return Util.FileType.Unknown;

            return Util.FileType.Mp3;
        }
    }
}
