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

using System;
using System.IO;
using System.Linq;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Xmp;
using NUnit.Framework;

namespace MetadataExtractor.Tests.Formats.Xmp
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class XmpReaderTest
    {
        private XmpDirectory _directory;

        [SetUp]
        public void SetUp()
        {
            var jpegSegments = new [] { File.ReadAllBytes("Tests/Data/withXmpAndIptc.jpg.app1.1") };
            var directories = new XmpReader().ReadJpegSegments(jpegSegments, JpegSegmentType.App1);
            _directory = directories.OfType<XmpDirectory>().ToList().Single();
            Assert.IsFalse(_directory.HasError);
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

        [Test]
        public void TestExtract_LensInformation()
        {
            // Note that this tag really holds a rational array, but XmpReader doesn't parse arrays
            Assert.AreEqual("24/1 70/1 0/0 0/0", _directory.GetString(XmpDirectory.TagLensInfo));
        }

//        Rational[] info = _directory.getRationalArray(XmpDirectory.TAG_LENS_INFO);
//        Assert.Equals(new Rational(24, 1), info[0]);
//        Assert.Equals(new Rational(70, 1), info[1]);
//        Assert.Equals(new Rational(0, 0), info[2]);
//        Assert.Equals(new Rational(0, 0), info[3]);

        [Test]
        public void TestExtract_HasXMPMeta()
        {
            Assert.IsNotNull(_directory.XmpMeta);
        }

        [Test]
        public void TestExtract_Lens()
        {
            Assert.AreEqual("EF24-70mm f/2.8L USM", _directory.GetString(XmpDirectory.TagLens));
        }

/*
        // this requires further research

        public void TestExtract_Format() throws Exception
        {
            assertEquals("image/tiff", _directory.getString(XmpDirectory.TAG_FORMAT));
        }

        public void TestExtract_Creator() throws Exception
        {
            assertEquals("", _directory.getString(XmpDirectory.TAG_CREATOR));
        }

        public void TestExtract_Rights() throws Exception
        {
            assertEquals("", _directory.getString(XmpDirectory.TAG_RIGHTS));
        }

        public void TestExtract_Description() throws Exception
        {
            assertEquals("", _directory.getString(XmpDirectory.TAG_DESCRIPTION));
        }
*/

        [Test]
        public void TestExtract_SerialNumber()
        {
            Assert.AreEqual("380319450", _directory.GetString(XmpDirectory.TagCameraSerialNumber));
        }

        [Test]
        public void TestExtract_Firmware()
        {
            Assert.AreEqual("1.2.1", _directory.GetString(XmpDirectory.TagFirmware));
        }

        [Test]
        public void TestExtract_Maker()
        {
            Assert.AreEqual("Canon", _directory.GetString(XmpDirectory.TagMake));
        }

        [Test]
        public void TestExtract_Model()
        {
            Assert.AreEqual("Canon EOS 7D", _directory.GetString(XmpDirectory.TagModel));
        }

        [Test]
        public void TestExtract_ExposureTime()
        {
            // Note XmpReader doesn't parse this as a rational even though it appears to be... need more examples
            Assert.AreEqual("1/125", _directory.GetString(XmpDirectory.TagExposureTime));
        }

//        Assert.Equals(new Rational(1, 125), _directory.getRational(XmpDirectory.TAG_EXPOSURE_TIME));

        [Test]
        public void TestExtract_ExposureProgram()
        {
            Assert.AreEqual(1, _directory.GetInt32(XmpDirectory.TagExposureProgram));
        }

        [Test]
        public void TestExtract_FNumber()
        {
            Assert.AreEqual(new Rational(11, 1), _directory.GetRational(XmpDirectory.TagFNumber));
        }

        [Test]
        public void TestExtract_FocalLength()
        {
            Assert.AreEqual(new Rational(57, 1), _directory.GetRational(XmpDirectory.TagFocalLength));
        }

        [Test]
        public void TestExtract_ShutterSpeed()
        {
            Assert.AreEqual(new Rational(6965784, 1000000), _directory.GetRational(XmpDirectory.TagShutterSpeed));
        }

        [Test]
        public void TestExtract_OriginalDateTime()
        {
            var actual = _directory.GetDateTimeNullable(XmpDirectory.TagDateTimeOriginal);
            // Underlying string value (in XMP data) is: 2010-12-12T12:41:35.00+01:00
            Assert.AreEqual(DateTime.ParseExact("11:41:35 12 12 2010 +0000", "hh:mm:ss dd MM yyyy Z", null), actual);
            Assert.AreEqual(new DateTime(2010, 12, 12, 11, 41, 35), actual.Value);
        }

        [Test]
        public void TestExtract_DigitizedDateTime()
        {
            var actual = _directory.GetDateTimeNullable(XmpDirectory.TagDateTimeDigitized);
            // Underlying string value (in XMP data) is: 2010-12-12T12:41:35.00+01:00
            Assert.AreEqual(DateTime.ParseExact("11:41:35 12 12 2010 +0000", "hh:mm:ss dd MM yyyy Z", null), actual);
            Assert.AreEqual(new DateTime(2010, 12, 12, 11, 41, 35), actual.Value);
        }

        [Test]
        public void TestGetXmpProperties()
        {
            var propertyMap = _directory.GetXmpProperties();
            Assert.AreEqual(179, propertyMap.Count);
            Assert.IsTrue(propertyMap.ContainsKey("photoshop:Country"));
            Assert.AreEqual("Deutschland", propertyMap["photoshop:Country"]);
            Assert.IsTrue(propertyMap.ContainsKey("tiff:ImageLength"));
            Assert.AreEqual("900", propertyMap["tiff:ImageLength"]);
        }
    }
}
