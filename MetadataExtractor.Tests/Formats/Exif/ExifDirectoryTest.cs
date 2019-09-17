// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Linq;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Exif
{
    /// <summary>
    /// Unit tests for <see cref="ExifSubIfdDirectory"/>, <see cref="ExifIfd0Directory"/>, <see cref="ExifThumbnailDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class ExifDirectoryTest
    {
        [Fact]
        public void GetDirectoryName()
        {
            Directory subIfdDirectory = new ExifSubIfdDirectory();
            Directory ifd0Directory = new ExifIfd0Directory();
            Directory thumbDirectory = new ExifThumbnailDirectory();
            Assert.False(subIfdDirectory.HasError);
            Assert.False(ifd0Directory.HasError);
            Assert.False(thumbDirectory.HasError);
            Assert.Equal("Exif IFD0", ifd0Directory.Name);
            Assert.Equal("Exif SubIFD", subIfdDirectory.Name);
            Assert.Equal("Exif Thumbnail", thumbDirectory.Name);
        }

        [Fact]
        public void Resolution()
        {
            var directories = ExifReaderTest.ProcessSegmentBytes("Data/withUncompressedRGBThumbnail.jpg.app1", JpegSegmentType.App1);

            var thumbnailDirectory = directories.OfType<ExifThumbnailDirectory>().FirstOrDefault();
            Assert.NotNull(thumbnailDirectory);
            Assert.Equal(72, thumbnailDirectory.GetInt32(ExifDirectoryBase.TagXResolution));

            var exifIfd0Directory = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
            Assert.NotNull(exifIfd0Directory);
            Assert.Equal(216, exifIfd0Directory.GetInt32(ExifDirectoryBase.TagXResolution));
        }

        [Fact]
        public void GeoLocation()
        {
            var gpsDirectory = ExifReaderTest.ProcessSegmentBytes<GpsDirectory>("Data/withExifAndIptc.jpg.app1.0", JpegSegmentType.App1);
            var geoLocation = gpsDirectory.GetGeoLocation();
            Assert.NotNull(geoLocation);
            Assert.Equal(54.989666666666665, geoLocation!.Latitude);
            Assert.Equal(-1.9141666666666666, geoLocation!.Longitude);
        }

        [Fact]
        public void GpsDate()
        {
            var gpsDirectory = ExifReaderTest.ProcessSegmentBytes<GpsDirectory>("Data/withPanasonicFaces.jpg.app1", JpegSegmentType.App1);
            Assert.Equal("2010:06:24", gpsDirectory.GetString(GpsDirectory.TagDateStamp));
            Assert.Equal("10/1 17/1 21/1", gpsDirectory.GetString(GpsDirectory.TagTimeStamp));
            Assert.True(gpsDirectory.TryGetGpsDate(out DateTime gpsDate));
            Assert.Equal(DateTimeKind.Utc, gpsDate.Kind);
            Assert.Equal(new DateTime(2010, 6, 24, 10, 17, 21), gpsDate);
        }
    }
}
