#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
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

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Provides human-readable string versions of the tags stored in a JpegDirectory.</summary>
    /// <remarks>
    /// Provides human-readable string versions of the tags stored in a JpegDirectory.
    /// Thanks to Darrell Silver (www.darrellsilver.com) for the initial version of this class.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class JpegDescriptor : TagDescriptor<JpegDirectory>
    {
        public JpegDescriptor([NotNull] JpegDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case JpegDirectory.TagCompressionType:
                    return GetImageCompressionTypeDescription();
                case JpegDirectory.TagComponentData1:
                    return GetComponentDataDescription(0);
                case JpegDirectory.TagComponentData2:
                    return GetComponentDataDescription(1);
                case JpegDirectory.TagComponentData3:
                    return GetComponentDataDescription(2);
                case JpegDirectory.TagComponentData4:
                    return GetComponentDataDescription(3);
                case JpegDirectory.TagDataPrecision:
                    return GetDataPrecisionDescription();
                case JpegDirectory.TagImageHeight:
                    return GetImageHeightDescription();
                case JpegDirectory.TagImageWidth:
                    return GetImageWidthDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetImageCompressionTypeDescription()
        {
            return GetIndexedDescription(JpegDirectory.TagCompressionType,
                "Baseline",
                "Extended sequential, Huffman",
                "Progressive, Huffman",
                "Lossless, Huffman",
                null, // no 4
                "Differential sequential, Huffman",
                "Differential progressive, Huffman",
                "Differential lossless, Huffman",
                "Reserved for JPEG extensions",
                "Extended sequential, arithmetic",
                "Progressive, arithmetic",
                "Lossless, arithmetic",
                null, // no 12
                "Differential sequential, arithmetic",
                "Differential progressive, arithmetic",
                "Differential lossless, arithmetic");
        }

        [CanBeNull]
        public string GetImageWidthDescription()
        {
            var value = Directory.GetString(JpegDirectory.TagImageWidth);

            return value == null ? null : value + " pixels";
        }

        [CanBeNull]
        public string GetImageHeightDescription()
        {
            var value = Directory.GetString(JpegDirectory.TagImageHeight);

            return value == null ? null : value + " pixels";
        }

        [CanBeNull]
        public string GetDataPrecisionDescription()
        {
            var value = Directory.GetString(JpegDirectory.TagDataPrecision);

            return value == null ? null : value + " bits";
        }

        [CanBeNull]
        public string GetComponentDataDescription(int componentNumber)
        {
            var value = Directory.GetComponent(componentNumber);

            if (value == null)
                return null;

            return value.Name + " component: Quantization table "
                + value.QuantizationTableNumber + ", Sampling factors "
                + value.HorizontalSamplingFactor + " horiz/" + value.VerticalSamplingFactor + " vert";
        }
    }
}
