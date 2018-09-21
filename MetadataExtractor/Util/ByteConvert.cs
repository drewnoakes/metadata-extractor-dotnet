using System;
using System.Buffers.Binary;
using JetBrains.Annotations;
namespace MetadataExtractor.Util
{
    public static class ByteConvert
    {
        [Pure]
        public static uint FromBigEndianToNative(uint bigEndian)
        {
            return BitConverter.IsLittleEndian
               ? BinaryPrimitives.ReverseEndianness(bigEndian)
               : bigEndian;
        }

        [Pure]
        public static ushort FromBigEndianToNative(ushort bigEndian)
        {
            return BitConverter.IsLittleEndian
              ? BinaryPrimitives.ReverseEndianness(bigEndian)
              : bigEndian;
        }

        [Pure]
        public static short FromBigEndianToNative(short bigEndian)
        {
            return BitConverter.IsLittleEndian 
                ? BinaryPrimitives.ReverseEndianness(bigEndian)
                : bigEndian;
        }
    }
}