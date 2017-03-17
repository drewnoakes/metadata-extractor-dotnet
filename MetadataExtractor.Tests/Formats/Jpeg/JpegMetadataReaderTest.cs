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

using System.Collections.Generic;
using System.Linq;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Jpeg
{
    /// <summary>Unit tests for <see cref="JpegMetadataReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JpegMetadataReaderTest
    {
#if !NETCOREAPP1_0
        [Fact]
        public void ExtractMetadataUsingPath()
        {
            Validate(JpegMetadataReader.ReadMetadata("Data/withExif.jpg"));
        }
#endif

        [Fact]
        public void ExtractMetadataUsingStream()
        {
            using (var stream = TestDataUtil.OpenRead("Data/withExif.jpg"))
                Validate(JpegMetadataReader.ReadMetadata(stream));
        }

        private static void Validate(IEnumerable<Directory> metadata)
        {
            var directory = metadata.OfType<ExifSubIfdDirectory>().FirstOrDefault();

            Assert.NotNull(directory);
            Assert.Equal("80", directory.GetString(ExifDirectoryBase.TagIsoEquivalent));
        }
    }
}
