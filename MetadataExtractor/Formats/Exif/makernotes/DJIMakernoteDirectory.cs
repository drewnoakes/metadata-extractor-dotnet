// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to DJI aircraft cameras.</summary>
    /// <remarks>Using information from https://metacpan.org/pod/distribution/Image-ExifTool/lib/Image/ExifTool/TagNames.pod#DJI-Tags</remarks>
    /// <author>Charlie Matherne, adapted from Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class DJIMakernoteDirectory : Directory
    {
        // Retrieved from
        // Tag ID   Tag Name                             Writable
        // ------   --------                             --------
        // 0x0001   Make                                 string
        // 0x0003   SpeedX                               float
        // 0x0004   SpeedY                               float
        // 0x0005   SpeedZ                               float
        // 0x0006   Pitch                                float
        // 0x0007   Yaw                                  float
        // 0x0008   Roll                                 float
        // 0x0009   CameraPitch                          float
        // 0x000a   CameraYaw                            float
        // 0x000b   CameraRoll                           float
        public const int TagMake = 0x0001;
        public const int TagSpeedX = 0x0003;
        public const int TagSpeedY = 0x0004;
        public const int TagSpeedZ = 0x0005;
        public const int TagAircraftPitch = 0x0006;
        public const int TagAircraftYaw = 0x0007;
        public const int TagAircraftRoll = 0x0008;
        public const int TagCameraPitch = 0x0009;
        public const int TagCameraYaw = 0x000a;
        public const int TagCameraRoll = 0x000b;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagMake, "Make" },
            { TagSpeedX, "Aircraft X Speed" },
            { TagSpeedY, "Aircraft Y Speed" },
            { TagSpeedZ, "Aircraft Z Speed" },
            { TagAircraftPitch, "Aircraft Pitch" },
            { TagAircraftYaw, "Aircraft Yaw" },
            { TagAircraftRoll, "Aircraft Roll" },
            { TagCameraPitch, "Camera Pitch" },
            { TagCameraYaw, "Camera Yaw" },
            { TagCameraRoll, "Camera Roll" }
        };

        public DJIMakernoteDirectory()
        {
            SetDescriptor(new DJIMakernoteDescriptor(this));
        }

        public override string Name => "DJI Makernote";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }

        /// <summary>
        /// Parses various tags in an attempt to obtain a single object representing the x speed,
        /// of the aircraft, at which this image was captured.
        /// </summary>
        /// <returns>The x speed of the aircraft for this image, if possible, otherwise <c>null</c>.</returns>
        public float? GetAircraftSpeedX() => this.TryGetSingle(TagSpeedX, out var value) ? value : (float?)null;

        /// <summary>
        /// Parses various tags in an attempt to obtain a single object representing the y speed,
        /// of the aircraft, at which this image was captured.
        /// </summary>
        /// <returns>The y speed of the aircraft for this image, if possible, otherwise <c>null</c>.</returns>
        public float? GetAircraftSpeedY() => this.TryGetSingle(TagSpeedY, out var value) ? (float?)value : null;

        /// <summary>
        /// Parses various tags in an attempt to obtain a single object representing the z speed,
        /// of the aircraft, at which this image was captured.
        /// </summary>
        /// <returns>The z speed of the aircraft for this image, if possible, otherwise <c>null</c>.</returns>
        public float? GetAircraftSpeedZ() => this.TryGetSingle(TagSpeedZ, out var value) ? (float?)value : null;

        /// <summary>
        /// Parses various tags in an attempt to obtain a single object representing the pitch,
        /// of the aircraft, at which this image was captured.
        /// </summary>
        /// <returns>The pitch of the aircraft for this image, if possible, otherwise <c>null</c>.</returns>
        public float? GetAircraftPitch() => this.TryGetSingle(TagAircraftPitch, out var value) ? (float?)value : null;

        /// <summary>
        /// Parses various tags in an attempt to obtain a single object representing the yaw,
        /// of the aircraft, at which this image was captured.
        /// </summary>
        /// <returns>The yaw of the aircraft for this image, if possible, otherwise <c>null</c>.</returns>
        public float? GetAircraftYaw() => this.TryGetSingle(TagAircraftYaw, out var value) ? (float?)value : null;

        /// <summary>
        /// Parses various tags in an attempt to obtain a single object representing the roll,
        /// of the aircraft, at which this image was captured.
        /// </summary>
        /// <returns>The roll of the aircraft for this image, if possible, otherwise <c>null</c>.</returns>
        public float? GetAircraftRoll() => this.TryGetSingle(TagAircraftRoll, out var value) ? (float?)value : null;

        /// <summary>
        /// Parses various tags in an attempt to obtain a single object representing the pitch,
        /// of the camera, at which this image was captured.
        /// </summary>
        /// <returns>The pitch of the camera for this image, if possible, otherwise <c>null</c>.</returns>
        public float? GetCameraPitch() => this.TryGetSingle(TagCameraPitch, out var value) ? (float?)value : null;

        /// <summary>
        /// Parses various tags in an attempt to obtain a single object representing the yaw,
        /// of the camera, at which this image was captured.
        /// </summary>
        /// <returns>The yaw of the camera for this image, if possible, otherwise <c>null</c>.</returns>
        public float? GetCameraYaw() => this.TryGetSingle(TagCameraYaw, out var value) ? (float?)value : null;

        /// <summary>
        /// Parses various tags in an attempt to obtain a single object representing the roll,
        /// of the camera, at which this image was captured.
        /// </summary>
        /// <returns>The roll of the camera for this image, if possible, otherwise <c>null</c>.</returns>
        public float? GetCameraRoll() => this.TryGetSingle(TagCameraRoll, out var value) ? (float?)value : null;
    }
}
