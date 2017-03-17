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
using System.Globalization;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="ReconyxHyperFireMakernoteDirectory"/>.
    /// </summary>
    /// <author>Todd West http://cascadescarnivoreproject.blogspot.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class ReconyxUltraFireMakernoteDescriptor : TagDescriptor<ReconyxUltraFireMakernoteDirectory>
    {
        public ReconyxUltraFireMakernoteDescriptor([NotNull] ReconyxUltraFireMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case ReconyxUltraFireMakernoteDirectory.TagLabel:
                    return Directory.GetString(tagType);
                case ReconyxUltraFireMakernoteDirectory.TagMakernoteID:
                    return "0x" + Directory.GetUInt32(tagType).ToString("x8");
                case ReconyxUltraFireMakernoteDirectory.TagMakernoteSize:
                    return Directory.GetUInt32(tagType).ToString();
                case ReconyxUltraFireMakernoteDirectory.TagMakernotePublicID:
                    return "0x" + Directory.GetUInt32(tagType).ToString("x8");
                case ReconyxUltraFireMakernoteDirectory.TagMakernotePublicSize:
                    return Directory.GetUInt16(tagType).ToString();
                case ReconyxUltraFireMakernoteDirectory.TagAmbientTemperatureFahrenheit:
                case ReconyxUltraFireMakernoteDirectory.TagAmbientTemperature:
                    return Directory.GetInt16(tagType).ToString();
                case ReconyxUltraFireMakernoteDirectory.TagCameraVersion:
                case ReconyxUltraFireMakernoteDirectory.TagUibVersion:
                case ReconyxUltraFireMakernoteDirectory.TagBtlVersion:
                case ReconyxUltraFireMakernoteDirectory.TagPexVersion:
                case ReconyxUltraFireMakernoteDirectory.TagEventType:
                    return Directory.GetString(tagType);
                case ReconyxUltraFireMakernoteDirectory.TagSequence:
                    var sequence = Directory.GetInt32Array(tagType);
                    return $"{sequence[0]}/{sequence[1]}";
                case ReconyxUltraFireMakernoteDirectory.TagEventNumber:
                    return Directory.GetUInt32(tagType).ToString();
                case ReconyxUltraFireMakernoteDirectory.TagDateTimeOriginal:
                    return Directory.GetDateTime(tagType).ToString("yyyy:MM:dd HH:mm:ss");
                case ReconyxUltraFireMakernoteDirectory.TagDayOfWeek:
                    return GetIndexedDescription(tagType, CultureInfo.CurrentCulture.DateTimeFormat.DayNames);
                case ReconyxUltraFireMakernoteDirectory.TagMoonPhase:
                    return GetIndexedDescription(tagType, "New", "Waxing Crescent", "First Quarter", "Waxing Gibbous", "Full", "Waning Gibbous", "Last Quarter", "Waning Crescent");
                case ReconyxUltraFireMakernoteDirectory.TagFlash:
                    return GetIndexedDescription(tagType, "Off", "On");
                case ReconyxUltraFireMakernoteDirectory.TagSerialNumber:
                    return Directory.GetString(tagType);
                case ReconyxUltraFireMakernoteDirectory.TagBatteryVoltage:
                    return Directory.GetDouble(tagType).ToString("0.000");
                case ReconyxUltraFireMakernoteDirectory.TagUserLabel:
                    return Directory.GetString(tagType);
                default:
                    return base.GetDescription(tagType);
            }
        }
    }
}
