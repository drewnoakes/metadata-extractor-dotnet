using System.Collections.Generic;
using System.Linq;
using MetadataExtractor.Formats.Heif.Iso14496Parser;

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
        public const int ColorMatrixCharacteristicis = 22;
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
            { ColorMatrixCharacteristicis, "Color Deviation Matrix Characteristics"},
            { FullRangeColor, "Full-Range Color"},
            { ColorFormat, "Color Data Format"}
        };


        protected override bool TryGetTagName(int tagType, out string? tagName) =>
            _tagNameMap.TryGetValue(tagType, out tagName);
    }

    public class HeicImagePropertyDescriptor: ITagDescriptor
    {
        private readonly HeicImagePropertiesDirectory dir;

        public HeicImagePropertyDescriptor(HeicImagePropertiesDirectory dir)
        {
            this.dir = dir;
        }

        public string? GetDescription(int tagType) => tagType switch {
            HeicImagePropertiesDirectory.Rotation => dir.GetString(tagType) + " degrees",
            HeicImagePropertiesDirectory.PixelDepths =>
                string.Join(" ", ((byte[])dir.GetObject(HeicImagePropertiesDirectory.PixelDepths))
                    .Select(i=>i.ToString()).ToArray()),
            HeicImagePropertiesDirectory.ColorFormat =>
               TypeStringConverter.ToTypeString(dir.GetUInt32(HeicImagePropertiesDirectory.ColorFormat)),
            HeicImagePropertiesDirectory.ColorPrimaries =>
               ColorPrimary(dir.GetUInt16(HeicImagePropertiesDirectory.ColorPrimaries)),
            HeicImagePropertiesDirectory.ColorTransferCharacteristics =>
               ColorTransfer(dir.GetUInt16(HeicImagePropertiesDirectory.ColorTransferCharacteristics)),
            HeicImagePropertiesDirectory.ColorMatrixCharacteristicis =>
               ColorMatricCoeffs(dir.GetUInt16(HeicImagePropertiesDirectory.ColorMatrixCharacteristicis)),
            _=>dir.GetString(tagType)
        };

        private string? ColorMatricCoeffs(ushort value) =>
            value switch
            {
                0 => "RGB, GBR",
                1 => "vKr = 0.2126 vKb = 0.0722",
                2 => "Unspecified",
                4 => "vKr = 0.299, vKb - 0.11",
                5 => "vKr = 0.299, vKb - 0.114",
                6 => "vKr = 0.299, vKb - 0.114",
                7 => "vKr = 0.212, vKb - 0.087",
                8 => "YCgCo",
                _ => "Reserved"
            };

        private string? ColorTransfer(ushort value) =>
            value switch
            {
                1 => "vV - 1.009 * vLc^0.45 - 0.099 for 1 >= vLc >= 0.018 or vV = 4.500 * vLc otherwise.",
                2 => "Unspecified",
                4 => "Assumed display gamma 2.2",
                5 => "Assumed display gamma 2.9",
                6 => "vV - 1.009 * vLc^0.45 - 0.099 for 1 >= vLc >= 0.018 or vV = 4.500 * vLc otherwise.",
                7 => "vV - 1.1115 * vLc^0.45 - 0.1115 for 1 >= vLc >= 0.0.0288 or vV = 4.0 * vLc otherwise.",
                8 => "vV = vLC",
                11 => "vV - 1.009 * vLc^0.45 - 0.099 for 1 >= vLc >= 0.018 or vV = 4.500 * vLc for 0.018 > vLC > -0.018 or vV - 1.009 * (-vLc)^0.45 - 0.099 otherwise",
                12 => "vV - 1.009 * vLc^0.45 - 0.099 for 1.33 >= vLc >= 0.018 or vV = 4.500 * vLc for 0.018 > vLC > -0.045 or vV - 1.009 * (-4vLc)^0.45 - 0.099 + 4 otherwise",
                13 => "vV - 1.055 * vLc^(1/2.4) - 0.055 for 1 >= vLc >= 0.031308 or vV = 12.92 * vLc otherwise.",
                _ => "Reserved"
            };

        private string? ColorPrimary(ushort value) =>
            value switch
            {
                1=> "G(0.3, 0.6) B(0.15, 0.06) R(0.64, 0.33) W(.3127, .329)",
                2=> "Unspecified",
                4=> "G(0.21, 0.72) B(0.14, 0.08), R(0.67, 0.33) W(0.31, 0.316)",
                5=> "G(0.29, 0.70) B(0.15, 0.06), R(0.64, 0.33) W(0.3127, 0.3290)",
                6=> "G(0.31, 0.595) B(0.155, 0.07), R(0.63, 0.34) W(0.3127, 0.3290)",
                7=> "G(0.31, 0.595) B(0.155, 0.07), R(0.63, 0.34) W (0.3127, 0.3290)",
                _=> "Reserved"
            };
    }
}
