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
using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Photoshop;
using MetadataExtractor.IO;
using NUnit.Framework;

namespace MetadataExtractor.Tests.Formats.Icc
{
    public sealed class IccReaderTest
    {
        [Test]
        public void TestExtract()
        {
            var app2Bytes = File.ReadAllBytes("Tests/Data/iccDataInvalid1.jpg.app2");
            // ICC data starts after a 14-byte preamble
            var icc = TestHelper.SkipBytes(app2Bytes, 14);
            var directory = new IccReader().Extract(new ByteArrayReader(icc));
            Assert.IsNotNull(directory);
            // TODO validate expected values
        }

        [Test]
        public void TestReadJpegSegments()
        {
            var app2Bytes = File.ReadAllBytes("Tests/Data/iccDataInvalid1.jpg.app2");
            var directory = new IccReader().ReadJpegSegments(new[] { app2Bytes }, JpegSegmentType.App2);
            Assert.IsNotNull(directory);
            // TODO validate expected values
        }
    }
}
