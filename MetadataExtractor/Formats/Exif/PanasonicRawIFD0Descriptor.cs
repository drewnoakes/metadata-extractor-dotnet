// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="PanasonicRawIfd0Directory"/>.
    /// </summary>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class PanasonicRawIfd0Descriptor(PanasonicRawIfd0Directory directory)
        : TagDescriptor<PanasonicRawIfd0Directory>(directory)
    {
        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                PanasonicRawIfd0Directory.TagPanasonicRawVersion => GetVersionBytesDescription(PanasonicRawIfd0Directory.TagPanasonicRawVersion, 2),
                PanasonicRawIfd0Directory.TagOrientation => GetOrientationDescription(PanasonicRawIfd0Directory.TagOrientation),
                _ => base.GetDescription(tagType),
            };
        }
    }
}
