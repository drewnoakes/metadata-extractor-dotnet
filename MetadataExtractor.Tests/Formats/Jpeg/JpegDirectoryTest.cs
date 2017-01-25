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

using MetadataExtractor.Formats.Jpeg;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Jpeg
{
    /// <summary>Unit tests for <see cref="JpegDirectory"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JpegDirectoryTest
    {
        private readonly JpegDirectory _directory;

        public JpegDirectoryTest()
        {
            _directory = new JpegDirectory();
        }

        [Fact]
        public void SetAndGetValue()
        {
            _directory.Set(123, 8);
            Assert.Equal(8, _directory.GetInt32(123));
        }

        [Fact]
        public void GetComponent_NotAdded()
        {
            Assert.Null(_directory.GetComponent(1));
        }

        // NOTE tests for individual tag values exist in JpegReaderTest

        [Fact]
        public void GetImageWidth()
        {
            _directory.Set(JpegDirectory.TagImageWidth, 123);

            Assert.Equal(123, _directory.GetImageWidth());
        }

        [Fact]
        public void GetImageHeight()
        {
            _directory.Set(JpegDirectory.TagImageHeight, 123);

            Assert.Equal(123, _directory.GetImageHeight());
        }

        [Fact]
        public void GetNumberOfComponents()
        {
            _directory.Set(JpegDirectory.TagNumberOfComponents, 3);

            Assert.Equal(3, _directory.GetNumberOfComponents());
            Assert.Equal("3", _directory.GetDescription(JpegDirectory.TagNumberOfComponents));
        }

        [Fact]
        public void GetComponent()
        {
            var component1 = new JpegComponent(1, 2, 3);
            var component2 = new JpegComponent(1, 2, 3);
            var component3 = new JpegComponent(1, 2, 3);
            var component4 = new JpegComponent(1, 2, 3);

            _directory.Set(JpegDirectory.TagComponentData1, component1);
            _directory.Set(JpegDirectory.TagComponentData2, component2);
            _directory.Set(JpegDirectory.TagComponentData3, component3);
            _directory.Set(JpegDirectory.TagComponentData4, component4);

            // component numbers are zero-indexed for this method
            Assert.Same(component1, _directory.GetComponent(0));
            Assert.Same(component2, _directory.GetComponent(1));
            Assert.Same(component3, _directory.GetComponent(2));
            Assert.Same(component4, _directory.GetComponent(3));
        }
    }
}
