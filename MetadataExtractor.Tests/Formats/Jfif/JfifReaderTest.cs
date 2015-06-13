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

using MetadataExtractor.Formats.Jfif;
using MetadataExtractor.IO;
using NUnit.Framework;

namespace MetadataExtractor.Tests.Formats.Jfif
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JfifReaderTest
    {
        [Test]
        public void TestRead()
        {
            var jfifData = new byte[] { 74, 70, 73, 70, 0, 1, 2, 1, 0, 108, 0, 108, 0, 0 };

            var metadata = new Metadata();
            new JfifReader().Extract(new ByteArrayReader(jfifData), metadata);

            Assert.AreEqual(1, metadata.GetDirectoryCount());

            var directory = metadata.GetFirstDirectoryOfType<JfifDirectory>();

            Assert.IsNotNull(directory);
            Assert.IsFalse(directory.HasErrors, directory.Errors.ToString());

            var tags = directory.Tags;

            Assert.AreEqual(4, tags.Count);
            Assert.AreEqual(JfifDirectory.TagVersion, tags[0].TagType);
            Assert.AreEqual(0x0102, directory.GetInt32(tags[0].TagType));
            Assert.AreEqual(JfifDirectory.TagUnits, tags[1].TagType);
            Assert.AreEqual(1, directory.GetInt32(tags[1].TagType));
            Assert.AreEqual(JfifDirectory.TagResX, tags[2].TagType);
            Assert.AreEqual(108, directory.GetInt32(tags[2].TagType));
            Assert.AreEqual(JfifDirectory.TagResY, tags[3].TagType);
            Assert.AreEqual(108, directory.GetInt32(tags[3].TagType));
        }
    }
}
