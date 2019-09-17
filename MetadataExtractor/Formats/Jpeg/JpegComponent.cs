// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#if !NETSTANDARD1_3
using System;
#endif

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
        public string Name
        {
            get
            {
                return Id switch
                {
                    1 => "Y",
                    2 => "Cb",
                    3 => "Cr",
                    4 => "I",
                    5 => "Q",
                    _ => $"Unknown ({Id})",
                };
            }
        }

        public int HorizontalSamplingFactor => (_samplingFactorByte >> 4) & 0x0F;

        public int VerticalSamplingFactor => _samplingFactorByte & 0x0F;

        public override string ToString()
            => $"Quantization table {QuantizationTableNumber}, Sampling factors {HorizontalSamplingFactor} horiz/{VerticalSamplingFactor} vert";
    }
}
