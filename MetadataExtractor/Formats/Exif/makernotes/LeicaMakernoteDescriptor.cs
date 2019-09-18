// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

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
        public LeicaMakernoteDescriptor(LeicaMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
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

        private string? GetCameraTemperatureDescription()
        {
            return GetFormattedInt(LeicaMakernoteDirectory.TagCameraTemperature, "{0} C");
        }

        private string? GetApproximateFNumberDescription()
        {
            return GetSimpleRational(LeicaMakernoteDirectory.TagApproximateFNumber);
        }

        private string? GetMeasuredLVDescription()
        {
            return GetSimpleRational(LeicaMakernoteDirectory.TagMeasuredLV);
        }

        private string? GetExternalSensorBrightnessValueDescription()
        {
            return GetSimpleRational(LeicaMakernoteDirectory.TagExternalSensorBrightnessValue);
        }

        private string? GetWhiteBalanceDescription()
        {
            return GetIndexedDescription(LeicaMakernoteDirectory.TagWhiteBalance,
                "Auto or Manual", "Daylight", "Fluorescent", "Tungsten", "Flash", "Cloudy", "Shadow");
        }

        private string? GetUserProfileDescription()
        {
            return GetIndexedDescription(LeicaMakernoteDirectory.TagQuality,
                1,
                "User Profile 1", "User Profile 2", "User Profile 3", "User Profile 0 (Dynamic)");
        }

        private string? GetQualityDescription()
        {
            return GetIndexedDescription(LeicaMakernoteDirectory.TagQuality,
                1,
                "Fine", "Basic");
        }
    }
}
