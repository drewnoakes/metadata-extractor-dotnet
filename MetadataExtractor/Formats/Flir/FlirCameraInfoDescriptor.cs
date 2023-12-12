// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using static MetadataExtractor.Formats.Flir.FlirCameraInfoDirectory;

namespace MetadataExtractor.Formats.Flir;

public sealed class FlirCameraInfoDescriptor : TagDescriptor<FlirCameraInfoDirectory>
{
    public FlirCameraInfoDescriptor(FlirCameraInfoDirectory directory)
        : base(directory)
    {
    }

    public override string? GetDescription(int tagType)
    {
        return tagType switch
        {
            TagReflectedApparentTemperature => KelvinToCelcius(),
            TagAtmosphericTemperature => KelvinToCelcius(),
            TagIRWindowTemperature => KelvinToCelcius(),
            TagRelativeHumidity => RelativeHumidity(),
            TagCameraTemperatureRangeMax => KelvinToCelcius(),
            TagCameraTemperatureRangeMin => KelvinToCelcius(),
            TagCameraTemperatureMaxClip => KelvinToCelcius(),
            TagCameraTemperatureMinClip => KelvinToCelcius(),
            TagCameraTemperatureMaxWarn => KelvinToCelcius(),
            TagCameraTemperatureMinWarn => KelvinToCelcius(),
            TagCameraTemperatureMaxSaturated => KelvinToCelcius(),
            TagCameraTemperatureMinSaturated => KelvinToCelcius(),
            _ => base.GetDescription(tagType)
        };

        string KelvinToCelcius()
        {
            float f = Directory.GetSingle(tagType) - 273.15f;
            return $"{f:N1} C";
        }

        string RelativeHumidity()
        {
            float f = Directory.GetSingle(tagType);
            float val = (f > 2 ? f / 100 : f);
            return $"{(val * 100):N1} %";
        }
    }
}
