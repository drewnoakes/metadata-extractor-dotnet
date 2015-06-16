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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;
using NUnit.Framework;

namespace MetadataExtractor.Tests.Formats.Jpeg
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JpegMetadataReaderTest
    {
        [Test]
        public void TestExtractMetadata()
        {
            Validate(JpegMetadataReader.ReadMetadata("Tests/Data/withExif.jpg"));
        }

        [Test]
        public void TestExtractMetadataUsingInputStream()
        {
            Validate(JpegMetadataReader.ReadMetadata(new FileStream("Tests/Data/withExif.jpg", FileMode.Open)));
        }

        private static void Validate(IReadOnlyList<Directory> metadata)
        {
            var directory = metadata.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            Assert.IsNotNull(directory);
            Assert.AreEqual("80", directory.GetString(ExifDirectoryBase.TagIsoEquivalent));
        }
    }
}
