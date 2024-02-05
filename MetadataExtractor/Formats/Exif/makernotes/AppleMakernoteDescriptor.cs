// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using static MetadataExtractor.Formats.Exif.Makernotes.AppleMakernoteDirectory;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="AppleMakernoteDirectory"/>.
    /// </summary>
    /// <remarks>Using information from http://owl.phy.queensu.ca/~phil/exiftool/TagNames/Apple.html</remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class AppleMakernoteDescriptor(AppleMakernoteDirectory directory)
        : TagDescriptor<AppleMakernoteDirectory>(directory)
    {
        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                TagAEStable => GetAEStableDescription(),
                TagAFStable => GetAEStableDescription(),
                TagHdrImageType => GetHdrImageTypeDescription(),
                TagAccelerationVector => GetAccelerationVectorDescription(),
                TagImageCaptureType => GetImageCaptureTypeDescription(),
                TagFrontFacingCamera => GetFrontFacingCameraDescription(),
                _ => base.GetDescription(tagType)
            };
        }

        public string? GetAEStableDescription()
        {
            return GetBooleanDescription(TagAEStable, "Yes", "No");
        }

        public string? GetAFStableDescription()
        {
            return GetBooleanDescription(TagAFStable, "Yes", "No");
        }

        public string? GetHdrImageTypeDescription()
        {
            return GetIndexedDescription(TagHdrImageType, 3, "HDR Image", "Original Image");
        }

        public string? GetAccelerationVectorDescription()
        {
            var values = Directory.GetRationalArray(TagAccelerationVector);
            if (values is null || values.Length != 3)
                return null;
            return $"{values[0].Absolute.ToDouble():N2}g {(values[0].IsPositive ? "left" : "right")}, " +
                   $"{values[1].Absolute.ToDouble():N2}g {(values[1].IsPositive ? "down" : "up")}, " +
                   $"{values[2].Absolute.ToDouble():N2}g {(values[2].IsPositive ? "forward" : "backward")}";
        }

        public string? GetImageCaptureTypeDescription()
        {
            if (Directory.TryGetInt32(TagImageCaptureType, out int value))
            {
                return value switch
                {
                    1 => "ProRAW",
                    2 => "Portrait",
                    10 => "Photo",
                    _ => base.GetDescription(TagImageCaptureType)
                };
            }

            return base.GetDescription(TagImageCaptureType);
        }

        public string? GetFrontFacingCameraDescription()
        {
            return GetBooleanDescription(TagFrontFacingCamera, "Yes", "No");
        }
    }
}
