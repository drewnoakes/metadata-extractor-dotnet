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

using MetadataExtractor.Formats.Exif.Makernotes;
using MetadataExtractor.Formats.Jpeg;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Exif
{
    /// <summary>Unit tests for Panasonic maker notes.</summary>
    /// <author>psandhaus</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <seealso cref="PanasonicMakernoteDirectory"/>
    /// <seealso cref="PanasonicMakernoteDescriptor"/>
    public sealed class PanasonicMakernoteTest
    {
        private readonly PanasonicMakernoteDirectory _panasonicDirectory;

        public PanasonicMakernoteTest()
        {
            _panasonicDirectory = ExifReaderTest.ProcessSegmentBytes<PanasonicMakernoteDirectory>("Data/withPanasonicFaces.jpg.app1", JpegSegmentType.App1);
        }

        [Fact]
        public void GetDetectedFaces()
        {
            Assert.Equal(
                new[] { new Face(142, 120, 76, 76) },
                _panasonicDirectory.GetDetectedFaces());
        }

        [Fact]
        public void GetRecognizedFaces()
        {
            Assert.Equal(
                new[] { new Face(142, 120, 76, 76, "NIELS", new Age(31, 7, 15, 0, 0, 0)) },
                _panasonicDirectory.GetRecognizedFaces());
        }
    }
}
