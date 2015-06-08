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

namespace Com.Drew.Metadata.Exif
{
    /// <summary>
    /// Unit tests for
    /// <see cref="ExifIfd0Descriptor"/>
    /// .
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class ExifIfd0DescriptorTest
    {
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestXResolutionDescription()
        {
            ExifIfd0Directory directory = new ExifIfd0Directory();
            directory.SetRational(ExifIfd0Directory.TagXResolution, new Rational(72, 1));
            // 2 is for 'Inch'
            directory.SetInt(ExifIfd0Directory.TagResolutionUnit, 2);
            ExifIfd0Descriptor descriptor = new ExifIfd0Descriptor(directory);
            Assert.AreEqual("72 dots per inch", descriptor.GetDescription(ExifIfd0Directory.TagXResolution));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestYResolutionDescription()
        {
            ExifIfd0Directory directory = new ExifIfd0Directory();
            directory.SetRational(ExifIfd0Directory.TagYResolution, new Rational(50, 1));
            // 3 is for 'cm'
            directory.SetInt(ExifIfd0Directory.TagResolutionUnit, 3);
            ExifIfd0Descriptor descriptor = new ExifIfd0Descriptor(directory);
            Assert.AreEqual("50 dots per cm", descriptor.GetDescription(ExifIfd0Directory.TagYResolution));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestWindowsXpFields()
        {
            ExifIfd0Directory directory = ExifReaderTest.ProcessBytes<ExifIfd0Directory>("Tests/Data/windowsXpFields.jpg.app1");
            Assert.AreEqual("Testing artist\x0", directory.GetString(ExifIfd0Directory.TagWinAuthor, "UTF-16LE"));
            Assert.AreEqual("Testing comments\x0", directory.GetString(ExifIfd0Directory.TagWinComment, "UTF-16LE"));
            Assert.AreEqual("Testing keywords\x0", directory.GetString(ExifIfd0Directory.TagWinKeywords, "UTF-16LE"));
            Assert.AreEqual("Testing subject\x0", directory.GetString(ExifIfd0Directory.TagWinSubject, "UTF-16LE"));
            Assert.AreEqual("Testing title\x0", directory.GetString(ExifIfd0Directory.TagWinTitle, "UTF-16LE"));
            ExifIfd0Descriptor descriptor = new ExifIfd0Descriptor(directory);
            Assert.AreEqual("Testing artist", descriptor.GetDescription(ExifIfd0Directory.TagWinAuthor));
            Assert.AreEqual("Testing comments", descriptor.GetDescription(ExifIfd0Directory.TagWinComment));
            Assert.AreEqual("Testing keywords", descriptor.GetDescription(ExifIfd0Directory.TagWinKeywords));
            Assert.AreEqual("Testing subject", descriptor.GetDescription(ExifIfd0Directory.TagWinSubject));
            Assert.AreEqual("Testing title", descriptor.GetDescription(ExifIfd0Directory.TagWinTitle));
        }
    }
}
