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

using MetadataExtractor.Formats.Exif;
using NUnit.Framework;

namespace MetadataExtractor.Tests.Formats.Exif
{
    /// <summary>
    /// Unit tests for <see cref="ExifInteropDescriptor"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class ExifInteropDescriptorTest
    {

        [Test]
        public void TestGetInteropVersionDescription()
        {
            var directory = new ExifInteropDirectory();
            directory.Set(ExifDirectoryBase.TagInteropVersion, new[] { 0, 1, 0, 0 });
            var descriptor = new ExifInteropDescriptor(directory);
            Assert.AreEqual("1.00", descriptor.GetDescription(ExifDirectoryBase.TagInteropVersion));
            Assert.AreEqual("1.00", descriptor.GetInteropVersionDescription());
        }


        [Test]
        public void TestGetInteropIndexDescription()
        {
            var directory = new ExifInteropDirectory();
            directory.Set(ExifDirectoryBase.TagInteropIndex, "R98");
            var descriptor = new ExifInteropDescriptor(directory);
            Assert.AreEqual("Recommended Exif Interoperability Rules (ExifR98)", descriptor.GetDescription(ExifDirectoryBase.TagInteropIndex));
            Assert.AreEqual("Recommended Exif Interoperability Rules (ExifR98)", descriptor.GetInteropIndexDescription());
        }
    }
}
