// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.GeoTiff;
using MetadataExtractor.Formats.Tiff;
using Xunit;

namespace MetadataExtractor.Tests.Formats.GeoTiff
{
    public sealed class GeoTiffTest
    {
        #region Helpers
        private static ExifIfd0Directory CheckExif(IEnumerable<Directory> metadata, int numberOfTags)
        {
            var exif = metadata.OfType<ExifIfd0Directory>().FirstOrDefault();
            Assert.NotNull(exif);
            Assert.False(exif.HasError);
            Assert.Equal(numberOfTags, exif.TagCount);
            return exif;
        }

        private static GeoTiffDirectory CheckGeoTiff(IEnumerable<Directory> metadata)
        {
            var geotiff = metadata.OfType<GeoTiffDirectory>().FirstOrDefault();
            Assert.NotNull(geotiff);
            Assert.False(geotiff.HasError);
            Assert.True(geotiff.TagCount > 0);
            return geotiff;
        }

        #endregion

        [Fact]
        public void TestGeogToWGS84GeoKey5()
        {
            IEnumerable<Directory> metadata = TiffMetadataReader.ReadMetadata("Data/GeogToWGS84GeoKey5.tif");
            Assert.NotNull(metadata);

            ExifIfd0Directory exif = CheckExif(metadata, 23);

            //Assert.Equal("[32 values]", exif.GetDescription(ExifDirectoryBase.TagGeoTiffGeoKeys));
            Assert.Equal("[768 values]", exif.GetDescription(ExifDirectoryBase.TagColorMap));
            Assert.Equal("0 0 1", exif.GetDescription(ExifDirectoryBase.TagPixelScale));
            Assert.Equal("50.5 50.5 0 9.001 52.001 0", exif.GetDescription(ExifDirectoryBase.TagModelTiePoint).Replace(',', '.'));
            Assert.Null(exif.GetDescription(ExifDirectoryBase.TagGeoTiffGeoAsciiParams));
            Assert.Null(exif.GetDescription(ExifDirectoryBase.TagGeoTiffGeoDoubleParams));
            Assert.Null(exif.GetDescription(ExifDirectoryBase.TagGdalMetadata));
            Assert.Null(exif.GetDescription(ExifDirectoryBase.TagGdalNoData));

            GeoTiffDirectory geotiff = CheckGeoTiff(metadata);

            Assert.Equal("Geographic", geotiff.GetDescription(GeoTiffDirectory.TagModelType));
            Assert.Equal("PixelIsArea", geotiff.GetDescription(GeoTiffDirectory.TagRasterType));
            Assert.Equal("User Defined", geotiff.GetDescription(GeoTiffDirectory.TagGeographicType));
            Assert.Equal("User Defined", geotiff.GetDescription(GeoTiffDirectory.TagGeodeticDatum));
            Assert.Equal("Angular Degree", geotiff.GetDescription(GeoTiffDirectory.TagGeographicAngularUnits));
            Assert.Equal("Bessel 1841", geotiff.GetDescription(GeoTiffDirectory.TagGeographicEllipsoid));
            Assert.Equal("598.1 73.7 418.2 0.202 0.045 -2.455 6.7", geotiff.GetDescription(GeoTiffDirectory.TagGeographicToWgs84).Replace(',', '.'));
            Assert.Equal(7, geotiff.TagCount);
        }

        [Fact]
        public void TestLibgeotiff()
        {
            foreach (string tiffFile in System.IO.Directory.GetFiles("Data/libgeotiff", "*.tif"))
            {
                IEnumerable<Directory> metadata = TiffMetadataReader.ReadMetadata(tiffFile);
                Assert.NotNull(metadata);
                CheckExif(metadata, 13);
                var description = tiffFile + "\n  " + CheckGeoTiff(metadata).Tags;
                Assert.DoesNotContain(description, "Unknown");
            }
        }
    }
}
