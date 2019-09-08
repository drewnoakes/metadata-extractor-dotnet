#region License
//
// Copyright 2002-2019 Drew Noakes
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

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