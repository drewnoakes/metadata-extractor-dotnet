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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to Reconyx HyperFire cameras.</summary>
    /// <author>Todd West http://cascadescarnivoreproject.blogspot.com</author>
    /// <remarks>Reconyx uses a fixed makernote block.  Tag values are the byte index of the tag within the makernote.</remarks>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class ReconyxHyperFireMakernoteDirectory : Directory
    {
        /// <summary>
        /// Version number used for identifying makernotes from Reconyx HyperFire cameras.
        /// </summary>
        public static readonly ushort MakernoteVersion = 61697;

        public const int TagMakernoteVersion = 0;
        public const int TagFirmwareVersion = 2;
        public const int TagTriggerMode = 12;
        public const int TagSequence = 14;
        public const int TagEventNumber = 18;
        public const int TagDateTimeOriginal = 22;
        public const int TagMoonPhase = 36;
        public const int TagAmbientTemperatureFahrenheit = 38;
        public const int TagAmbientTemperature = 40;
        public const int TagSerialNumber = 42;
        public const int TagContrast = 72;
        public const int TagBrightness = 74;
        public const int TagSharpness = 76;
        public const int TagSaturation = 78;
        public const int TagInfraredIlluminator = 80;
        public const int TagMotionSensitivity = 82;
        public const int TagBatteryVoltage = 84;
        public const int TagUserLabel = 86;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
             { TagMakernoteVersion, "Makernote Version" },
             { TagFirmwareVersion, "Firmware Version" },
             { TagTriggerMode, "Trigger Mode" },
             { TagSequence, "Sequence" },
             { TagEventNumber, "Event Number" },
             { TagDateTimeOriginal, "Date/Time Original" },
             { TagMoonPhase, "Moon Phase" },
             { TagAmbientTemperatureFahrenheit, "Ambient Temperature Fahrenheit" },
             { TagAmbientTemperature, "Ambient Temperature" },
             { TagSerialNumber, "Serial Number" },
             { TagContrast, "Contrast" },
             { TagBrightness, "Brightness" },
             { TagSharpness, "Sharpness" },
             { TagSaturation, "Saturation" },
             { TagInfraredIlluminator, "Infrared Illuminator" },
             { TagMotionSensitivity, "Motion Sensitivity" },
             { TagBatteryVoltage, "Battery Voltage" },
             { TagUserLabel, "User Label" }
        };

        public ReconyxHyperFireMakernoteDirectory()
        {
            SetDescriptor(new ReconyxHyperFireMakernoteDescriptor(this));
        }

        public override string Name => "Reconyx HyperFire Makernote";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
