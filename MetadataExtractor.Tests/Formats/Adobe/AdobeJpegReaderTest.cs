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

using System.IO;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Adobe;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Adobe
{
    /// <summary>Unit tests for <see cref="AdobeJpegReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class AdobeJpegReaderTest
    {
        [NotNull]
        private static AdobeJpegDirectory ProcessBytes([NotNull] string filePath)
        {
            return new AdobeJpegReader()
                .Extract(new SequentialByteArrayReader(File.ReadAllBytes(TestDataUtil.GetPath(filePath))));
        }

        [Fact]
        public void SegmentTypes()
        {
            Assert.Equal(
                new[] { JpegSegmentType.AppE },
                ((IJpegSegmentMetadataReader)new AdobeJpegReader()).SegmentTypes);
        }

        [Fact]
        public void ReadAdobeJpegMetadata1()
        {
            var directory = ProcessBytes("Data/adobeJpeg1.jpg.appe");

            Assert.False(directory.HasError, directory.Errors.ToString());
            Assert.Equal(4, directory.TagCount);
            Assert.Equal(1, directory.GetInt32(AdobeJpegDirectory.TagColorTransform));
            Assert.Equal(25600, directory.GetInt32(AdobeJpegDirectory.TagDctEncodeVersion));
            Assert.Equal(128, directory.GetInt32(AdobeJpegDirectory.TagApp14Flags0));
            Assert.Equal(0, directory.GetInt32(AdobeJpegDirectory.TagApp14Flags1));
        }
    }
}
