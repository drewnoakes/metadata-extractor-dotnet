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

using Com.Drew.Lang;
using Com.Drew.Metadata.Exif.Makernotes;
using NUnit.Framework;

namespace Com.Drew.Metadata.Exif
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class NikonType2MakernoteTest2
    {
        private NikonType2MakernoteDirectory _nikonDirectory;

        private ExifIfd0Directory _exifIfd0Directory;

        private ExifSubIfdDirectory _exifSubIfdDirectory;

        private ExifThumbnailDirectory _thumbDirectory;


        [SetUp]
        public void SetUp()
        {
            var metadata = ExifReaderTest.ProcessBytes("Tests/Data/nikonMakernoteType2b.jpg.app1");
            _nikonDirectory = metadata.GetFirstDirectoryOfType<NikonType2MakernoteDirectory>();
            _exifIfd0Directory = metadata.GetFirstDirectoryOfType<ExifIfd0Directory>();
            _exifSubIfdDirectory = metadata.GetFirstDirectoryOfType<ExifSubIfdDirectory>();
            _thumbDirectory = metadata.GetFirstDirectoryOfType<ExifThumbnailDirectory>();
            Assert.IsNotNull(_nikonDirectory);
            Assert.IsNotNull(_exifSubIfdDirectory);
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

        [Test]
        public void TestNikonMakernote_MatchesKnownValues()
        {
            Assert.AreEqual("0 1 0 0", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagFirmwareVersion));
            Assert.AreEqual("0 0", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagIso1));
            Assert.AreEqual("COLOR", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagColorMode));
            Assert.AreEqual("NORMAL ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagQualityAndFileFormat));
            Assert.AreEqual("AUTO        ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagCameraWhiteBalance));
            Assert.AreEqual("AUTO  ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagCameraSharpening));
            Assert.AreEqual("AF-C  ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagAfType));
            Assert.AreEqual("NORMAL      ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagFlashSyncMode));
            //        assertEquals(new Rational(4416,500), _nikonDirectory.getRational(NikonType3MakernoteDirectory.TAG_UNKNOWN_2));
            Assert.AreEqual("AUTO  ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagIsoMode));
            Assert.AreEqual(1300, _nikonDirectory.GetInt(unchecked(0x0011)));
            Assert.AreEqual("AUTO         ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagImageAdjustment));
            Assert.AreEqual("OFF         ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagAdapter));
            Assert.AreEqual(0, _nikonDirectory.GetInt(NikonType2MakernoteDirectory.TagManualFocusDistance));
            Assert.AreEqual(1, _nikonDirectory.GetInt(NikonType2MakernoteDirectory.TagDigitalZoom));
            Assert.AreEqual("                ", _nikonDirectory.GetString(unchecked(0x008f)));
            Assert.AreEqual(0, _nikonDirectory.GetInt(unchecked(0x0094)));
            Assert.AreEqual("FPNR", _nikonDirectory.GetString(unchecked(0x0095)));
            Assert.AreEqual("80 114 105 110 116 73 77 0 48 49 48 48 0 0 13 0 1 0 22 0 22 0 2 0 1 0 0 0 3 0 94 0 0 0 7 0 0 0 0 0 8 0 0 0 0 0 9 0 0 0 0 0 " +
                            "10 0 0 0 0 0 11 0 166 0 0 0 12 0 0 0 0 0 13 0 0 0 0 0 14 0 190 0 0 0 0 1 5 0 0 0 1 1 1 0 0 0 9 17 0 0 16 39 0 0 11 15 0 0 16 " +
                            "39 0 0 151 5 0 0 16 39 0 0 176 8 0 0 16 39 0 0 1 28 0 0 16 39 0 0 94 2 0 0 16 39 0 0 139 0 0 0 16 39 0 0 203 3 0 0 16 39 " +
                            "0 0 229 27 0 0 16 39 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
                            _nikonDirectory.GetString(unchecked(0x0e00)));
            //        assertEquals("PrintIM", _nikonDirectory.getString(0x0e00));
            Assert.AreEqual(1394, _nikonDirectory.GetInt(unchecked(0x0e10)));
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

        [Test]
        public void TestExifDirectory_MatchesKnownValues()
        {
            Assert.AreEqual("          ", _exifIfd0Directory.GetString(ExifDirectoryBase.TagImageDescription));
            Assert.AreEqual("NIKON", _exifIfd0Directory.GetString(ExifDirectoryBase.TagMake));
            Assert.AreEqual("E995", _exifIfd0Directory.GetString(ExifDirectoryBase.TagModel));
            Assert.AreEqual(300, _exifIfd0Directory.GetDouble(ExifDirectoryBase.TagXResolution), 0.001);
            Assert.AreEqual(300, _exifIfd0Directory.GetDouble(ExifDirectoryBase.TagYResolution), 0.001);
            Assert.AreEqual(2, _exifIfd0Directory.GetInt(ExifDirectoryBase.TagResolutionUnit));
            Assert.AreEqual("E995v1.6", _exifIfd0Directory.GetString(ExifDirectoryBase.TagSoftware));
            Assert.AreEqual("2002:08:29 17:31:40", _exifIfd0Directory.GetString(ExifDirectoryBase.TagDatetime));
            Assert.AreEqual(1, _exifIfd0Directory.GetInt(ExifDirectoryBase.TagYcbcrPositioning));
            Assert.AreEqual(new Rational(2439024, 100000000), _exifSubIfdDirectory.GetRational(ExifDirectoryBase.TagExposureTime));
            Assert.AreEqual(2.6, _exifSubIfdDirectory.GetDouble(ExifDirectoryBase.TagFnumber), 0.001);
            Assert.AreEqual(2, _exifSubIfdDirectory.GetInt(ExifDirectoryBase.TagExposureProgram));
            Assert.AreEqual(100, _exifSubIfdDirectory.GetInt(ExifDirectoryBase.TagIsoEquivalent));
            Assert.AreEqual("48 50 49 48", _exifSubIfdDirectory.GetString(ExifDirectoryBase.TagExifVersion));
            Assert.AreEqual("2002:08:29 17:31:40", _exifSubIfdDirectory.GetString(ExifDirectoryBase.TagDatetimeDigitized));
            Assert.AreEqual("2002:08:29 17:31:40", _exifSubIfdDirectory.GetString(ExifDirectoryBase.TagDatetimeOriginal));
            Assert.AreEqual("1 2 3 0", _exifSubIfdDirectory.GetString(ExifDirectoryBase.TagComponentsConfiguration));
            Assert.AreEqual(0, _exifSubIfdDirectory.GetInt(ExifDirectoryBase.TagExposureBias));
            Assert.AreEqual("0", _exifSubIfdDirectory.GetString(ExifDirectoryBase.TagMaxAperture));
            Assert.AreEqual(5, _exifSubIfdDirectory.GetInt(ExifDirectoryBase.TagMeteringMode));
            Assert.AreEqual(0, _exifSubIfdDirectory.GetInt(ExifDirectoryBase.TagWhiteBalance));
            Assert.AreEqual(1, _exifSubIfdDirectory.GetInt(ExifDirectoryBase.TagFlash));
            Assert.AreEqual(8.2, _exifSubIfdDirectory.GetDouble(ExifDirectoryBase.TagFocalLength), 0.001);
            Assert.AreEqual("0 0 0 0 0 0 0 0 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32", _exifSubIfdDirectory.GetString(ExifDirectoryBase.TagUserComment));
            Assert.AreEqual("48 49 48 48", _exifSubIfdDirectory.GetString(ExifDirectoryBase.TagFlashpixVersion));
            Assert.AreEqual(1, _exifSubIfdDirectory.GetInt(ExifDirectoryBase.TagColorSpace));
            Assert.AreEqual(2048, _exifSubIfdDirectory.GetInt(ExifDirectoryBase.TagExifImageWidth));
            Assert.AreEqual(1536, _exifSubIfdDirectory.GetInt(ExifDirectoryBase.TagExifImageHeight));
            Assert.AreEqual(3, _exifSubIfdDirectory.GetInt(ExifDirectoryBase.TagFileSource));
            Assert.AreEqual(1, _exifSubIfdDirectory.GetInt(ExifDirectoryBase.TagSceneType));
        }

    /*
        [Exif Thumbnail] Thumbnail Compression = JPEG (old-style)
        [Exif Thumbnail] X Resolution = 72 dots per inch
        [Exif Thumbnail] Y Resolution = 72 dots per inch
        [Exif Thumbnail] Resolution Unit = Inch
        [Exif Thumbnail] Thumbnail Offset = 1494 bytes
        [Exif Thumbnail] Thumbnail Length = 6077 bytes
    */

        [Test]
        public void TestExifThumbnailDirectory_MatchesKnownValues()
        {
            Assert.AreEqual(6, _thumbDirectory.GetInt(ExifThumbnailDirectory.TagThumbnailCompression));
            Assert.AreEqual(1494, _thumbDirectory.GetInt(ExifThumbnailDirectory.TagThumbnailOffset));
            Assert.AreEqual(6077, _thumbDirectory.GetInt(ExifThumbnailDirectory.TagThumbnailLength));
            Assert.AreEqual(1494, _thumbDirectory.GetInt(ExifThumbnailDirectory.TagThumbnailOffset));
            Assert.AreEqual(72, _thumbDirectory.GetInt(ExifDirectoryBase.TagXResolution));
            Assert.AreEqual(72, _thumbDirectory.GetInt(ExifDirectoryBase.TagYResolution));
        }
    }
}
