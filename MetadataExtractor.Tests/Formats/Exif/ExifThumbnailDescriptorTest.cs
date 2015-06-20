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

using MetadataExtractor.Formats.Exif;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Exif
{
    /// <summary>JUnit test case for class ExifThumbnailDescriptor.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class ExifThumbnailDescriptorTest
    {

        [Fact]
        public void TestGetYCbCrSubsamplingDescription()
        {
            var directory = new ExifThumbnailDirectory();
            directory.Set(ExifDirectoryBase.TagYcbcrSubsampling, new[] { 2, 1 });
            var descriptor = new ExifThumbnailDescriptor(directory);
            Assert.Equal("YCbCr4:2:2", descriptor.GetDescription(ExifDirectoryBase.TagYcbcrSubsampling));
            Assert.Equal("YCbCr4:2:2", descriptor.GetYCbCrSubsamplingDescription());
            directory.Set(ExifDirectoryBase.TagYcbcrSubsampling, new[] { 2, 2 });
            Assert.Equal("YCbCr4:2:0", descriptor.GetDescription(ExifDirectoryBase.TagYcbcrSubsampling));
            Assert.Equal("YCbCr4:2:0", descriptor.GetYCbCrSubsamplingDescription());
        }
    }
}
