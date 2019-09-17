// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text;
using MetadataExtractor.Formats.Png;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Png
{
    /// <summary>Unit tests for <see cref="PngDescriptor"/>.</summary>
    /// <author>Akihiko Kusanagi</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public sealed class PngDescriptorTest
    {
        [Fact]
        public void GetColorTypeDescription()
        {
            PngDirectory directory = new PngDirectory(PngChunkType.IHDR);
            PngDescriptor descriptor = new PngDescriptor(directory);

            directory.Set(PngDirectory.TagColorType, 6);
            Assert.Equal("True Color with Alpha", descriptor.GetColorTypeDescription());
            Assert.Equal("True Color with Alpha", directory.GetDescription(PngDirectory.TagColorType));
        }

        [Fact]
        public void GetCompressionTypeDescription()
        {
            PngDirectory directory = new PngDirectory(PngChunkType.IHDR);
            PngDescriptor descriptor = new PngDescriptor(directory);

            directory.Set(PngDirectory.TagCompressionType, 0);
            Assert.Equal("Deflate", descriptor.GetCompressionTypeDescription());
            Assert.Equal("Deflate", directory.GetDescription(PngDirectory.TagCompressionType));
        }

        [Fact]
        public void GetFilterMethodDescription()
        {
            PngDirectory directory = new PngDirectory(PngChunkType.IHDR);
            PngDescriptor descriptor = new PngDescriptor(directory);

            directory.Set(PngDirectory.TagFilterMethod, 0);
            Assert.Equal("Adaptive", descriptor.GetFilterMethodDescription());
            Assert.Equal("Adaptive", directory.GetDescription(PngDirectory.TagFilterMethod));
        }

        [Fact]
        public void GetInterlaceMethodDescription()
        {
            PngDirectory directory = new PngDirectory(PngChunkType.IHDR);
            PngDescriptor descriptor = new PngDescriptor(directory);

            directory.Set(PngDirectory.TagInterlaceMethod, 1);
            Assert.Equal("Adam7 Interlace", descriptor.GetInterlaceMethodDescription());
            Assert.Equal("Adam7 Interlace", directory.GetDescription(PngDirectory.TagInterlaceMethod));
        }

        [Fact]
        public void GetPaletteHasTransparencyDescription()
        {
            PngDirectory directory = new PngDirectory(PngChunkType.tRNS);
            PngDescriptor descriptor = new PngDescriptor(directory);

            directory.Set(PngDirectory.TagPaletteHasTransparency, 1);
            Assert.Equal("Yes", descriptor.GetPaletteHasTransparencyDescription());
            Assert.Equal("Yes", directory.GetDescription(PngDirectory.TagPaletteHasTransparency));
        }

        [Fact]
        public void GetIsSrgbColorSpaceDescription()
        {
            PngDirectory directory = new PngDirectory(PngChunkType.sRGB);
            PngDescriptor descriptor = new PngDescriptor(directory);

            directory.Set(PngDirectory.TagSrgbRenderingIntent, 0);
            Assert.Equal("Perceptual", descriptor.GetIsSrgbColorSpaceDescription());
            Assert.Equal("Perceptual", directory.GetDescription(PngDirectory.TagSrgbRenderingIntent));
        }

        [Fact]
        public void GetUnitSpecifierDescription()
        {
            PngDirectory directory = new PngDirectory(PngChunkType.pHYs);
            PngDescriptor descriptor = new PngDescriptor(directory);

            directory.Set(PngDirectory.TagUnitSpecifier, 1);
            Assert.Equal("Metres", descriptor.GetUnitSpecifierDescription());
            Assert.Equal("Metres", directory.GetDescription(PngDirectory.TagUnitSpecifier));
        }
        
        [Fact]
        public void GetTextualDataDescription()
        {
            var _latin1Encoding = Encoding.GetEncoding("iso-8859-1"); // Latin-1

            var textPairs = new List<KeyValuePair>();
            StringValue value = new StringValue(_latin1Encoding.GetBytes("value"), _latin1Encoding);
            textPairs.Add(new KeyValuePair("keyword", value));

            PngDirectory directory = new PngDirectory(PngChunkType.tEXt);
            PngDescriptor descriptor = new PngDescriptor(directory);
            directory.Set(PngDirectory.TagTextualData, textPairs);
            Assert.Equal("keyword: value", descriptor.GetTextualDataDescription());
            Assert.Equal("keyword: value", directory.GetDescription(PngDirectory.TagTextualData));

            directory = new PngDirectory(PngChunkType.zTXt);
            descriptor = new PngDescriptor(directory);
            directory.Set(PngDirectory.TagTextualData, textPairs);
            Assert.Equal("keyword: value", descriptor.GetTextualDataDescription());
            Assert.Equal("keyword: value", directory.GetDescription(PngDirectory.TagTextualData));

            directory = new PngDirectory(PngChunkType.iTXt);
            descriptor = new PngDescriptor(directory);
            directory.Set(PngDirectory.TagTextualData, textPairs);
            Assert.Equal("keyword: value", descriptor.GetTextualDataDescription());
            Assert.Equal("keyword: value", directory.GetDescription(PngDirectory.TagTextualData));
        }

        [Fact]
        public void GetBackgroundColorDescription()
        {
            PngDirectory directory = new PngDirectory(PngChunkType.bKGD);
            PngDescriptor descriptor = new PngDescriptor(directory);

            directory.Set(PngDirectory.TagBackgroundColor, new byte[]{52});
            Assert.Equal("Palette Index 52", descriptor.GetBackgroundColorDescription());
            Assert.Equal("Palette Index 52", directory.GetDescription(PngDirectory.TagBackgroundColor));
            directory.Set(PngDirectory.TagBackgroundColor, new byte[]{0, 52});
            Assert.Equal("Greyscale Level 52", descriptor.GetBackgroundColorDescription());
            Assert.Equal("Greyscale Level 52", directory.GetDescription(PngDirectory.TagBackgroundColor));
            directory.Set(PngDirectory.TagBackgroundColor, new byte[]{0, 50, 0, 51, 0, 52});
            Assert.Equal("R 50, G 51, B 52", descriptor.GetBackgroundColorDescription());
            Assert.Equal("R 50, G 51, B 52", directory.GetDescription(PngDirectory.TagBackgroundColor));
        }
    }
}
