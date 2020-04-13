// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496
{
    public class BitsReader
    {
        private readonly SequentialReader _source;

        private byte _mask = 0;
        private byte _currentByte = 0;

        public BitsReader(SequentialReader source)
        {
            _source = source;
        }

        public ulong GetUInt64(int bits)
        {
            Debug.Assert(bits <= 64);
            ulong ret = 0;
            for (int i = 0; i < bits; i++)
            {
                ret <<= 1;
                if (GetBit())
                {
                    ret |= 1;
                }
            }

            return ret;
        }

        public uint GetUInt32(int bits)
        {
            Debug.Assert(bits <= 32);
            return (uint)GetUInt64(bits);
        }

        public ushort GetUInt16(int bits)
        {
            Debug.Assert(bits <= 32);
            return (ushort)GetUInt64(bits);
        }

        public byte GetByte(int bits)
        {
            Debug.Assert(bits <= 8);
            return (byte)GetUInt64(bits);
        }

        public bool GetBit()
        {
            if (_mask == 0)
                ReadWholeByteFromSource();
            var ret = (_mask & _currentByte) == _mask;
            _mask >>= 1;
            return ret;
        }

        private void ReadWholeByteFromSource()
        {
            _currentByte = _source.GetByte();
            _mask = 0b1000_0000;
        }
    }
}
