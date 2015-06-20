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

using System.IO;
using System.Linq;
using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.IO;
using NUnit.Framework;

namespace MetadataExtractor.Tests.Formats.Icc
{
    public sealed class IccReaderTest
    {
        // TODO add a test with well-formed ICC data and assert output values are correct

        [Test]
        public void TestExtract_InvalidData()
        {
            var app2Bytes = File.ReadAllBytes("Tests/Data/iccDataInvalid1.jpg.app2");

            // When in an APP2 segment, ICC data starts after a 14-byte preamble
            var icc = TestHelper.SkipBytes(app2Bytes, 14);
            var directory = new IccReader().Extract(new ByteArrayReader(icc));
            Assert.IsNotNull(directory);
            Assert.IsTrue(directory.HasError);
        }

        [Test]
        public void TestReadJpegSegments_InvalidData()
        {
            var app2Bytes = File.ReadAllBytes("Tests/Data/iccDataInvalid1.jpg.app2");
            var directory = new IccReader().ReadJpegSegments(new[] { app2Bytes }, JpegSegmentType.App2);
            Assert.IsNotNull(directory);
            Assert.IsTrue(directory.Single().HasError);
        }

        [Test]
        public void GetStringFromUInt32()
        {
            Assert.AreEqual("ABCD", IccReader.GetStringFromUInt32(0x41424344u));
        }
    }
}
