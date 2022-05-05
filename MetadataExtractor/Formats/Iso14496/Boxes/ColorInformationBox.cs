// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Iso14496.Boxes
{
    internal sealed class ColorInformationBox : Box
    {
        public const uint NclxTag = 0x6E636C78; // nclx
        public const uint RICCTag = 0x72494343; // rICC
        public const uint ProfTag = 0x70726F66; // prof

        public uint ColorType { get; }
        public ushort ColorPrimaries { get; }
        public ushort TransferCharacteristics { get; }
        public ushort MatrixCharacteristics { get; }
        public bool FullRangeFlag { get; }
        public byte[] IccProfile { get; }

        public ColorInformationBox(BoxLocation location, SequentialReader sr)
            : base(location)
        {
            ColorType = sr.GetUInt32();

            byte[] emptyByteArray = System.Array.Empty<byte>();
            switch (ColorType)
            {
                case NclxTag:
                {
                    ColorPrimaries = sr.GetUInt16();
                    TransferCharacteristics = sr.GetUInt16();
                    MatrixCharacteristics = sr.GetUInt16();
                    FullRangeFlag = (sr.GetByte() & 128) == 128;
                    IccProfile = emptyByteArray;
                    break;
                }
                case RICCTag:
                case ProfTag:
                {
                    IccProfile = ReadRemainingData(sr);
                    break;
                }
                default:
                {
                    IccProfile = emptyByteArray;
                    break;
                }
            }
        }
    }
}
