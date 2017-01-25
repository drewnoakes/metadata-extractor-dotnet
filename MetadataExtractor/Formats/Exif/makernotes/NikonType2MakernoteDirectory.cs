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

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to Nikon (type 2) cameras.</summary>
    /// <remarks>
    /// Type-2 applies to the E990 and D-series cameras such as the E990, D1, D70 and D100.
    /// <para />
    /// Thanks to Fabrizio Giudici for publishing his reverse-engineering of the D100 makernote data.
    /// http://www.timelesswanderings.net/equipment/D100/NEF.html
    /// <para />
    /// Note that the camera implements image protection (locking images) via the file's 'readonly' attribute.  Similarly
    /// image hiding uses the 'hidden' attribute (observed on the D70).  Consequently, these values are not available here.
    /// <para />
    /// Additional sample images have been observed, and their tag values recorded in API documentation for each tag's field.
    /// New tags have subsequently been added since Fabrizio's observations.
    /// <para />
    /// In earlier models (such as the E990 and D1), this directory begins at the first byte of the makernote IFD.  In
    /// later models, the IFD was given the standard prefix to indicate the camera models (most other manufacturers also
    /// provide this prefix to aid in software decoding).
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class NikonType2MakernoteDirectory : Directory
    {
        /// <summary>
        /// Values observed
        /// - 0200 (D70)
        /// - 0200 (D1X)
        /// </summary>
        public const int TagFirmwareVersion = 0x0001;

        /// <summary>
        /// Values observed
        /// - 0 250
        /// - 0 400
        /// </summary>
        public const int TagIso1 = 0x0002;

        /// <summary>The camera's color mode, as an uppercase string.</summary>
        /// <remarks>
        /// The camera's color mode, as an uppercase string.  Examples include:
        /// <list type="bullet">
        /// <item><c>B &amp; W</c></item>
        /// <item><c>COLOR</c></item>
        /// <item><c>COOL</c></item>
        /// <item><c>SEPIA</c></item>
        /// <item><c>VIVID</c></item>
        /// </list>
        /// </remarks>
        public const int TagColorMode = 0x0003;

        /// <summary>The camera's quality setting, as an uppercase string.</summary>
        /// <remarks>
        /// The camera's quality setting, as an uppercase string.  Examples include:
        /// <list type="bullet">
        /// <item><c>BASIC</c></item>
        /// <item><c>FINE</c></item>
        /// <item><c>NORMAL</c></item>
        /// <item><c>RAW</c></item>
        /// <item><c>RAW2.7M</c></item>
        /// </list>
        /// </remarks>
        public const int TagQualityAndFileFormat = 0x0004;

        /// <summary>The camera's white balance setting, as an uppercase string.</summary>
        /// <remarks>
        /// The camera's white balance setting, as an uppercase string.  Examples include:
        /// <list type="bullet">
        /// <item><c>AUTO</c></item>
        /// <item><c>CLOUDY</c></item>
        /// <item><c>FLASH</c></item>
        /// <item><c>FLUORESCENT</c></item>
        /// <item><c>INCANDESCENT</c></item>
        /// <item><c>PRESET</c></item>
        /// <item><c>PRESET0</c></item>
        /// <item><c>PRESET1</c></item>
        /// <item><c>PRESET3</c></item>
        /// <item><c>SUNNY</c></item>
        /// <item><c>WHITE PRESET</c></item>
        /// <item><c>4350K</c></item>
        /// <item><c>5000K</c></item>
        /// <item><c>DAY WHITE FL</c></item>
        /// <item><c>SHADE</c></item>
        /// </list>
        /// </remarks>
        public const int TagCameraWhiteBalance = 0x0005;

        /// <summary>The camera's sharpening setting, as an uppercase string.</summary>
        /// <remarks>
        /// The camera's sharpening setting, as an uppercase string.  Examples include:
        /// <list type="bullet">
        /// <item><c>AUTO</c></item>
        /// <item><c>HIGH</c></item>
        /// <item><c>LOW</c></item>
        /// <item><c>NONE</c></item>
        /// <item><c>NORMAL</c></item>
        /// <item><c>MED.H</c></item>
        /// <item><c>MED.L</c></item>
        /// </list>
        /// </remarks>
        public const int TagCameraSharpening = 0x0006;

        /// <summary>The camera's auto-focus mode, as an uppercase string.</summary>
        /// <remarks>
        /// The camera's auto-focus mode, as an uppercase string.  Examples include:
        /// <list type="bullet">
        /// <item><c>AF-C</c></item>
        /// <item><c>AF-S</c></item>
        /// <item><c>MANUAL</c></item>
        /// <item><c>AF-A</c></item>
        /// </list>
        /// </remarks>
        public const int TagAfType = 0x0007;

        /// <summary>The camera's flash setting, as an uppercase string.</summary>
        /// <remarks>
        /// The camera's flash setting, as an uppercase string.  Examples include:
        /// <list type="bullet">
        /// <item><c></c></item>
        /// <item><c>NORMAL</c></item>
        /// <item><c>RED-EYE</c></item>
        /// <item><c>SLOW</c></item>
        /// <item><c>NEW_TTL</c></item>
        /// <item><c>REAR</c></item>
        /// <item><c>REAR SLOW</c></item>
        /// </list>
        /// Note: when TAG_AUTO_FLASH_MODE is blank (whitespace), Nikon Browser displays "Flash Sync Mode: Not Attached"
        /// </remarks>
        public const int TagFlashSyncMode = 0x0008;

        /// <summary>The type of flash used in the photograph, as a string.</summary>
        /// <remarks>
        /// The type of flash used in the photograph, as a string.  Examples include:
        /// <list type="bullet">
        /// <item><c></c></item>
        /// <item><c>Built-in,TTL</c></item>
        /// <item><c>NEW_TTL</c> Nikon Browser interprets as "D-TTL"</item>
        /// <item><c>Built-in,M</c></item>
        /// <item><c>Optional,TTL</c> with speedlight SB800, flash sync mode as "NORMAL"</item>
        /// </list>
        /// </remarks>
        public const int TagAutoFlashMode = 0x0009;

        /// <summary>An unknown tag, as a rational.</summary>
        /// <remarks>
        /// An unknown tag, as a rational.  Several values given here:
        /// http://gvsoft.homedns.org/exif/makernote-nikon-type2.html#0x000b
        /// </remarks>
        public const int TagUnknown34 = 0x000A;

        /// <summary>The camera's white balance bias setting, as an uint16 array having either one or two elements.</summary>
        /// <remarks>
        /// The camera's white balance bias setting, as an uint16 array having either one or two elements.
        /// <list type="bullet">
        /// <item><c>0</c></item>
        /// <item><c>1</c></item>
        /// <item><c>-3</c></item>
        /// <item><c>-2</c></item>
        /// <item><c>-1</c></item>
        /// <item><c>0,0</c></item>
        /// <item><c>1,0</c></item>
        /// <item><c>5,-5</c></item>
        /// </list>
        /// </remarks>
        public const int TagCameraWhiteBalanceFine = 0x000B;

        /// <summary>
        /// The first two numbers are coefficients to multiply red and blue channels according to white balance as set in the
        /// camera.
        /// </summary>
        /// <remarks>
        /// The first two numbers are coefficients to multiply red and blue channels according to white balance as set in the
        /// camera. The meaning of the third and the fourth numbers is unknown.
        /// Values observed
        /// - 2.25882352 1.76078431 0.0 0.0
        /// - 10242/1 34305/1 0/1 0/1
        /// - 234765625/100000000 1140625/1000000 1/1 1/1
        /// </remarks>
        public const int TagCameraWhiteBalanceRbCoeff = 0x000C;

        /// <summary>The camera's program shift setting, as an array of four integers.</summary>
        /// <remarks>
        /// The camera's program shift setting, as an array of four integers.
        /// The value, in EV, is calculated as <c>a*b/c</c>.
        /// <list type="bullet">
        /// <item><c>0,1,3,0</c> = 0 EV</item>
        /// <item><c>1,1,3,0</c> = 0.33 EV</item>
        /// <item><c>-3,1,3,0</c> = -1 EV</item>
        /// <item><c>1,1,2,0</c> = 0.5 EV</item>
        /// <item><c>2,1,6,0</c> = 0.33 EV</item>
        /// </list>
        /// </remarks>
        public const int TagProgramShift = 0x000D;

        /// <summary>The exposure difference, as an array of four integers.</summary>
        /// <remarks>
        /// The exposure difference, as an array of four integers.
        /// The value, in EV, is calculated as <c>a*b/c</c>.
        /// <list type="bullet">
        /// <item><c>-105,1,12,0</c> = -8.75 EV</item>
        /// <item><c>-72,1,12,0</c> = -6.00 EV</item>
        /// <item><c>-11,1,12,0</c> = -0.92 EV</item>
        /// </list>
        /// </remarks>
        public const int TagExposureDifference = 0x000E;

        /// <summary>The camera's ISO mode, as an uppercase string.</summary>
        /// <remarks>
        /// The camera's ISO mode, as an uppercase string.
        /// <list type="bullet">
        /// <item><c>AUTO</c></item>
        /// <item><c>MANUAL</c></item>
        /// </list>
        /// </remarks>
        public const int TagIsoMode = 0x000F;

        /// <summary>Added during merge of Type2 &amp; Type3.</summary>
        /// <remarks>Added during merge of Type2 &amp; Type3.  May apply to earlier models, such as E990 and D1.</remarks>
        public const int TagDataDump = 0x0010;

        /// <summary>
        /// Preview to another IFD (?)
        /// <para />
        /// Details here: http://gvsoft.homedns.org/exif/makernote-nikon-2-tag0x0011.html
        /// // TODO if this is another IFD, decode it
        /// </summary>
        public const int TagPreviewIfd = 0x0011;

        /// <summary>The flash compensation, as an array of four integers.</summary>
        /// <remarks>
        /// The flash compensation, as an array of four integers.
        /// The value, in EV, is calculated as <c>a*b/c</c>.
        /// <list type="bullet">
        /// <item><c>-18,1,6,0</c> = -3 EV</item>
        /// <item><c>4,1,6,0</c> = 0.67 EV</item>
        /// <item><c>6,1,6,0</c> = 1 EV</item>
        /// </list>
        /// </remarks>
        public const int TagAutoFlashCompensation = 0x0012;

        /// <summary>The requested ISO value, as an array of two integers.</summary>
        /// <remarks>
        /// The requested ISO value, as an array of two integers.
        /// <list type="bullet">
        /// <item><c>0,0</c></item>
        /// <item><c>0,125</c></item>
        /// <item><c>1,2500</c></item>
        /// </list>
        /// </remarks>
        public const int TagIsoRequested = 0x0013;

        /// <summary>Defines the photo corner coordinates, in 8 bytes.</summary>
        /// <remarks>
        /// Defines the photo corner coordinates, in 8 bytes.  Treated as four 16-bit integers, they
        /// decode as: top-left (x,y); bot-right (x,y)
        /// - 0 0 49163 53255
        /// - 0 0 3008 2000 (the image dimensions were 3008x2000) (D70)
        /// <list type="bullet">
        /// <item><c>0,0,4288,2848</c> The max resolution of the D300 camera</item>
        /// <item><c>0,0,3008,2000</c> The max resolution of the D70 camera</item>
        /// <item><c>0,0,4256,2832</c> The max resolution of the D3 camera</item>
        /// </list>
        /// </remarks>
        public const int TagImageBoundary = 0x0016;

        /// <summary>The flash exposure compensation, as an array of four integers.</summary>
        /// <remarks>
        /// The flash exposure compensation, as an array of four integers.
        /// The value, in EV, is calculated as <c>a*b/c</c>.
        /// <list type="bullet">
        /// <item><c>0,0,0,0</c> = 0 EV</item>
        /// <item><c>0,1,6,0</c> = 0 EV</item>
        /// <item><c>4,1,6,0</c> = 0.67 EV</item>
        /// </list>
        /// </remarks>
        public const int TagFlashExposureCompensation = 0x0017;

        /// <summary>The flash bracket compensation, as an array of four integers.</summary>
        /// <remarks>
        /// The flash bracket compensation, as an array of four integers.
        /// The value, in EV, is calculated as <c>a*b/c</c>.
        /// <list type="bullet">
        /// <item><c>0,0,0,0</c> = 0 EV</item>
        /// <item><c>0,1,6,0</c> = 0 EV</item>
        /// <item><c>4,1,6,0</c> = 0.67 EV</item>
        /// </list>
        /// </remarks>
        public const int TagFlashBracketCompensation = 0x0018;

        /// <summary>The AE bracket compensation, as a rational number.</summary>
        /// <remarks>
        /// The AE bracket compensation, as a rational number.
        /// <list type="bullet">
        /// <item><c>0/0</c></item>
        /// <item><c>0/1</c></item>
        /// <item><c>0/6</c></item>
        /// <item><c>4/6</c></item>
        /// <item><c>6/6</c></item>
        /// </list>
        /// </remarks>
        public const int TagAeBracketCompensation = 0x0019;

        /// <summary>Flash mode, as a string.</summary>
        /// <remarks>
        /// Flash mode, as a string.
        /// <list type="bullet">
        /// <item><c></c></item>
        /// <item><c>Red Eye Reduction</c></item>
        /// <item><c>D-Lighting</c></item>
        /// <item><c>Distortion control</c></item>
        /// </list>
        /// </remarks>
        public const int TagFlashMode = 0x001a;

        public const int TagCropHighSpeed = 0x001b;

        public const int TagExposureTuning = 0x001c;

        /// <summary>The camera's serial number, as a string.</summary>
        /// <remarks>
        /// The camera's serial number, as a string.
        /// Note that D200 is always blank, and D50 is always <c>"D50"</c>.
        /// </remarks>
        public const int TagCameraSerialNumber = 0x001d;

        /// <summary>The camera's color space setting.</summary>
        /// <remarks>
        /// The camera's color space setting.
        /// <list type="bullet">
        /// <item><c>1</c> sRGB</item>
        /// <item><c>2</c> Adobe RGB</item>
        /// </list>
        /// </remarks>
        public const int TagColorSpace = 0x001e;

        public const int TagVrInfo = 0x001f;

        public const int TagImageAuthentication = 0x0020;

        public const int TagUnknown35 = 0x0021;

        /// <summary>The active D-Lighting setting.</summary>
        /// <remarks>
        /// The active D-Lighting setting.
        /// <list type="bullet">
        /// <item><c>0</c> Off</item>
        /// <item><c>1</c> Low</item>
        /// <item><c>3</c> Normal</item>
        /// <item><c>5</c> High</item>
        /// <item><c>7</c> Extra High</item>
        /// <item><c>65535</c> Auto</item>
        /// </list>
        /// </remarks>
        public const int TagActiveDLighting = 0x0022;

        public const int TagPictureControl = 0x0023;

        public const int TagWorldTime = 0x0024;

        public const int TagIsoInfo = 0x0025;

        public const int TagUnknown36 = 0x0026;

        public const int TagUnknown37 = 0x0027;

        public const int TagUnknown38 = 0x0028;

        public const int TagUnknown39 = 0x0029;

        /// <summary>The camera's vignette control setting.</summary>
        /// <remarks>
        /// The camera's vignette control setting.
        /// <list type="bullet">
        /// <item><c>0</c> Off</item>
        /// <item><c>1</c> Low</item>
        /// <item><c>3</c> Normal</item>
        /// <item><c>5</c> High</item>
        /// </list>
        /// </remarks>
        public const int TagVignetteControl = 0x002a;

        public const int TagUnknown40 = 0x002b;

        public const int TagUnknown41 = 0x002c;

        public const int TagUnknown42 = 0x002d;

        public const int TagUnknown43 = 0x002e;

        public const int TagUnknown44 = 0x002f;

        public const int TagUnknown45 = 0x0030;

        public const int TagUnknown46 = 0x0031;

        /// <summary>The camera's image adjustment setting, as a string.</summary>
        /// <remarks>
        /// The camera's image adjustment setting, as a string.
        /// <list type="bullet">
        /// <item><c>AUTO</c></item>
        /// <item><c>CONTRAST(+)</c></item>
        /// <item><c>CONTRAST(-)</c></item>
        /// <item><c>NORMAL</c></item>
        /// <item><c>B &amp; W</c></item>
        /// <item><c>BRIGHTNESS(+)</c></item>
        /// <item><c>BRIGHTNESS(-)</c></item>
        /// <item><c>SEPIA</c></item>
        /// </list>
        /// </remarks>
        public const int TagImageAdjustment = 0x0080;

        /// <summary>The camera's tone compensation setting, as a string.</summary>
        /// <remarks>
        /// The camera's tone compensation setting, as a string.
        /// <list type="bullet">
        /// <item><c>NORMAL</c></item>
        /// <item><c>LOW</c></item>
        /// <item><c>MED.L</c></item>
        /// <item><c>MED.H</c></item>
        /// <item><c>HIGH</c></item>
        /// <item><c>AUTO</c></item>
        /// </list>
        /// </remarks>
        public const int TagCameraToneCompensation = 0x0081;

        /// <summary>A description of any auxiliary lens, as a string.</summary>
        /// <remarks>
        /// A description of any auxiliary lens, as a string.
        /// <list type="bullet">
        /// <item><c>OFF</c></item>
        /// <item><c>FISHEYE 1</c></item>
        /// <item><c>FISHEYE 2</c></item>
        /// <item><c>TELEPHOTO 2</c></item>
        /// <item><c>WIDE ADAPTER</c></item>
        /// </list>
        /// </remarks>
        public const int TagAdapter = 0x0082;

        /// <summary>The type of lens used, as a byte.</summary>
        /// <remarks>
        /// The type of lens used, as a byte.
        /// <list type="bullet">
        /// <item><c>0x00</c> AF</item>
        /// <item><c>0x01</c> MF</item>
        /// <item><c>0x02</c> D</item>
        /// <item><c>0x06</c> G, D</item>
        /// <item><c>0x08</c> VR</item>
        /// <item><c>0x0a</c> VR, D</item>
        /// <item><c>0x0e</c> VR, G, D</item>
        /// </list>
        /// </remarks>
        public const int TagLensType = 0x0083;

        /// <summary>A pair of focal/max-fstop values that describe the lens used.</summary>
        /// <remarks>
        /// A pair of focal/max-fstop values that describe the lens used.
        /// Values observed
        /// - 180.0,180.0,2.8,2.8 (D100)
        /// - 240/10 850/10 35/10 45/10
        /// - 18-70mm f/3.5-4.5 (D70)
        /// - 17-35mm f/2.8-2.8 (D1X)
        /// - 70-200mm f/2.8-2.8 (D70)
        /// Nikon Browser identifies the lens as "18-70mm F/3.5-4.5 G" which
        /// is identical to metadata extractor, except for the "G".  This must
        /// be coming from another tag...
        /// </remarks>
        public const int TagLens = 0x0084;

        /// <summary>Added during merge of Type2 &amp; Type3.</summary>
        /// <remarks>Added during merge of Type2 &amp; Type3.  May apply to earlier models, such as E990 and D1.</remarks>
        public const int TagManualFocusDistance = 0x0085;

        /// <summary>The amount of digital zoom used.</summary>
        public const int TagDigitalZoom = 0x0086;

        /// <summary>Whether the flash was used in this image.</summary>
        /// <remarks>
        /// Whether the flash was used in this image.
        /// <list type="bullet">
        /// <item><c>0</c> Flash Not Used</item>
        /// <item><c>1</c> Manual Flash</item>
        /// <item><c>3</c> Flash Not Ready</item>
        /// <item><c>7</c> External Flash</item>
        /// <item><c>8</c> Fired, Commander Mode</item>
        /// <item><c>9</c> Fired, TTL Mode</item>
        /// </list>
        /// </remarks>
        public const int TagFlashUsed = 0x0087;

        /// <summary>The position of the autofocus target.</summary>
        public const int TagAfFocusPosition = 0x0088;

        /// <summary>The camera's shooting mode.</summary>
        /// <remarks>
        /// The camera's shooting mode.
        /// <para />
        /// A bit-array with:
        /// <list type="bullet">
        /// <item><c>0</c> Single Frame</item>
        /// <item><c>1</c> Continuous</item>
        /// <item><c>2</c> Delay</item>
        /// <item><c>8</c> PC Control</item>
        /// <item><c>16</c> Exposure Bracketing</item>
        /// <item><c>32</c> Auto ISO</item>
        /// <item><c>64</c> White-Balance Bracketing</item>
        /// <item><c>128</c> IR Control</item>
        /// </list>
        /// </remarks>
        public const int TagShootingMode = 0x0089;

        public const int TagUnknown20 = 0x008A;

        /// <summary>Lens stops, as an array of four integers.</summary>
        /// <remarks>
        /// Lens stops, as an array of four integers.
        /// The value, in EV, is calculated as <c>a*b/c</c>.
        /// <list type="bullet">
        /// <item><c>64,1,12,0</c> = 5.33 EV</item>
        /// <item><c>72,1,12,0</c> = 6 EV</item>
        /// </list>
        /// </remarks>
        public const int TagLensStops = 0x008B;

        public const int TagContrastCurve = 0x008C;

        /// <summary>The color space as set in the camera, as a string.</summary>
        /// <remarks>
        /// The color space as set in the camera, as a string.
        /// <list type="bullet">
        /// <item><c>MODE1</c> = Mode 1 (sRGB)</item>
        /// <item><c>MODE1a</c> = Mode 1 (sRGB)</item>
        /// <item><c>MODE2</c> = Mode 2 (Adobe RGB)</item>
        /// <item><c>MODE3</c> = Mode 2 (sRGB): Higher Saturation</item>
        /// <item><c>MODE3a</c> = Mode 2 (sRGB): Higher Saturation</item>
        /// <item><c>B &amp; W</c> = B &amp; W</item>
        /// </list>
        /// </remarks>
        public const int TagCameraColorMode = 0x008D;

        public const int TagUnknown47 = 0x008E;

        /// <summary>The camera's scene mode, as a string.</summary>
        /// <remarks>
        /// The camera's scene mode, as a string.  Examples include:
        /// <list type="bullet">
        /// <item><c>BEACH/SNOW</c></item>
        /// <item><c>CLOSE UP</c></item>
        /// <item><c>NIGHT PORTRAIT</c></item>
        /// <item><c>PORTRAIT</c></item>
        /// <item><c>ANTI-SHAKE</c></item>
        /// <item><c>BACK LIGHT</c></item>
        /// <item><c>BEST FACE</c></item>
        /// <item><c>BEST</c></item>
        /// <item><c>COPY</c></item>
        /// <item><c>DAWN/DUSK</c></item>
        /// <item><c>FACE-PRIORITY</c></item>
        /// <item><c>FIREWORKS</c></item>
        /// <item><c>FOOD</c></item>
        /// <item><c>HIGH SENS.</c></item>
        /// <item><c>LAND SCAPE</c></item>
        /// <item><c>MUSEUM</c></item>
        /// <item><c>PANORAMA ASSIST</c></item>
        /// <item><c>PARTY/INDOOR</c></item>
        /// <item><c>SCENE AUTO</c></item>
        /// <item><c>SMILE</c></item>
        /// <item><c>SPORT</c></item>
        /// <item><c>SPORT CONT.</c></item>
        /// <item><c>SUNSET</c></item>
        /// </list>
        /// </remarks>
        public const int TagSceneMode = 0x008F;

        /// <summary>The lighting type, as a string.</summary>
        /// <remarks>
        /// The lighting type, as a string.  Examples include:
        /// <list type="bullet">
        /// <item><c></c></item>
        /// <item><c>NATURAL</c></item>
        /// <item><c>SPEEDLIGHT</c></item>
        /// <item><c>COLORED</c></item>
        /// <item><c>MIXED</c></item>
        /// <item><c>NORMAL</c></item>
        /// </list>
        /// </remarks>
        public const int TagLightSource = 0x0090;

        /// <summary>Advertised as ASCII, but actually isn't.</summary>
        /// <remarks>
        /// Advertised as ASCII, but actually isn't.  A variable number of bytes (eg. 18 to 533).  Actual number of bytes
        /// appears fixed for a given camera model.
        /// </remarks>
        public const int TagShotInfo = 0x0091;

        /// <summary>The hue adjustment as set in the camera.</summary>
        /// <remarks>The hue adjustment as set in the camera.  Values observed are either 0 or 3.</remarks>
        public const int TagCameraHueAdjustment = 0x0092;

        /// <summary>The NEF (RAW) compression.</summary>
        /// <remarks>
        /// The NEF (RAW) compression.  Examples include:
        /// <list type="bullet">
        /// <item><c>1</c> Lossy (Type 1)</item>
        /// <item><c>2</c> Uncompressed</item>
        /// <item><c>3</c> Lossless</item>
        /// <item><c>4</c> Lossy (Type 2)</item>
        /// </list>
        /// </remarks>
        public const int TagNefCompression = 0x0093;

        /// <summary>The saturation level, as a signed integer.</summary>
        /// <remarks>
        /// The saturation level, as a signed integer.  Examples include:
        /// <list type="bullet">
        /// <item><c>+3</c></item>
        /// <item><c>+2</c></item>
        /// <item><c>+1</c></item>
        /// <item><c>0</c> Normal</item>
        /// <item><c>-1</c></item>
        /// <item><c>-2</c></item>
        /// <item><c>-3</c> (B&amp;W)</item>
        /// </list>
        /// </remarks>
        public const int TagSaturation = 0x0094;

        /// <summary>The type of noise reduction, as a string.</summary>
        /// <remarks>
        /// The type of noise reduction, as a string.  Examples include:
        /// <list type="bullet">
        /// <item><c>OFF</c></item>
        /// <item><c>FPNR</c></item>
        /// </list>
        /// </remarks>
        public const int TagNoiseReduction = 0x0095;

        public const int TagLinearizationTable = 0x0096;

        public const int TagColorBalance = 0x0097;

        public const int TagLensData = 0x0098;

        /// <summary>The NEF (RAW) thumbnail size, as an integer array with two items representing [width,height].</summary>
        public const int TagNefThumbnailSize = 0x0099;

        /// <summary>The sensor pixel size, as a pair of rational numbers.</summary>
        public const int TagSensorPixelSize = 0x009A;

        public const int TagUnknown10 = 0x009B;

        public const int TagSceneAssist = 0x009C;

        public const int TagUnknown11 = 0x009D;

        public const int TagRetouchHistory = 0x009E;

        public const int TagUnknown12 = 0x009F;

        /// <summary>The camera serial number, as a string.</summary>
        /// <remarks>
        /// The camera serial number, as a string.
        /// <list type="bullet">
        /// <item><c>NO= 00002539</c></item>
        /// <item><c>NO= -1000d71</c></item>
        /// <item><c>PKG597230621263</c></item>
        /// <item><c>PKG5995671330625116</c></item>
        /// <item><c>PKG49981281631130677</c></item>
        /// <item><c>BU672230725063</c></item>
        /// <item><c>NO= 200332c7</c></item>
        /// <item><c>NO= 30045efe</c></item>
        /// </list>
        /// </remarks>
        public const int TagCameraSerialNumber2 = 0x00A0;

        public const int TagImageDataSize = 0x00A2;

        public const int TagUnknown27 = 0x00A3;

        public const int TagUnknown28 = 0x00A4;

        public const int TagImageCount = 0x00A5;

        public const int TagDeletedImageCount = 0x00A6;

        /// <summary>The number of total shutter releases.</summary>
        /// <remarks>The number of total shutter releases.  This value increments for each exposure (observed on D70).</remarks>
        public const int TagExposureSequenceNumber = 0x00A7;

        public const int TagFlashInfo = 0x00A8;

        /// <summary>The camera's image optimisation, as a string.</summary>
        /// <remarks>
        /// The camera's image optimisation, as a string.
        /// <list type="bullet">
        /// <item><c></c></item>
        /// <item><c>NORMAL</c></item>
        /// <item><c>CUSTOM</c></item>
        /// <item><c>BLACK AND WHITE</c></item>
        /// <item><c>LAND SCAPE</c></item>
        /// <item><c>MORE VIVID</c></item>
        /// <item><c>PORTRAIT</c></item>
        /// <item><c>SOFT</c></item>
        /// <item><c>VIVID</c></item>
        /// </list>
        /// </remarks>
        public const int TagImageOptimisation = 0x00A9;

        /// <summary>The camera's saturation level, as a string.</summary>
        /// <remarks>
        /// The camera's saturation level, as a string.
        /// <list type="bullet">
        /// <item><c></c></item>
        /// <item><c>NORMAL</c></item>
        /// <item><c>AUTO</c></item>
        /// <item><c>ENHANCED</c></item>
        /// <item><c>MODERATE</c></item>
        /// </list>
        /// </remarks>
        public const int TagSaturation2 = 0x00AA;

        /// <summary>The camera's digital vari-program setting, as a string.</summary>
        /// <remarks>
        /// The camera's digital vari-program setting, as a string.
        /// <list type="bullet">
        /// <item><c></c></item>
        /// <item><c>AUTO</c></item>
        /// <item><c>AUTO(FLASH OFF)</c></item>
        /// <item><c>CLOSE UP</c></item>
        /// <item><c>LANDSCAPE</c></item>
        /// <item><c>NIGHT PORTRAIT</c></item>
        /// <item><c>PORTRAIT</c></item>
        /// <item><c>SPORT</c></item>
        /// </list>
        /// </remarks>
        public const int TagDigitalVariProgram = 0x00AB;

        /// <summary>The camera's digital vari-program setting, as a string.</summary>
        /// <remarks>
        /// The camera's digital vari-program setting, as a string.
        /// <list type="bullet">
        /// <item><c></c></item>
        /// <item><c>VR-ON</c></item>
        /// <item><c>VR-OFF</c></item>
        /// <item><c>VR-HYBRID</c></item>
        /// <item><c>VR-ACTIVE</c></item>
        /// </list>
        /// </remarks>
        public const int TagImageStabilisation = 0x00AC;

        /// <summary>The camera's digital vari-program setting, as a string.</summary>
        /// <remarks>
        /// The camera's digital vari-program setting, as a string.
        /// <list type="bullet">
        /// <item><c></c></item>
        /// <item><c>HYBRID</c></item>
        /// <item><c>STANDARD</c></item>
        /// </list>
        /// </remarks>
        public const int TagAfResponse = 0x00AD;

        public const int TagUnknown29 = 0x00AE;

        public const int TagUnknown30 = 0x00AF;

        public const int TagMultiExposure = 0x00B0;

        /// <summary>The camera's high ISO noise reduction setting, as an integer.</summary>
        /// <remarks>
        /// The camera's high ISO noise reduction setting, as an integer.
        /// <list type="bullet">
        /// <item><c>0</c> Off</item>
        /// <item><c>1</c> Minimal</item>
        /// <item><c>2</c> Low</item>
        /// <item><c>4</c> Normal</item>
        /// <item><c>6</c> High</item>
        /// </list>
        /// </remarks>
        public const int TagHighIsoNoiseReduction = 0x00B1;

        public const int TagUnknown31 = 0x00B2;

        public const int TagUnknown32 = 0x00B3;

        public const int TagUnknown33 = 0x00B4;

        public const int TagUnknown48 = 0x00B5;

        public const int TagPowerUpTime = 0x00B6;

        public const int TagAfInfo2 = 0x00B7;

        public const int TagFileInfo = 0x00B8;

        public const int TagAfTune = 0x00B9;

        public const int TagUnknown49 = 0x00BB;

        public const int TagUnknown50 = 0x00BD;

        public const int TagUnknown51 = 0x0103;

        public const int TagPrintImageMatchingInfo = 0x0E00;

        /// <summary>Data about changes set by Nikon Capture Editor.</summary>
        /// <remarks>
        /// Data about changes set by Nikon Capture Editor.
        /// Values observed
        /// </remarks>
        public const int TagNikonCaptureData = 0x0E01;

        public const int TagUnknown52 = 0x0E05;

        public const int TagUnknown53 = 0x0E08;

        public const int TagNikonCaptureVersion = 0x0E09;

        public const int TagNikonCaptureOffsets = 0x0E0E;

        public const int TagNikonScan = 0x0E10;

        public const int TagUnknown54 = 0x0E19;

        public const int TagNefBitDepth = 0x0E22;

        public const int TagUnknown55 = 0x0E23;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagFirmwareVersion, "Firmware Version" },
            { TagIso1, "ISO" },
            { TagQualityAndFileFormat, "Quality & File Format" },
            { TagCameraWhiteBalance, "White Balance" },
            { TagCameraSharpening, "Sharpening" },
            { TagAfType, "AF Type" },
            { TagCameraWhiteBalanceFine, "White Balance Fine" },
            { TagCameraWhiteBalanceRbCoeff, "White Balance RB Coefficients" },
            { TagIsoRequested, "ISO" },
            { TagIsoMode, "ISO Mode" },
            { TagDataDump, "Data Dump" },
            { TagProgramShift, "Program Shift" },
            { TagExposureDifference, "Exposure Difference" },
            { TagPreviewIfd, "Preview IFD" },
            { TagLensType, "Lens Type" },
            { TagFlashUsed, "Flash Used" },
            { TagAfFocusPosition, "AF Focus Position" },
            { TagShootingMode, "Shooting Mode" },
            { TagLensStops, "Lens Stops" },
            { TagContrastCurve, "Contrast Curve" },
            { TagLightSource, "Light source" },
            { TagShotInfo, "Shot Info" },
            { TagColorBalance, "Color Balance" },
            { TagLensData, "Lens Data" },
            { TagNefThumbnailSize, "NEF Thumbnail Size" },
            { TagSensorPixelSize, "Sensor Pixel Size" },
            { TagUnknown10, "Unknown 10" },
            { TagSceneAssist, "Scene Assist" },
            { TagUnknown11, "Unknown 11" },
            { TagRetouchHistory, "Retouch History" },
            { TagUnknown12, "Unknown 12" },
            { TagFlashSyncMode, "Flash Sync Mode" },
            { TagAutoFlashMode, "Auto Flash Mode" },
            { TagAutoFlashCompensation, "Auto Flash Compensation" },
            { TagExposureSequenceNumber, "Exposure Sequence Number" },
            { TagColorMode, "Color Mode" },
            { TagUnknown20, "Unknown 20" },
            { TagImageBoundary, "Image Boundary" },
            { TagFlashExposureCompensation, "Flash Exposure Compensation" },
            { TagFlashBracketCompensation, "Flash Bracket Compensation" },
            { TagAeBracketCompensation, "AE Bracket Compensation" },
            { TagFlashMode, "Flash Mode" },
            { TagCropHighSpeed, "Crop High Speed" },
            { TagExposureTuning, "Exposure Tuning" },
            { TagCameraSerialNumber, "Camera Serial Number" },
            { TagColorSpace, "Color Space" },
            { TagVrInfo, "VR Info" },
            { TagImageAuthentication, "Image Authentication" },
            { TagUnknown35, "Unknown 35" },
            { TagActiveDLighting, "Active D-Lighting" },
            { TagPictureControl, "Picture Control" },
            { TagWorldTime, "World Time" },
            { TagIsoInfo, "ISO Info" },
            { TagUnknown36, "Unknown 36" },
            { TagUnknown37, "Unknown 37" },
            { TagUnknown38, "Unknown 38" },
            { TagUnknown39, "Unknown 39" },
            { TagVignetteControl, "Vignette Control" },
            { TagUnknown40, "Unknown 40" },
            { TagUnknown41, "Unknown 41" },
            { TagUnknown42, "Unknown 42" },
            { TagUnknown43, "Unknown 43" },
            { TagUnknown44, "Unknown 44" },
            { TagUnknown45, "Unknown 45" },
            { TagUnknown46, "Unknown 46" },
            { TagUnknown47, "Unknown 47" },
            { TagSceneMode, "Scene Mode" },
            { TagCameraSerialNumber2, "Camera Serial Number" },
            { TagImageDataSize, "Image Data Size" },
            { TagUnknown27, "Unknown 27" },
            { TagUnknown28, "Unknown 28" },
            { TagImageCount, "Image Count" },
            { TagDeletedImageCount, "Deleted Image Count" },
            { TagSaturation2, "Saturation" },
            { TagDigitalVariProgram, "Digital Vari Program" },
            { TagImageStabilisation, "Image Stabilisation" },
            { TagAfResponse, "AF Response" },
            { TagUnknown29, "Unknown 29" },
            { TagUnknown30, "Unknown 30" },
            { TagMultiExposure, "Multi Exposure" },
            { TagHighIsoNoiseReduction, "High ISO Noise Reduction" },
            { TagUnknown31, "Unknown 31" },
            { TagUnknown32, "Unknown 32" },
            { TagUnknown33, "Unknown 33" },
            { TagUnknown48, "Unknown 48" },
            { TagPowerUpTime, "Power Up Time" },
            { TagAfInfo2, "AF Info 2" },
            { TagFileInfo, "File Info" },
            { TagAfTune, "AF Tune" },
            { TagFlashInfo, "Flash Info" },
            { TagImageOptimisation, "Image Optimisation" },
            { TagImageAdjustment, "Image Adjustment" },
            { TagCameraToneCompensation, "Tone Compensation" },
            { TagAdapter, "Adapter" },
            { TagLens, "Lens" },
            { TagManualFocusDistance, "Manual Focus Distance" },
            { TagDigitalZoom, "Digital Zoom" },
            { TagCameraColorMode, "Colour Mode" },
            { TagCameraHueAdjustment, "Camera Hue Adjustment" },
            { TagNefCompression, "NEF Compression" },
            { TagSaturation, "Saturation" },
            { TagNoiseReduction, "Noise Reduction" },
            { TagLinearizationTable, "Linearization Table" },
            { TagNikonCaptureData, "Nikon Capture Data" },
            { TagUnknown49, "Unknown 49" },
            { TagUnknown50, "Unknown 50" },
            { TagUnknown51, "Unknown 51" },
            { TagPrintImageMatchingInfo, "Print Image Matching (PIM) Info" },
            { TagUnknown52, "Unknown 52" },
            { TagUnknown53, "Unknown 53" },
            { TagNikonCaptureVersion, "Nikon Capture Version" },
            { TagNikonCaptureOffsets, "Nikon Capture Offsets" },
            { TagNikonScan, "Nikon Scan" },
            { TagUnknown54, "Unknown 54" },
            { TagNefBitDepth, "NEF Bit Depth" },
            { TagUnknown55, "Unknown 55" }
        };

        public NikonType2MakernoteDirectory()
        {
            SetDescriptor(new NikonType2MakernoteDescriptor(this));
        }

        public override string Name => "Nikon Makernote";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
