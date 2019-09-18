// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.IO;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PngChromaticities
    {
        public int WhitePointX { get; }
        public int WhitePointY { get; }
        public int RedX { get; }
        public int RedY { get; }
        public int GreenX { get; }
        public int GreenY { get; }
        public int BlueX { get; }
        public int BlueY { get; }

        /// <exception cref="PngProcessingException"/>
        public PngChromaticities(byte[] bytes)
        {
            if (bytes.Length != 8*4)
                throw new PngProcessingException("Invalid number of bytes");

            var reader = new SequentialByteArrayReader(bytes);

            try
            {
                WhitePointX = reader.GetInt32();
                WhitePointY = reader.GetInt32();
                RedX = reader.GetInt32();
                RedY = reader.GetInt32();
                GreenX = reader.GetInt32();
                GreenY = reader.GetInt32();
                BlueX = reader.GetInt32();
                BlueY = reader.GetInt32();
            }
            catch (IOException ex)
            {
                throw new PngProcessingException(ex);
            }
        }
    }
}
