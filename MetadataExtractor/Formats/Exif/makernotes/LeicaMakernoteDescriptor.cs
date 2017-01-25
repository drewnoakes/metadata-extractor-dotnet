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
    /// Provides human-readable string representations of tag values stored in a <see cref="LeicaMakernoteDirectory"/>.
    /// <para />
    /// Tag reference from: http://gvsoft.homedns.org/exif/makernote-leica-type1.html
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class LeicaMakernoteDescriptor : TagDescriptor<LeicaMakernoteDirectory>
    {
        public LeicaMakernoteDescriptor([NotNull] LeicaMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case LeicaMakernoteDirectory.TagQuality:
                    return GetQualityDescription();
                case LeicaMakernoteDirectory.TagUserProfile:
                    return GetUserProfileDescription();
//              case LeicaMakernoteDirectory.TagSerial:
//                  return GetSerialNumberDescription();
                case LeicaMakernoteDirectory.TagWhiteBalance:
                    return GetWhiteBalanceDescription();
                case LeicaMakernoteDirectory.TagExternalSensorBrightnessValue:
                    return GetExternalSensorBrightnessValueDescription();
                case LeicaMakernoteDirectory.TagMeasuredLV:
                    return GetMeasuredLVDescription();
                case LeicaMakernoteDirectory.TagApproximateFNumber:
                    return GetApproximateFNumberDescription();
                case LeicaMakernoteDirectory.TagCameraTemperature:
                    return GetCameraTemperatureDescription();
                case LeicaMakernoteDirectory.TagWBRedLevel:
                case LeicaMakernoteDirectory.TagWBBlueLevel:
                case LeicaMakernoteDirectory.TagWBGreenLevel:
                    return GetSimpleRational(tagType);
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        private string GetCameraTemperatureDescription()
        {
            return GetFormattedInt(LeicaMakernoteDirectory.TagCameraTemperature, "{0} C");
        }

        [CanBeNull]
        private string GetApproximateFNumberDescription()
        {
            return GetSimpleRational(LeicaMakernoteDirectory.TagApproximateFNumber);
        }

        [CanBeNull]
        private string GetMeasuredLVDescription()
        {
            return GetSimpleRational(LeicaMakernoteDirectory.TagMeasuredLV);
        }

        [CanBeNull]
        private string GetExternalSensorBrightnessValueDescription()
        {
            return GetSimpleRational(LeicaMakernoteDirectory.TagExternalSensorBrightnessValue);
        }

        [CanBeNull]
        private string GetWhiteBalanceDescription()
        {
            return GetIndexedDescription(LeicaMakernoteDirectory.TagWhiteBalance,
                "Auto or Manual", "Daylight", "Fluorescent", "Tungsten", "Flash", "Cloudy", "Shadow");
        }

        [CanBeNull]
        private string GetUserProfileDescription()
        {
            return GetIndexedDescription(LeicaMakernoteDirectory.TagQuality,
                1,
                "User Profile 1", "User Profile 2", "User Profile 3", "User Profile 0 (Dynamic)");
        }

        [CanBeNull]
        private string GetQualityDescription()
        {
            return GetIndexedDescription(LeicaMakernoteDirectory.TagQuality,
                1,
                "Fine", "Basic");
        }
    }
}
