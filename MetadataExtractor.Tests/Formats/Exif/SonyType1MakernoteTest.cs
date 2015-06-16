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

using MetadataExtractor.Formats.Exif.Makernotes;
using NUnit.Framework;

namespace MetadataExtractor.Tests.Formats.Exif
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class SonyType1MakernoteTest
    {

        [Test]
        public void TestSonyType1Makernote()
        {
            var directory = ExifReaderTest.ProcessSegmentBytes<SonyType1MakernoteDirectory>("Tests/Data/sonyType1.jpg.app1");
            Assert.IsNotNull(directory);
            Assert.IsFalse(directory.HasErrors);
            var descriptor = new SonyType1MakernoteDescriptor(directory);
            Assert.IsNull(directory.GetObject(SonyType1MakernoteDirectory.TagColorTemperature));
            Assert.IsNull(descriptor.GetColorTemperatureDescription());
            Assert.IsNull(directory.GetObject(SonyType1MakernoteDirectory.TagSceneMode));
            Assert.IsNull(descriptor.GetSceneModeDescription());
            Assert.IsNull(directory.GetObject(SonyType1MakernoteDirectory.TagZoneMatching));
            Assert.IsNull(descriptor.GetZoneMatchingDescription());
            Assert.IsNull(directory.GetObject(SonyType1MakernoteDirectory.TagDynamicRangeOptimiser));
            Assert.IsNull(descriptor.GetDynamicRangeOptimizerDescription());
            Assert.IsNull(directory.GetObject(SonyType1MakernoteDirectory.TagImageStabilisation));
            Assert.IsNull(descriptor.GetImageStabilizationDescription());
            Assert.IsNull(directory.GetObject(SonyType1MakernoteDirectory.TagColorMode));
            Assert.IsNull(descriptor.GetColorModeDescription());
            Assert.AreEqual("On (Shooting)", descriptor.GetAntiBlurDescription());
            Assert.AreEqual("Program", descriptor.GetExposureModeDescription());
            Assert.AreEqual("Off", descriptor.GetLongExposureNoiseReductionDescription());
            Assert.AreEqual("Off", descriptor.GetMacroDescription());
            Assert.AreEqual("Normal", descriptor.GetJpegQualityDescription());
        }
    }
}
