#region License
//
// Copyright 2002-2015 Drew Noakes
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

using System.Globalization;
using System.Threading;
using MetadataExtractor.Formats.Exif.Makernotes;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Exif
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class NikonType2MakernoteTest1
    {
        private readonly NikonType2MakernoteDirectory _nikonDirectory;
        private readonly NikonType2MakernoteDescriptor _descriptor;

        public NikonType2MakernoteTest1()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");
            _nikonDirectory = ExifReaderTest.ProcessSegmentBytes<NikonType2MakernoteDirectory>("Tests/Data/nikonMakernoteType2a.jpg.app1");
            Assert.NotNull(_nikonDirectory);
            _descriptor = new NikonType2MakernoteDescriptor(_nikonDirectory);
        }

    /*
        [Nikon Makernote] Firmware Version = 0200
        [Nikon Makernote] ISO = 0 320
        [Nikon Makernote] File Format = FINE
        [Nikon Makernote] White Balance = FLASH
        [Nikon Makernote] Sharpening = AUTO
        [Nikon Makernote] AF Type = AF-C
        [Nikon Makernote] Unknown 17 = NORMAL
        [Nikon Makernote] Unknown 18 =
        [Nikon Makernote] White Balance Fine = 0
        [Nikon Makernote] Unknown 01 =
        [Nikon Makernote] Unknown 02 =
        [Nikon Makernote] Unknown 03 = 914
        [Nikon Makernote] Unknown 19 =
        [Nikon Makernote] ISO = 0 320
        [Nikon Makernote] Tone Compensation = AUTO
        [Nikon Makernote] Unknown 04 = 6
        [Nikon Makernote] Lens Focal/Max-FStop pairs = 240/10 850/10 35/10 45/10
        [Nikon Makernote] Unknown 05 = 0
        [Nikon Makernote] Unknown 06 = 
        [Nikon Makernote] Unknown 07 = 1
        [Nikon Makernote] Unknown 20 = 0
        [Nikon Makernote] Unknown 08 = @
        [Nikon Makernote] Colour Mode = MODE1
        [Nikon Makernote] Unknown 10 = NATURAL
        [Nikon Makernote] Unknown 11 = 0100


        -
        [Nikon Makernote] Camera Hue = 0
        [Nikon Makernote] Noise Reduction = OFF
        [Nikon Makernote] Unknown 12 = 0100

        [Nikon Makernote] Unknown 13 = 0100{t@7b,4x,D"Y
        [Nikon Makernote] Unknown 15 = 78/10 78/10
    */

        [Fact]
        public void TestNikonMakernote_MatchesKnownValues()
        {
            Assert.Equal("48 50 48 48", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagFirmwareVersion));
            Assert.Equal("0 320", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagIso1));
            Assert.Equal("0 320", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagIsoRequested));
            Assert.Equal("FLASH       ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagCameraWhiteBalance));
            Assert.Equal("AUTO  ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagCameraSharpening));
            Assert.Equal("AF-C  ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagAfType));
            Assert.Equal("NORMAL      ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagFlashSyncMode));
            Assert.Equal("0", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagCameraWhiteBalanceFine));
            Assert.Equal("914", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagPreviewIfd));
            Assert.Equal("AUTO    ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagCameraToneCompensation));
            Assert.Equal("6", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagLensType));
            Assert.Equal("240/10 850/10 35/10 45/10", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagLens));
            Assert.Equal("0", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagFlashUsed));
            Assert.Equal("1", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagShootingMode));
            Assert.Equal("0", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagUnknown20));
            Assert.Equal("MODE1   ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagCameraColorMode));
            Assert.Equal("NATURAL    ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagLightSource));
            Assert.Equal("0", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagCameraHueAdjustment));
            Assert.Equal("OFF ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagNoiseReduction));
            Assert.Equal("78/10 78/10", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagSensorPixelSize));
        }

        /// <exception cref="MetadataException"/>
        [Fact]
        public void TestGetLensDescription()
        {
            Assert.Equal("24-85mm f/3.5-4.5", _descriptor.GetDescription(NikonType2MakernoteDirectory.TagLens));
            Assert.Equal("24-85mm f/3.5-4.5", _descriptor.GetLensDescription());
        }

        /// <exception cref="MetadataException"/>
        [Fact]
        public void TestGetHueAdjustmentDescription()
        {
            Assert.Equal("0 degrees", _descriptor.GetDescription(NikonType2MakernoteDirectory.TagCameraHueAdjustment));
            Assert.Equal("0 degrees", _descriptor.GetHueAdjustmentDescription());
        }


        [Fact]
        public void TestGetColorModeDescription()
        {
            Assert.Equal("Mode I (sRGB)", _descriptor.GetDescription(NikonType2MakernoteDirectory.TagCameraColorMode));
            Assert.Equal("Mode I (sRGB)", _descriptor.GetColorModeDescription());
        }


        [Fact]
        public void TestGetAutoFlashCompensationDescription()
        {
            var directory = new NikonType2MakernoteDirectory();
            var descriptor = new NikonType2MakernoteDescriptor(directory);
            // no entry exists
            Assert.Null(descriptor.GetAutoFlashCompensationDescription());
            directory.Set(NikonType2MakernoteDirectory.TagAutoFlashCompensation, new sbyte[] { 0x06, 0x01, 0x06 });
            Assert.Equal("1 EV", descriptor.GetAutoFlashCompensationDescription());
            directory.Set(NikonType2MakernoteDirectory.TagAutoFlashCompensation, new sbyte[] { 0x04, 0x01, 0x06 });
            Assert.Equal("0.67 EV", descriptor.GetAutoFlashCompensationDescription());
            directory.Set(NikonType2MakernoteDirectory.TagAutoFlashCompensation, new sbyte[] { 0x02, 0x01, 0x06 });
            Assert.Equal("0.33 EV", descriptor.GetAutoFlashCompensationDescription());
            directory.Set(NikonType2MakernoteDirectory.TagAutoFlashCompensation, new sbyte[] { unchecked((sbyte)0xFE), 0x01, 0x06 });
            Assert.Equal("-0.33 EV", descriptor.GetAutoFlashCompensationDescription());
        }
    }
}
