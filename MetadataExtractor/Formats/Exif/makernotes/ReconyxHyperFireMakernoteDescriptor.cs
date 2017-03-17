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

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="ReconyxHyperFireMakernoteDirectory"/>.
    /// </summary>
    /// <author>Todd West http://cascadescarnivoreproject.blogspot.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class ReconyxHyperFireMakernoteDescriptor : TagDescriptor<ReconyxHyperFireMakernoteDirectory>
    {
        public ReconyxHyperFireMakernoteDescriptor([NotNull] ReconyxHyperFireMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case ReconyxHyperFireMakernoteDirectory.TagMakernoteVersion:
                    return Directory.GetUInt16(tagType).ToString();
                case ReconyxHyperFireMakernoteDirectory.TagFirmwareVersion:
                    // invokes Version.ToString()
                    return Directory.GetString(tagType);
                case ReconyxHyperFireMakernoteDirectory.TagTriggerMode:
                    return Directory.GetString(tagType);
                case ReconyxHyperFireMakernoteDirectory.TagSequence:
                    var sequence = Directory.GetInt32Array(tagType);
                    return $"{sequence[0]}/{sequence[1]}";
                case ReconyxHyperFireMakernoteDirectory.TagEventNumber:
                    return Directory.GetUInt32(tagType).ToString();
                case ReconyxHyperFireMakernoteDirectory.TagMotionSensitivity:
                    return Directory.GetUInt16(tagType).ToString();
                case ReconyxHyperFireMakernoteDirectory.TagBatteryVoltage:
                    return Directory.GetDouble(tagType).ToString("0.000");
                case ReconyxHyperFireMakernoteDirectory.TagDateTimeOriginal:
                    return Directory.GetDateTime(tagType).ToString("yyyy:MM:dd HH:mm:ss");
                case ReconyxHyperFireMakernoteDirectory.TagMoonPhase:
                    return GetIndexedDescription(tagType, "New", "Waxing Crescent", "First Quarter", "Waxing Gibbous", "Full", "Waning Gibbous", "Last Quarter", "Waning Crescent");
                case ReconyxHyperFireMakernoteDirectory.TagAmbientTemperatureFahrenheit:
                case ReconyxHyperFireMakernoteDirectory.TagAmbientTemperature:
                    return Directory.GetInt16(tagType).ToString();
                case ReconyxHyperFireMakernoteDirectory.TagSerialNumber:
                    return Directory.GetString(tagType);
                case ReconyxHyperFireMakernoteDirectory.TagContrast:
                case ReconyxHyperFireMakernoteDirectory.TagBrightness:
                case ReconyxHyperFireMakernoteDirectory.TagSharpness:
                case ReconyxHyperFireMakernoteDirectory.TagSaturation:
                    return Directory.GetUInt16(tagType).ToString();
                case ReconyxHyperFireMakernoteDirectory.TagInfraredIlluminator:
                    return GetIndexedDescription(tagType, "Off", "On");
                case ReconyxHyperFireMakernoteDirectory.TagUserLabel:
                    return Directory.GetString(tagType);
                default:
                    return base.GetDescription(tagType);
            }
        }
    }
}
