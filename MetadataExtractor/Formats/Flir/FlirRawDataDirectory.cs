// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace MetadataExtractor.Formats.Flir
{
    public sealed class FlirRawDataDirectory : Directory
    {
        public const int TagRawThermalImageWidth = 2;
        public const int TagRawThermalImageHeight = 4;
        public const int TagRawThermalImageType = 34;
        public const int TagRawThermalImage = 100;

        public override string Name => "FLIR Raw Data";

        private static readonly Dictionary<int, string> _nameByTag = new()
        {
            { TagRawThermalImageWidth, "Raw Thermal Image Width" },
            { TagRawThermalImageHeight, "Raw Thermal Image Height" },
            { TagRawThermalImageType, "Raw Thermal Image Type" },
            { TagRawThermalImage, "Raw Thermal Image" }
        };

        public FlirRawDataDirectory() : base(_nameByTag)
        {
            SetDescriptor(new TagDescriptor<FlirRawDataDirectory>(this));
        }
    }
}
