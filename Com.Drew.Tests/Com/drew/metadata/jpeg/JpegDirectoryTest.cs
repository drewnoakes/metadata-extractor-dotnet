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

using NUnit.Framework;
using Sharpen;

namespace Com.Drew.Metadata.Jpeg
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class JpegDirectoryTest
    {
        private JpegDirectory _directory;

        [SetUp]
        public virtual void SetUp()
        {
            _directory = new JpegDirectory();
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestSetAndGetValue()
        {
            _directory.SetInt(123, 8);
            Assert.AreEqual(8, _directory.GetInt(123));
        }

        [Test]
        public virtual void TestGetComponent_NotAdded()
        {
            Assert.IsNull(_directory.GetComponent(1));
        }

        // NOTE tests for individual tag values exist in JpegReaderTest.java
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetImageWidth()
        {
            _directory.SetInt(JpegDirectory.TagImageWidth, 123);
            Assert.AreEqual(123, _directory.GetImageWidth());
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetImageHeight()
        {
            _directory.SetInt(JpegDirectory.TagImageHeight, 123);
            Assert.AreEqual(123, _directory.GetImageHeight());
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetNumberOfComponents()
        {
            _directory.SetInt(JpegDirectory.TagNumberOfComponents, 3);
            Assert.AreEqual(3, _directory.GetNumberOfComponents());
            Assert.AreEqual("3", _directory.GetDescription(JpegDirectory.TagNumberOfComponents));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetComponent()
        {
            JpegComponent component1 = new JpegComponent(1, 2, 3);
            JpegComponent component2 = new JpegComponent(1, 2, 3);
            JpegComponent component3 = new JpegComponent(1, 2, 3);
            JpegComponent component4 = new JpegComponent(1, 2, 3);
            _directory.SetObject(JpegDirectory.TagComponentData1, component1);
            _directory.SetObject(JpegDirectory.TagComponentData2, component2);
            _directory.SetObject(JpegDirectory.TagComponentData3, component3);
            _directory.SetObject(JpegDirectory.TagComponentData4, component4);
            // component numbers are zero-indexed for this method
            Assert.AreSame(component1, _directory.GetComponent(0));
            Assert.AreSame(component2, _directory.GetComponent(1));
            Assert.AreSame(component3, _directory.GetComponent(2));
            Assert.AreSame(component4, _directory.GetComponent(3));
        }
    }
}
