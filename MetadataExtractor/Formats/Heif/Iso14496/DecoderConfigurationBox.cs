// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496
{
    public class DecoderConfigurationBox : Box
    {
        public byte ConfigurationVersion { get; }
        public byte GeneralProfileSpace { get; }
        public byte GeneralTierTag { get; }
        public byte GeneralProfileIdc { get; }
        public uint GeneralProfileCompatibilityFlags { get; }
        public byte[] GeneralConstraintIndicatorFlags { get; } = new byte[6];
        public byte GeneralLevelIdc { get; }
        public ushort MinSpacialSegmentationIdc { get; }
        public byte ParallelismType { get; }

        public byte ChromaFormat { get; }

        /*
         These fields appear in the standard document but are never read in the parser
        public ushort PicWidthInLumaSamples { get; }
        public ushort PicHeightInLumaSamples { get; }
        public ushort ConfWinLeftOffset { get; }
        public ushort ConfWinRightOffset { get; }
        public ushort ConfWinTopOffset { get; }
        public ushort ConfWinBottomOffset { get; }
        */
        public byte BitDepthLumaMinus8 { get; }
        public byte BitDepthChromaMinus8 { get; }
        public ushort AvgFrameRate { get; }
        public byte ConstantFrameRate { get; }
        public byte NumTemporalLayers { get; }
        public byte TemporalIdNested { get; }
        public byte LengthSizeMinus1 { get; }

        public DecoderConfigurationBox(BoxLocation location, SequentialReader sr) : base(location)
        {
            var bitReader = new BitReader(sr);
            ConfigurationVersion = bitReader.GetByte(8);
            GeneralProfileSpace = bitReader.GetByte(2);
            GeneralTierTag = bitReader.GetByte(1);
            GeneralProfileIdc = bitReader.GetByte(5);
            GeneralProfileCompatibilityFlags = bitReader.GetUInt32(32);
            for (int i = 0; i < 6; i++)
            {
                GeneralConstraintIndicatorFlags[i] = bitReader.GetByte(8);
            }

            GeneralLevelIdc = bitReader.GetByte(8);
            bitReader.GetUInt32(4); // reserved should be all 1's
            MinSpacialSegmentationIdc = bitReader.GetUInt16(12);
            bitReader.GetUInt32(6); // reserved should be all 1's
            ParallelismType = bitReader.GetByte(2);
            bitReader.GetUInt32(6); // reserved should be all 1's
            ChromaFormat = bitReader.GetByte(2);
            bitReader.GetUInt32(5); // reserved should be all 1's
            BitDepthLumaMinus8 = bitReader.GetByte(3);
            bitReader.GetUInt32(5); // reserved should be all 1's
            BitDepthChromaMinus8 = bitReader.GetByte(3);
            AvgFrameRate = bitReader.GetUInt16(16);
            ConstantFrameRate = bitReader.GetByte(2);
            NumTemporalLayers = bitReader.GetByte(3);
            TemporalIdNested = bitReader.GetByte(1);
            LengthSizeMinus1 = bitReader.GetByte(2);
        }
    }
}
