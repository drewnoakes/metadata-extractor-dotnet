// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using System;

namespace MetadataExtractor.Util
{
    public static class ByteConvert
    {
        [Pure]
        public static uint FromBigEndianToNative(uint bigEndian)
        {
            if (BitConverter.IsLittleEndian == false)
                return bigEndian;

            var bytes = BitConverter.GetBytes(bigEndian);
            Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, startIndex: 0);
        }

        [Pure]
        public static ushort FromBigEndianToNative(ushort bigEndian)
        {
            if (BitConverter.IsLittleEndian == false)
                return bigEndian;

            var bytes = BitConverter.GetBytes(bigEndian);
            Array.Reverse(bytes);
            return BitConverter.ToUInt16(bytes, startIndex: 0);
        }

        [Pure]
        public static short FromBigEndianToNative(short bigEndian)
        {
            if (BitConverter.IsLittleEndian == false)
                return bigEndian;

            var bytes = BitConverter.GetBytes(bigEndian);
            Array.Reverse(bytes);
            return BitConverter.ToInt16(bytes, startIndex: 0);
        }

        [Pure]
        public static int ToInt32BigEndian(byte[] bytes) => bytes[0] << 24 |
                                                            bytes[1] << 16 |
                                                            bytes[2] << 8 |
                                                            bytes[3];

        [Pure]
        public static int ToInt32LittleEndian(byte[] bytes) => bytes[0] |
                                                               bytes[1] << 8 |
                                                               bytes[2] << 16 |
                                                               bytes[3] << 24;
    }
}
