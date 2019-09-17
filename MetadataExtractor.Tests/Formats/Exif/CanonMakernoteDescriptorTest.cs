// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Exif.Makernotes;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Exif
{
    /// <summary>Unit tests for <see cref="CanonMakernoteDescriptor"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class CanonMakernoteDescriptorTest
    {
        [Fact, UseCulture("en-GB")]
        public void GetFlashBiasDescription()
        {
            var directory = new CanonMakernoteDirectory();
            var descriptor = new CanonMakernoteDescriptor(directory);
            // set and check values
            directory.Set(CanonMakernoteDirectory.FocalLength.TagFlashBias, 0xFFC0);
            Assert.Equal("-2.0 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.Set(CanonMakernoteDirectory.FocalLength.TagFlashBias, 0xffd4);
            Assert.Equal("-1.375 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.Set(CanonMakernoteDirectory.FocalLength.TagFlashBias, 0x0000);
            Assert.Equal("0.0 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.Set(CanonMakernoteDirectory.FocalLength.TagFlashBias, 0x000c);
            Assert.Equal("0.375 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.Set(CanonMakernoteDirectory.FocalLength.TagFlashBias, 0x0010);
            Assert.Equal("0.5 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.Set(CanonMakernoteDirectory.FocalLength.TagFlashBias, 0x0014);
            Assert.Equal("0.625 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.Set(CanonMakernoteDirectory.FocalLength.TagFlashBias, 0x0020);
            Assert.Equal("1.0 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.Set(CanonMakernoteDirectory.FocalLength.TagFlashBias, 0x0030);
            Assert.Equal("1.5 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.Set(CanonMakernoteDirectory.FocalLength.TagFlashBias, 0x0034);
            Assert.Equal("1.625 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.Set(CanonMakernoteDirectory.FocalLength.TagFlashBias, 0x0040);
            Assert.Equal("2.0 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
        }
    }
}
