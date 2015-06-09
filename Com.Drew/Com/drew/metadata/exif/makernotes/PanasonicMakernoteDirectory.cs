/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using System.Collections.Generic;
using System.IO;
using Com.Drew.Lang;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif.Makernotes
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
        public const int TagQualityMode = unchecked(0x0001);

        public const int TagFirmwareVersion = unchecked(0x0002);

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
        public const int TagWhiteBalance = unchecked(0x0003);

        /// <summary>
        /// <br />
        /// 1 = Auto                <br />
        /// 2 = Manual              <br />
        /// 4 =  Auto, Focus Button <br />
        /// 5 = Auto, Continuous    <br />
        /// </summary>
        public const int TagFocusMode = unchecked(0x0007);

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
        public const int TagAfAreaMode = unchecked(0x000f);

        /// <summary>
        /// <br />
        /// 2 = On, Mode 1   <br />
        /// 3 = Off          <br />
        /// 4 = On, Mode 2   <br />
        /// </summary>
        public const int TagImageStabilization = unchecked(0x001a);

        /// <summary>
        /// <br />
        /// 1 = On    <br />
        /// 2 = Off   <br />
        /// </summary>
        public const int TagMacroMode = unchecked(0x001C);

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
        public const int TagRecordMode = unchecked(0x001F);

        /// <summary>
        /// 1 = Yes <br />
        /// 2 = No  <br />
        /// </summary>
        public const int TagAudio = unchecked(0x0020);

        /// <summary>No idea, what this is</summary>
        public const int TagUnknownDataDump = unchecked(0x0021);

        public const int TagEasyMode = unchecked(0x0022);

        public const int TagWhiteBalanceBias = unchecked(0x0023);

        public const int TagFlashBias = unchecked(0x0024);

        /// <summary>
        /// this number is unique, and contains the date of manufacture,
        /// but is not the same as the number printed on the camera body
        /// </summary>
        public const int TagInternalSerialNumber = unchecked(0x0025);

        /// <summary>Panasonic Exif Version</summary>
        public const int TagExifVersion = unchecked(0x0026);

        /// <summary>
        /// 1 = Off           <br />
        /// 2 = Warm          <br />
        /// 3 = Cool          <br />
        /// 4 = Black &amp; White <br />
        /// 5 = Sepia         <br />
        /// </summary>
        public const int TagColorEffect = unchecked(0x0028);

        /// <summary>
        /// 4 Bytes <br />
        /// Time in 1/100 s from when the camera was powered on to when the
        /// image is written to memory card
        /// </summary>
        public const int TagUptime = unchecked(0x0029);

        /// <summary>
        /// 0 = Off        <br />
        /// 1 = On         <br />
        /// 2 = Infinite   <br />
        /// 4 = Unlimited  <br />
        /// </summary>
        public const int TagBurstMode = unchecked(0x002a);

        public const int TagSequenceNumber = unchecked(0x002b);

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
        public const int TagContrastMode = unchecked(0x002c);

        /// <summary>
        /// 0 = Standard      <br />
        /// 1 = Low (-1)      <br />
        /// 2 = High (+1)     <br />
        /// 3 = Lowest (-2)   <br />
        /// 4 = Highest (+2)  <br />
        /// </summary>
        public const int TagNoiseReduction = unchecked(0x002d);

        /// <summary>
        /// 1 = Off   <br />
        /// 2 = 10 s  <br />
        /// 3 = 2 s   <br />
        /// </summary>
        public const int TagSelfTimer = unchecked(0x002e);

        /// <summary>
        /// 1 = 0 DG    <br />
        /// 3 = 180 DG  <br />
        /// 6 =  90 DG  <br />
        /// 8 = 270 DG  <br />
        /// </summary>
        public const int TagRotation = unchecked(0x0030);

        /// <summary>
        /// 1 = Fired <br />
        /// 2 = Enabled nut not used <br />
        /// 3 = Disabled but required <br />
        /// 4 = Disabled and not required
        /// </summary>
        public const int TagAfAssistLamp = unchecked(0x0031);

        /// <summary>
        /// 0 = Normal <br />
        /// 1 = Natural<br />
        /// 2 = Vivid
        /// </summary>
        public const int TagColorMode = unchecked(0x0032);

        public const int TagBabyAge = unchecked(0x0033);

        /// <summary>
        /// 1 = Standard <br />
        /// 2 = Extended
        /// </summary>
        public const int TagOpticalZoomMode = unchecked(0x0034);

        /// <summary>
        /// 1 = Off <br />
        /// 2 = Wide <br />
        /// 3 = Telephoto <br />
        /// 4 = Macro
        /// </summary>
        public const int TagConversionLens = unchecked(0x0035);

        public const int TagTravelDay = unchecked(0x0036);

        /// <summary>0 = Normal</summary>
        public const int TagContrast = unchecked(0x0039);

        /// <summary>
        /// <br />
        /// 1 = Home <br />
        /// 2 = Destination
        /// </summary>
        public const int TagWorldTimeLocation = unchecked(0x003a);

        /// <summary>
        /// 1 = Off   <br />
        /// 2 = On
        /// </summary>
        public const int TagTextStamp = unchecked(0x003b);

        public const int TagProgramIso = unchecked(0x003c);

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
        public const int TagAdvancedSceneMode = unchecked(0x003d);

        /// <summary>
        /// 1 = Off   <br />
        /// 2 = On
        /// </summary>
        public const int TagTextStamp1 = unchecked(0x003e);

        public const int TagFacesDetected = unchecked(0x003f);

        public const int TagSaturation = unchecked(0x0040);

        public const int TagSharpness = unchecked(0x0041);

        public const int TagFilmMode = unchecked(0x0042);

        /// <summary>WB adjust AB.</summary>
        /// <remarks>WB adjust AB. Positive is a shift toward blue.</remarks>
        public const int TagWbAdjustAb = unchecked(0x0046);

        /// <summary>WB adjust GM.</summary>
        /// <remarks>WB adjust GM. Positive is a shift toward green.</remarks>
        public const int TagWbAdjustGm = unchecked(0x0047);

        public const int TagAfPointPosition = unchecked(0x004d);

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
        public const int TagFaceDetectionInfo = unchecked(0x004e);

        public const int TagLensType = unchecked(0x0051);

        public const int TagLensSerialNumber = unchecked(0x0052);

        public const int TagAccessoryType = unchecked(0x0053);

        /// <summary>
        /// (decoded as two 16-bit signed integers)
        /// '-1 1' = Slim Low
        /// '-3 2' = Slim High
        /// '0 0' = Off
        /// '1 1' = Stretch Low
        /// '3 2' = Stretch High
        /// </summary>
        public const int TagTransform = unchecked(0x0059);

        /// <summary>
        /// 0 = Off <br />
        /// 1 = Low <br />
        /// 2 = Standard <br />
        /// 3 = High
        /// </summary>
        public const int TagIntelligentExposure = unchecked(0x005d);

        /// <summary>Info at http://www.ozhiker.com/electronics/pjmt/jpeg_info/pim.html</summary>
        public const int TagPrintImageMatchingInfo = unchecked(0x0E00);

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
        public const int TagFaceRecognitionInfo = unchecked(0x0061);

        /// <summary>
        /// 0 = No <br />
        /// 1 = Yes
        /// </summary>
        public const int TagFlashWarning = unchecked(0x0062);

        public const int TagRecognizedFaceFlags = unchecked(0x0063);

        public const int TagTitle = unchecked(0x0065);

        public const int TagBabyName = unchecked(0x0066);

        public const int TagLocation = unchecked(0x0067);

        public const int TagCountry = unchecked(0x0069);

        public const int TagState = unchecked(0x006b);

        public const int TagCity = unchecked(0x006d);

        public const int TagLandmark = unchecked(0x006f);

        /// <summary>
        /// 0 = Off <br />
        /// 2 = Auto <br />
        /// 3 = On
        /// </summary>
        public const int TagIntelligentResolution = unchecked(0x0070);

        public const int TagMakernoteVersion = unchecked(0x8000);

        public const int TagSceneMode = unchecked(0x8001);

        public const int TagWbRedLevel = unchecked(0x8004);

        public const int TagWbGreenLevel = unchecked(0x8005);

        public const int TagWbBlueLevel = unchecked(0x8006);

        public const int TagFlashFired = unchecked(0x8007);

        public const int TagTextStamp2 = unchecked(0x8008);

        public const int TagTextStamp3 = unchecked(0x8009);

        public const int TagBabyAge1 = unchecked(0x8010);

        /// <summary>
        /// (decoded as two 16-bit signed integers)
        /// '-1 1' = Slim Low
        /// '-3 2' = Slim High
        /// '0 0' = Off
        /// '1 1' = Stretch Low
        /// '3 2' = Stretch High
        /// </summary>
        public const int TagTransform1 = unchecked(0x8012);

        [NotNull] private static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static PanasonicMakernoteDirectory()
        {
            TagNameMap.Put(TagQualityMode, "Quality Mode");
            TagNameMap.Put(TagFirmwareVersion, "Version");
            TagNameMap.Put(TagWhiteBalance, "White Balance");
            TagNameMap.Put(TagFocusMode, "Focus Mode");
            TagNameMap.Put(TagAfAreaMode, "AF Area Mode");
            TagNameMap.Put(TagImageStabilization, "Image Stabilization");
            TagNameMap.Put(TagMacroMode, "Macro Mode");
            TagNameMap.Put(TagRecordMode, "Record Mode");
            TagNameMap.Put(TagAudio, "Audio");
            TagNameMap.Put(TagInternalSerialNumber, "Internal Serial Number");
            TagNameMap.Put(TagUnknownDataDump, "Unknown Data Dump");
            TagNameMap.Put(TagEasyMode, "Easy Mode");
            TagNameMap.Put(TagWhiteBalanceBias, "White Balance Bias");
            TagNameMap.Put(TagFlashBias, "Flash Bias");
            TagNameMap.Put(TagExifVersion, "Exif Version");
            TagNameMap.Put(TagColorEffect, "Color Effect");
            TagNameMap.Put(TagUptime, "Camera Uptime");
            TagNameMap.Put(TagBurstMode, "Burst Mode");
            TagNameMap.Put(TagSequenceNumber, "Sequence Number");
            TagNameMap.Put(TagContrastMode, "Contrast Mode");
            TagNameMap.Put(TagNoiseReduction, "Noise Reduction");
            TagNameMap.Put(TagSelfTimer, "Self Timer");
            TagNameMap.Put(TagRotation, "Rotation");
            TagNameMap.Put(TagAfAssistLamp, "AF Assist Lamp");
            TagNameMap.Put(TagColorMode, "Color Mode");
            TagNameMap.Put(TagBabyAge, "Baby Age");
            TagNameMap.Put(TagOpticalZoomMode, "Optical Zoom Mode");
            TagNameMap.Put(TagConversionLens, "Conversion Lens");
            TagNameMap.Put(TagTravelDay, "Travel Day");
            TagNameMap.Put(TagContrast, "Contrast");
            TagNameMap.Put(TagWorldTimeLocation, "World Time Location");
            TagNameMap.Put(TagTextStamp, "Text Stamp");
            TagNameMap.Put(TagProgramIso, "Program ISO");
            TagNameMap.Put(TagAdvancedSceneMode, "Advanced Scene Mode");
            TagNameMap.Put(TagPrintImageMatchingInfo, "Print Image Matching (PIM) Info");
            TagNameMap.Put(TagFacesDetected, "Number of Detected Faces");
            TagNameMap.Put(TagSaturation, "Saturation");
            TagNameMap.Put(TagSharpness, "Sharpness");
            TagNameMap.Put(TagFilmMode, "Film Mode");
            TagNameMap.Put(TagWbAdjustAb, "White Balance Adjust (AB)");
            TagNameMap.Put(TagWbAdjustGm, "White Balance Adjust (GM)");
            TagNameMap.Put(TagAfPointPosition, "Af Point Position");
            TagNameMap.Put(TagFaceDetectionInfo, "Face Detection Info");
            TagNameMap.Put(TagLensType, "Lens Type");
            TagNameMap.Put(TagLensSerialNumber, "Lens Serial Number");
            TagNameMap.Put(TagAccessoryType, "Accessory Type");
            TagNameMap.Put(TagTransform, "Transform");
            TagNameMap.Put(TagIntelligentExposure, "Intelligent Exposure");
            TagNameMap.Put(TagFaceRecognitionInfo, "Face Recognition Info");
            TagNameMap.Put(TagFlashWarning, "Flash Warning");
            TagNameMap.Put(TagRecognizedFaceFlags, "Recognized Face Flags");
            TagNameMap.Put(TagTitle, "Title");
            TagNameMap.Put(TagBabyName, "Baby Name");
            TagNameMap.Put(TagLocation, "Location");
            TagNameMap.Put(TagCountry, "Country");
            TagNameMap.Put(TagState, "State");
            TagNameMap.Put(TagCity, "City");
            TagNameMap.Put(TagLandmark, "Landmark");
            TagNameMap.Put(TagIntelligentResolution, "Intelligent Resolution");
            TagNameMap.Put(TagMakernoteVersion, "Makernote Version");
            TagNameMap.Put(TagSceneMode, "Scene Mode");
            TagNameMap.Put(TagWbRedLevel, "White Balance (Red)");
            TagNameMap.Put(TagWbGreenLevel, "White Balance (Green)");
            TagNameMap.Put(TagWbBlueLevel, "White Balance (Blue)");
            TagNameMap.Put(TagFlashFired, "Flash Fired");
            TagNameMap.Put(TagTextStamp1, "Text Stamp 1");
            TagNameMap.Put(TagTextStamp2, "Text Stamp 2");
            TagNameMap.Put(TagTextStamp3, "Text Stamp 3");
            TagNameMap.Put(TagBabyAge1, "Baby Age 1");
            TagNameMap.Put(TagTransform1, "Transform 1");
        }

        public PanasonicMakernoteDirectory()
        {
            SetDescriptor(new PanasonicMakernoteDescriptor(this));
        }

        public override string GetName()
        {
            return "Panasonic Makernote";
        }

        protected override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }

        [CanBeNull]
        public Face[] GetDetectedFaces()
        {
            byte[] bytes = GetByteArray(TagFaceDetectionInfo);
            if (bytes == null)
            {
                return null;
            }
            RandomAccessReader reader = new ByteArrayReader(bytes);
            reader.SetMotorolaByteOrder(false);
            try
            {
                int faceCount = reader.GetUInt16(0);
                if (faceCount == 0)
                {
                    return null;
                }
                Face[] faces = new Face[faceCount];
                for (int i = 0; i < faceCount; i++)
                {
                    int offset = 2 + i * 8;
                    faces[i] = new Face(reader.GetUInt16(offset), reader.GetUInt16(offset + 2), reader.GetUInt16(offset + 4), reader.GetUInt16(offset + 6), null, null);
                }
                return faces;
            }
            catch (IOException)
            {
                return null;
            }
        }

        [CanBeNull]
        public Face[] GetRecognizedFaces()
        {
            byte[] bytes = GetByteArray(TagFaceRecognitionInfo);
            if (bytes == null)
            {
                return null;
            }
            RandomAccessReader reader = new ByteArrayReader(bytes);
            reader.SetMotorolaByteOrder(false);
            try
            {
                int faceCount = reader.GetUInt16(0);
                if (faceCount == 0)
                {
                    return null;
                }
                Face[] faces = new Face[faceCount];
                for (int i = 0; i < faceCount; i++)
                {
                    int offset = 4 + i * 44;
                    string name = reader.GetString(offset, 20, "ASCII").Trim();
                    string age = reader.GetString(offset + 28, 20, "ASCII").Trim();
                    faces[i] = new Face(reader.GetUInt16(offset + 20), reader.GetUInt16(offset + 22), reader.GetUInt16(offset + 24), reader.GetUInt16(offset + 26), name, Age.FromPanasonicString(age));
                }
                return faces;
            }
            catch (IOException)
            {
                return null;
            }
        }

        /// <summary>Attempts to convert the underlying string value (as stored in the directory) into an Age object.</summary>
        /// <param name="tag">The tag identifier.</param>
        /// <returns>The parsed Age object, or null if the tag was empty of the value unable to be parsed.</returns>
        [CanBeNull]
        public Age GetAge(int tag)
        {
            string ageString = GetString(tag);
            if (ageString == null)
            {
                return null;
            }
            return Age.FromPanasonicString(ageString);
        }
    }
}
