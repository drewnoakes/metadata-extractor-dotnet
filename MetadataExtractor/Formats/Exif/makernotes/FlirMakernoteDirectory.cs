// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    public class FlirMakernoteDirectory : Directory
    {
        public const int TagImageTemperatureMax = 0x0001;
        public const int TagImageTemperatureMin = 0x0002;
        public const int TagEmissivity = 0x0003;
        public const int TagUnknownTemperature = 0x0004;
        public const int TagCameraTemperatureRangeMin = 0x0005;
        public const int TagCameraTemperatureRangeMax = 0x0006;

        private static readonly Dictionary<int, string> _tagNameMap = new()
        {
            { TagImageTemperatureMax, "Image Temperature Max" },
            { TagImageTemperatureMin, "Image Temperature Min" },
            { TagEmissivity, "Emissivity" },
            { TagUnknownTemperature, "Unknown Temperature" },
            { TagCameraTemperatureRangeMin, "Camera Temperature Range Max" },
            { TagCameraTemperatureRangeMax, "Camera Temperature Range Min" }
        };

        public FlirMakernoteDirectory() : base(_tagNameMap)
        {
            SetDescriptor(new FlirMakernoteDescriptor(this));
        }

        public override string Name => "FLIR Makernote";
    }
}
