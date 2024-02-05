// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to Apple cameras.</summary>
    /// <remarks>Using information from https://exiftool.org/TagNames/Apple.html</remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class AppleMakernoteDirectory : Directory
    {
#pragma warning disable format
        public const int TagMakernoteVersion           = 0x0001;
        public const int TagAEMatrix                   = 0x0002;
        public const int TagRunTime                    = 0x0003;
        public const int TagAEStable                   = 0x0004;
        public const int TagAETarget                   = 0x0005;
        public const int TagAEAverage                  = 0x0006;
        public const int TagAFStable                   = 0x0007;
        /// <summary>
        /// XYZ coordinates of the acceleration vector in units of g.
        /// As viewed from the front of the phone,
        /// positive X is toward the left side,
        /// positive Y is toward the bottom,
        /// positive Z points into the face of the phone
        /// </summary>
        public const int TagAccelerationVector         = 0x0008;
        public const int TagHdrImageType               = 0x000a;
        /// <summary>
        /// Unique ID for all images in a burst.
        /// </summary>
        public const int TagBurstUuid                  = 0x000b;
        public const int TagFocusDistanceRange         = 0x000c;
        public const int TagOisMode                    = 0x000f;
        public const int TagContentIdentifier          = 0x0011;
        public const int TagImageCaptureType           = 0x0014;
        public const int TagImageUniqueId              = 0x0015;
        public const int TagLivePhotoId                = 0x0017;
        public const int TagImageProcessingFlags       = 0x0019;
        public const int TagQualityHint                = 0x001a;
        public const int TagLuminanceNoiseAmplitude    = 0x001d;
        public const int TagImageCaptureRequestID      = 0x0020;
        public const int TagHdrHeadroom                = 0x0021;
        public const int TagSceneFlags                 = 0x0025;
        public const int TagSignalToNoiseRatioType     = 0x0026;
        public const int TagSignalToNoiseRatio         = 0x0027;
        public const int TagPhotoIdentifier            = 0x002b;
        public const int TagFocusPosition              = 0x002f;
        public const int TagHdrGain                    = 0x0030;
        public const int TagAFMeasuredDepth            = 0x0038;
        public const int TagAFConfidence               = 0x003d;
        public const int TagColorCorrectionMatrix      = 0x003e;
        public const int TagGreenGhostMitigationStatus = 0x003f;
        public const int TagSemanticStyle              = 0x0040;
        public const int TagSemanticStyleRenderingVer  = 0x0041;
        public const int TagSemanticStylePreset        = 0x0042;
        public const int TagFrontFacingCamera          = 0x0045;
#pragma warning restore format

        private static readonly Dictionary<int, string> _tagNameMap = new()
        {
            { TagMakernoteVersion, "Makernote Version" },
            { TagAEMatrix, "AE Matrix" },
            { TagRunTime, "Run Time" },
            { TagAEStable, "AE Stable" },
            { TagAETarget, "AE Target" },
            { TagAEAverage, "AE Average" },
            { TagAFStable, "AF Stable" },
            { TagAccelerationVector, "Acceleration Vector" },
            { TagHdrImageType, "HDR Image Type" },
            { TagBurstUuid, "Burst UUID" },
            { TagFocusDistanceRange, "Focus Distance Range" },
            { TagOisMode, "OIS Mode" },
            { TagContentIdentifier, "Content Identifier" },
            { TagImageCaptureType, "Image Capture Type" },
            { TagImageUniqueId, "Image Unique ID" },
            { TagLivePhotoId, "Live Photo ID" },
            { TagImageProcessingFlags, "Image Processing Flags" },
            { TagQualityHint, "Quality Hint" },
            { TagLuminanceNoiseAmplitude, "Luminance Noise Amplitude" },
            { TagImageCaptureRequestID, "Image Capture Request ID" },
            { TagHdrHeadroom, "HDR Headroom" },
            { TagSceneFlags, "Scene Flags" },
            { TagSignalToNoiseRatioType, "Signal-to-Noise Ratio Type" },
            { TagSignalToNoiseRatio, "Signal-to-Noise Ratio" },
            { TagPhotoIdentifier, "Photo Identifier" },
            { TagFocusPosition, "Focus Position" },
            { TagHdrGain, "HDR Gain" },
            { TagAFMeasuredDepth, "AF Measured Depth" },
            { TagAFConfidence, "AF Confidence" },
            { TagColorCorrectionMatrix, "Color Correction Matrix" },
            { TagGreenGhostMitigationStatus, "Green Ghost Mitigation Status" },
            { TagSemanticStyle, "Semantic Style" },
            { TagSemanticStyleRenderingVer, "Semantic Style Rendering Ver" },
            { TagSemanticStylePreset, "Semantic Style Preset" },
            { TagFrontFacingCamera, "Front Facing Camera" }
        };

        public AppleMakernoteDirectory() : base(_tagNameMap)
        {
            SetDescriptor(new AppleMakernoteDescriptor(this));
        }

        public override string Name => "Apple Makernote";
    }
}
