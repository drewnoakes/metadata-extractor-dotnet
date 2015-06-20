/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using System.Linq;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Exif.Makernotes;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Exif
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class NikonType2MakernoteTest2
    {
        private readonly NikonType2MakernoteDirectory _nikonDirectory;
        private readonly ExifIfd0Directory _exifIfd0Directory;
        private readonly ExifSubIfdDirectory _exifSubIfdDirectory;
        private readonly ExifThumbnailDirectory _thumbDirectory;

        public NikonType2MakernoteTest2()
        {
            var metadata = ExifReaderTest.ProcessSegmentBytes("Tests/Data/nikonMakernoteType2b.jpg.app1").ToList();

            _nikonDirectory = metadata.OfType<NikonType2MakernoteDirectory>().SingleOrDefault();
            _exifIfd0Directory = metadata.OfType<ExifIfd0Directory>().SingleOrDefault();
            _exifSubIfdDirectory = metadata.OfType<ExifSubIfdDirectory>().SingleOrDefault();
            _thumbDirectory = metadata.OfType<ExifThumbnailDirectory>().SingleOrDefault();

            Assert.NotNull(_nikonDirectory);
            Assert.NotNull(_exifSubIfdDirectory);
        }

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

        [Fact]
        public void TestNikonMakernote_MatchesKnownValues()
        {
            Assert.Equal("0 1 0 0", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagFirmwareVersion));
            Assert.Equal("0 0", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagIso1));
            Assert.Equal("COLOR", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagColorMode));
            Assert.Equal("NORMAL ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagQualityAndFileFormat));
            Assert.Equal("AUTO        ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagCameraWhiteBalance));
            Assert.Equal("AUTO  ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagCameraSharpening));
            Assert.Equal("AF-C  ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagAfType));
            Assert.Equal("NORMAL      ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagFlashSyncMode));
            //        assertEquals(new Rational(4416,500), _nikonDirectory.getRational(NikonType3MakernoteDirectory.TAG_UNKNOWN_2));
            Assert.Equal("AUTO  ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagIsoMode));
            Assert.Equal(1300, _nikonDirectory.GetInt32(0x0011));
            Assert.Equal("AUTO         ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagImageAdjustment));
            Assert.Equal("OFF         ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagAdapter));
            Assert.Equal(0, _nikonDirectory.GetInt32(NikonType2MakernoteDirectory.TagManualFocusDistance));
            Assert.Equal(1, _nikonDirectory.GetInt32(NikonType2MakernoteDirectory.TagDigitalZoom));
            Assert.Equal("                ", _nikonDirectory.GetString(0x008f));
            Assert.Equal(0, _nikonDirectory.GetInt32(0x0094));
            Assert.Equal("FPNR", _nikonDirectory.GetString(0x0095));
            Assert.Equal("80 114 105 110 116 73 77 0 48 49 48 48 0 0 13 0 1 0 22 0 22 0 2 0 1 0 0 0 3 0 94 0 0 0 7 0 0 0 0 0 8 0 0 0 0 0 9 0 0 0 0 0 " +
                            "10 0 0 0 0 0 11 0 166 0 0 0 12 0 0 0 0 0 13 0 0 0 0 0 14 0 190 0 0 0 0 1 5 0 0 0 1 1 1 0 0 0 9 17 0 0 16 39 0 0 11 15 0 0 16 " +
                            "39 0 0 151 5 0 0 16 39 0 0 176 8 0 0 16 39 0 0 1 28 0 0 16 39 0 0 94 2 0 0 16 39 0 0 139 0 0 0 16 39 0 0 203 3 0 0 16 39 " +
                            "0 0 229 27 0 0 16 39 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
                            _nikonDirectory.GetString(0x0e00));
            //        assertEquals("PrintIM", _nikonDirectory.getString(0x0e00));
            Assert.Equal(1394, _nikonDirectory.GetInt32(0x0e10));
        }

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

        [Fact]
        public void TestExifDirectory_MatchesKnownValues()
        {
            Assert.Equal("          ", _exifIfd0Directory.GetString(ExifDirectoryBase.TagImageDescription));
            Assert.Equal("NIKON", _exifIfd0Directory.GetString(ExifDirectoryBase.TagMake));
            Assert.Equal("E995", _exifIfd0Directory.GetString(ExifDirectoryBase.TagModel));
            Assert.Equal(300, _exifIfd0Directory.GetDouble(ExifDirectoryBase.TagXResolution), 3);
            Assert.Equal(300, _exifIfd0Directory.GetDouble(ExifDirectoryBase.TagYResolution), 3);
            Assert.Equal(2, _exifIfd0Directory.GetInt32(ExifDirectoryBase.TagResolutionUnit));
            Assert.Equal("E995v1.6", _exifIfd0Directory.GetString(ExifDirectoryBase.TagSoftware));
            Assert.Equal("2002:08:29 17:31:40", _exifIfd0Directory.GetString(ExifDirectoryBase.TagDateTime));
            Assert.Equal(1, _exifIfd0Directory.GetInt32(ExifDirectoryBase.TagYcbcrPositioning));
            Assert.Equal(new Rational(2439024, 100000000), _exifSubIfdDirectory.GetRational(ExifDirectoryBase.TagExposureTime));
            Assert.Equal(2.6, _exifSubIfdDirectory.GetDouble(ExifDirectoryBase.TagFnumber), 3);
            Assert.Equal(2, _exifSubIfdDirectory.GetInt32(ExifDirectoryBase.TagExposureProgram));
            Assert.Equal(100, _exifSubIfdDirectory.GetInt32(ExifDirectoryBase.TagIsoEquivalent));
            Assert.Equal("48 50 49 48", _exifSubIfdDirectory.GetString(ExifDirectoryBase.TagExifVersion));
            Assert.Equal("2002:08:29 17:31:40", _exifSubIfdDirectory.GetString(ExifDirectoryBase.TagDateTimeDigitized));
            Assert.Equal("2002:08:29 17:31:40", _exifSubIfdDirectory.GetString(ExifDirectoryBase.TagDateTimeOriginal));
            Assert.Equal("1 2 3 0", _exifSubIfdDirectory.GetString(ExifDirectoryBase.TagComponentsConfiguration));
            Assert.Equal(0, _exifSubIfdDirectory.GetInt32(ExifDirectoryBase.TagExposureBias));
            Assert.Equal("0", _exifSubIfdDirectory.GetString(ExifDirectoryBase.TagMaxAperture));
            Assert.Equal(5, _exifSubIfdDirectory.GetInt32(ExifDirectoryBase.TagMeteringMode));
            Assert.Equal(0, _exifSubIfdDirectory.GetInt32(ExifDirectoryBase.TagWhiteBalance));
            Assert.Equal(1, _exifSubIfdDirectory.GetInt32(ExifDirectoryBase.TagFlash));
            Assert.Equal(8.2, _exifSubIfdDirectory.GetDouble(ExifDirectoryBase.TagFocalLength), 3);
            Assert.Equal("0 0 0 0 0 0 0 0 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32", _exifSubIfdDirectory.GetString(ExifDirectoryBase.TagUserComment));
            Assert.Equal("48 49 48 48", _exifSubIfdDirectory.GetString(ExifDirectoryBase.TagFlashpixVersion));
            Assert.Equal(1, _exifSubIfdDirectory.GetInt32(ExifDirectoryBase.TagColorSpace));
            Assert.Equal(2048, _exifSubIfdDirectory.GetInt32(ExifDirectoryBase.TagExifImageWidth));
            Assert.Equal(1536, _exifSubIfdDirectory.GetInt32(ExifDirectoryBase.TagExifImageHeight));
            Assert.Equal(3, _exifSubIfdDirectory.GetInt32(ExifDirectoryBase.TagFileSource));
            Assert.Equal(1, _exifSubIfdDirectory.GetInt32(ExifDirectoryBase.TagSceneType));
        }

    /*
        [Exif Thumbnail] Thumbnail Compression = JPEG (old-style)
        [Exif Thumbnail] X Resolution = 72 dots per inch
        [Exif Thumbnail] Y Resolution = 72 dots per inch
        [Exif Thumbnail] Resolution Unit = Inch
        [Exif Thumbnail] Thumbnail Offset = 1494 bytes
        [Exif Thumbnail] Thumbnail Length = 6077 bytes
    */

        [Fact]
        public void TestExifThumbnailDirectory_MatchesKnownValues()
        {
            Assert.Equal(6, _thumbDirectory.GetInt32(ExifDirectoryBase.TagCompression));
            Assert.Equal(1494, _thumbDirectory.GetInt32(ExifThumbnailDirectory.TagThumbnailOffset));
            Assert.Equal(6077, _thumbDirectory.GetInt32(ExifThumbnailDirectory.TagThumbnailLength));
            Assert.Equal(1494, _thumbDirectory.GetInt32(ExifThumbnailDirectory.TagThumbnailOffset));
            Assert.Equal(72, _thumbDirectory.GetInt32(ExifDirectoryBase.TagXResolution));
            Assert.Equal(72, _thumbDirectory.GetInt32(ExifDirectoryBase.TagYResolution));
        }
    }
}
