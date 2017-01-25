#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using JetBrains.Annotations;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to Panasonic and Leica cameras.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Philipp Sandhaus</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class PanasonicMakernoteDirectory : Directory
    {
        /// <summary>
        /// 2 = High
        /// 3 = Normal
        /// 6 = Very High
        /// 7 = Raw
        /// 9 = Motion Picture
        /// </summary>
        public const int TagQualityMode = 0x0001;

        public const int TagFirmwareVersion = 0x0002;

        /// <summary>
        /// 1 = Auto
        /// 2 = Daylight
        /// 3 = Cloudy
        /// 4 = Incandescent
        /// 5 = Manual
        /// 8 = Flash
        /// 10 = Black &amp; White
        /// 11 = Manual
        /// 12 = Shade
        /// </summary>
        public const int TagWhiteBalance = 0x0003;

        /// <summary>
        /// 1 = Auto
        /// 2 = Manual
        /// 4 =  Auto, Focus Button
        /// 5 = Auto, Continuous
        /// </summary>
        public const int TagFocusMode = 0x0007;

        /// <summary>
        /// 2 bytes
        /// (DMC-FZ10)
        /// '0 1' = Spot Mode On
        /// '0 16' = Spot Mode Off
        /// '(other models)
        /// 16 = Normal?
        /// '0 1' = 9-area
        /// '0 16' = 3-area (high speed)
        /// '1 0' = Spot Focusing
        /// '1 1' = 5-area
        /// '16 0' = 1-area
        /// '16 16' = 1-area (high speed)
        /// '32 0' = Auto or Face Detect
        /// '32 1' = 3-area (left)?
        /// '32 2' = 3-area (center)?
        /// '32 3' = 3-area (right)?
        /// '64 0' = Face Detect
        /// </summary>
        public const int TagAfAreaMode = 0x000f;

        /// <summary>
        /// 2 = On, Mode 1
        /// 3 = Off
        /// 4 = On, Mode 2
        /// </summary>
        public const int TagImageStabilization = 0x001a;

        /// <summary>
        /// 1 = On
        /// 2 = Off
        /// </summary>
        public const int TagMacroMode = 0x001C;

        /// <summary>
        /// 1 = Normal
        /// 2 = Portrait
        /// 3 = Scenery
        /// 4 = Sports
        /// 5 = Night Portrait
        /// 6 = Program
        /// 7 = Aperture Priority
        /// 8 = Shutter Priority
        /// 9 = Macro
        /// 10= Spot
        /// 11= Manual
        /// 12= Movie Preview
        /// 13= Panning
        /// 14= Simple
        /// 15= Color Effects
        /// 16= Self Portrait
        /// 17= Economy
        /// 18= Fireworks
        /// 19= Party
        /// 20= Snow
        /// 21= Night Scenery
        /// 22= Food
        /// 23= Baby
        /// 24= Soft Skin
        /// 25= Candlelight
        /// 26= Starry Night
        /// 27= High Sensitivity
        /// 28= Panorama Assist
        /// 29= Underwater
        /// 30= Beach
        /// 31= Aerial Photo
        /// 32= Sunset
        /// 33= Pet
        /// 34= Intelligent ISO
        /// 35= Clipboard
        /// 36= High Speed Continuous Shooting
        /// 37= Intelligent Auto
        /// 39= Multi-aspect
        /// 41= Transform
        /// 42= Flash Burst
        /// 43= Pin Hole
        /// 44= Film Grain
        /// 45= My Color
        /// 46= Photo Frame
        /// 51= HDR
        /// </summary>
        public const int TagRecordMode = 0x001F;

        /// <summary>
        /// 1 = Yes
        /// 2 = No
        /// </summary>
        public const int TagAudio = 0x0020;

        /// <summary>No idea, what this is</summary>
        public const int TagUnknownDataDump = 0x0021;

        public const int TagEasyMode = 0x0022;

        public const int TagWhiteBalanceBias = 0x0023;

        public const int TagFlashBias = 0x0024;

        /// <summary>
        /// this number is unique, and contains the date of manufacture,
        /// but is not the same as the number printed on the camera body
        /// </summary>
        public const int TagInternalSerialNumber = 0x0025;

        /// <summary>Panasonic Exif Version</summary>
        public const int TagExifVersion = 0x0026;

        /// <summary>
        /// 1 = Off
        /// 2 = Warm
        /// 3 = Cool
        /// 4 = Black &amp; White
        /// 5 = Sepia
        /// </summary>
        public const int TagColorEffect = 0x0028;

        /// <summary>
        /// 4 Bytes
        /// Time in 1/100 s from when the camera was powered on to when the
        /// image is written to memory card
        /// </summary>
        public const int TagUptime = 0x0029;

        /// <summary>
        /// 0 = Off
        /// 1 = On
        /// 2 = Infinite
        /// 4 = Unlimited
        /// </summary>
        public const int TagBurstMode = 0x002a;

        public const int TagSequenceNumber = 0x002b;

        /// <summary>
        /// (this decoding seems to work for some models such as the LC1, LX2, FZ7, FZ8, FZ18 and FZ50, but may not be correct for other models such as the FX10, G1, L1, L10 and LC80)
        /// 0x0 = Normal
        /// 0x1 = Low
        /// 0x2 = High
        /// 0x6 = Medium Low
        /// 0x7 = Medium High
        /// 0x100 = Low
        /// 0x110 = Normal
        /// 0x120 = High
        /// (these values are used by the GF1)
        /// 0 = -2
        /// 1 = -1
        /// 2 = Normal
        /// 3 = +1
        /// 4 = +2
        /// 7 = Nature (Color Film)
        /// 12 = Smooth (Color Film) or Pure (My Color)
        /// 17 = Dynamic (B&amp;W Film)
        /// 22 = Smooth (B&amp;W Film)
        /// 27 = Dynamic (Color Film)
        /// 32 = Vibrant (Color Film) or Expressive (My Color)
        /// 33 = Elegant (My Color)
        /// 37 = Nostalgic (Color Film)
        /// 41 = Dynamic Art (My Color)
        /// 42 = Retro (My Color)
        /// </summary>
        public const int TagContrastMode = 0x002c;

        /// <summary>
        /// 0 = Standard
        /// 1 = Low (-1)
        /// 2 = High (+1)
        /// 3 = Lowest (-2)
        /// 4 = Highest (+2)
        /// </summary>
        public const int TagNoiseReduction = 0x002d;

        /// <summary>
        /// 1 = Off
        /// 2 = 10 s
        /// 3 = 2 s
        /// </summary>
        public const int TagSelfTimer = 0x002e;

        /// <summary>
        /// 1 = 0 DG
        /// 3 = 180 DG
        /// 6 =  90 DG
        /// 8 = 270 DG
        /// </summary>
        public const int TagRotation = 0x0030;

        /// <summary>
        /// 1 = Fired
        /// 2 = Enabled nut not used
        /// 3 = Disabled but required
        /// 4 = Disabled and not required
        /// </summary>
        public const int TagAfAssistLamp = 0x0031;

        /// <summary>
        /// 0 = Normal
        /// 1 = Natural
        /// 2 = Vivid
        /// </summary>
        public const int TagColorMode = 0x0032;

        public const int TagBabyAge = 0x0033;

        /// <summary>
        /// 1 = Standard
        /// 2 = Extended
        /// </summary>
        public const int TagOpticalZoomMode = 0x0034;

        /// <summary>
        /// 1 = Off
        /// 2 = Wide
        /// 3 = Telephoto
        /// 4 = Macro
        /// </summary>
        public const int TagConversionLens = 0x0035;

        public const int TagTravelDay = 0x0036;

        /// <summary>0 = Normal</summary>
        public const int TagContrast = 0x0039;

        /// <summary>
        ///
        /// 1 = Home
        /// 2 = Destination
        /// </summary>
        public const int TagWorldTimeLocation = 0x003a;

        /// <summary>
        /// 1 = Off
        /// 2 = On
        /// </summary>
        public const int TagTextStamp = 0x003b;

        public const int TagProgramIso = 0x003c;

        /// <summary>
        ///
        /// 1 = Normal
        /// 2 = Outdoor/Illuminations/Flower/HDR Art
        /// 3 = Indoor/Architecture/Objects/HDR B&amp;W
        /// 4 = Creative
        /// 5 = Auto
        /// 7 = Expressive
        /// 8 = Retro
        /// 9 = Pure
        /// 10 = Elegant
        /// 12 = Monochrome
        /// 13 = Dynamic Art
        /// 14 = Silhouette
        /// </summary>
        public const int TagAdvancedSceneMode = 0x003d;

        /// <summary>
        /// 1 = Off
        /// 2 = On
        /// </summary>
        public const int TagTextStamp1 = 0x003e;

        public const int TagFacesDetected = 0x003f;
        public const int TagSaturation = 0x0040;
        public const int TagSharpness = 0x0041;
        public const int TagFilmMode = 0x0042;

        public const int TagColorTempKelvin = 0x0044;
        public const int TagBracketSettings = 0x0045;

        /// <summary>WB adjust AB.</summary>
        /// <remarks>WB adjust AB. Positive is a shift toward blue.</remarks>
        public const int TagWbAdjustAb = 0x0046;

        /// <summary>WB adjust GM.</summary>
        /// <remarks>WB adjust GM. Positive is a shift toward green.</remarks>
        public const int TagWbAdjustGm = 0x0047;

        public const int TagFlashCurtain = 0x0048;
        public const int TagLongExposureNoiseReduction = 0x0049;

        public const int TagPanasonicImageWidth = 0x004b;
        public const int TagPanasonicImageHeight = 0x004c;
        public const int TagAfPointPosition = 0x004d;

        /// <summary>
        ///
        /// Integer (16Bit) Indexes:
        /// 0  Number Face Positions (maybe less than Faces Detected)
        /// 1-4 Face Position 1
        /// 5-8 Face Position 2
        /// and so on
        ///
        /// The four Integers are interpreted as follows:
        /// (XYWH)  X,Y Center of Face,  (W,H) Width and Height
        /// All values are in respect to double the size of the thumbnail image
        /// </summary>
        public const int TagFaceDetectionInfo = 0x004e;

        public const int TagLensType = 0x0051;
        public const int TagLensSerialNumber = 0x0052;
        public const int TagAccessoryType = 0x0053;
        public const int TagAccessorySerialNumber = 0x0054;

        /// <summary>
        /// (decoded as two 16-bit signed integers)
        /// '-1 1' = Slim Low
        /// '-3 2' = Slim High
        /// '0 0' = Off
        /// '1 1' = Stretch Low
        /// '3 2' = Stretch High
        /// </summary>
        public const int TagTransform = 0x0059;

        /// <summary>
        /// 0 = Off
        /// 1 = Low
        /// 2 = Standard
        /// 3 = High
        /// </summary>
        public const int TagIntelligentExposure = 0x005d;

        public const int TagLensFirmwareVersion = 0x0060;
        public const int TagBurstSpeed = 0x0077;
        public const int TagIntelligentDRange = 0x0079;
        public const int TagClearRetouch = 0x007c;
        public const int TagCity2 = 0x0080;
        public const int TagPhotoStyle = 0x0089;
        public const int TagShadingCompensation = 0x008a;

        public const int TagAccelerometerZ = 0x008c;
        public const int TagAccelerometerX = 0x008d;
        public const int TagAccelerometerY = 0x008e;
        public const int TagCameraOrientation = 0x008f;
        public const int TagRollAngle = 0x0090;
        public const int TagPitchAngle = 0x0091;
        public const int TagSweepPanoramaDirection = 0x0093;
        public const int TagSweepPanoramaFieldOfView = 0x0094;
        public const int TagTimerRecording = 0x0096;

        public const int TagInternalNDFilter = 0x009d;
        public const int TagHDR = 0x009e;
        public const int TagShutterType = 0x009f;

        public const int TagClearRetouchValue = 0x00a3;
        public const int TagTouchAe = 0x00ab;

        /// <summary>Info at http://www.ozhiker.com/electronics/pjmt/jpeg_info/pim.html</summary>
        public const int TagPrintImageMatchingInfo = 0x0E00;

        /// <summary>
        /// Byte Indexes:
        /// 0    Int (2  Byte) Number of Recognized Faces
        /// 4    String(20 Byte)    Recognized Face 1 Name
        /// 24    4 Int (8 Byte)     Recognized Face 1 Position  (Same Format as Face Detection)
        /// 32    String(20 Byte)    Recognized Face 1 Age
        /// 52    String(20 Byte)    Recognized Face 2 Name
        /// 72    4 Int (8 Byte)     Recognized Face 2 Position  (Same Format as Face Detection)
        /// 80    String(20 Byte)    Recognized Face 2 Age
        ///
        /// And so on
        ///
        /// The four Integers are interpreted as follows:
        /// (XYWH)  X,Y Center of Face,  (W,H) Width and Height
        /// All values are in respect to double the size of the thumbnail image
        /// </summary>
        public const int TagFaceRecognitionInfo = 0x0061;

        /// <summary>
        /// 0 = No
        /// 1 = Yes
        /// </summary>
        public const int TagFlashWarning = 0x0062;

        public const int TagRecognizedFaceFlags = 0x0063;
        public const int TagTitle = 0x0065;
        public const int TagBabyName = 0x0066;
        public const int TagLocation = 0x0067;
        public const int TagCountry = 0x0069;
        public const int TagState = 0x006b;
        public const int TagCity = 0x006d;
        public const int TagLandmark = 0x006f;

        /// <summary>
        /// 0 = Off
        /// 2 = Auto
        /// 3 = On
        /// </summary>
        public const int TagIntelligentResolution = 0x0070;

        public const int TagMakernoteVersion = 0x8000;
        public const int TagSceneMode = 0x8001;
        public const int TagWbRedLevel = 0x8004;
        public const int TagWbGreenLevel = 0x8005;
        public const int TagWbBlueLevel = 0x8006;
        public const int TagFlashFired = 0x8007;
        public const int TagTextStamp2 = 0x8008;
        public const int TagTextStamp3 = 0x8009;
        public const int TagBabyAge1 = 0x8010;

        /// <summary>
        /// (decoded as two 16-bit signed integers)
        /// '-1 1' = Slim Low
        /// '-3 2' = Slim High
        /// '0 0' = Off
        /// '1 1' = Stretch Low
        /// '3 2' = Stretch High
        /// </summary>
        public const int TagTransform1 = 0x8012;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagQualityMode, "Quality Mode" },
            { TagFirmwareVersion, "Version" },
            { TagWhiteBalance, "White Balance" },
            { TagFocusMode, "Focus Mode" },
            { TagAfAreaMode, "AF Area Mode" },
            { TagImageStabilization, "Image Stabilization" },
            { TagMacroMode, "Macro Mode" },
            { TagRecordMode, "Record Mode" },
            { TagAudio, "Audio" },
            { TagInternalSerialNumber, "Internal Serial Number" },
            { TagUnknownDataDump, "Unknown Data Dump" },
            { TagEasyMode, "Easy Mode" },
            { TagWhiteBalanceBias, "White Balance Bias" },
            { TagFlashBias, "Flash Bias" },
            { TagExifVersion, "Exif Version" },
            { TagColorEffect, "Color Effect" },
            { TagUptime, "Camera Uptime" },
            { TagBurstMode, "Burst Mode" },
            { TagSequenceNumber, "Sequence Number" },
            { TagContrastMode, "Contrast Mode" },
            { TagNoiseReduction, "Noise Reduction" },
            { TagSelfTimer, "Self Timer" },
            { TagRotation, "Rotation" },
            { TagAfAssistLamp, "AF Assist Lamp" },
            { TagColorMode, "Color Mode" },
            { TagBabyAge, "Baby Age" },
            { TagOpticalZoomMode, "Optical Zoom Mode" },
            { TagConversionLens, "Conversion Lens" },
            { TagTravelDay, "Travel Day" },
            { TagContrast, "Contrast" },
            { TagWorldTimeLocation, "World Time Location" },
            { TagTextStamp, "Text Stamp" },
            { TagProgramIso, "Program ISO" },
            { TagAdvancedSceneMode, "Advanced Scene Mode" },
            { TagPrintImageMatchingInfo, "Print Image Matching (PIM) Info" },
            { TagFacesDetected, "Number of Detected Faces" },
            { TagSaturation, "Saturation" },
            { TagSharpness, "Sharpness" },
            { TagFilmMode, "Film Mode" },
            { TagColorTempKelvin, "Color Temp Kelvin" },
            { TagBracketSettings, "Bracket Settings" },
            { TagWbAdjustAb, "White Balance Adjust (AB)" },
            { TagWbAdjustGm, "White Balance Adjust (GM)" },

            { TagFlashCurtain, "Flash Curtain" },
            { TagLongExposureNoiseReduction, "Long Exposure Noise Reduction" },
            { TagPanasonicImageWidth, "Panasonic Image Width" },
            { TagPanasonicImageHeight, "Panasonic Image Height" },

            { TagAfPointPosition, "Af Point Position" },
            { TagFaceDetectionInfo, "Face Detection Info" },
            { TagLensType, "Lens Type" },
            { TagLensSerialNumber, "Lens Serial Number" },
            { TagAccessoryType, "Accessory Type" },
            { TagAccessorySerialNumber, "Accessory Serial Number" },
            { TagTransform, "Transform" },
            { TagIntelligentExposure, "Intelligent Exposure" },
            { TagLensFirmwareVersion, "Lens Firmware Version" },
            { TagFaceRecognitionInfo, "Face Recognition Info" },
            { TagFlashWarning, "Flash Warning" },
            { TagRecognizedFaceFlags, "Recognized Face Flags" },
            { TagTitle, "Title" },
            { TagBabyName, "Baby Name" },
            { TagLocation, "Location" },
            { TagCountry, "Country" },
            { TagState, "State" },
            { TagCity, "City" },
            { TagLandmark, "Landmark" },
            { TagIntelligentResolution, "Intelligent Resolution" },
            { TagBurstSpeed, "Burst Speed" },
            { TagIntelligentDRange, "Intelligent D-Range" },
            { TagClearRetouch, "Clear Retouch" },
            { TagCity2, "City 2" },
            { TagPhotoStyle, "Photo Style" },
            { TagShadingCompensation, "Shading Compensation" },

            { TagAccelerometerZ, "Accelerometer Z" },
            { TagAccelerometerX, "Accelerometer X" },
            { TagAccelerometerY, "Accelerometer Y" },
            { TagCameraOrientation, "Camera Orientation" },
            { TagRollAngle, "Roll Angle" },
            { TagPitchAngle, "Pitch Angle" },
            { TagSweepPanoramaDirection, "Sweep Panorama Direction" },
            { TagSweepPanoramaFieldOfView, "Sweep Panorama Field Of View" },
            { TagTimerRecording, "Timer Recording" },

            { TagInternalNDFilter, "Internal ND Filter" },
            { TagHDR, "HDR" },
            { TagShutterType, "Shutter Type" },
            { TagClearRetouchValue, "Clear Retouch Value" },
            { TagTouchAe, "Touch AE" },

            { TagMakernoteVersion, "Makernote Version" },
            { TagSceneMode, "Scene Mode" },
            { TagWbRedLevel, "White Balance (Red)" },
            { TagWbGreenLevel, "White Balance (Green)" },
            { TagWbBlueLevel, "White Balance (Blue)" },
            { TagFlashFired, "Flash Fired" },
            { TagTextStamp1, "Text Stamp 1" },
            { TagTextStamp2, "Text Stamp 2" },
            { TagTextStamp3, "Text Stamp 3" },
            { TagBabyAge1, "Baby Age 1" },
            { TagTransform1, "Transform 1" }
        };

        public PanasonicMakernoteDirectory()
        {
            SetDescriptor(new PanasonicMakernoteDescriptor(this));
        }

        public override string Name => "Panasonic Makernote";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }

        [NotNull, ItemNotNull]
        public IEnumerable<Face> GetDetectedFaces()
        {
            return ParseFaces(this.GetByteArray(TagFaceDetectionInfo), 2, 0, 8);
        }

        [NotNull, ItemNotNull]
        public IEnumerable<Face> GetRecognizedFaces()
        {
            return ParseFaces(this.GetByteArray(TagFaceRecognitionInfo), 4, 20, 44);
        }

        [NotNull]
        private static IEnumerable<Face> ParseFaces(byte[] bytes, int firstRecordOffset, int posOffset, int recordLength)
        {
            if (bytes == null)
                yield break;

            var reader = new ByteArrayReader(bytes, isMotorolaByteOrder: false);

            int faceCount = reader.GetUInt16(0);

            if (faceCount == 0 || bytes.Length < firstRecordOffset + faceCount*recordLength)
                yield break;

            posOffset += firstRecordOffset;

            for (int i = 0, recordOffset = firstRecordOffset; i < faceCount; i++, recordOffset += recordLength, posOffset += recordLength)
            {
                yield return new Face(
                    x: reader.GetUInt16(posOffset),
                    y: reader.GetUInt16(posOffset + 2),
                    width: reader.GetUInt16(posOffset + 4),
                    height: reader.GetUInt16(posOffset + 6),
                    name: recordLength == 44 ? reader.GetString(recordOffset, 20, Encoding.UTF8).Trim(' ', '\0') : null,
                    age: recordLength == 44 ? Age.FromPanasonicString(reader.GetString(recordOffset + 28, 20, Encoding.UTF8).Trim(' ', '\0')) : null);
            }
        }

        /// <summary>Attempts to convert the underlying string value (as stored in the directory) into an Age object.</summary>
        /// <param name="tag">The tag identifier.</param>
        /// <returns>The parsed Age object, or null if the tag was empty of the value unable to be parsed.</returns>
        [CanBeNull]
        public Age GetAge(int tag)
        {
            var ageString = this.GetString(tag);
            return ageString == null ? null : Age.FromPanasonicString(ageString);
        }
    }
}
