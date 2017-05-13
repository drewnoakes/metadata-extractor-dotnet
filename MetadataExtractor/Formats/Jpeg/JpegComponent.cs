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

#if !NETSTANDARD1_3
using System;
#endif
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>
    /// Stores information about a JPEG image component such as the component id, horiz/vert sampling factor and
    /// quantization table number.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
#if !NETSTANDARD1_3
    [Serializable]
#endif
    public sealed class JpegComponent
    {
        private readonly byte _samplingFactorByte;

        public JpegComponent(byte componentId, byte samplingFactorByte, byte quantizationTableNumber)
        {
            Id = componentId;
            _samplingFactorByte = samplingFactorByte;
            QuantizationTableNumber = quantizationTableNumber;
        }

        public byte Id { get; }

        public byte QuantizationTableNumber { get; }

        /// <summary>Returns the component name (one of: Y, Cb, Cr, I, or Q)</summary>
        /// <value>the component name</value>
        [NotNull]
        public string Name
        {
            get
            {
                switch (Id)
                {
                    case 1:
                        return "Y";
                    case 2:
                        return "Cb";
                    case 3:
                        return "Cr";
                    case 4:
                        return "I";
                    case 5:
                        return "Q";
                    default:
                        return $"Unknown ({Id})";
                }
            }
        }

        public int HorizontalSamplingFactor => (_samplingFactorByte >> 4) & 0x0F;

        public int VerticalSamplingFactor => _samplingFactorByte & 0x0F;
    }
}
