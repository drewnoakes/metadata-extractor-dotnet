using JetBrains.Annotations;
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
        //Tag ID   Tag Name                             Writable
        //------   --------                             --------
        //0x0001   Make                                 string
        //0x0003   SpeedX                               float
        //0x0004   SpeedY                               float
        //0x0005   SpeedZ                               float
        //0x0006   Pitch                                float
        //0x0007   Yaw                                  float
        //0x0008   Roll                                 float
        //0x0009   CameraPitch                          float
        //0x000a   CameraYaw                            float
        //0x000b   CameraRoll                           float
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
        [CanBeNull]
        public float? GetAircraftSpeedX()
        {
            float value;
            if (!this.TryGetSingle(TagSpeedX, out value))
                return null;

            return value;
        }

        /// <summary>
        /// Parses various tags in an attempt to obtain a single object representing the y speed,
        /// of the aircraft, at which this image was captured.
        /// </summary>
        /// <returns>The y speed of the aircraft for this image, if possible, otherwise <c>null</c>.</returns>
        [CanBeNull]
        public float? GetAircraftSpeedY()
        {
            float value;
            if (!this.TryGetSingle(TagSpeedY, out value))
                return null;

            return value;
        }

        /// <summary>
        /// Parses various tags in an attempt to obtain a single object representing the z speed,
        /// of the aircraft, at which this image was captured.
        /// </summary>
        /// <returns>The z speed of the aircraft for this image, if possible, otherwise <c>null</c>.</returns>
        [CanBeNull]
        public float? GetAircraftSpeedZ()
        {
            float value;
            if (!this.TryGetSingle(TagSpeedZ, out value))
                return null;

            return value;
        }

        /// <summary>
        /// Parses various tags in an attempt to obtain a single object representing the pitch,
        /// of the aircraft, at which this image was captured.
        /// </summary>
        /// <returns>The pitch of the aircraft for this image, if possible, otherwise <c>null</c>.</returns>
        [CanBeNull]
        public float? GetAircraftPitch()
        {
            float value;
            if (!this.TryGetSingle(TagAircraftPitch, out value))
                return null;

            return value;
        }

        /// <summary>
        /// Parses various tags in an attempt to obtain a single object representing the yaw,
        /// of the aircraft, at which this image was captured.
        /// </summary>
        /// <returns>The yaw of the aircraft for this image, if possible, otherwise <c>null</c>.</returns>
        [CanBeNull]
        public float? GetAircraftYaw()
        {
            float value;
            if (!this.TryGetSingle(TagAircraftYaw, out value))
                return null;

            return value;
        }

        /// <summary>
        /// Parses various tags in an attempt to obtain a single object representing the roll,
        /// of the aircraft, at which this image was captured.
        /// </summary>
        /// <returns>The roll of the aircraft for this image, if possible, otherwise <c>null</c>.</returns>
        [CanBeNull]
        public float? GetAircraftRoll()
        {
            float value;
            if (!this.TryGetSingle(TagAircraftRoll, out value))
                return null;

            return value;
        }

        /// <summary>
        /// Parses various tags in an attempt to obtain a single object representing the pitch,
        /// of the camera, at which this image was captured.
        /// </summary>
        /// <returns>The pitch of the camera for this image, if possible, otherwise <c>null</c>.</returns>
        [CanBeNull]
        public float? GetCameraPitch()
        {
            float value;
            if (!this.TryGetSingle(TagCameraPitch, out value))
                return null;
            
            return value;
        }

        /// <summary>
        /// Parses various tags in an attempt to obtain a single object representing the yaw,
        /// of the camera, at which this image was captured.
        /// </summary>
        /// <returns>The yaw of the camera for this image, if possible, otherwise <c>null</c>.</returns>
        [CanBeNull]
        public float? GetCameraYaw()
        {
            float value;
            if (!this.TryGetSingle(TagCameraYaw, out value))
                return null;

            return value;
        }

        /// <summary>
        /// Parses various tags in an attempt to obtain a single object representing the roll,
        /// of the camera, at which this image was captured.
        /// </summary>
        /// <returns>The roll of the camera for this image, if possible, otherwise <c>null</c>.</returns>
        [CanBeNull]
        public float? GetCameraRoll()
        {
            float value;
            if (!this.TryGetSingle(TagCameraRoll, out value))
                return null;

            return value;
        }
    }
}