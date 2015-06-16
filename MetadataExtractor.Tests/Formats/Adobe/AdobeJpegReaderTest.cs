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
using JetBrains.Annotations;
using MetadataExtractor.Formats.Adobe;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.IO;
using NUnit.Framework;

namespace MetadataExtractor.Tests.Formats.Adobe
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class AdobeJpegReaderTest
    {
        [NotNull]
        public static AdobeJpegDirectory ProcessBytes([NotNull] string filePath)
        {
            return new AdobeJpegReader()
                .Extract(new SequentialByteArrayReader(File.ReadAllBytes(filePath)));
        }

        [Test]
        public void TestSegmentTypes()
        {
            CollectionAssert.AreEqual(new[] { JpegSegmentType.AppE }, new AdobeJpegReader().GetSegmentTypes());
        }

        [Test]
        public void TestReadAdobeJpegMetadata1()
        {
            var directory = ProcessBytes("Tests/Data/adobeJpeg1.jpg.appe");

            Assert.IsFalse(directory.HasErrors, directory.Errors.ToString());
            Assert.AreEqual(4, directory.TagCount);
            Assert.AreEqual(1, directory.GetInt32(AdobeJpegDirectory.TagColorTransform));
            Assert.AreEqual(25600, directory.GetInt32(AdobeJpegDirectory.TagDctEncodeVersion));
            Assert.AreEqual(128, directory.GetInt32(AdobeJpegDirectory.TagApp14Flags0));
            Assert.AreEqual(0, directory.GetInt32(AdobeJpegDirectory.TagApp14Flags1));
        }
    }
}
