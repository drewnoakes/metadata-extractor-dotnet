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

using System;
using System.Linq;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Xmp;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Xmp
{
    /// <summary>Unit tests for <see cref="XmpReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class XmpReaderTest
    {
        private readonly XmpDirectory _directory;

        public XmpReaderTest()
        {
            var jpegSegments = new [] { new JpegSegment(JpegSegmentType.App1, TestDataUtil.GetBytes("Data/withXmpAndIptc.jpg.app1.1"), offset: 0) };
            var directories = new XmpReader().ReadJpegSegments(jpegSegments);
            _directory = directories.OfType<XmpDirectory>().ToList().Single();
            Assert.False(_directory.HasError);
        }

        /*
        [Xmp] Lens Information = 24/1 70/1 0/0 0/0
        [Xmp] Lens = EF24-70mm f/2.8L USM
        [Xmp] Serial Number = 380319450
        [Xmp] Firmware = 1.2.1
        [Xmp] Make = Canon
        [Xmp] Model = Canon EOS 7D
        [Xmp] Exposure Time = 1/125 sec
        [Xmp] Exposure Program = Manual control
        [Xmp] Aperture Value = F11
        [Xmp] F-Number = F11
        [Xmp] Focal Length = 57.0 mm
        [Xmp] Shutter Speed Value = 1/124 sec
        [Xmp] Date/Time Original = Sun Dec 12 11:41:35 GMT 2010
        [Xmp] Date/Time Digitized = Sun Dec 12 11:41:35 GMT 2010
        */

        [Fact]
        public void Extract_LensInformation()
        {
            // Note that this tag really holds a rational array, but XmpReader doesn't parse arrays
            Assert.Equal("24/1 70/1 0/0 0/0", _directory.GetString(XmpDirectory.TagLensInfo));
        }

//        Rational[] info = _directory.getRationalArray(XmpDirectory.TAG_LENS_INFO);
//        Assert.Equals(new Rational(24, 1), info[0]);
//        Assert.Equals(new Rational(70, 1), info[1]);
//        Assert.Equals(new Rational(0, 0), info[2]);
//        Assert.Equals(new Rational(0, 0), info[3]);

        [Fact]
        public void Extract_HasXMPMeta()
        {
            Assert.NotNull(_directory.XmpMeta);
        }

        [Fact]
        public void Extract_Lens()
        {
            Assert.Equal("EF24-70mm f/2.8L USM", _directory.GetString(XmpDirectory.TagLens));
        }

/*
        // this requires further research

        public void Extract_Format()
        {
            Assert.Equal("image/tiff", _directory.GetString(XmpDirectory.TagFormat));
        }

        public void Extract_Creator()
        {
            Assert.Equal("", _directory.GetString(XmpDirectory.TagCreator));
        }

        public void Extract_Rights()
        {
            Assert.Equal("", _directory.GetString(XmpDirectory.TagRights));
        }

        public void Extract_Description()
        {
            Assert.Equal("", _directory.GetString(XmpDirectory.TagDescription));
        }
*/

        [Fact]
        public void Extract_SerialNumber()
        {
            Assert.Equal("380319450", _directory.GetString(XmpDirectory.TagCameraSerialNumber));
        }

        [Fact]
        public void Extract_Firmware()
        {
            Assert.Equal("1.2.1", _directory.GetString(XmpDirectory.TagFirmware));
        }

        [Fact]
        public void Extract_Maker()
        {
            Assert.Equal("Canon", _directory.GetString(XmpDirectory.TagMake));
        }

        [Fact]
        public void Extract_Model()
        {
            Assert.Equal("Canon EOS 7D", _directory.GetString(XmpDirectory.TagModel));
        }

        [Fact]
        public void Extract_ExposureTime()
        {
            // Note XmpReader doesn't parse this as a rational even though it appears to be... need more examples
            Assert.Equal("1/125", _directory.GetString(XmpDirectory.TagExposureTime));
        }

//        Assert.Equals(new Rational(1, 125), _directory.getRational(XmpDirectory.TAG_EXPOSURE_TIME));

        [Fact]
        public void Extract_ExposureProgram()
        {
            Assert.Equal(1, _directory.GetInt32(XmpDirectory.TagExposureProgram));
        }

        [Fact]
        public void Extract_FNumber()
        {
            Assert.Equal(new Rational(11, 1), _directory.GetRational(XmpDirectory.TagFNumber));
        }

        [Fact]
        public void Extract_FocalLength()
        {
            Assert.Equal(new Rational(57, 1), _directory.GetRational(XmpDirectory.TagFocalLength));
        }

        [Fact]
        public void Extract_ShutterSpeed()
        {
            Assert.Equal(new Rational(6965784, 1000000), _directory.GetRational(XmpDirectory.TagShutterSpeed));
        }

        [Fact(Skip = "TODO fix XMP support for DateTime with offset")]
        public void Extract_OriginalDateTime()
        {
            var actual = _directory.GetDateTime(XmpDirectory.TagDateTimeOriginal);
            // Underlying string value (in XMP data) is: 2010-12-12T12:41:35.00+01:00
            Assert.Equal(DateTime.ParseExact("11:41:35 12 12 2010 +0000", "hh:mm:ss dd MM yyyy zzz", null), actual);
            Assert.Equal(new DateTime(2010, 12, 12, 11, 41, 35), actual);
        }

        [Fact(Skip = "TODO fix XMP support for DateTime with offset")]
        public void Extract_DigitizedDateTime()
        {
            var actual = _directory.GetDateTime(XmpDirectory.TagDateTimeDigitized);
            // Underlying string value (in XMP data) is: 2010-12-12T12:41:35.00+01:00
            Assert.Equal(DateTime.ParseExact("11:41:35 12 12 2010 +0000", "hh:mm:ss dd MM yyyy zzz", null), actual);
            Assert.Equal(new DateTime(2010, 12, 12, 11, 41, 35), actual);
        }

        [Fact]
        public void GetXmpProperties()
        {
            var propertyMap = _directory.GetXmpProperties();

            Assert.Equal(179, propertyMap.Count);

            Assert.True(propertyMap.ContainsKey("photoshop:Country"));
            Assert.Equal("Deutschland", propertyMap["photoshop:Country"]);

            Assert.True(propertyMap.ContainsKey("tiff:ImageLength"));
            Assert.Equal("900", propertyMap["tiff:ImageLength"]);
        }
    }
}
