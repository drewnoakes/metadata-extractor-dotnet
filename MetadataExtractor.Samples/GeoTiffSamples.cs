// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Linq;
using GeographicLib;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.GeoTiff;

#pragma warning disable format

namespace MetadataExtractor.Samples
{
    internal static class GeoTiffSamples
    {
        public static void Main(string[] args)
        {
            // This sample determines the bounds of a GeoTIFF image in terms of latitude and
            // longitude.

            if (args.Length != 1)
            {
                Console.Error.WriteLine("Pass a path as a single argument.");
                return;
            }

            var directories = ImageMetadataReader.ReadMetadata(args[0]);

            // There are multiple ways that a GeoTIFF image can specify its mapping from
            // pixel space to model space (e.g. UTM WGS84).
            //
            // The most common approach is to specify one or more "tie points" as well
            // as "pixel scale" values. That approach will be used here.
            //
            // Some images may provide this mapping using different data, such as a
            // fully specified transformation matrix. Depending upon your application,
            // you may need more flexible handling here.

            // We need data from two directories

            var ifd0 = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
            var geoTiff = directories.OfType<GeoTiffDirectory>().FirstOrDefault();

            if (ifd0 is null || geoTiff is null)
            {
                Console.Error.WriteLine("IFD0 or GeoTiff directories not found.");
                return;
            }

            // Map from the GeoTIFF spec's numeric value for projected coordinate system
            // type, to a WGS84 UTM zone and hemisphere. Images using a different datum will
            // not be processed here.

            var projectedCoordSystemType = geoTiff.GetUInt16(GeoTiffDirectory.TagProjectedCSType);

            (bool isNorthernHemisphere, int zone) = projectedCoordSystemType switch
            {
                >= 32601 and <= 32660 => (true, projectedCoordSystemType - 32600),
                >= 32701 and <= 32760 => (true, projectedCoordSystemType - 32700),
                _ => (false, -1)
            };

            if (zone == -1)
            {
                Console.Error.WriteLine("Projected coordinate system type must be a WGS84 UTM zone.");
                return;
            }

            Console.Out.WriteLine($"WGS84 UTM Zone: {zone}{(isNorthernHemisphere ? 'N' : 'S')}");

            // This sample is only coded to support model space values in units of meters.
            // Other units are possible, and can be coded for.

            var projectedLinearUnits = geoTiff.GetUInt16(GeoTiffDirectory.TagProjLinearUnits);

            if (projectedLinearUnits != 9001)
            {
                // TODO support other projection units
                Console.Error.WriteLine("Projection units not in meters.");
                return;
            }

            // Tie points are specified as an array of values. The array's length is a multiple of six.
            //
            // Each six values (I,J,K, X,Y,Z) correlate a point in image space (I,J,K) to a point in
            // model space (X,Y,Z).
            //
            // Most commonly the (0,0) pixel of the image is used. The K and Z values are generally
            // ignored.
            //
            // In this sample we only handle this simple case. More complex examples may have
            // non-zero values for the image space coordinates, or specify non-zero K/Z values.
            // Other files may specify multiple tie points, potentially as an alternative to
            // using pixel scale.

            var tiePoint = ifd0.GetObject(ExifDirectoryBase.TagModelTiePoint) as double[];

            if (tiePoint is null)
            {
                Console.Error.WriteLine("Unable to read the model tie point tag.");
                return;
            }

            if (tiePoint.Length < 6)
            {
                Console.Error.WriteLine("Model tie point should have at least six values.");
                return;
            }

            if (tiePoint[0] != 0 || tiePoint[1] != 0 || tiePoint[2] != 0 || tiePoint[5] != 0)
            {
                // We expect 0,0,0, X,Y,0
                Console.Error.WriteLine("Unsupported model tie point structure.");
                return;
            }

            var x = tiePoint[3]; // Meters east
            var y = tiePoint[4]; // Meters north

            Console.Out.WriteLine($"Tie point in model space (WGS84 UTM): {x}, {y}");

            // Pixel scale defines the size, in meters, or each pixel in the image.

            var pixelScale = ifd0.GetObject(ExifDirectoryBase.TagPixelScale) as double[];

            if (pixelScale is null)
            {
                Console.Error.WriteLine("Unable to read the pixel scale tag.");
                return;
            }

            if (pixelScale.Length < 2)
            {
                Console.Error.WriteLine("Pixel scale should have at least two values.");
                return;
            }

            var xScale = pixelScale[0];
            var yScale = pixelScale[1];

            Console.Out.WriteLine($"Pixel scale: {xScale}, {yScale}");

            // We multiple the pixel scale by the image dimensions to compute the width
            // and height of the image in meters.

            var width = ifd0.GetInt32(ExifDirectoryBase.TagImageWidth);
            var height = ifd0.GetInt32(ExifDirectoryBase.TagImageHeight);

            Console.Out.WriteLine($"Image dimensions in pixels: {width}, {height}");

            var dx = width * xScale;
            var dy = height * yScale;

            Console.Out.WriteLine($"Image dimensions in meters: {dx}, {dy}");

            // By combining the tie point with our width and height, we can compute the
            // position of all corners in model space.

            var upperLeftUtm  = (X: x,      Y: y     );
            var lowerLeftUtm  = (X: x,      Y: y + dy);
            var lowerRightUtm = (X: x + dx, Y: y + dy);
            var upperRightUtm = (X: x + dx, Y: y     );

            Console.Out.WriteLine($"Corners (WGS84 UTM): [\n  [{upperLeftUtm}],\n  [{lowerLeftUtm}],\n  [{lowerRightUtm}],\n  [{upperRightUtm}]\n]");

            // Convert the corners into latitute/longitude pairs.

            var upperLeftLatLon  = UtmToLatLon(zone, isNorthernHemisphere, upperLeftUtm);
            var lowerLeftLatLon  = UtmToLatLon(zone, isNorthernHemisphere, lowerLeftUtm);
            var lowerRightLatLon = UtmToLatLon(zone, isNorthernHemisphere, lowerRightUtm);
            var upperRightLatLon = UtmToLatLon(zone, isNorthernHemisphere, upperRightUtm);

            Console.Out.WriteLine($"Corners (lat/lon): [\n  [{upperLeftLatLon}],\n  [{lowerLeftLatLon}],\n  [{lowerRightLatLon}],\n  [{upperRightLatLon}]\n]");

            static GeoLocation UtmToLatLon(int zone, bool isNorthernHemisphere, (double X, double Y) pos)
            {
                var (lat, lon) = UTMUPS.Reverse(zone, isNorthernHemisphere, pos.X, pos.Y);

                return new(lat, lon);
            }
        }
    }
}
