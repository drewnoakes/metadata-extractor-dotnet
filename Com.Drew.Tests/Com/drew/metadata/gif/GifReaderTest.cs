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
using Sharpen;

namespace Com.Drew.Metadata.Gif
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class GifReaderTest
    {
        /// <exception cref="System.Exception"/>
        [NotNull]
        public static GifHeaderDirectory ProcessBytes([NotNull] string file)
        {
            Com.Drew.Metadata.Metadata metadata = new Com.Drew.Metadata.Metadata();
            InputStream stream = new FileInputStream(file);
            new GifReader().Extract(new Com.Drew.Lang.StreamReader(stream), metadata);
            stream.Close();
            GifHeaderDirectory directory = metadata.GetFirstDirectoryOfType<GifHeaderDirectory>();
            NUnit.Framework.Assert.IsNotNull(directory);
            return directory;
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestMsPaintGif()
        {
            GifHeaderDirectory directory = ProcessBytes("Tests/Data/mspaint-10x10.gif");
            Sharpen.Tests.IsFalse(directory.HasErrors());
            Sharpen.Tests.AreEqual("89a", directory.GetString(GifHeaderDirectory.TagGifFormatVersion));
            Sharpen.Tests.AreEqual(10, directory.GetInt(GifHeaderDirectory.TagImageWidth));
            Sharpen.Tests.AreEqual(10, directory.GetInt(GifHeaderDirectory.TagImageHeight));
            Sharpen.Tests.AreEqual(256, directory.GetInt(GifHeaderDirectory.TagColorTableSize));
            Sharpen.Tests.IsFalse(directory.GetBoolean(GifHeaderDirectory.TagIsColorTableSorted));
            Sharpen.Tests.AreEqual(8, directory.GetInt(GifHeaderDirectory.TagBitsPerPixel));
            Sharpen.Tests.IsTrue(directory.GetBoolean(GifHeaderDirectory.TagHasGlobalColorTable));
            Sharpen.Tests.AreEqual(0, directory.GetInt(GifHeaderDirectory.TagTransparentColorIndex));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestPhotoshopGif()
        {
            GifHeaderDirectory directory = ProcessBytes("Tests/Data/photoshop-8x12-32colors-alpha.gif");
            Sharpen.Tests.IsFalse(directory.HasErrors());
            Sharpen.Tests.AreEqual("89a", directory.GetString(GifHeaderDirectory.TagGifFormatVersion));
            Sharpen.Tests.AreEqual(8, directory.GetInt(GifHeaderDirectory.TagImageWidth));
            Sharpen.Tests.AreEqual(12, directory.GetInt(GifHeaderDirectory.TagImageHeight));
            Sharpen.Tests.AreEqual(32, directory.GetInt(GifHeaderDirectory.TagColorTableSize));
            Sharpen.Tests.IsFalse(directory.GetBoolean(GifHeaderDirectory.TagIsColorTableSorted));
            Sharpen.Tests.AreEqual(5, directory.GetInt(GifHeaderDirectory.TagBitsPerPixel));
            Sharpen.Tests.IsTrue(directory.GetBoolean(GifHeaderDirectory.TagHasGlobalColorTable));
            Sharpen.Tests.AreEqual(8, directory.GetInt(GifHeaderDirectory.TagTransparentColorIndex));
        }
    }
}
