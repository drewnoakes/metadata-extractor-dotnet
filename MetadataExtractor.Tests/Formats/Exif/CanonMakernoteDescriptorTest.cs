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

using MetadataExtractor.Formats.Exif.Makernotes;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Exif
{
    /// <summary>Unit tests for <see cref="CanonMakernoteDescriptor"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class CanonMakernoteDescriptorTest
    {
        [Fact, UseCulture("en-GB")]
        public void GetFlashBiasDescription()
        {
            var directory = new CanonMakernoteDirectory();
            var descriptor = new CanonMakernoteDescriptor(directory);
            // set and check values
            directory.Set(CanonMakernoteDirectory.FocalLength.TagFlashBias, 0xFFC0);
            Assert.Equal("-2.0 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.Set(CanonMakernoteDirectory.FocalLength.TagFlashBias, 0xffd4);
            Assert.Equal("-1.375 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.Set(CanonMakernoteDirectory.FocalLength.TagFlashBias, 0x0000);
            Assert.Equal("0.0 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.Set(CanonMakernoteDirectory.FocalLength.TagFlashBias, 0x000c);
            Assert.Equal("0.375 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.Set(CanonMakernoteDirectory.FocalLength.TagFlashBias, 0x0010);
            Assert.Equal("0.5 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.Set(CanonMakernoteDirectory.FocalLength.TagFlashBias, 0x0014);
            Assert.Equal("0.625 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.Set(CanonMakernoteDirectory.FocalLength.TagFlashBias, 0x0020);
            Assert.Equal("1.0 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.Set(CanonMakernoteDirectory.FocalLength.TagFlashBias, 0x0030);
            Assert.Equal("1.5 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.Set(CanonMakernoteDirectory.FocalLength.TagFlashBias, 0x0034);
            Assert.Equal("1.625 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
            directory.Set(CanonMakernoteDirectory.FocalLength.TagFlashBias, 0x0040);
            Assert.Equal("2.0 EV", descriptor.GetDescription(CanonMakernoteDirectory.FocalLength.TagFlashBias));
        }
    }
}
