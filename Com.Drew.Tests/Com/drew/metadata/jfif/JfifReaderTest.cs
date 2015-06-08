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
    public class JfifReaderTest
    {
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestRead()
        {
            sbyte[] jfifData = new sbyte[] { 74, 70, 73, 70, 0, 1, 2, 1, 0, 108, 0, 108, 0, 0 };
            Metadata metadata = new Metadata();
            JfifReader reader = new JfifReader();
            reader.Extract(new ByteArrayReader(jfifData), metadata);
            Tests.AreEqual(1, metadata.GetDirectoryCount());
            JfifDirectory directory = metadata.GetFirstDirectoryOfType<JfifDirectory>();
            Assert.IsNotNull(directory);
            Tests.IsFalse(Extensions.ConvertToString(directory.GetErrors()), directory.HasErrors());
            Tag[] tags = Collections.ToArray(directory.GetTags(), new Tag[directory.GetTagCount()]);
            Tests.AreEqual(4, tags.Length);
            Tests.AreEqual(JfifDirectory.TagVersion, tags[0].GetTagType());
            Tests.AreEqual(unchecked((int)(0x0102)), directory.GetInt(tags[0].GetTagType()));
            Tests.AreEqual(JfifDirectory.TagUnits, tags[1].GetTagType());
            Tests.AreEqual(1, directory.GetInt(tags[1].GetTagType()));
            Tests.AreEqual(JfifDirectory.TagResx, tags[2].GetTagType());
            Tests.AreEqual(108, directory.GetInt(tags[2].GetTagType()));
            Tests.AreEqual(JfifDirectory.TagResy, tags[3].GetTagType());
            Tests.AreEqual(108, directory.GetInt(tags[3].GetTagType()));
        }
    }
}
