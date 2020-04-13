using System.Collections.Generic;

namespace MetadataExtractor.Formats.Heif
{
    public class HeicImagePropertiesDirectory: Directory
    {
        public override string Name { get; }

        public HeicImagePropertiesDirectory(string name)
        {
            Name = name;
            SetDescriptor(new HeicImagePropertyDescriptor(this));
        }

        public const int ImageWidth = 1;
        public const int ImageHeight = 2;
        public const int Rotation = 3;
        public const int PixelDepths = 4;
        public const int ConfigurationVersion = 5;
        public const int GeneralProfileSpace = 6;
        public const int GeneralTierTag = 7;
        public const int GeneralProfileIdc = 8;
        public const int GeneralProfileCompatibilityTag = 9;
        public const int GeneralLevelIdc = 10;
        public const int MinSpacialSegmentationIdc = 11;
        public const int ParallelismType = 12;
        public const int ChromaFormat = 13;
        public const int BitDepthLuma = 14;
        public const int BitDepthChroma = 15;
        public const int AverageFrameRate = 16;
        public const int ConstantFrameRate = 17;
        public const int NumTemporalLayers=18;
        public const int LengthSize = 19;
        public const int ColorPrimaries = 20;
        public const int ColorTransferCharacteristics = 21;
        public const int ColorMatrixCharacteristics = 22;
        public const int FullRangeColor = 23;
        public const int ColorFormat = 24;
        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>()
        {
            { ImageHeight, "Image Height" },
            { ImageWidth, "Image Width" },
            { Rotation, "Default Rotation"},
            { PixelDepths, "Pixel Depth in Bits"},
            { ConfigurationVersion, "HEVC Configuration Version"},
            { GeneralProfileSpace, "General Profile Space"},
            { GeneralTierTag, "General Tiler Tag"},
            { GeneralProfileIdc, "General Profile"},
            { GeneralProfileCompatibilityTag, "General Profile Compatibility"},
            { GeneralLevelIdc, "General Level"},
            { MinSpacialSegmentationIdc, "Minimum Spacial Segmentation"},
            { ParallelismType, "Parallelism Type"},
            { ChromaFormat, "Chroma Format"},
            { BitDepthLuma, "Luma Bit Depth"},
            { BitDepthChroma, "Chroma Bit Depth"},
            { AverageFrameRate, "Average Frame Rate"},
            { ConstantFrameRate, "Constant Frame Rate"},
            { NumTemporalLayers, "Number of Temporal Layers"},
            { LengthSize, "Length or Size"},
            { ColorPrimaries, "Primary Color Definitions"},
            { ColorTransferCharacteristics, "Optical Color Transfer Characteristic"},
            { ColorMatrixCharacteristics, "Color Deviation Matrix Characteristics"},
            { FullRangeColor, "Full-Range Color"},
            { ColorFormat, "Color Data Format"}
        };


        protected override bool TryGetTagName(int tagType, out string? tagName) =>
            _tagNameMap.TryGetValue(tagType, out tagName);
    }
}
