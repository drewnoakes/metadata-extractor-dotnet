// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Exif.Makernotes;
using MetadataExtractor.Formats.Jpeg;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Exif
{
    /// <summary>Unit tests for Sony (Type 1) maker notes.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class SonyType1MakernoteTest
    {
        [Fact]
        public void File1()
        {
            var directory = ExifReaderTest.ProcessSegmentBytes<SonyType1MakernoteDirectory>("Data/sonyType1.jpg.app1", JpegSegmentType.App1);
            Assert.NotNull(directory);
            Assert.False(directory.HasError);
            var descriptor = new SonyType1MakernoteDescriptor(directory);
            Assert.Null(directory.GetObject(SonyType1MakernoteDirectory.TagColorTemperature));
            Assert.Null(descriptor.GetColorTemperatureDescription());
            Assert.Null(directory.GetObject(SonyType1MakernoteDirectory.TagSceneMode));
            Assert.Null(descriptor.GetSceneModeDescription());
            Assert.Null(directory.GetObject(SonyType1MakernoteDirectory.TagZoneMatching));
            Assert.Null(descriptor.GetZoneMatchingDescription());
            Assert.Null(directory.GetObject(SonyType1MakernoteDirectory.TagDynamicRangeOptimiser));
            Assert.Null(descriptor.GetDynamicRangeOptimizerDescription());
            Assert.Null(directory.GetObject(SonyType1MakernoteDirectory.TagImageStabilisation));
            Assert.Null(descriptor.GetImageStabilizationDescription());
            Assert.Null(directory.GetObject(SonyType1MakernoteDirectory.TagColorMode));
            Assert.Null(descriptor.GetColorModeDescription());
            Assert.Equal("On (Shooting)", descriptor.GetAntiBlurDescription());
            Assert.Equal("Program", descriptor.GetExposureModeDescription());
            Assert.Equal("Off", descriptor.GetLongExposureNoiseReductionDescription());
            Assert.Equal("Off", descriptor.GetMacroDescription());
            Assert.Equal("Normal", descriptor.GetJpegQualityDescription());
        }
    }
}
