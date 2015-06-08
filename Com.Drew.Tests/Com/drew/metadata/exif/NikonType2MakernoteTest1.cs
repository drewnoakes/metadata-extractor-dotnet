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
using System.Globalization;
using Com.Drew.Metadata.Exif.Makernotes;
using Sharpen;

namespace Com.Drew.Metadata.Exif
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class NikonType2MakernoteTest1
    {
        private NikonType2MakernoteDirectory _nikonDirectory;

        private NikonType2MakernoteDescriptor _descriptor;

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.SetUp]
        public virtual void SetUp()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = Sharpen.Extensions.CreateLocale("en", "GB");
            _nikonDirectory = ExifReaderTest.ProcessBytes<NikonType2MakernoteDirectory>("Tests/Data/nikonMakernoteType2a.jpg.app1");
            NUnit.Framework.Assert.IsNotNull(_nikonDirectory);
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
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestNikonMakernote_MatchesKnownValues()
        {
            Sharpen.Tests.AreEqual("48 50 48 48", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagFirmwareVersion));
            Sharpen.Tests.AreEqual("0 320", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagIso1));
            Sharpen.Tests.AreEqual("0 320", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagIsoRequested));
            Sharpen.Tests.AreEqual("FLASH       ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagCameraWhiteBalance));
            Sharpen.Tests.AreEqual("AUTO  ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagCameraSharpening));
            Sharpen.Tests.AreEqual("AF-C  ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagAfType));
            Sharpen.Tests.AreEqual("NORMAL      ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagFlashSyncMode));
            Sharpen.Tests.AreEqual("0", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagCameraWhiteBalanceFine));
            Sharpen.Tests.AreEqual("914", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagPreviewIfd));
            Sharpen.Tests.AreEqual("AUTO    ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagCameraToneCompensation));
            Sharpen.Tests.AreEqual("6", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagLensType));
            Sharpen.Tests.AreEqual("240/10 850/10 35/10 45/10", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagLens));
            Sharpen.Tests.AreEqual("0", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagFlashUsed));
            Sharpen.Tests.AreEqual("1", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagShootingMode));
            Sharpen.Tests.AreEqual("0", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagUnknown20));
            Sharpen.Tests.AreEqual("MODE1   ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagCameraColorMode));
            Sharpen.Tests.AreEqual("NATURAL    ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagLightSource));
            Sharpen.Tests.AreEqual("0", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagCameraHueAdjustment));
            Sharpen.Tests.AreEqual("OFF ", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagNoiseReduction));
            Sharpen.Tests.AreEqual("78/10 78/10", _nikonDirectory.GetString(NikonType2MakernoteDirectory.TagSensorPixelSize));
        }

        /// <exception cref="Com.Drew.Metadata.MetadataException"/>
        [NUnit.Framework.Test]
        public virtual void TestGetLensDescription()
        {
            Sharpen.Tests.AreEqual("24-85mm f/3.5-4.5", _descriptor.GetDescription(NikonType2MakernoteDirectory.TagLens));
            Sharpen.Tests.AreEqual("24-85mm f/3.5-4.5", _descriptor.GetLensDescription());
        }

        /// <exception cref="Com.Drew.Metadata.MetadataException"/>
        [NUnit.Framework.Test]
        public virtual void TestGetHueAdjustmentDescription()
        {
            Sharpen.Tests.AreEqual("0 degrees", _descriptor.GetDescription(NikonType2MakernoteDirectory.TagCameraHueAdjustment));
            Sharpen.Tests.AreEqual("0 degrees", _descriptor.GetHueAdjustmentDescription());
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestGetColorModeDescription()
        {
            Sharpen.Tests.AreEqual("Mode I (sRGB)", _descriptor.GetDescription(NikonType2MakernoteDirectory.TagCameraColorMode));
            Sharpen.Tests.AreEqual("Mode I (sRGB)", _descriptor.GetColorModeDescription());
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestGetAutoFlashCompensationDescription()
        {
            NikonType2MakernoteDirectory directory = new NikonType2MakernoteDirectory();
            NikonType2MakernoteDescriptor descriptor = new NikonType2MakernoteDescriptor(directory);
            // no entry exists
            NUnit.Framework.Assert.IsNull(descriptor.GetAutoFlashCompensationDescription());
            directory.SetByteArray(NikonType2MakernoteDirectory.TagAutoFlashCompensation, new sbyte[] { unchecked((int)(0x06)), unchecked((int)(0x01)), unchecked((int)(0x06)) });
            Sharpen.Tests.AreEqual("1 EV", descriptor.GetAutoFlashCompensationDescription());
            directory.SetByteArray(NikonType2MakernoteDirectory.TagAutoFlashCompensation, new sbyte[] { unchecked((int)(0x04)), unchecked((int)(0x01)), unchecked((int)(0x06)) });
            Sharpen.Tests.AreEqual("0.67 EV", descriptor.GetAutoFlashCompensationDescription());
            directory.SetByteArray(NikonType2MakernoteDirectory.TagAutoFlashCompensation, new sbyte[] { unchecked((int)(0x02)), unchecked((int)(0x01)), unchecked((int)(0x06)) });
            Sharpen.Tests.AreEqual("0.33 EV", descriptor.GetAutoFlashCompensationDescription());
            directory.SetByteArray(NikonType2MakernoteDirectory.TagAutoFlashCompensation, new sbyte[] { unchecked((sbyte)0xFE), unchecked((int)(0x01)), unchecked((int)(0x06)) });
            Sharpen.Tests.AreEqual("-0.33 EV", descriptor.GetAutoFlashCompensationDescription());
        }
    }
}
