#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System.Text;
using MetadataExtractor.Formats.Exif;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Exif
{
    /// <summary>Unit tests for <see cref="ExifSubIfdDescriptor"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class ExifSubIfdDescriptorTest
    {
        [Fact]
        public void UserCommentDescription_EmptyEncoding()
        {
            var commentBytes = Encoding.UTF8.GetBytes("\x0\x0\x0\x0\x0\x0\x0\x0This is a comment");
            var directory = new ExifSubIfdDirectory();
            directory.Set(ExifDirectoryBase.TagUserComment, commentBytes);
            var descriptor = new ExifSubIfdDescriptor(directory);
            Assert.Equal("This is a comment", descriptor.GetDescription(ExifDirectoryBase.TagUserComment));
        }

        [Fact]
        public void UserCommentDescription_AsciiHeaderAsciiEncoding()
        {
            var commentBytes = Encoding.UTF8.GetBytes("ASCII\x0\x0This is a comment");
            var directory = new ExifSubIfdDirectory();
            directory.Set(ExifDirectoryBase.TagUserComment, commentBytes);
            var descriptor = new ExifSubIfdDescriptor(directory);
            Assert.Equal("This is a comment", descriptor.GetDescription(ExifDirectoryBase.TagUserComment));
        }

        [Fact]
        public void UserCommentDescription_BlankAscii()
        {
            var commentBytes = Encoding.UTF8.GetBytes("ASCII\x0\x0\x0          ");
            var directory = new ExifSubIfdDirectory();
            directory.Set(ExifDirectoryBase.TagUserComment, commentBytes);
            var descriptor = new ExifSubIfdDescriptor(directory);
            Assert.Equal(string.Empty, descriptor.GetDescription(ExifDirectoryBase.TagUserComment));
        }

        [Fact]
        public void UserCommentDescription_ZeroLengthAscii1()
        {
            // the 10-byte encoding region is only partially full
            var commentBytes = Encoding.UTF8.GetBytes("ASCII\x0\x0\x0");
            var directory = new ExifSubIfdDirectory();
            directory.Set(ExifDirectoryBase.TagUserComment, commentBytes);
            var descriptor = new ExifSubIfdDescriptor(directory);
            Assert.Equal("ASCII", descriptor.GetDescription(ExifDirectoryBase.TagUserComment));
        }

        [Fact]
        public void UserCommentDescription_ZeroLengthAscii2()
        {
            // fill the 10-byte encoding region
            var commentBytes = Encoding.UTF8.GetBytes("ASCII\x0\x0\x0\x0\x0");
            var directory = new ExifSubIfdDirectory();
            directory.Set(ExifDirectoryBase.TagUserComment, commentBytes);
            var descriptor = new ExifSubIfdDescriptor(directory);
            Assert.Equal(string.Empty, descriptor.GetDescription(ExifDirectoryBase.TagUserComment));
        }

        [Fact]
        public void UnicodeComment_ActualBytes()
        {
            var commentBytes = new byte[] { 85, 78, 73, 67, 79, 68, 69, 0, 84, 0, 104, 0, 105, 0, 115, 0, 32, 0, 109, 0, 97, 0, 114, 0, 109, 0, 111, 0, 116, 0, 32, 0, 105, 0, 115, 0, 32, 0, 103, 0, 101, 0, 116, 0, 116, 0, 105, 0, 110, 0, 103, 0, 32
                , 0, 99, 0, 108, 0, 111, 0, 115, 0, 101, 0, 46, 0, 46, 0, 46, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0,
                32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32
                , 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32,
                0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0 };
            var directory = new ExifSubIfdDirectory();
            directory.Set(ExifDirectoryBase.TagUserComment, commentBytes);
            var descriptor = new ExifSubIfdDescriptor(directory);
            Assert.Equal("This marmot is getting close...", descriptor.GetDescription(ExifDirectoryBase.TagUserComment));
        }

        [Fact]
        public void UnicodeComment_Ascii()
        {
            var commentBytes = new byte[] { 65, 83, 67, 73, 73, 0, 0, 0, 73, 32, 97, 109, 32, 97, 32, 99, 111, 109, 109, 101, 110, 116, 46, 32, 89, 101, 121, 46, 0 };
            var directory = new ExifSubIfdDirectory();
            directory.Set(ExifDirectoryBase.TagUserComment, commentBytes);
            var descriptor = new ExifSubIfdDescriptor(directory);
            Assert.Equal("I am a comment. Yey.", descriptor.GetDescription(ExifDirectoryBase.TagUserComment));
        }
    }
}
