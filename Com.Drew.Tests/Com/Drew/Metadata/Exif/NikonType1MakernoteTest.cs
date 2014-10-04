/*
 * Copyright 2002-2013 Drew Noakes
 *
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
 *    http://drewnoakes.com/code/exif/
 *    http://code.google.com/p/metadata-extractor/
 */
 using Com.Drew.Lang;
using Com.Drew.Metadata.Exif;
using Com.Drew.Metadata.Exif.Makernotes;
 using NUnit.Framework;
 using Sharpen;

namespace Com.Drew.Metadata.Exif
{
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class NikonType1MakernoteTest
	{
		private NikonType1MakernoteDirectory _nikonDirectory;

		private ExifIFD0Directory _exifIFD0Directory;

		private ExifSubIFDDirectory _exifSubIFDDirectory;

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
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.SetUp]
		public virtual void SetUp()
		{
			Com.Drew.Metadata.Metadata metadata = ExifReaderTest.ProcessBytes("Tests/Data/nikonMakernoteType1.jpg.app1");
			_nikonDirectory = metadata.GetDirectory<NikonType1MakernoteDirectory>();
			_exifSubIFDDirectory = metadata.GetDirectory<ExifSubIFDDirectory>();
			_exifIFD0Directory = metadata.GetDirectory<ExifIFD0Directory>();
			_thumbDirectory = metadata.GetDirectory<ExifThumbnailDirectory>();
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
		/// <exception cref="System.Exception"/>
        [NUnit.Framework.Test, SetCulture("en-US")]
		public virtual void TestNikonMakernote_MatchesKnownValues()
		{
			Sharpen.Tests.IsTrue(_nikonDirectory.GetTagCount() > 0);
			Sharpen.Tests.AreEqual(8, _nikonDirectory.GetDouble(NikonType1MakernoteDirectory.TagUnknown1), 0.0001);
			Sharpen.Tests.AreEqual(12, _nikonDirectory.GetInt(NikonType1MakernoteDirectory.TagQuality));
			Sharpen.Tests.AreEqual(1, _nikonDirectory.GetInt(NikonType1MakernoteDirectory.TagColorMode));
			Sharpen.Tests.AreEqual(3, _nikonDirectory.GetInt(NikonType1MakernoteDirectory.TagImageAdjustment));
			Sharpen.Tests.AreEqual(0, _nikonDirectory.GetInt(NikonType1MakernoteDirectory.TagCcdSensitivity));
			Sharpen.Tests.AreEqual(0, _nikonDirectory.GetInt(NikonType1MakernoteDirectory.TagWhiteBalance));
			Sharpen.Tests.AreEqual(0, _nikonDirectory.GetInt(NikonType1MakernoteDirectory.TagFocus));
			Sharpen.Tests.AreEqual(string.Empty, _nikonDirectory.GetString(NikonType1MakernoteDirectory.TagUnknown2));
			Sharpen.Tests.AreEqual(0, _nikonDirectory.GetDouble(NikonType1MakernoteDirectory.TagDigitalZoom), 0.0001);
			Sharpen.Tests.AreEqual(0, _nikonDirectory.GetInt(NikonType1MakernoteDirectory.TagConverter));
			long[] unknown3 = (long[])_nikonDirectory.GetObject(NikonType1MakernoteDirectory.TagUnknown3);
			long[] expected = new long[] { 0, 0, 16777216, 0, 2685774096L, 0, 34833, 6931, 16178, 4372, 4372, 3322676767L, 3373084416L, 15112, 0, 0, 1151495, 252903424, 17, 0, 0, 844038208, 55184128, 218129428, 1476410198
				, 370540566, 4044363286L, 16711749, 204629079, 1729 };
			NUnit.Framework.Assert.IsNotNull(unknown3);
			Sharpen.Tests.AreEqual(expected.Length, unknown3.Length);
			for (int i = 0; i < expected.Length; i++)
			{
				Sharpen.Tests.AreEqual(expected[i], unknown3[i]);
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
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestExifDirectory_MatchesKnownValues()
		{
			Sharpen.Tests.AreEqual("          ", _exifIFD0Directory.GetString(ExifIFD0Directory.TagImageDescription));
			Sharpen.Tests.AreEqual("NIKON", _exifIFD0Directory.GetString(ExifIFD0Directory.TagMake));
			Sharpen.Tests.AreEqual("E950", _exifIFD0Directory.GetString(ExifIFD0Directory.TagModel));
			Sharpen.Tests.AreEqual(1, _exifIFD0Directory.GetInt(ExifIFD0Directory.TagOrientation));
			Sharpen.Tests.AreEqual(300, _exifIFD0Directory.GetDouble(ExifIFD0Directory.TagXResolution), 0.001);
			Sharpen.Tests.AreEqual(300, _exifIFD0Directory.GetDouble(ExifIFD0Directory.TagYResolution), 0.001);
			Sharpen.Tests.AreEqual(2, _exifIFD0Directory.GetInt(ExifIFD0Directory.TagResolutionUnit));
			Sharpen.Tests.AreEqual("v981-79", _exifIFD0Directory.GetString(ExifIFD0Directory.TagSoftware));
			Sharpen.Tests.AreEqual("2001:04:06 11:51:40", _exifIFD0Directory.GetString(ExifIFD0Directory.TagDatetime));
			Sharpen.Tests.AreEqual(2, _exifIFD0Directory.GetInt(ExifIFD0Directory.TagYcbcrPositioning));
			Sharpen.Tests.AreEqual(new Rational(1, 77), _exifSubIFDDirectory.GetRational(ExifSubIFDDirectory.TagExposureTime));
			Sharpen.Tests.AreEqual(5.5, _exifSubIFDDirectory.GetDouble(ExifSubIFDDirectory.TagFnumber), 0.001);
			Sharpen.Tests.AreEqual(2, _exifSubIFDDirectory.GetInt(ExifSubIFDDirectory.TagExposureProgram));
			Sharpen.Tests.AreEqual(80, _exifSubIFDDirectory.GetInt(ExifSubIFDDirectory.TagIsoEquivalent));
			Sharpen.Tests.AreEqual("48 50 49 48", _exifSubIFDDirectory.GetString(ExifSubIFDDirectory.TagExifVersion));
			Sharpen.Tests.AreEqual("2001:04:06 11:51:40", _exifSubIFDDirectory.GetString(ExifSubIFDDirectory.TagDatetimeDigitized));
			Sharpen.Tests.AreEqual("2001:04:06 11:51:40", _exifSubIFDDirectory.GetString(ExifSubIFDDirectory.TagDatetimeOriginal));
			Sharpen.Tests.AreEqual("1 2 3 0", _exifSubIFDDirectory.GetString(ExifSubIFDDirectory.TagComponentsConfiguration));
			Sharpen.Tests.AreEqual(4, _exifSubIFDDirectory.GetInt(ExifSubIFDDirectory.TagCompressedAverageBitsPerPixel));
			Sharpen.Tests.AreEqual(0, _exifSubIFDDirectory.GetInt(ExifSubIFDDirectory.TagExposureBias));
			// this 2.6 *apex*, which is F2.5
			Sharpen.Tests.AreEqual(2.6, _exifSubIFDDirectory.GetDouble(ExifSubIFDDirectory.TagMaxAperture), 0.001);
			Sharpen.Tests.AreEqual(5, _exifSubIFDDirectory.GetInt(ExifSubIFDDirectory.TagMeteringMode));
			Sharpen.Tests.AreEqual(0, _exifSubIFDDirectory.GetInt(ExifSubIFDDirectory.TagWhiteBalance));
			Sharpen.Tests.AreEqual(0, _exifSubIFDDirectory.GetInt(ExifSubIFDDirectory.TagFlash));
			Sharpen.Tests.AreEqual(12.8, _exifSubIFDDirectory.GetDouble(ExifSubIFDDirectory.TagFocalLength), 0.001);
			Sharpen.Tests.AreEqual("0 0 0 0 0 0 0 0 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32 32"
				, _exifSubIFDDirectory.GetString(ExifSubIFDDirectory.TagUserComment));
			Sharpen.Tests.AreEqual("48 49 48 48", _exifSubIFDDirectory.GetString(ExifSubIFDDirectory.TagFlashpixVersion));
			Sharpen.Tests.AreEqual(1, _exifSubIFDDirectory.GetInt(ExifSubIFDDirectory.TagColorSpace));
			Sharpen.Tests.AreEqual(1600, _exifSubIFDDirectory.GetInt(ExifSubIFDDirectory.TagExifImageWidth));
			Sharpen.Tests.AreEqual(1200, _exifSubIFDDirectory.GetInt(ExifSubIFDDirectory.TagExifImageHeight));
			Sharpen.Tests.AreEqual(3, _exifSubIFDDirectory.GetInt(ExifSubIFDDirectory.TagFileSource));
			Sharpen.Tests.AreEqual(1, _exifSubIFDDirectory.GetInt(ExifSubIFDDirectory.TagSceneType));
			Sharpen.Tests.AreEqual(6, _thumbDirectory.GetInt(ExifThumbnailDirectory.TagThumbnailCompression));
			Sharpen.Tests.AreEqual(2036, _thumbDirectory.GetInt(ExifThumbnailDirectory.TagThumbnailOffset));
			Sharpen.Tests.AreEqual(4662, _thumbDirectory.GetInt(ExifThumbnailDirectory.TagThumbnailLength));
		}
	}
}
