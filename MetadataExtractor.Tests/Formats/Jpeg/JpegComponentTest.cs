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

using MetadataExtractor.Formats.Jpeg;
using NUnit.Framework;

namespace MetadataExtractor.Tests.Formats.Jpeg
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JpegComponentTest
    {
        [Test]
        public void TestGetComponentCharacter()
        {
            Assert.AreEqual("Y",  new JpegComponent(1, 2, 3).GetComponentName());
            Assert.AreEqual("Cb", new JpegComponent(2, 2, 3).GetComponentName());
            Assert.AreEqual("Cr", new JpegComponent(3, 2, 3).GetComponentName());
            Assert.AreEqual("I",  new JpegComponent(4, 2, 3).GetComponentName());
            Assert.AreEqual("Q",  new JpegComponent(5, 2, 3).GetComponentName());
        }
    }
}
