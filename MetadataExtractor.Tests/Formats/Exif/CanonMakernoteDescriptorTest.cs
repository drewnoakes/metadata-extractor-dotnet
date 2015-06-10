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

using MetadataExtractor.Formats.Exif.makernotes;
using NUnit.Framework;

namespace Com.Drew.Metadata.Exif
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class CanonMakernoteDescriptorTest
    {

        [Test, SetCulture("en-GB")]
        public void TestGetFlashBiasDescription()
        {
            var directory = new CanonMakernoteDirectory();
            var descriptor = new CanonMakernoteDescriptor(directory);
            // set and check values
            directory.SetInt(CanonMakernoteDirectory.FocalLength.TagFlashBias, unchecked(0xFFC0));
            Assert.AreEqual("-2.0 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.SetInt(CanonMakernoteDirectory.FocalLength.TagFlashBias, unchecked(0xffd4));
            Assert.AreEqual("-1.375 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.SetInt(CanonMakernoteDirectory.FocalLength.TagFlashBias, unchecked(0x0000));
            Assert.AreEqual("0.0 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.SetInt(CanonMakernoteDirectory.FocalLength.TagFlashBias, unchecked(0x000c));
            Assert.AreEqual("0.375 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.SetInt(CanonMakernoteDirectory.FocalLength.TagFlashBias, unchecked(0x0010));
            Assert.AreEqual("0.5 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.SetInt(CanonMakernoteDirectory.FocalLength.TagFlashBias, unchecked(0x0014));
            Assert.AreEqual("0.625 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.SetInt(CanonMakernoteDirectory.FocalLength.TagFlashBias, unchecked(0x0020));
            Assert.AreEqual("1.0 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.SetInt(CanonMakernoteDirectory.FocalLength.TagFlashBias, unchecked(0x0030));
            Assert.AreEqual("1.5 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.SetInt(CanonMakernoteDirectory.FocalLength.TagFlashBias, unchecked(0x0034));
            Assert.AreEqual("1.625 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.SetInt(CanonMakernoteDirectory.FocalLength.TagFlashBias, unchecked(0x0040));
            Assert.AreEqual("2.0 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
        }
    }
}
