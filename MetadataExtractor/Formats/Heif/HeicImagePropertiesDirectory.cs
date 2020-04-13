using System.Collections.Generic;

namespace MetadataExtractor.Formats.Heif
{
    public class HeicImagePropertiesDirectory : Directory
    {
        public override string Name { get; }

        public HeicImagePropertiesDirectory(string name)
        {
            Name = name;
            SetDescriptor(new HeicImagePropertyDescriptor(this));
        }

        public const int TagImageWidth = 1;
        public const int TagImageHeight = 2;
        public const int TagRotation = 3;
        public const int TagPixelDepths = 4;
        public const int TagConfigurationVersion = 5;
        public const int TagGeneralProfileSpace = 6;
        public const int TagGeneralTierTag = 7;
        public const int TagGeneralProfileIdc = 8;
        public const int TagGeneralProfileCompatibilityTag = 9;
        public const int TagGeneralLevelIdc = 10;
        public const int TagMinSpacialSegmentationIdc = 11;
        public const int TagParallelismType = 12;
        public const int TagChromaFormat = 13;
        public const int TagBitDepthLuma = 14;
        public const int TagBitDepthChroma = 15;
        public const int TagAverageFrameRate = 16;
        public const int TagConstantFrameRate = 17;
        public const int TagNumTemporalLayers = 18;
        public const int TagLengthSize = 19;
        public const int TagColorPrimaries = 20;
        public const int TagColorTransferCharacteristics = 21;
        public const int TagColorMatrixCharacteristics = 22;
        public const int TagFullRangeColor = 23;
        public const int TagColorFormat = 24;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>()
        {
            { TagImageHeight, "Image Height" },
            { TagImageWidth, "Image Width" },
            { TagRotation, "Default Rotation" },
            { TagPixelDepths, "Pixel Depth in Bits" },
            { TagConfigurationVersion, "HEVC Configuration Version" },
            { TagGeneralProfileSpace, "General Profile Space" },
            { TagGeneralTierTag, "General Tiler Tag" },
            { TagGeneralProfileIdc, "General Profile" },
            { TagGeneralProfileCompatibilityTag, "General Profile Compatibility" },
            { TagGeneralLevelIdc, "General Level" },
            { TagMinSpacialSegmentationIdc, "Minimum Spacial Segmentation" },
            { TagParallelismType, "Parallelism Type" },
            { TagChromaFormat, "Chroma Format" },
            { TagBitDepthLuma, "Luma Bit Depth" },
            { TagBitDepthChroma, "Chroma Bit Depth" },
            { TagAverageFrameRate, "Average Frame Rate" },
            { TagConstantFrameRate, "Constant Frame Rate" },
            { TagNumTemporalLayers, "Number of Temporal Layers" },
            { TagLengthSize, "Length or Size" },
            { TagColorPrimaries, "Primary Color Definitions" },
            { TagColorTransferCharacteristics, "Optical Color Transfer Characteristic" },
            { TagColorMatrixCharacteristics, "Color Deviation Matrix Characteristics" },
            { TagFullRangeColor, "Full-Range Color" },
            { TagColorFormat, "Color Data Format" }
        };

        protected override bool TryGetTagName(int tagType, out string? tagName) =>
            _tagNameMap.TryGetValue(tagType, out tagName);
    }
}
