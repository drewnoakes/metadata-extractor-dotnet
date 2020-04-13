using System.Diagnostics;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496Parser
{
    public class BitsReader
    {
        private readonly SequentialReader source;

        public BitsReader(SequentialReader source)
        {
            this.source = source;
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

        private byte mask = 0;
        private byte currentByte = 0;

        public bool GetBit()
        {
            if (mask == 0)
                ReadWholeByteFromSource();
            var ret = (mask & currentByte) == mask;
            mask >>= 1;
            return ret;
        }

        private void ReadWholeByteFromSource()
        {
            currentByte = source.GetByte();
            mask = 128;
        }
    }
}
