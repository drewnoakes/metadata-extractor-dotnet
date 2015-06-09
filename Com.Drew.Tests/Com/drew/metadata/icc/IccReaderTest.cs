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
using Com.Drew.Testing;
using Com.Drew.Tools;
using NUnit.Framework;

namespace Com.Drew.Metadata.Icc
{
    public sealed class IccReaderTest
    {
        /// <exception cref="System.Exception"/>
        [Test]
        public void TestExtract()
        {
            byte[] app2Bytes = System.IO.File.ReadAllBytes("Tests/Data/iccDataInvalid1.jpg.app2");
            // ICC data starts after a 14-byte preamble
            byte[] icc = TestHelper.SkipBytes(app2Bytes, 14);
            Metadata metadata = new Metadata();
            new IccReader().Extract(new ByteArrayReader(icc), metadata);
            IccDirectory directory = metadata.GetFirstDirectoryOfType<IccDirectory>();
            Assert.IsNotNull(directory);
        }
        // TODO validate expected values
        //        for (Tag tag : directory.getTags()) {
        //            System.out.println(tag);
        //        }
    }
}
