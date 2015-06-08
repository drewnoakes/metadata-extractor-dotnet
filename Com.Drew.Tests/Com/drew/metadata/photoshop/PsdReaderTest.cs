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

using System;
using Com.Drew.Lang;
using JetBrains.Annotations;
using NUnit.Framework;
using Sharpen;

namespace Com.Drew.Metadata.Photoshop
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PsdReaderTest
    {
        /// <exception cref="System.Exception"/>
        [NotNull]
        public static PsdHeaderDirectory ProcessBytes([NotNull] string file)
        {
            Metadata metadata = new Metadata();
            InputStream stream = new FileInputStream(new FilePath(file));
            try
            {
                new PsdReader().Extract(new StreamReader(stream), metadata);
            }
            catch (Exception e)
            {
                stream.Close();
                throw;
            }
            PsdHeaderDirectory directory = metadata.GetFirstDirectoryOfType<PsdHeaderDirectory>();
            Assert.IsNotNull(directory);
            return directory;
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void Test8X8X8BitGrayscale()
        {
            PsdHeaderDirectory directory = ProcessBytes("Tests/Data/8x4x8bit-Grayscale.psd");
            Assert.AreEqual(8, directory.GetInt(PsdHeaderDirectory.TagImageWidth));
            Assert.AreEqual(4, directory.GetInt(PsdHeaderDirectory.TagImageHeight));
            Assert.AreEqual(8, directory.GetInt(PsdHeaderDirectory.TagBitsPerChannel));
            Assert.AreEqual(1, directory.GetInt(PsdHeaderDirectory.TagChannelCount));
            Assert.AreEqual(1, directory.GetInt(PsdHeaderDirectory.TagColorMode));
        }

        // 1 = grayscale
        /// <exception cref="System.Exception"/>
        [Test]
        public void Test10X12X16BitCmyk()
        {
            PsdHeaderDirectory directory = ProcessBytes("Tests/Data/10x12x16bit-CMYK.psd");
            Assert.AreEqual(10, directory.GetInt(PsdHeaderDirectory.TagImageWidth));
            Assert.AreEqual(12, directory.GetInt(PsdHeaderDirectory.TagImageHeight));
            Assert.AreEqual(16, directory.GetInt(PsdHeaderDirectory.TagBitsPerChannel));
            Assert.AreEqual(4, directory.GetInt(PsdHeaderDirectory.TagChannelCount));
            Assert.AreEqual(4, directory.GetInt(PsdHeaderDirectory.TagColorMode));
        }
        // 4 = CMYK
    }
}
