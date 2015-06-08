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

namespace Com.Drew.Metadata.Bmp
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class BmpReaderTest
    {
        /// <exception cref="System.Exception"/>
        [NotNull]
        public static BmpHeaderDirectory ProcessBytes([NotNull] string file)
        {
            Com.Drew.Metadata.Metadata metadata = new Com.Drew.Metadata.Metadata();
            InputStream stream = new FileInputStream(file);
            new BmpReader().Extract(new Com.Drew.Lang.StreamReader(stream), metadata);
            stream.Close();
            BmpHeaderDirectory directory = metadata.GetFirstDirectoryOfType<BmpHeaderDirectory>();
            NUnit.Framework.Assert.IsNotNull(directory);
            return directory;
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestMsPaint16color()
        {
            BmpHeaderDirectory directory = ProcessBytes("Tests/Data/16color-10x10.bmp");
            Sharpen.Tests.IsFalse(directory.HasErrors());
            Sharpen.Tests.AreEqual(10, directory.GetInt(BmpHeaderDirectory.TagImageWidth));
            Sharpen.Tests.AreEqual(10, directory.GetInt(BmpHeaderDirectory.TagImageHeight));
            Sharpen.Tests.AreEqual(4, directory.GetInt(BmpHeaderDirectory.TagBitsPerPixel));
            Sharpen.Tests.AreEqual("None", directory.GetDescription(BmpHeaderDirectory.TagCompression));
            Sharpen.Tests.AreEqual(0, directory.GetInt(BmpHeaderDirectory.TagXPixelsPerMeter));
            Sharpen.Tests.AreEqual(0, directory.GetInt(BmpHeaderDirectory.TagYPixelsPerMeter));
            Sharpen.Tests.AreEqual(0, directory.GetInt(BmpHeaderDirectory.TagPaletteColourCount));
            Sharpen.Tests.AreEqual(0, directory.GetInt(BmpHeaderDirectory.TagImportantColourCount));
            Sharpen.Tests.AreEqual(1, directory.GetInt(BmpHeaderDirectory.TagColourPlanes));
            Sharpen.Tests.AreEqual(40, directory.GetInt(BmpHeaderDirectory.TagHeaderSize));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestMsPaint24bpp()
        {
            BmpHeaderDirectory directory = ProcessBytes("Tests/Data/24bpp-10x10.bmp");
            Sharpen.Tests.IsFalse(directory.HasErrors());
            Sharpen.Tests.AreEqual(10, directory.GetInt(BmpHeaderDirectory.TagImageWidth));
            Sharpen.Tests.AreEqual(10, directory.GetInt(BmpHeaderDirectory.TagImageHeight));
            Sharpen.Tests.AreEqual(24, directory.GetInt(BmpHeaderDirectory.TagBitsPerPixel));
            Sharpen.Tests.AreEqual("None", directory.GetDescription(BmpHeaderDirectory.TagCompression));
            Sharpen.Tests.AreEqual(0, directory.GetInt(BmpHeaderDirectory.TagXPixelsPerMeter));
            Sharpen.Tests.AreEqual(0, directory.GetInt(BmpHeaderDirectory.TagYPixelsPerMeter));
            Sharpen.Tests.AreEqual(0, directory.GetInt(BmpHeaderDirectory.TagPaletteColourCount));
            Sharpen.Tests.AreEqual(0, directory.GetInt(BmpHeaderDirectory.TagImportantColourCount));
            Sharpen.Tests.AreEqual(1, directory.GetInt(BmpHeaderDirectory.TagColourPlanes));
            Sharpen.Tests.AreEqual(40, directory.GetInt(BmpHeaderDirectory.TagHeaderSize));
        }
    }
}
