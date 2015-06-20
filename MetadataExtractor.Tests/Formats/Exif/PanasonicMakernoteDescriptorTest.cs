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

using MetadataExtractor.Formats.Exif.Makernotes;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Exif
{
    /// <author>psandhaus, Drew Noakes</author>
    public sealed class PanasonicMakernoteDescriptorTest
    {
        private readonly PanasonicMakernoteDirectory _panasonicDirectory;

        public PanasonicMakernoteDescriptorTest()
        {
            _panasonicDirectory = ExifReaderTest.ProcessSegmentBytes<PanasonicMakernoteDirectory>("Tests/Data/withPanasonicFaces.jpg.app1");
        }

        [Fact]
        public void TestGetDetectedFaces()
        {
            Assert.Equal(
                new[] { new Face(142, 120, 76, 76) },
                _panasonicDirectory.GetDetectedFaces());
        }

        [Fact]
        public void TestGetRecognizedFaces()
        {
            Assert.Equal(
                new[] { new Face(142, 120, 76, 76, "NIELS", new Age(31, 7, 15, 0, 0, 0)) },
                _panasonicDirectory.GetRecognizedFaces());
        }
    }
}
