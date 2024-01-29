// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Buffers.Binary;

namespace MetadataExtractor.Util
{
    public static class ByteConvert
    {
        public static uint FromBigEndianToNative(uint bigEndian)
        {
            return BitConverter.IsLittleEndian
               ? BinaryPrimitives.ReverseEndianness(bigEndian)
               : bigEndian;
        }

        public static ushort FromBigEndianToNative(ushort bigEndian)
        {
            return BitConverter.IsLittleEndian
              ? BinaryPrimitives.ReverseEndianness(bigEndian)
              : bigEndian;
        }

        public static short FromBigEndianToNative(short bigEndian)
        {
            return BitConverter.IsLittleEndian
                ? BinaryPrimitives.ReverseEndianness(bigEndian)
                : bigEndian;
        }
    }
}
