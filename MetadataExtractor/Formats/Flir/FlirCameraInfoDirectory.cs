// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Flir
{
    public sealed class FlirCameraInfoDirectory : Directory
    {
        public const int TagEmissivity = 32;
        public const int TagObjectDistance = 36;
        public const int TagReflectedApparentTemperature = 40;
        public const int TagAtmosphericTemperature = 44;
        public const int TagIRWindowTemperature = 48;
        public const int TagIRWindowTransmission = 52;
        public const int TagRelativeHumidity = 60;
        public const int TagPlanckR1 = 88;
        public const int TagPlanckB = 92;
        public const int TagPlanckF = 96;
        public const int TagAtmosphericTransAlpha1 = 112;
        public const int TagAtmosphericTransAlpha2 = 116;
        public const int TagAtmosphericTransBeta1 = 120;
        public const int TagAtmosphericTransBeta2 = 124;
        public const int TagAtmosphericTransX = 128;
        public const int TagCameraTemperatureRangeMax = 144;
        public const int TagCameraTemperatureRangeMin = 148;
        public const int TagCameraTemperatureMaxClip = 152;
        public const int TagCameraTemperatureMinClip = 156;
        public const int TagCameraTemperatureMaxWarn = 160;
        public const int TagCameraTemperatureMinWarn = 164;
        public const int TagCameraTemperatureMaxSaturated = 168;
        public const int TagCameraTemperatureMinSaturated = 172;
        public const int TagCameraModel = 212;
        public const int TagCameraPartNumber = 244;
        public const int TagCameraSerialNumber = 260;
        public const int TagCameraSoftware = 276;
        public const int TagLensModel = 368;
        public const int TagLensPartNumber = 400;
        public const int TagLensSerialNumber = 416;
        public const int TagFieldOfView = 436;
        public const int TagFilterModel = 492;
        public const int TagFilterPartNumber = 508;
        public const int TagFilterSerialNumber = 540;
        public const int TagPlanckO = 776;
        public const int TagPlanckR2 = 780;
        public const int TagRawValueRangeMin = 784;
        public const int TagRawValueRangeMax = 786;
        public const int TagRawValueMedian = 824;
        public const int TagRawValueRange = 828;
        public const int TagDateTimeOriginal = 900;
        public const int TagFocusStepCount = 912;
        public const int TagFocusDistance = 1116;
        public const int TagFrameRate = 1124;

        public override string Name => "FLIR Camera Info";

        private static readonly Dictionary<int, string> _nameByTag = new()
        {
            { TagEmissivity, "Emissivity" },
            { TagObjectDistance, "Object Distance" },
            { TagReflectedApparentTemperature, "Reflected Apparent Temperature" },
            { TagAtmosphericTemperature, "Atmospheric Temperature" },
            { TagIRWindowTemperature, "IR Window Temperature" },
            { TagIRWindowTransmission, "IR Window Transmission" },
            { TagRelativeHumidity, "Relative Humidity" },
            { TagPlanckR1, "Planck R1" },
            { TagPlanckB, "Planck B" },
            { TagPlanckF, "Planck F" },
            { TagAtmosphericTransAlpha1, "Atmospheric Trans Alpha1" },
            { TagAtmosphericTransAlpha2, "Atmospheric Trans Alpha2" },
            { TagAtmosphericTransBeta1, "Atmospheric Trans Beta1" },
            { TagAtmosphericTransBeta2, "Atmospheric Trans Beta2" },
            { TagAtmosphericTransX, "Atmospheric Trans X" },
            { TagCameraTemperatureRangeMax, "Camera Temperature Range Max" },
            { TagCameraTemperatureRangeMin, "Camera Temperature Range Min" },
            { TagCameraTemperatureMaxClip, "Camera Temperature Max Clip" },
            { TagCameraTemperatureMinClip, "Camera Temperature Min Clip" },
            { TagCameraTemperatureMaxWarn, "Camera Temperature Max Warn" },
            { TagCameraTemperatureMinWarn, "Camera Temperature Min Warn" },
            { TagCameraTemperatureMaxSaturated, "Camera Temperature Max Saturated" },
            { TagCameraTemperatureMinSaturated, "Camera Temperature Min Saturated" },
            { TagCameraModel, "Camera Model" },
            { TagCameraPartNumber, "Camera Part Number" },
            { TagCameraSerialNumber, "Camera Serial Number" },
            { TagCameraSoftware, "Camera Software" },
            { TagLensModel, "Lens Model" },
            { TagLensPartNumber, "Lens Part Number" },
            { TagLensSerialNumber, "Lens Serial Number" },
            { TagFieldOfView, "Field Of View" },
            { TagFilterModel, "Filter Model" },
            { TagFilterPartNumber, "Filter Part Number" },
            { TagFilterSerialNumber, "Filter Serial Number" },
            { TagPlanckO, "Planck O" },
            { TagPlanckR2, "Planck R2" },
            { TagRawValueRangeMin, "Raw Value Range Min" },
            { TagRawValueRangeMax, "Raw Value Range Max" },
            { TagRawValueMedian, "Raw Value Median" },
            { TagRawValueRange, "Raw Value Range" },
            { TagDateTimeOriginal, "Date Time Original" },
            { TagFocusStepCount, "Focus Step Count" },
            { TagFocusDistance, "Focus Distance" },
            { TagFrameRate, "Frame Rate" }
        };

        public FlirCameraInfoDirectory() : base(_nameByTag)
        {
            SetDescriptor(new FlirCameraInfoDescriptor(this));
        }
    }
}
