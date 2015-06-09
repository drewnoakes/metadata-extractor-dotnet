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

using Com.Drew.Lang;
using NUnit.Framework;
using Sharpen;

namespace Com.Drew.Metadata.Jfif
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JfifReaderTest
    {
        /// <exception cref="System.Exception"/>
        [Test]
        public void TestRead()
        {
            byte[] jfifData = new byte[] { 74, 70, 73, 70, 0, 1, 2, 1, 0, 108, 0, 108, 0, 0 };
            Metadata metadata = new Metadata();
            JfifReader reader = new JfifReader();
            reader.Extract(new ByteArrayReader(jfifData), metadata);
            Assert.AreEqual(1, metadata.GetDirectoryCount());
            JfifDirectory directory = metadata.GetFirstDirectoryOfType<JfifDirectory>();
            Assert.IsNotNull(directory);
            Assert.IsFalse(directory.HasErrors(), Extensions.ConvertToString(directory.GetErrors()));
            Tag[] tags = Collections.ToArray(directory.GetTags(), new Tag[directory.GetTagCount()]);
            Assert.AreEqual(4, tags.Length);
            Assert.AreEqual(JfifDirectory.TagVersion, tags[0].GetTagType());
            Assert.AreEqual(unchecked(0x0102), directory.GetInt(tags[0].GetTagType()));
            Assert.AreEqual(JfifDirectory.TagUnits, tags[1].GetTagType());
            Assert.AreEqual(1, directory.GetInt(tags[1].GetTagType()));
            Assert.AreEqual(JfifDirectory.TagResx, tags[2].GetTagType());
            Assert.AreEqual(108, directory.GetInt(tags[2].GetTagType()));
            Assert.AreEqual(JfifDirectory.TagResy, tags[3].GetTagType());
            Assert.AreEqual(108, directory.GetInt(tags[3].GetTagType()));
        }
    }
}
