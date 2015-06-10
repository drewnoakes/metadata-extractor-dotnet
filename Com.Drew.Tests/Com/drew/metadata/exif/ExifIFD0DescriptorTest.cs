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

using System.Text;
using Com.Drew.Lang;
using NUnit.Framework;

namespace Com.Drew.Metadata.Exif
{
    /// <summary>
    /// Unit tests for <see cref="ExifIfd0Descriptor"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class ExifIfd0DescriptorTest
    {

        [Test]
        public void TestXResolutionDescription()
        {
            var directory = new ExifIfd0Directory();
            directory.SetRational(ExifDirectoryBase.TagXResolution, new Rational(72, 1));
            // 2 is for 'Inch'
            directory.SetInt(ExifDirectoryBase.TagResolutionUnit, 2);
            var descriptor = new ExifIfd0Descriptor(directory);
            Assert.AreEqual("72 dots per inch", descriptor.GetDescription(ExifDirectoryBase.TagXResolution));
        }


        [Test]
        public void TestYResolutionDescription()
        {
            var directory = new ExifIfd0Directory();
            directory.SetRational(ExifDirectoryBase.TagYResolution, new Rational(50, 1));
            // 3 is for 'cm'
            directory.SetInt(ExifDirectoryBase.TagResolutionUnit, 3);
            var descriptor = new ExifIfd0Descriptor(directory);
            Assert.AreEqual("50 dots per cm", descriptor.GetDescription(ExifDirectoryBase.TagYResolution));
        }


        [Test]
        public void TestWindowsXpFields()
        {
            var directory = ExifReaderTest.ProcessBytes<ExifIfd0Directory>("Tests/Data/windowsXpFields.jpg.app1");
            Assert.AreEqual("Testing artist\x0", directory.GetString(ExifDirectoryBase.TagWinAuthor, Encoding.Unicode));
            Assert.AreEqual("Testing comments\x0", directory.GetString(ExifDirectoryBase.TagWinComment, Encoding.Unicode));
            Assert.AreEqual("Testing keywords\x0", directory.GetString(ExifDirectoryBase.TagWinKeywords, Encoding.Unicode));
            Assert.AreEqual("Testing subject\x0", directory.GetString(ExifDirectoryBase.TagWinSubject, Encoding.Unicode));
            Assert.AreEqual("Testing title\x0", directory.GetString(ExifDirectoryBase.TagWinTitle, Encoding.Unicode));
            var descriptor = new ExifIfd0Descriptor(directory);
            Assert.AreEqual("Testing artist", descriptor.GetDescription(ExifDirectoryBase.TagWinAuthor));
            Assert.AreEqual("Testing comments", descriptor.GetDescription(ExifDirectoryBase.TagWinComment));
            Assert.AreEqual("Testing keywords", descriptor.GetDescription(ExifDirectoryBase.TagWinKeywords));
            Assert.AreEqual("Testing subject", descriptor.GetDescription(ExifDirectoryBase.TagWinSubject));
            Assert.AreEqual("Testing title", descriptor.GetDescription(ExifDirectoryBase.TagWinTitle));
        }
    }
}
