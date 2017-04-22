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

using System.Collections.Generic;
using System.Linq;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Exif.Makernotes;
using MetadataExtractor.Formats.Jpeg;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Exif
{
    /// <summary>Unit tests for Nikon (Type 2) makernote handling.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class NikonType2MakernoteTest
    {
        [Fact, UseCulture("en-GB")]
        public void NikonType2MakernoteTest1()
        {
            var directory = ExifReaderTest.ProcessSegmentBytes<NikonType2MakernoteDirectory>("Data/nikonMakernoteType2a.jpg.app1", JpegSegmentType.App1);

            Assert.NotNull(directory);

            /*
                [Nikon Makernote] Firmware Version = 0200
                [Nikon Makernote] ISO = 0 320
                [Nikon Makernote] File Format = FINE
                [Nikon Makernote] White Balance = FLASH
                [Nikon Makernote] Sharpening = AUTO
                [Nikon Makernote] AF Type = AF-C
                [Nikon Makernote] Unknown 17 = NORMAL
                [Nikon Makernote] Unknown 18 =
                [Nikon Makernote] White Balance Fine = 0
                [Nikon Makernote] Unknown 01 =
                [Nikon Makernote] Unknown 02 =
                [Nikon Makernote] Unknown 03 = 914
                [Nikon Makernote] Unknown 19 =
                [Nikon Makernote] ISO = 0 320
                [Nikon Makernote] Tone Compensation = AUTO
                [Nikon Makernote] Unknown 04 = 6
                [Nikon Makernote] Lens Focal/Max-FStop pairs = 240/10 850/10 35/10 45/10
                [Nikon Makernote] Unknown 05 = 0
                [Nikon Makernote] Unknown 06 = 
                [Nikon Makernote] Unknown 07 = 1
                [Nikon Makernote] Unknown 20 = 0
                [Nikon Makernote] Unknown 08 = @
                [Nikon Makernote] Colour Mode = MODE1
                [Nikon Makernote] Unknown 10 = NATURAL
                [Nikon Makernote] Unknown 11 = 0100

                [Nikon Makernote] Camera Hue = 0
                [Nikon Makernote] Noise Reduction = OFF
                [Nikon Makernote] Unknown 12 = 0100

                [Nikon Makernote] Unknown 13 = 0100{t@7b,4x,D"Y
                [Nikon Makernote] Unknown 15 = 78/10 78/10
            */

            Assert.Equal("48 50 48 48",               directory.GetString(NikonType2MakernoteDirectory.TagFirmwareVersion));
            Assert.Equal("0 320",                     directory.GetString(NikonType2MakernoteDirectory.TagIso1));
            Assert.Equal("0 320",                     directory.GetString(NikonType2MakernoteDirectory.TagIsoRequested));
            Assert.Equal("FLASH       ",              directory.GetString(NikonType2MakernoteDirectory.TagCameraWhiteBalance));
            Assert.Equal("AUTO  ",                    directory.GetString(NikonType2MakernoteDirectory.TagCameraSharpening));
            Assert.Equal("AF-C  ",                    directory.GetString(NikonType2MakernoteDirectory.TagAfType));
            Assert.Equal("NORMAL      ",              directory.GetString(NikonType2MakernoteDirectory.TagFlashSyncMode));
            Assert.Equal("0",                         directory.GetString(NikonType2MakernoteDirectory.TagCameraWhiteBalanceFine));
            Assert.Equal("914",                       directory.GetString(NikonType2MakernoteDirectory.TagPreviewIfd));
            Assert.Equal("AUTO    ",                  directory.GetString(NikonType2MakernoteDirectory.TagCameraToneCompensation));
            Assert.Equal("6",                         directory.GetString(NikonType2MakernoteDirectory.TagLensType));
            Assert.Equal("240/10 850/10 35/10 45/10", directory.GetString(NikonType2MakernoteDirectory.TagLens));
            Assert.Equal("0",                         directory.GetString(NikonType2MakernoteDirectory.TagFlashUsed));
            Assert.Equal("1",                         directory.GetString(NikonType2MakernoteDirectory.TagShootingMode));
            Assert.Equal("0",                         directory.GetString(NikonType2MakernoteDirectory.TagUnknown20));
            Assert.Equal("MODE1   ",                  directory.GetString(NikonType2MakernoteDirectory.TagCameraColorMode));
            Assert.Equal("NATURAL    ",               directory.GetString(NikonType2MakernoteDirectory.TagLightSource));
            Assert.Equal("0",                         directory.GetString(NikonType2MakernoteDirectory.TagCameraHueAdjustment));
            Assert.Equal("OFF ",                      directory.GetString(NikonType2MakernoteDirectory.TagNoiseReduction));
            Assert.Equal("78/10 78/10",               directory.GetString(NikonType2MakernoteDirectory.TagSensorPixelSize));

            var descriptor = new NikonType2MakernoteDescriptor(directory);

            Assert.Equal("24-85mm f/3.5-4.5", descriptor.GetDescription(NikonType2MakernoteDirectory.TagLens));
            Assert.Equal("24-85mm f/3.5-4.5", descriptor.GetLensDescription());

            Assert.Equal("0 degrees", descriptor.GetDescription(NikonType2MakernoteDirectory.TagCameraHueAdjustment));
            Assert.Equal("0 degrees", descriptor.GetHueAdjustmentDescription());

            Assert.Equal("Mode I (sRGB)", descriptor.GetDescription(NikonType2MakernoteDirectory.TagCameraColorMode));
            Assert.Equal("Mode I (sRGB)", descriptor.GetColorModeDescription());
        }

        [Fact, UseCulture("en-GB")]
        public void NikonType2MakernoteTest2()
        {
            var directories = ExifReaderTest.ProcessSegmentBytes("Data/nikonMakernoteType2b.jpg.app1", JpegSegmentType.App1).ToList();

            /*
                [Nikon Makernote] Makernote Unknown 1 =
                [Nikon Makernote] ISO Setting = Unknown (0 0)
                [Nikon Makernote] Color Mode = COLOR
                [Nikon Makernote] Quality = NORMAL
                [Nikon Makernote] White Balance = AUTO
                [Nikon Makernote] Image Sharpening = AUTO
                [Nikon Makernote] Focus Mode = AF-C
                [Nikon Makernote] Flash Setting = NORMAL
                [Nikon Makernote] Makernote Unknown 2 = 4416/500
                [Nikon Makernote] ISO Selection = AUTO
                [Nikon Makernote] Unknown tag (0x0011) = 1300
                [Nikon Makernote] Image Adjustment = AUTO
                [Nikon Makernote] Adapter = OFF
                [Nikon Makernote] Focus Distance = 0
                [Nikon Makernote] Digital Zoom = No digital zoom
                [Nikon Makernote] AF Focus Position = Unknown ()
                [Nikon Makernote] Unknown tag (0x008f) =
                [Nikon Makernote] Unknown tag (0x0094) = 0
                [Nikon Makernote] Unknown tag (0x0095) = FPNR
                [Nikon Makernote] Unknown tag (0x0e00) = PrintIM
                [Nikon Makernote] Unknown tag (0x0e10) = 1394
            */

            var nikonDirectory = directories.OfType<NikonType2MakernoteDirectory>().SingleOrDefault();

            Assert.NotNull(nikonDirectory);

            Assert.Equal("0 1 0 0", nikonDirectory.GetString(NikonType2MakernoteDirectory.TagFirmwareVersion));
            Assert.Equal("0 0", nikonDirectory.GetString(NikonType2MakernoteDirectory.TagIso1));
            Assert.Equal("COLOR", nikonDirectory.GetString(NikonType2MakernoteDirectory.TagColorMode));
            Assert.Equal("NORMAL ", nikonDirectory.GetString(NikonType2MakernoteDirectory.TagQualityAndFileFormat));
            Assert.Equal("AUTO        ", nikonDirectory.GetString(NikonType2MakernoteDirectory.TagCameraWhiteBalance));
            Assert.Equal("AUTO  ", nikonDirectory.GetString(NikonType2MakernoteDirectory.TagCameraSharpening));
            Assert.Equal("AF-C  ", nikonDirectory.GetString(NikonType2MakernoteDirectory.TagAfType));
            Assert.Equal("NORMAL      ", nikonDirectory.GetString(NikonType2MakernoteDirectory.TagFlashSyncMode));
            //        Assert.Equal(new Rational(4416,500), _nikonDirectory.getRational(NikonType3MakernoteDirectory.TAG_UNKNOWN_2));
            Assert.Equal("AUTO  ", nikonDirectory.GetString(NikonType2MakernoteDirectory.TagIsoMode));
            Assert.Equal(1300, nikonDirectory.GetInt32(0x0011));
            Assert.Equal("AUTO         ", nikonDirectory.GetString(NikonType2MakernoteDirectory.TagImageAdjustment));
            Assert.Equal("OFF         ", nikonDirectory.GetString(NikonType2MakernoteDirectory.TagAdapter));
            Assert.Equal(0, nikonDirectory.GetInt32(NikonType2MakernoteDirectory.TagManualFocusDistance));
            Assert.Equal(1, nikonDirectory.GetInt32(NikonType2MakernoteDirectory.TagDigitalZoom));
            Assert.Equal("                ", nikonDirectory.GetString(0x008f));
            Assert.Equal(0, nikonDirectory.GetInt32(0x0094));
            Assert.Equal("FPNR", nikonDirectory.GetString(0x0095));

            // PrintIM
            var expectedData = new Dictionary<int, string>
            {
                { 0x0000, "0100" },
                { 0x0001, "0x00160016" },
                { 0x0002, "0x00000001" },
                { 0x0003, "0x0000005e" },
                { 0x0007, "0x00000000" },
                { 0x0008, "0x00000000" },
                { 0x0009, "0x00000000" },
                { 0x000A, "0x00000000" },
                { 0x000B, "0x000000a6" },
                { 0x000C, "0x00000000" },
                { 0x000D, "0x00000000" },
                { 0x000E, "0x000000be" },
                { 0x0100, "0x00000005" },
                { 0x0101, "0x00000001" }
            };

            var nikonPrintImDirectory = directories.OfType<PrintIMDirectory>().SingleOrDefault();

            Assert.NotNull(nikonPrintImDirectory);

            Assert.Equal(expectedData.Count, nikonPrintImDirectory.Tags.Count);
            foreach(var expected in expectedData)
            {
                Assert.Equal(expected.Value, nikonPrintImDirectory.GetDescription(expected.Key));
            }

            /*Assert.Equal("80 114 105 110 116 73 77 0 48 49 48 48 0 0 13 0 1 0 22 0 22 0 2 0 1 0 0 0 3 0 94 0 0 0 7 0 0 0 0 0 8 0 0 0 0 0 9 0 0 0 0 0 " +
                            "10 0 0 0 0 0 11 0 166 0 0 0 12 0 0 0 0 0 13 0 0 0 0 0 14 0 190 0 0 0 0 1 5 0 0 0 1 1 1 0 0 0 9 17 0 0 16 39 0 0 11 15 0 0 16 " +
                            "39 0 0 151 5 0 0 16 39 0 0 176 8 0 0 16 39 0 0 1 28 0 0 16 39 0 0 94 2 0 0 16 39 0 0 139 0 0 0 16 39 0 0 203 3 0 0 16 39 " +
                            "0 0 229 27 0 0 16 39 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
                            nikonPrintImDirectory.GetString(0x0e00));*/
            //            Assert.Equals("PrintIM", _nikonDirectory.GetString(0x0e00));
            Assert.Equal(1394, nikonDirectory.GetInt32(0x0e10));

            /*
                [Exif] Image Description =
                [Exif] Make = NIKON
                [Exif] Model = E995
                [Exif] X Resolution = 300 dots per inch
                [Exif] Y Resolution = 300 dots per inch
                [Exif] Resolution Unit = Inch
                [Exif] Software = E995v1.6
                [Exif] Date/Time = 2002:08:29 17:31:40
                [Exif] YCbCr Positioning = Center of pixel array
                [Exif] Exposure Time = 2439024/100000000 sec
                [Exif] F-Number = F2.6
                [Exif] Exposure Program = Program normal
                [Exif] ISO Speed Ratings = 100
                [Exif] Exif Version = 2.10
                [Exif] Date/Time Original = 2002:08:29 17:31:40
                [Exif] Date/Time Digitized = 2002:08:29 17:31:40
                [Exif] Components Configuration = YCbCr
                [Exif] Exposure Bias Value = 0 EV
                [Exif] Max Aperture Value = F1
                [Exif] Metering Mode = Multi-segment
                [Exif] White Balance = Unknown
                [Exif] Flash = Flash fired
                [Exif] Focal Length = 8.2 mm
                [Exif] User Comment =
                [Exif] FlashPix Version = 1.00
                [Exif] Color Space = sRGB
                [Exif] Exif Image Width = 2048 pixels
                [Exif] Exif Image Height = 1536 pixels
                [Exif] File Source = Digital Still Camera (DSC)
                [Exif] Scene Type = Directly photographed image
            */

            var ifd0Directory = directories.OfType<ExifIfd0Directory>().SingleOrDefault();

            Assert.NotNull(ifd0Directory);

            Assert.Equal("          ", ifd0Directory.GetString(ExifDirectoryBase.TagImageDescription));
            Assert.Equal("NIKON", ifd0Directory.GetString(ExifDirectoryBase.TagMake));
            Assert.Equal("E995", ifd0Directory.GetString(ExifDirectoryBase.TagModel));
            Assert.Equal(300, ifd0Directory.GetDouble(ExifDirectoryBase.TagXResolution), 3);
            Assert.Equal(300, ifd0Directory.GetDouble(ExifDirectoryBase.TagYResolution), 3);
            Assert.Equal(2, ifd0Directory.GetInt32(ExifDirectoryBase.TagResolutionUnit));
            Assert.Equal("E995v1.6", ifd0Directory.GetString(ExifDirectoryBase.TagSoftware));
            Assert.Equal("2002:08:29 17:31:40", ifd0Directory.GetString(ExifDirectoryBase.TagDateTime));
            Assert.Equal(1, ifd0Directory.GetInt32(ExifDirectoryBase.TagYCbCrPositioning));

            var subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().SingleOrDefault();

            Assert.NotNull(subIfdDirectory);

            Assert.Equal(new Rational(2439024, 100000000), subIfdDirectory.GetRational(ExifDirectoryBase.TagExposureTime));
            Assert.Equal(2.6, subIfdDirectory.GetDouble(ExifDirectoryBase.TagFNumber), 3);
            Assert.Equal(2, subIfdDirectory.GetInt32(ExifDirectoryBase.TagExposureProgram));
            Assert.Equal(100, subIfdDirectory.GetInt32(ExifDirectoryBase.TagIsoEquivalent));
            Assert.Equal("48 50 49 48", subIfdDirectory.GetString(ExifDirectoryBase.TagExifVersion));
            Assert.Equal("2002:08:29 17:31:40", subIfdDirectory.GetString(ExifDirectoryBase.TagDateTimeDigitized));
            Assert.Equal("2002:08:29 17:31:40", subIfdDirectory.GetString(ExifDirectoryBase.TagDateTimeOriginal));
            Assert.Equal("1 2 3 0", subIfdDirectory.GetString(ExifDirectoryBase.TagComponentsConfiguration));
            Assert.Equal(0, subIfdDirectory.GetInt32(ExifDirectoryBase.TagExposureBias));
            Assert.Equal("0", subIfdDirectory.GetString(ExifDirectoryBase.TagMaxAperture));
            Assert.Equal(5, subIfdDirectory.GetInt32(ExifDirectoryBase.TagMeteringMode));
            Assert.Equal(0, subIfdDirectory.GetInt32(ExifDirectoryBase.TagWhiteBalance));
            Assert.Equal(1, subIfdDirectory.GetInt32(ExifDirectoryBase.TagFlash));
            Assert.Equal(8.2, subIfdDirectory.GetDouble(ExifDirectoryBase.TagFocalLength), 3);
            Assert.Equal("0 0 0 0 0 0 0 0 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32", subIfdDirectory.GetString(ExifDirectoryBase.TagUserComment));
            Assert.Equal("48 49 48 48", subIfdDirectory.GetString(ExifDirectoryBase.TagFlashpixVersion));
            Assert.Equal(1, subIfdDirectory.GetInt32(ExifDirectoryBase.TagColorSpace));
            Assert.Equal(2048, subIfdDirectory.GetInt32(ExifDirectoryBase.TagExifImageWidth));
            Assert.Equal(1536, subIfdDirectory.GetInt32(ExifDirectoryBase.TagExifImageHeight));
            Assert.Equal(3, subIfdDirectory.GetInt32(ExifDirectoryBase.TagFileSource));
            Assert.Equal(1, subIfdDirectory.GetInt32(ExifDirectoryBase.TagSceneType));

            /*
                [Exif Thumbnail] Thumbnail Compression = JPEG (old-style)
                [Exif Thumbnail] X Resolution = 72 dots per inch
                [Exif Thumbnail] Y Resolution = 72 dots per inch
                [Exif Thumbnail] Resolution Unit = Inch
                [Exif Thumbnail] Thumbnail Offset = 1494 bytes
                [Exif Thumbnail] Thumbnail Length = 6077 bytes
            */

            var thumbDirectory = directories.OfType<ExifThumbnailDirectory>().SingleOrDefault();

            Assert.NotNull(thumbDirectory);

            Assert.Equal(6, thumbDirectory.GetInt32(ExifDirectoryBase.TagCompression));
            Assert.Equal(1494, thumbDirectory.GetInt32(ExifThumbnailDirectory.TagThumbnailOffset));
            Assert.Equal(6077, thumbDirectory.GetInt32(ExifThumbnailDirectory.TagThumbnailLength));
            Assert.Equal(1494, thumbDirectory.GetInt32(ExifThumbnailDirectory.TagThumbnailOffset));
            Assert.Equal(72, thumbDirectory.GetInt32(ExifDirectoryBase.TagXResolution));
            Assert.Equal(72, thumbDirectory.GetInt32(ExifDirectoryBase.TagYResolution));
        }

        [Fact, UseCulture("en-GB")]
        public void GetAutoFlashCompensationDescription()
        {
            var directory = new NikonType2MakernoteDirectory();
            var descriptor = new NikonType2MakernoteDescriptor(directory);
            // no entry exists
            Assert.Null(descriptor.GetAutoFlashCompensationDescription());
            directory.Set(NikonType2MakernoteDirectory.TagAutoFlashCompensation, new sbyte[] { 0x06, 0x01, 0x06 });
            Assert.Equal("1 EV", descriptor.GetAutoFlashCompensationDescription());
            directory.Set(NikonType2MakernoteDirectory.TagAutoFlashCompensation, new sbyte[] { 0x04, 0x01, 0x06 });
            Assert.Equal("0.67 EV", descriptor.GetAutoFlashCompensationDescription());
            directory.Set(NikonType2MakernoteDirectory.TagAutoFlashCompensation, new sbyte[] { 0x02, 0x01, 0x06 });
            Assert.Equal("0.33 EV", descriptor.GetAutoFlashCompensationDescription());
            directory.Set(NikonType2MakernoteDirectory.TagAutoFlashCompensation, new sbyte[] { unchecked((sbyte)0xFE), 0x01, 0x06 });
            Assert.Equal("-0.33 EV", descriptor.GetAutoFlashCompensationDescription());
        }
    }
}
