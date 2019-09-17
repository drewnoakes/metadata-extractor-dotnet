// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Exif
{
    /// <summary>Unit tests for <see cref="ExifIfd0Descriptor"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class ExifIfd0DescriptorTest
    {
        [Fact]
        public void XResolutionDescription()
        {
            var directory = new ExifIfd0Directory();
            directory.Set(ExifDirectoryBase.TagXResolution, new Rational(72, 1));
            // 2 is for 'Inch'
            directory.Set(ExifDirectoryBase.TagResolutionUnit, 2);

            var descriptor = new ExifIfd0Descriptor(directory);
            Assert.Equal("72 dots per inch", descriptor.GetDescription(ExifDirectoryBase.TagXResolution));
        }

        [Fact]
        public void YResolutionDescription()
        {
            var directory = new ExifIfd0Directory();
            directory.Set(ExifDirectoryBase.TagYResolution, new Rational(50, 1));
            // 3 is for 'cm'
            directory.Set(ExifDirectoryBase.TagResolutionUnit, 3);

            var descriptor = new ExifIfd0Descriptor(directory);
            Assert.Equal("50 dots per cm", descriptor.GetDescription(ExifDirectoryBase.TagYResolution));
        }

        [Fact]
        public void WindowsXpFields()
        {
            var directory = ExifReaderTest.ProcessSegmentBytes<ExifIfd0Directory>("Data/windowsXpFields.jpg.app1", JpegSegmentType.App1);
            Assert.Equal("Testing artist\x0", directory.GetString(ExifDirectoryBase.TagWinAuthor, Encoding.Unicode));
            Assert.Equal("Testing comments\x0", directory.GetString(ExifDirectoryBase.TagWinComment, Encoding.Unicode));
            Assert.Equal("Testing keywords\x0", directory.GetString(ExifDirectoryBase.TagWinKeywords, Encoding.Unicode));
            Assert.Equal("Testing subject\x0", directory.GetString(ExifDirectoryBase.TagWinSubject, Encoding.Unicode));
            Assert.Equal("Testing title\x0", directory.GetString(ExifDirectoryBase.TagWinTitle, Encoding.Unicode));

            var descriptor = new ExifIfd0Descriptor(directory);
            Assert.Equal("Testing artist", descriptor.GetDescription(ExifDirectoryBase.TagWinAuthor));
            Assert.Equal("Testing comments", descriptor.GetDescription(ExifDirectoryBase.TagWinComment));
            Assert.Equal("Testing keywords", descriptor.GetDescription(ExifDirectoryBase.TagWinKeywords));
            Assert.Equal("Testing subject", descriptor.GetDescription(ExifDirectoryBase.TagWinSubject));
            Assert.Equal("Testing title", descriptor.GetDescription(ExifDirectoryBase.TagWinTitle));
        }
    }
}
