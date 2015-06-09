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
using NUnit.Framework;
using Sharpen;

namespace Com.Drew.Metadata.Exif
{
    /// <summary>JUnit test case for class ExifSubIFDDescriptor.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class ExifSubIfdDescriptorTest
    {
        /// <exception cref="System.Exception"/>
        [Test]
        public void TestUserCommentDescription_EmptyEncoding()
        {
            byte[] commentBytes = Encoding.UTF8.GetBytes("\x0\x0\x0\x0\x0\x0\x0\x0This is a comment");
            ExifSubIfdDirectory directory = new ExifSubIfdDirectory();
            directory.SetByteArray(ExifDirectoryBase.TagUserComment, commentBytes);
            ExifSubIfdDescriptor descriptor = new ExifSubIfdDescriptor(directory);
            Assert.AreEqual("This is a comment", descriptor.GetDescription(ExifDirectoryBase.TagUserComment));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestUserCommentDescription_AsciiHeaderAsciiEncoding()
        {
            byte[] commentBytes = Encoding.UTF8.GetBytes("ASCII\x0\x0This is a comment");
            ExifSubIfdDirectory directory = new ExifSubIfdDirectory();
            directory.SetByteArray(ExifDirectoryBase.TagUserComment, commentBytes);
            ExifSubIfdDescriptor descriptor = new ExifSubIfdDescriptor(directory);
            Assert.AreEqual("This is a comment", descriptor.GetDescription(ExifDirectoryBase.TagUserComment));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestUserCommentDescription_BlankAscii()
        {
            byte[] commentBytes = Encoding.UTF8.GetBytes("ASCII\x0\x0\x0          ");
            ExifSubIfdDirectory directory = new ExifSubIfdDirectory();
            directory.SetByteArray(ExifDirectoryBase.TagUserComment, commentBytes);
            ExifSubIfdDescriptor descriptor = new ExifSubIfdDescriptor(directory);
            Assert.AreEqual(string.Empty, descriptor.GetDescription(ExifDirectoryBase.TagUserComment));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestUserCommentDescription_ZeroLengthAscii1()
        {
            // the 10-byte encoding region is only partially full
            byte[] commentBytes = Encoding.UTF8.GetBytes("ASCII\x0\x0\x0");
            ExifSubIfdDirectory directory = new ExifSubIfdDirectory();
            directory.SetByteArray(ExifDirectoryBase.TagUserComment, commentBytes);
            ExifSubIfdDescriptor descriptor = new ExifSubIfdDescriptor(directory);
            Assert.AreEqual("ASCII", descriptor.GetDescription(ExifDirectoryBase.TagUserComment));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestUserCommentDescription_ZeroLengthAscii2()
        {
            // fill the 10-byte encoding region
            byte[] commentBytes = Encoding.UTF8.GetBytes("ASCII\x0\x0\x0\x0\x0");
            ExifSubIfdDirectory directory = new ExifSubIfdDirectory();
            directory.SetByteArray(ExifDirectoryBase.TagUserComment, commentBytes);
            ExifSubIfdDescriptor descriptor = new ExifSubIfdDescriptor(directory);
            Assert.AreEqual(string.Empty, descriptor.GetDescription(ExifDirectoryBase.TagUserComment));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestUnicodeComment_ActualBytes()
        {
            byte[] commentBytes = new byte[] { 85, 78, 73, 67, 79, 68, 69, 0, 84, 0, 104, 0, 105, 0, 115, 0, 32, 0, 109, 0, 97, 0, 114, 0, 109, 0, 111, 0, 116, 0, 32, 0, 105, 0, 115, 0, 32, 0, 103, 0, 101, 0, 116, 0, 116, 0, 105, 0, 110, 0, 103, 0, 32
                , 0, 99, 0, 108, 0, 111, 0, 115, 0, 101, 0, 46, 0, 46, 0, 46, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0,
                32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32
                , 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32,
                0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0 };
            ExifSubIfdDirectory directory = new ExifSubIfdDirectory();
            directory.SetByteArray(ExifDirectoryBase.TagUserComment, commentBytes);
            ExifSubIfdDescriptor descriptor = new ExifSubIfdDescriptor(directory);
            Assert.AreEqual("This marmot is getting close...", descriptor.GetDescription(ExifDirectoryBase.TagUserComment));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestUnicodeComment_Ascii()
        {
            byte[] commentBytes = new byte[] { 65, 83, 67, 73, 73, 0, 0, 0, 73, 32, 97, 109, 32, 97, 32, 99, 111, 109, 109, 101, 110, 116, 46, 32, 89, 101, 121, 46, 0 };
            ExifSubIfdDirectory directory = new ExifSubIfdDirectory();
            directory.SetByteArray(ExifDirectoryBase.TagUserComment, commentBytes);
            ExifSubIfdDescriptor descriptor = new ExifSubIfdDescriptor(directory);
            Assert.AreEqual("I am a comment. Yey.", descriptor.GetDescription(ExifDirectoryBase.TagUserComment));
        }
    }
}
