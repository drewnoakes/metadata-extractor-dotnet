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

using MetadataExtractor.Formats.Jpeg;
using NUnit.Framework;

namespace MetadataExtractor.Tests.Formats.Jpeg
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JpegDescriptorTest
    {
        private JpegDirectory _directory;

        private JpegDescriptor _descriptor;


        [SetUp]
        public void SetUp()
        {
            _directory = new JpegDirectory();
            _descriptor = new JpegDescriptor(_directory);
        }


        [Test]
        public void TestGetComponentDataDescription_InvalidComponentNumber()
        {
            Assert.IsNull(_descriptor.GetComponentDataDescription(1));
        }


        [Test]
        public void TestGetImageWidthDescription()
        {
            _directory.SetInt(JpegDirectory.TagImageWidth, 123);
            Assert.AreEqual("123 pixels", _descriptor.GetImageWidthDescription());
            Assert.AreEqual("123 pixels", _directory.GetDescription(JpegDirectory.TagImageWidth));
        }


        [Test]
        public void TestGetImageHeightDescription()
        {
            _directory.SetInt(JpegDirectory.TagImageHeight, 123);
            Assert.AreEqual("123 pixels", _descriptor.GetImageHeightDescription());
            Assert.AreEqual("123 pixels", _directory.GetDescription(JpegDirectory.TagImageHeight));
        }


        [Test]
        public void TestGetDataPrecisionDescription()
        {
            _directory.SetInt(JpegDirectory.TagDataPrecision, 8);
            Assert.AreEqual("8 bits", _descriptor.GetDataPrecisionDescription());
            Assert.AreEqual("8 bits", _directory.GetDescription(JpegDirectory.TagDataPrecision));
        }

        /// <exception cref="MetadataException"/>
        [Test]
        public void TestGetComponentDescription()
        {
            var component1 = new JpegComponent(1, unchecked(0x22), 0);
            _directory.SetObject(JpegDirectory.TagComponentData1, component1);
            Assert.AreEqual("Y component: Quantization table 0, Sampling factors 2 horiz/2 vert", _directory.GetDescription(JpegDirectory.TagComponentData1));
            Assert.AreEqual("Y component: Quantization table 0, Sampling factors 2 horiz/2 vert", _descriptor.GetComponentDataDescription(0));
        }
    }
}
