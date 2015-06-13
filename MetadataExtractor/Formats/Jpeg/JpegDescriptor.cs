/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Provides human-readable string versions of the tags stored in a JpegDirectory.</summary>
    /// <remarks>
    /// Provides human-readable string versions of the tags stored in a JpegDirectory.
    /// Thanks to Darrell Silver (www.darrellsilver.com) for the initial version of this class.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
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
                {
                    return GetImageCompressionTypeDescription();
                }

                case JpegDirectory.TagComponentData1:
                {
                    return GetComponentDataDescription(0);
                }

                case JpegDirectory.TagComponentData2:
                {
                    return GetComponentDataDescription(1);
                }

                case JpegDirectory.TagComponentData3:
                {
                    return GetComponentDataDescription(2);
                }

                case JpegDirectory.TagComponentData4:
                {
                    return GetComponentDataDescription(3);
                }

                case JpegDirectory.TagDataPrecision:
                {
                    return GetDataPrecisionDescription();
                }

                case JpegDirectory.TagImageHeight:
                {
                    return GetImageHeightDescription();
                }

                case JpegDirectory.TagImageWidth:
                {
                    return GetImageWidthDescription();
                }

                default:
                {
                    return base.GetDescription(tagType);
                }
            }
        }

        [CanBeNull]
        public string GetImageCompressionTypeDescription()
        {
            var value = Directory.GetInt32Nullable(JpegDirectory.TagCompressionType);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case 0:
                {
                    // Note there is no 2 or 12
                    return "Baseline";
                }

                case 1:
                {
                    return "Extended sequential, Huffman";
                }

                case 2:
                {
                    return "Progressive, Huffman";
                }

                case 3:
                {
                    return "Lossless, Huffman";
                }

                case 5:
                {
                    return "Differential sequential, Huffman";
                }

                case 6:
                {
                    return "Differential progressive, Huffman";
                }

                case 7:
                {
                    return "Differential lossless, Huffman";
                }

                case 8:
                {
                    return "Reserved for JPEG extensions";
                }

                case 9:
                {
                    return "Extended sequential, arithmetic";
                }

                case 10:
                {
                    return "Progressive, arithmetic";
                }

                case 11:
                {
                    return "Lossless, arithmetic";
                }

                case 13:
                {
                    return "Differential sequential, arithmetic";
                }

                case 14:
                {
                    return "Differential progressive, arithmetic";
                }

                case 15:
                {
                    return "Differential lossless, arithmetic";
                }

                default:
                {
                    return "Unknown type: " + value;
                }
            }
        }

        [CanBeNull]
        public string GetImageWidthDescription()
        {
            var value = Directory.GetString(JpegDirectory.TagImageWidth);
            if (value == null)
            {
                return null;
            }
            return value + " pixels";
        }

        [CanBeNull]
        public string GetImageHeightDescription()
        {
            var value = Directory.GetString(JpegDirectory.TagImageHeight);
            if (value == null)
            {
                return null;
            }
            return value + " pixels";
        }

        [CanBeNull]
        public string GetDataPrecisionDescription()
        {
            var value = Directory.GetString(JpegDirectory.TagDataPrecision);
            if (value == null)
            {
                return null;
            }
            return value + " bits";
        }

        [CanBeNull]
        public string GetComponentDataDescription(int componentNumber)
        {
            var value = Directory.GetComponent(componentNumber);
            if (value == null)
            {
                return null;
            }
            return value.GetComponentName() + " component: Quantization table " + value.GetQuantizationTableNumber() + ", Sampling factors " + value.GetHorizontalSamplingFactor() + " horiz/" + value.GetVerticalSamplingFactor() + " vert";
        }
    }
}
