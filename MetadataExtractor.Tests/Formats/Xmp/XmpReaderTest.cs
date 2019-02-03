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

        [Fact]
        public void Extract_HasXMPMeta()
        {
            Assert.NotNull(_directory.XmpMeta);
        }

        [Fact]
        public void testExtract_PropertyCount()
        {
            Assert.Equal(179, _directory.GetInt32(XmpDirectory.TagXmpValueCount));
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
