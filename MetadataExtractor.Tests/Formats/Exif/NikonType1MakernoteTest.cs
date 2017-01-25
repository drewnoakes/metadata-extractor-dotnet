#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System.Linq;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Exif.Makernotes;
using MetadataExtractor.Formats.Jpeg;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Exif
{
    /// <summary>Unit tests for Nikon (Type 1) makernote handling.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class NikonType1MakernoteTest
    {
        /*
            [Interoperability] Interoperability Index = Recommended Exif Interoperability Rules (ExifR98)
            [Interoperability] Interoperability Version = 1.00

            [Jpeg] Data Precision = 8 bits
            [Jpeg] Image Width = 600 pixels
            [Jpeg] Image Height = 800 pixels
            [Jpeg] Number of Components = 3
            [Jpeg] Component 1 = Y component: Quantization table 0, Sampling factors 1 horiz/1 vert
            [Jpeg] Component 2 = Cb component: Quantization table 1, Sampling factors 1 horiz/1 vert
            [Jpeg] Component 3 = Cr component: Quantization table 1, Sampling factors 1 horiz/1 vert
        */

        [Fact, UseCulture("en-GB")]
        public void TestFile1()
        {
            var metadata = ExifReaderTest.ProcessSegmentBytes("Data/nikonMakernoteType1.jpg.app1", JpegSegmentType.App1).ToList();

            /*
                [Nikon Makernote] Makernote Unknown 1 = 08.00
                [Nikon Makernote] Quality = Unknown (12)
                [Nikon Makernote] Color Mode = Color
                [Nikon Makernote] Image Adjustment = Contrast +
                [Nikon Makernote] CCD Sensitivity = ISO80
                [Nikon Makernote] White Balance = Auto
                [Nikon Makernote] Focus = 0
                [Nikon Makernote] Makernote Unknown 2 =
                [Nikon Makernote] Digital Zoom = No digital zoom
                [Nikon Makernote] Fisheye Converter = None
                [Nikon Makernote] Makernote Unknown 3 = 0 0 16777216 0 2685774096 0 34833 6931 16178 4372 4372 3322676767 3373084416 15112 0 0 1151495 252903424 17 0 0 844038208 55184128 218129428 1476410198 370540566 4044363286 16711749 204629079 1729
            */
            var nikonDirectory = metadata.OfType<NikonType1MakernoteDirectory>().SingleOrDefault();

            Assert.NotNull(nikonDirectory);
            Assert.True(nikonDirectory.TagCount > 0);
            Assert.Equal("08.00", nikonDirectory.GetString(NikonType1MakernoteDirectory.TagUnknown1));
            Assert.Equal(12, nikonDirectory.GetInt32(NikonType1MakernoteDirectory.TagQuality));
            Assert.Equal(1,  nikonDirectory.GetInt32(NikonType1MakernoteDirectory.TagColorMode));
            Assert.Equal(3,  nikonDirectory.GetInt32(NikonType1MakernoteDirectory.TagImageAdjustment));
            Assert.Equal(0,  nikonDirectory.GetInt32(NikonType1MakernoteDirectory.TagCcdSensitivity));
            Assert.Equal(0,  nikonDirectory.GetInt32(NikonType1MakernoteDirectory.TagWhiteBalance));
            Assert.Equal(0,  nikonDirectory.GetInt32(NikonType1MakernoteDirectory.TagFocus));
            Assert.Equal("", nikonDirectory.GetString(NikonType1MakernoteDirectory.TagUnknown2));
            Assert.Equal(0,  nikonDirectory.GetDouble(NikonType1MakernoteDirectory.TagDigitalZoom), 5);
            Assert.Equal(0,  nikonDirectory.GetInt32(NikonType1MakernoteDirectory.TagConverter));

            Assert.Equal(
                new uint[] { 0, 0, 16777216, 0, 2685774096, 0, 34833, 6931, 16178, 4372, 4372, 3322676767, 3373084416, 15112, 0, 0, 1151495, 252903424, 17, 0, 0, 844038208, 55184128, 218129428, 1476410198, 370540566, 4044363286, 16711749, 204629079, 1729 },
                (uint[])nikonDirectory.GetObject(NikonType1MakernoteDirectory.TagUnknown3));

            /*
                [Exif] Image Description =
                [Exif] Make = NIKON
                [Exif] Model = E950
                [Exif] Orientation = top, left side
                [Exif] X Resolution = 300 dots per inch
                [Exif] Y Resolution = 300 dots per inch
                [Exif] Resolution Unit = Inch
                [Exif] Software = v981-79
                [Exif] Date/Time = 2001:04:06 11:51:40
                [Exif] YCbCr Positioning = Datum point
                [Exif] Exposure Time = 1/77 sec
                [Exif] F-Number = F5.5
                [Exif] Exposure Program = Program normal
                [Exif] ISO Speed Ratings = 80
                [Exif] Exif Version = 2.10
                [Exif] Date/Time Original = 2001:04:06 11:51:40
                [Exif] Date/Time Digitized = 2001:04:06 11:51:40
                [Exif] Components Configuration = YCbCr
                [Exif] Compressed Bits Per Pixel = 4 bits/pixel
                [Exif] Exposure Bias Value = 0
                [Exif] Max Aperture Value = F2.5
                [Exif] Metering Mode = Multi-segment
                [Exif] Light Source = Unknown
                [Exif] Flash = No flash fired
                [Exif] Focal Length = 12.8 mm
                [Exif] User Comment =
                [Exif] FlashPix Version = 1.00
                [Exif] Color Space = sRGB
                [Exif] Exif Image Width = 1600 pixels
                [Exif] Exif Image Height = 1200 pixels
                [Exif] File Source = Digital Still Camera (DSC)
                [Exif] Scene Type = Directly photographed image
                [Exif] Compression = JPEG compression
                [Exif] Thumbnail Offset = 2036 bytes
                [Exif] Thumbnail Length = 4662 bytes
                [Exif] Thumbnail Data = [4662 bytes of thumbnail data]
            */

            var ifd0Directory = metadata.OfType<ExifIfd0Directory>().SingleOrDefault();

            Assert.NotNull(ifd0Directory);
            Assert.Equal("          ",          ifd0Directory.GetString(ExifDirectoryBase.TagImageDescription));
            Assert.Equal("NIKON",               ifd0Directory.GetString(ExifDirectoryBase.TagMake));
            Assert.Equal("E950",                ifd0Directory.GetString(ExifDirectoryBase.TagModel));
            Assert.Equal(1,                     ifd0Directory.GetInt32(ExifDirectoryBase.TagOrientation));
            Assert.Equal(300,                   ifd0Directory.GetDouble(ExifDirectoryBase.TagXResolution), 3);
            Assert.Equal(300,                   ifd0Directory.GetDouble(ExifDirectoryBase.TagYResolution), 3);
            Assert.Equal(2,                     ifd0Directory.GetInt32(ExifDirectoryBase.TagResolutionUnit));
            Assert.Equal("v981-79",             ifd0Directory.GetString(ExifDirectoryBase.TagSoftware));
            Assert.Equal("2001:04:06 11:51:40", ifd0Directory.GetString(ExifDirectoryBase.TagDateTime));
            Assert.Equal(2,                     ifd0Directory.GetInt32(ExifDirectoryBase.TagYCbCrPositioning));

            var subIfdDirectory = metadata.OfType<ExifSubIfdDirectory>().SingleOrDefault();

            Assert.NotNull(subIfdDirectory);
            Assert.Equal(new Rational(1, 77), subIfdDirectory.GetRational(ExifDirectoryBase.TagExposureTime));
            Assert.Equal(5.5, subIfdDirectory.GetDouble(ExifDirectoryBase.TagFNumber), 3);
            Assert.Equal(2, subIfdDirectory.GetInt32(ExifDirectoryBase.TagExposureProgram));
            Assert.Equal(80, subIfdDirectory.GetInt32(ExifDirectoryBase.TagIsoEquivalent));
            Assert.Equal("48 50 49 48", subIfdDirectory.GetString(ExifDirectoryBase.TagExifVersion));
            Assert.Equal("2001:04:06 11:51:40", subIfdDirectory.GetString(ExifDirectoryBase.TagDateTimeDigitized));
            Assert.Equal("2001:04:06 11:51:40", subIfdDirectory.GetString(ExifDirectoryBase.TagDateTimeOriginal));
            Assert.Equal("1 2 3 0", subIfdDirectory.GetString(ExifDirectoryBase.TagComponentsConfiguration));
            Assert.Equal(4, subIfdDirectory.GetInt32(ExifDirectoryBase.TagCompressedAverageBitsPerPixel));
            Assert.Equal(0, subIfdDirectory.GetInt32(ExifDirectoryBase.TagExposureBias));
            // this 2.6 *apex*, which is F2.5
            Assert.Equal(2.6, subIfdDirectory.GetDouble(ExifDirectoryBase.TagMaxAperture), 3);
            Assert.Equal(5, subIfdDirectory.GetInt32(ExifDirectoryBase.TagMeteringMode));
            Assert.Equal(0, subIfdDirectory.GetInt32(ExifDirectoryBase.TagWhiteBalance));
            Assert.Equal(0, subIfdDirectory.GetInt32(ExifDirectoryBase.TagFlash));
            Assert.Equal(12.8, subIfdDirectory.GetDouble(ExifDirectoryBase.TagFocalLength), 3);
            Assert.Equal(
                "0 0 0 0 0 0 0 0 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32",
                subIfdDirectory.GetString(ExifDirectoryBase.TagUserComment));
            Assert.Equal("48 49 48 48", subIfdDirectory.GetString(ExifDirectoryBase.TagFlashpixVersion));
            Assert.Equal(1, subIfdDirectory.GetInt32(ExifDirectoryBase.TagColorSpace));
            Assert.Equal(1600, subIfdDirectory.GetInt32(ExifDirectoryBase.TagExifImageWidth));
            Assert.Equal(1200, subIfdDirectory.GetInt32(ExifDirectoryBase.TagExifImageHeight));
            Assert.Equal(3, subIfdDirectory.GetInt32(ExifDirectoryBase.TagFileSource));
            Assert.Equal(1, subIfdDirectory.GetInt32(ExifDirectoryBase.TagSceneType));

            var thumbDirectory = metadata.OfType<ExifThumbnailDirectory>().SingleOrDefault();

            Assert.NotNull(thumbDirectory);
            Assert.Equal(6, thumbDirectory.GetInt32(ExifDirectoryBase.TagCompression));
            Assert.Equal(2036, thumbDirectory.GetInt32(ExifThumbnailDirectory.TagThumbnailOffset));
            Assert.Equal(4662, thumbDirectory.GetInt32(ExifThumbnailDirectory.TagThumbnailLength));
        }
    }
}
