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

using System.IO;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Gif;
using MetadataExtractor.IO;
using NUnit.Framework;

namespace MetadataExtractor.Tests.Formats.Gif
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class GifReaderTest
    {
        [NotNull]
        public static GifHeaderDirectory ProcessBytes([NotNull] string file)
        {
            using (var stream = new FileStream(file, FileMode.Open))
                return new GifReader().Extract(new SequentialStreamReader(stream));
        }

        [Test]
        public void TestMsPaintGif()
        {
            var directory = ProcessBytes("Tests/Data/mspaint-10x10.gif");
            Assert.IsFalse(directory.HasErrors);
            Assert.AreEqual("89a", directory.GetString(GifHeaderDirectory.TagGifFormatVersion));
            Assert.AreEqual(10, directory.GetInt32(GifHeaderDirectory.TagImageWidth));
            Assert.AreEqual(10, directory.GetInt32(GifHeaderDirectory.TagImageHeight));
            Assert.AreEqual(256, directory.GetInt32(GifHeaderDirectory.TagColorTableSize));
            Assert.IsFalse(directory.GetBoolean(GifHeaderDirectory.TagIsColorTableSorted));
            Assert.AreEqual(8, directory.GetInt32(GifHeaderDirectory.TagBitsPerPixel));
            Assert.IsTrue(directory.GetBoolean(GifHeaderDirectory.TagHasGlobalColorTable));
            Assert.AreEqual(0, directory.GetInt32(GifHeaderDirectory.TagTransparentColorIndex));
        }

        [Test]
        public void TestPhotoshopGif()
        {
            var directory = ProcessBytes("Tests/Data/photoshop-8x12-32colors-alpha.gif");
            Assert.IsFalse(directory.HasErrors);
            Assert.AreEqual("89a", directory.GetString(GifHeaderDirectory.TagGifFormatVersion));
            Assert.AreEqual(8, directory.GetInt32(GifHeaderDirectory.TagImageWidth));
            Assert.AreEqual(12, directory.GetInt32(GifHeaderDirectory.TagImageHeight));
            Assert.AreEqual(32, directory.GetInt32(GifHeaderDirectory.TagColorTableSize));
            Assert.IsFalse(directory.GetBoolean(GifHeaderDirectory.TagIsColorTableSorted));
            Assert.AreEqual(5, directory.GetInt32(GifHeaderDirectory.TagBitsPerPixel));
            Assert.IsTrue(directory.GetBoolean(GifHeaderDirectory.TagHasGlobalColorTable));
            Assert.AreEqual(8, directory.GetInt32(GifHeaderDirectory.TagTransparentColorIndex));
        }
    }
}
