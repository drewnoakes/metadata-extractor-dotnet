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

using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Exif.makernotes;
using NUnit.Framework;

namespace Com.Drew.Metadata.Exif
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class NikonType1MakernoteTest
    {
        private NikonType1MakernoteDirectory _nikonDirectory;

        private ExifIfd0Directory _exifIfd0Directory;

        private ExifSubIfdDirectory _exifSubIfdDirectory;

        private ExifThumbnailDirectory _thumbDirectory;

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

        [SetUp]
        public void SetUp()
        {
            var metadata = ExifReaderTest.ProcessBytes("Tests/Data/nikonMakernoteType1.jpg.app1");
            _nikonDirectory = metadata.GetFirstDirectoryOfType<NikonType1MakernoteDirectory>();
            _exifSubIfdDirectory = metadata.GetFirstDirectoryOfType<ExifSubIfdDirectory>();
            _exifIfd0Directory = metadata.GetFirstDirectoryOfType<ExifIfd0Directory>();
            _thumbDirectory = metadata.GetFirstDirectoryOfType<ExifThumbnailDirectory>();
        }

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

        [Test, SetCulture("en-GB")]
        public void TestNikonMakernote_MatchesKnownValues()
        {
            Assert.IsTrue(_nikonDirectory.GetTagCount() > 0);
            Assert.AreEqual(8, _nikonDirectory.GetDouble(NikonType1MakernoteDirectory.TagUnknown1), 0.0001);
            Assert.AreEqual(12, _nikonDirectory.GetInt(NikonType1MakernoteDirectory.TagQuality));
            Assert.AreEqual(1, _nikonDirectory.GetInt(NikonType1MakernoteDirectory.TagColorMode));
            Assert.AreEqual(3, _nikonDirectory.GetInt(NikonType1MakernoteDirectory.TagImageAdjustment));
            Assert.AreEqual(0, _nikonDirectory.GetInt(NikonType1MakernoteDirectory.TagCcdSensitivity));
            Assert.AreEqual(0, _nikonDirectory.GetInt(NikonType1MakernoteDirectory.TagWhiteBalance));
            Assert.AreEqual(0, _nikonDirectory.GetInt(NikonType1MakernoteDirectory.TagFocus));
            Assert.AreEqual(string.Empty, _nikonDirectory.GetString(NikonType1MakernoteDirectory.TagUnknown2));
            Assert.AreEqual(0, _nikonDirectory.GetDouble(NikonType1MakernoteDirectory.TagDigitalZoom), 0.0001);
            Assert.AreEqual(0, _nikonDirectory.GetInt(NikonType1MakernoteDirectory.TagConverter));
            var unknown3 = (uint[])_nikonDirectory.GetObject(NikonType1MakernoteDirectory.TagUnknown3);
            var expected = new uint[] { 0, 0, 16777216, 0, 2685774096, 0, 34833, 6931, 16178, 4372, 4372, 3322676767, 3373084416, 15112, 0, 0, 1151495, 252903424, 17, 0, 0, 844038208, 55184128, 218129428, 1476410198, 370540566, 4044363286, 16711749
                , 204629079, 1729 };
            Assert.IsNotNull(unknown3);
            Assert.AreEqual(expected.Length, unknown3.Length);
            for (var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], (object)unknown3[i]);
            }
        }

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

        [Test]
        public void TestExifDirectory_MatchesKnownValues()
        {
            Assert.AreEqual("          ", _exifIfd0Directory.GetString(ExifDirectoryBase.TagImageDescription));
            Assert.AreEqual("NIKON", _exifIfd0Directory.GetString(ExifDirectoryBase.TagMake));
            Assert.AreEqual("E950", _exifIfd0Directory.GetString(ExifDirectoryBase.TagModel));
            Assert.AreEqual(1, _exifIfd0Directory.GetInt(ExifDirectoryBase.TagOrientation));
            Assert.AreEqual(300, _exifIfd0Directory.GetDouble(ExifDirectoryBase.TagXResolution), 0.001);
            Assert.AreEqual(300, _exifIfd0Directory.GetDouble(ExifDirectoryBase.TagYResolution), 0.001);
            Assert.AreEqual(2, _exifIfd0Directory.GetInt(ExifDirectoryBase.TagResolutionUnit));
            Assert.AreEqual("v981-79", _exifIfd0Directory.GetString(ExifDirectoryBase.TagSoftware));
            Assert.AreEqual("2001:04:06 11:51:40", _exifIfd0Directory.GetString(ExifDirectoryBase.TagDatetime));
            Assert.AreEqual(2, _exifIfd0Directory.GetInt(ExifDirectoryBase.TagYcbcrPositioning));
            Assert.AreEqual(new Rational(1, 77), _exifSubIfdDirectory.GetRational(ExifDirectoryBase.TagExposureTime));
            Assert.AreEqual(5.5, _exifSubIfdDirectory.GetDouble(ExifDirectoryBase.TagFnumber), 0.001);
            Assert.AreEqual(2, _exifSubIfdDirectory.GetInt(ExifDirectoryBase.TagExposureProgram));
            Assert.AreEqual(80, _exifSubIfdDirectory.GetInt(ExifDirectoryBase.TagIsoEquivalent));
            Assert.AreEqual("48 50 49 48", _exifSubIfdDirectory.GetString(ExifDirectoryBase.TagExifVersion));
            Assert.AreEqual("2001:04:06 11:51:40", _exifSubIfdDirectory.GetString(ExifDirectoryBase.TagDatetimeDigitized));
            Assert.AreEqual("2001:04:06 11:51:40", _exifSubIfdDirectory.GetString(ExifDirectoryBase.TagDatetimeOriginal));
            Assert.AreEqual("1 2 3 0", _exifSubIfdDirectory.GetString(ExifDirectoryBase.TagComponentsConfiguration));
            Assert.AreEqual(4, _exifSubIfdDirectory.GetInt(ExifDirectoryBase.TagCompressedAverageBitsPerPixel));
            Assert.AreEqual(0, _exifSubIfdDirectory.GetInt(ExifDirectoryBase.TagExposureBias));
            // this 2.6 *apex*, which is F2.5
            Assert.AreEqual(2.6, _exifSubIfdDirectory.GetDouble(ExifDirectoryBase.TagMaxAperture), 0.001);
            Assert.AreEqual(5, _exifSubIfdDirectory.GetInt(ExifDirectoryBase.TagMeteringMode));
            Assert.AreEqual(0, _exifSubIfdDirectory.GetInt(ExifDirectoryBase.TagWhiteBalance));
            Assert.AreEqual(0, _exifSubIfdDirectory.GetInt(ExifDirectoryBase.TagFlash));
            Assert.AreEqual(12.8, _exifSubIfdDirectory.GetDouble(ExifDirectoryBase.TagFocalLength), 0.001);
            Assert.AreEqual("0 0 0 0 0 0 0 0 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32", _exifSubIfdDirectory.GetString(ExifDirectoryBase.TagUserComment));
            Assert.AreEqual("48 49 48 48", _exifSubIfdDirectory.GetString(ExifDirectoryBase.TagFlashpixVersion));
            Assert.AreEqual(1, _exifSubIfdDirectory.GetInt(ExifDirectoryBase.TagColorSpace));
            Assert.AreEqual(1600, _exifSubIfdDirectory.GetInt(ExifDirectoryBase.TagExifImageWidth));
            Assert.AreEqual(1200, _exifSubIfdDirectory.GetInt(ExifDirectoryBase.TagExifImageHeight));
            Assert.AreEqual(3, _exifSubIfdDirectory.GetInt(ExifDirectoryBase.TagFileSource));
            Assert.AreEqual(1, _exifSubIfdDirectory.GetInt(ExifDirectoryBase.TagSceneType));
            Assert.AreEqual(6, _thumbDirectory.GetInt(ExifThumbnailDirectory.TagThumbnailCompression));
            Assert.AreEqual(2036, _thumbDirectory.GetInt(ExifThumbnailDirectory.TagThumbnailOffset));
            Assert.AreEqual(4662, _thumbDirectory.GetInt(ExifThumbnailDirectory.TagThumbnailLength));
        }
    }
}
