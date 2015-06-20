#region License
//
// Copyright 2002-2015 Drew Noakes
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
using System.Text;
using JetBrains.Annotations;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to Panasonic and Leica cameras.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Philipp Sandhaus</author>
    public sealed class PanasonicMakernoteDirectory : Directory
    {
        /// <summary>
        /// <br />
        /// 2 = High            <br />
        /// 3 = Normal          <br />
        /// 6 = Very High       <br />
        /// 7 = Raw             <br />
        /// 9 = Motion Picture  <br />
        /// </summary>
        public const int TagQualityMode = 0x0001;

        public const int TagFirmwareVersion = 0x0002;

        /// <summary>
        /// <br />
        /// 1 = Auto            <br />
        /// 2 = Daylight        <br />
        /// 3 = Cloudy          <br />
        /// 4 = Incandescent    <br />
        /// 5 = Manual          <br />
        /// 8 = Flash           <br />
        /// 10 = Black &amp; White  <br />
        /// 11 = Manual         <br />
        /// 12 = Shade          <br />
        /// </summary>
        public const int TagWhiteBalance = 0x0003;

        /// <summary>
        /// <br />
        /// 1 = Auto                <br />
        /// 2 = Manual              <br />
        /// 4 =  Auto, Focus Button <br />
        /// 5 = Auto, Continuous    <br />
        /// </summary>
        public const int TagFocusMode = 0x0007;

        /// <summary>
        /// <br />
        /// 2 bytes                         <br />
        /// (DMC-FZ10)                      <br />
        /// '0 1' = Spot Mode On            <br />
        /// '0 16' = Spot Mode Off          <br />
        /// '(other models)                 <br />
        /// 16 = Normal?                    <br />
        /// '0 1' = 9-area                  <br />
        /// '0 16' = 3-area (high speed)    <br />
        /// '1 0' = Spot Focusing           <br />
        /// '1 1' = 5-area                  <br />
        /// '16 0' = 1-area                 <br />
        /// '16 16' = 1-area (high speed)   <br />
        /// '32 0' = Auto or Face Detect    <br />
        /// '32 1' = 3-area (left)?         <br />
        /// '32 2' = 3-area (center)?       <br />
        /// '32 3' = 3-area (right)?        <br />
        /// '64 0' = Face Detect            <br />
        /// </summary>
        public const int TagAfAreaMode = 0x000f;

        /// <summary>
        /// <br />
        /// 2 = On, Mode 1   <br />
        /// 3 = Off          <br />
        /// 4 = On, Mode 2   <br />
        /// </summary>
        public const int TagImageStabilization = 0x001a;

        /// <summary>
        /// <br />
        /// 1 = On    <br />
        /// 2 = Off   <br />
        /// </summary>
        public const int TagMacroMode = 0x001C;

        /// <summary>
        /// <br />
        /// 1 = Normal                            <br />
        /// 2 = Portrait                          <br />
        /// 3 = Scenery                           <br />
        /// 4 = Sports                            <br />
        /// 5 = Night Portrait                    <br />
        /// 6 = Program                           <br />
        /// 7 = Aperture Priority                 <br />
        /// 8 = Shutter Priority                  <br />
        /// 9 = Macro                             <br />
        /// 10= Spot                              <br />
        /// 11= Manual                            <br />
        /// 12= Movie Preview                     <br />
        /// 13= Panning                           <br />
        /// 14= Simple                            <br />
        /// 15= Color Effects                     <br />
        /// 16= Self Portrait                     <br />
        /// 17= Economy                           <br />
        /// 18= Fireworks                         <br />
        /// 19= Party                             <br />
        /// 20= Snow                              <br />
        /// 21= Night Scenery                     <br />
        /// 22= Food                              <br />
        /// 23= Baby                              <br />
        /// 24= Soft Skin                         <br />
        /// 25= Candlelight                       <br />
        /// 26= Starry Night                      <br />
        /// 27= High Sensitivity                  <br />
        /// 28= Panorama Assist                   <br />
        /// 29= Underwater                        <br />
        /// 30= Beach                             <br />
        /// 31= Aerial Photo                      <br />
        /// 32= Sunset                            <br />
        /// 33= Pet                               <br />
        /// 34= Intelligent ISO                   <br />
        /// 35= Clipboard                         <br />
        /// 36= High Speed Continuous Shooting    <br />
        /// 37= Intelligent Auto                  <br />
        /// 39= Multi-aspect                      <br />
        /// 41= Transform                         <br />
        /// 42= Flash Burst                       <br />
        /// 43= Pin Hole                          <br />
        /// 44= Film Grain                        <br />
        /// 45= My Color                          <br />
        /// 46= Photo Frame                       <br />
        /// 51= HDR                               <br />
        /// </summary>
        public const int TagRecordMode = 0x001F;

        /// <summary>
        /// 1 = Yes <br />
        /// 2 = No  <br />
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
        /// 1 = Off           <br />
        /// 2 = Warm          <br />
        /// 3 = Cool          <br />
        /// 4 = Black &amp; White <br />
        /// 5 = Sepia         <br />
        /// </summary>
        public const int TagColorEffect = 0x0028;

        /// <summary>
        /// 4 Bytes <br />
        /// Time in 1/100 s from when the camera was powered on to when the
        /// image is written to memory card
        /// </summary>
        public const int TagUptime = 0x0029;

        /// <summary>
        /// 0 = Off        <br />
        /// 1 = On         <br />
        /// 2 = Infinite   <br />
        /// 4 = Unlimited  <br />
        /// </summary>
        public const int TagBurstMode = 0x002a;

        public const int TagSequenceNumber = 0x002b;

        /// <summary>
        /// (this decoding seems to work for some models such as the LC1, LX2, FZ7, FZ8, FZ18 and FZ50, but may not be correct for other models such as the FX10, G1, L1, L10 and LC80) <br />
        /// 0x0 = Normal                                            <br />
        /// 0x1 = Low                                               <br />
        /// 0x2 = High                                              <br />
        /// 0x6 = Medium Low                                        <br />
        /// 0x7 = Medium High                                       <br />
        /// 0x100 = Low                                             <br />
        /// 0x110 = Normal                                          <br />
        /// 0x120 = High                                            <br />
        /// (these values are used by the GF1)                      <br />
        /// 0 = -2                                                  <br />
        /// 1 = -1                                                  <br />
        /// 2 = Normal                                              <br />
        /// 3 = +1                                                  <br />
        /// 4 = +2                                                  <br />
        /// 7 = Nature (Color Film)                                 <br />
        /// 12 = Smooth (Color Film) or Pure (My Color)             <br />
        /// 17 = Dynamic (B&amp;W Film)                                 <br />
        /// 22 = Smooth (B&amp;W Film)                                  <br />
        /// 27 = Dynamic (Color Film)                               <br />
        /// 32 = Vibrant (Color Film) or Expressive (My Color)      <br />
        /// 33 = Elegant (My Color)                                 <br />
        /// 37 = Nostalgic (Color Film)                             <br />
        /// 41 = Dynamic Art (My Color)                             <br />
        /// 42 = Retro (My Color)                                   <br />
        /// </summary>
        public const int TagContrastMode = 0x002c;

        /// <summary>
        /// 0 = Standard      <br />
        /// 1 = Low (-1)      <br />
        /// 2 = High (+1)     <br />
        /// 3 = Lowest (-2)   <br />
        /// 4 = Highest (+2)  <br />
        /// </summary>
        public const int TagNoiseReduction = 0x002d;

        /// <summary>
        /// 1 = Off   <br />
        /// 2 = 10 s  <br />
        /// 3 = 2 s   <br />
        /// </summary>
        public const int TagSelfTimer = 0x002e;

        /// <summary>
        /// 1 = 0 DG    <br />
        /// 3 = 180 DG  <br />
        /// 6 =  90 DG  <br />
        /// 8 = 270 DG  <br />
        /// </summary>
        public const int TagRotation = 0x0030;

        /// <summary>
        /// 1 = Fired <br />
        /// 2 = Enabled nut not used <br />
        /// 3 = Disabled but required <br />
        /// 4 = Disabled and not required
        /// </summary>
        public const int TagAfAssistLamp = 0x0031;

        /// <summary>
        /// 0 = Normal <br />
        /// 1 = Natural<br />
        /// 2 = Vivid
        /// </summary>
        public const int TagColorMode = 0x0032;

        public const int TagBabyAge = 0x0033;

        /// <summary>
        /// 1 = Standard <br />
        /// 2 = Extended
        /// </summary>
        public const int TagOpticalZoomMode = 0x0034;

        /// <summary>
        /// 1 = Off <br />
        /// 2 = Wide <br />
        /// 3 = Telephoto <br />
        /// 4 = Macro
        /// </summary>
        public const int TagConversionLens = 0x0035;

        public const int TagTravelDay = 0x0036;

        /// <summary>0 = Normal</summary>
        public const int TagContrast = 0x0039;

        /// <summary>
        /// <br />
        /// 1 = Home <br />
        /// 2 = Destination
        /// </summary>
        public const int TagWorldTimeLocation = 0x003a;

        /// <summary>
        /// 1 = Off   <br />
        /// 2 = On
        /// </summary>
        public const int TagTextStamp = 0x003b;

        public const int TagProgramIso = 0x003c;

        /// <summary>
        /// <br />
        /// 1 = Normal                               <br />
        /// 2 = Outdoor/Illuminations/Flower/HDR Art <br />
        /// 3 = Indoor/Architecture/Objects/HDR B&amp;W  <br />
        /// 4 = Creative                             <br />
        /// 5 = Auto                                 <br />
        /// 7 = Expressive                           <br />
        /// 8 = Retro                                <br />
        /// 9 = Pure                                 <br />
        /// 10 = Elegant                             <br />
        /// 12 = Monochrome                          <br />
        /// 13 = Dynamic Art                         <br />
        /// 14 = Silhouette                          <br />
        /// </summary>
        public const int TagAdvancedSceneMode = 0x003d;

        /// <summary>
        /// 1 = Off   <br />
        /// 2 = On
        /// </summary>
        public const int TagTextStamp1 = 0x003e;

        public const int TagFacesDetected = 0x003f;

        public const int TagSaturation = 0x0040;

        public const int TagSharpness = 0x0041;

        public const int TagFilmMode = 0x0042;

        /// <summary>WB adjust AB.</summary>
        /// <remarks>WB adjust AB. Positive is a shift toward blue.</remarks>
        public const int TagWbAdjustAb = 0x0046;

        /// <summary>WB adjust GM.</summary>
        /// <remarks>WB adjust GM. Positive is a shift toward green.</remarks>
        public const int TagWbAdjustGm = 0x0047;

        public const int TagAfPointPosition = 0x004d;

        /// <summary>
        /// <br />
        /// Integer (16Bit) Indexes:                                             <br />
        /// 0  Number Face Positions (maybe less than Faces Detected)            <br />
        /// 1-4 Face Position 1                                                  <br />
        /// 5-8 Face Position 2                                                  <br />
        /// and so on                                                            <br />
        /// <br />
        /// The four Integers are interpreted as follows:                        <br />
        /// (XYWH)  X,Y Center of Face,  (W,H) Width and Height                  <br />
        /// All values are in respect to double the size of the thumbnail image  <br />
        /// </summary>
        public const int TagFaceDetectionInfo = 0x004e;

        public const int TagLensType = 0x0051;

        public const int TagLensSerialNumber = 0x0052;

        public const int TagAccessoryType = 0x0053;

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
        /// 0 = Off <br />
        /// 1 = Low <br />
        /// 2 = Standard <br />
        /// 3 = High
        /// </summary>
        public const int TagIntelligentExposure = 0x005d;

        /// <summary>Info at http://www.ozhiker.com/electronics/pjmt/jpeg_info/pim.html</summary>
        public const int TagPrintImageMatchingInfo = 0x0E00;

        /// <summary>
        /// Byte Indexes:                                                                       <br />
        /// 0    Int (2  Byte) Number of Recognized Faces                                      <br />
        /// 4    String(20 Byte)    Recognized Face 1 Name                                     <br />
        /// 24    4 Int (8 Byte)     Recognized Face 1 Position  (Same Format as Face Detection)  <br />
        /// 32    String(20 Byte)    Recognized Face 1 Age                                      <br />
        /// 52    String(20 Byte)    Recognized Face 2 Name                                     <br />
        /// 72    4 Int (8 Byte)     Recognized Face 2 Position  (Same Format as Face Detection)  <br />
        /// 80    String(20 Byte)    Recognized Face 2 Age                                      <br />
        /// <br />
        /// And so on                                                                           <br />
        /// <br />
        /// The four Integers are interpreted as follows:                                       <br />
        /// (XYWH)  X,Y Center of Face,  (W,H) Width and Height                                 <br />
        /// All values are in respect to double the size of the thumbnail image                 <br />
        /// </summary>
        public const int TagFaceRecognitionInfo = 0x0061;

        /// <summary>
        /// 0 = No <br />
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
        /// 0 = Off <br />
        /// 2 = Auto <br />
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

        [NotNull] private static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static PanasonicMakernoteDirectory()
        {
            TagNameMap[TagQualityMode] = "Quality Mode";
            TagNameMap[TagFirmwareVersion] = "Version";
            TagNameMap[TagWhiteBalance] = "White Balance";
            TagNameMap[TagFocusMode] = "Focus Mode";
            TagNameMap[TagAfAreaMode] = "AF Area Mode";
            TagNameMap[TagImageStabilization] = "Image Stabilization";
            TagNameMap[TagMacroMode] = "Macro Mode";
            TagNameMap[TagRecordMode] = "Record Mode";
            TagNameMap[TagAudio] = "Audio";
            TagNameMap[TagInternalSerialNumber] = "Internal Serial Number";
            TagNameMap[TagUnknownDataDump] = "Unknown Data Dump";
            TagNameMap[TagEasyMode] = "Easy Mode";
            TagNameMap[TagWhiteBalanceBias] = "White Balance Bias";
            TagNameMap[TagFlashBias] = "Flash Bias";
            TagNameMap[TagExifVersion] = "Exif Version";
            TagNameMap[TagColorEffect] = "Color Effect";
            TagNameMap[TagUptime] = "Camera Uptime";
            TagNameMap[TagBurstMode] = "Burst Mode";
            TagNameMap[TagSequenceNumber] = "Sequence Number";
            TagNameMap[TagContrastMode] = "Contrast Mode";
            TagNameMap[TagNoiseReduction] = "Noise Reduction";
            TagNameMap[TagSelfTimer] = "Self Timer";
            TagNameMap[TagRotation] = "Rotation";
            TagNameMap[TagAfAssistLamp] = "AF Assist Lamp";
            TagNameMap[TagColorMode] = "Color Mode";
            TagNameMap[TagBabyAge] = "Baby Age";
            TagNameMap[TagOpticalZoomMode] = "Optical Zoom Mode";
            TagNameMap[TagConversionLens] = "Conversion Lens";
            TagNameMap[TagTravelDay] = "Travel Day";
            TagNameMap[TagContrast] = "Contrast";
            TagNameMap[TagWorldTimeLocation] = "World Time Location";
            TagNameMap[TagTextStamp] = "Text Stamp";
            TagNameMap[TagProgramIso] = "Program ISO";
            TagNameMap[TagAdvancedSceneMode] = "Advanced Scene Mode";
            TagNameMap[TagPrintImageMatchingInfo] = "Print Image Matching (PIM) Info";
            TagNameMap[TagFacesDetected] = "Number of Detected Faces";
            TagNameMap[TagSaturation] = "Saturation";
            TagNameMap[TagSharpness] = "Sharpness";
            TagNameMap[TagFilmMode] = "Film Mode";
            TagNameMap[TagWbAdjustAb] = "White Balance Adjust (AB)";
            TagNameMap[TagWbAdjustGm] = "White Balance Adjust (GM)";
            TagNameMap[TagAfPointPosition] = "Af Point Position";
            TagNameMap[TagFaceDetectionInfo] = "Face Detection Info";
            TagNameMap[TagLensType] = "Lens Type";
            TagNameMap[TagLensSerialNumber] = "Lens Serial Number";
            TagNameMap[TagAccessoryType] = "Accessory Type";
            TagNameMap[TagTransform] = "Transform";
            TagNameMap[TagIntelligentExposure] = "Intelligent Exposure";
            TagNameMap[TagFaceRecognitionInfo] = "Face Recognition Info";
            TagNameMap[TagFlashWarning] = "Flash Warning";
            TagNameMap[TagRecognizedFaceFlags] = "Recognized Face Flags";
            TagNameMap[TagTitle] = "Title";
            TagNameMap[TagBabyName] = "Baby Name";
            TagNameMap[TagLocation] = "Location";
            TagNameMap[TagCountry] = "Country";
            TagNameMap[TagState] = "State";
            TagNameMap[TagCity] = "City";
            TagNameMap[TagLandmark] = "Landmark";
            TagNameMap[TagIntelligentResolution] = "Intelligent Resolution";
            TagNameMap[TagMakernoteVersion] = "Makernote Version";
            TagNameMap[TagSceneMode] = "Scene Mode";
            TagNameMap[TagWbRedLevel] = "White Balance (Red)";
            TagNameMap[TagWbGreenLevel] = "White Balance (Green)";
            TagNameMap[TagWbBlueLevel] = "White Balance (Blue)";
            TagNameMap[TagFlashFired] = "Flash Fired";
            TagNameMap[TagTextStamp1] = "Text Stamp 1";
            TagNameMap[TagTextStamp2] = "Text Stamp 2";
            TagNameMap[TagTextStamp3] = "Text Stamp 3";
            TagNameMap[TagBabyAge1] = "Baby Age 1";
            TagNameMap[TagTransform1] = "Transform 1";
        }

        public PanasonicMakernoteDirectory()
        {
            SetDescriptor(new PanasonicMakernoteDescriptor(this));
        }

        public override string Name
        {
            get { return "Panasonic Makernote"; }
        }

        protected override IReadOnlyDictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
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

        private static IEnumerable<Face> ParseFaces(byte[] bytes, int firstRecordOffset, int posOffset, int recordLength)
        {
            if (bytes == null)
                yield break;

            var reader = new ByteArrayReader(bytes) { IsMotorolaByteOrder = false };

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
                    name: recordLength == 44 ? reader.GetString(recordOffset, 20, Encoding.ASCII).Trim(' ', '\0') : null,
                    age: recordLength == 44 ? Age.FromPanasonicString(reader.GetString(recordOffset + 28, 20, Encoding.ASCII).Trim(' ', '\0')) : null);
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
