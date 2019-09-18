// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Exif.Makernotes;
using MetadataExtractor.Formats.Jpeg;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Exif
{
    /// <summary>Unit tests for Panasonic maker notes.</summary>
    /// <author>psandhaus</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <seealso cref="PanasonicMakernoteDirectory"/>
    /// <seealso cref="PanasonicMakernoteDescriptor"/>
    public sealed class PanasonicMakernoteTest
    {
        private readonly PanasonicMakernoteDirectory _panasonicDirectory;

        public PanasonicMakernoteTest()
        {
            _panasonicDirectory = ExifReaderTest.ProcessSegmentBytes<PanasonicMakernoteDirectory>("Data/withPanasonicFaces.jpg.app1", JpegSegmentType.App1);
        }

        [Fact]
        public void GetDetectedFaces()
        {
            Assert.Equal(
                new[] { new Face(142, 120, 76, 76) },
                _panasonicDirectory.GetDetectedFaces());
        }

        [Fact]
        public void GetRecognizedFaces()
        {
            Assert.Equal(
                new[] { new Face(142, 120, 76, 76, "NIELS", new Age(31, 7, 15, 0, 0, 0)) },
                _panasonicDirectory.GetRecognizedFaces());
        }
    }
}
