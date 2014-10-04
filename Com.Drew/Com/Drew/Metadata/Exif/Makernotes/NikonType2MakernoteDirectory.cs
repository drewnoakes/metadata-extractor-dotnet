/*
 * Copyright 2002-2013 Drew Noakes
 *
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
 *    http://drewnoakes.com/code/exif/
 *    http://code.google.com/p/metadata-extractor/
 */
using System.Collections.Generic;
using Com.Drew.Metadata.Exif.Makernotes;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif.Makernotes
{
	/// <summary>Describes tags specific to Nikon (type 2) cameras.</summary>
	/// <remarks>
	/// Describes tags specific to Nikon (type 2) cameras.  Type-2 applies to the E990 and D-series cameras such as the E990, D1,
	/// D70 and D100.
	/// <p/>
	/// Thanks to Fabrizio Giudici for publishing his reverse-engineering of the D100 makernote data.
	/// http://www.timelesswanderings.net/equipment/D100/NEF.html
	/// <p/>
	/// Note that the camera implements image protection (locking images) via the file's 'readonly' attribute.  Similarly
	/// image hiding uses the 'hidden' attribute (observed on the D70).  Consequently, these values are not available here.
	/// <p/>
	/// Additional sample images have been observed, and their tag values recorded in javadoc comments for each tag's field.
	/// New tags have subsequently been added since Fabrizio's observations.
	/// <p/>
	/// In earlier models (such as the E990 and D1), this directory begins at the first byte of the makernote IFD.  In
	/// later models, the IFD was given the standard prefix to indicate the camera models (most other manufacturers also
	/// provide this prefix to aid in software decoding).
	/// </remarks>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class NikonType2MakernoteDirectory : Com.Drew.Metadata.Directory
	{
		/// <summary>
		/// Values observed
		/// - 0200 (D70)
		/// - 0200 (D1X)
		/// </summary>
		public const int TagFirmwareVersion = unchecked((int)(0x0001));

		/// <summary>
		/// Values observed
		/// - 0 250
		/// - 0 400
		/// </summary>
		public const int TagIso1 = unchecked((int)(0x0002));

		/// <summary>The camera's color mode, as an uppercase string.</summary>
		/// <remarks>
		/// The camera's color mode, as an uppercase string.  Examples include:
		/// <ul>
		/// <li><code>B & W</code></li>
		/// <li><code>COLOR</code></li>
		/// <li><code>COOL</code></li>
		/// <li><code>SEPIA</code></li>
		/// <li><code>VIVID</code></li>
		/// </ul>
		/// </remarks>
		public const int TagColorMode = unchecked((int)(0x0003));

		/// <summary>The camera's quality setting, as an uppercase string.</summary>
		/// <remarks>
		/// The camera's quality setting, as an uppercase string.  Examples include:
		/// <ul>
		/// <li><code>BASIC</code></li>
		/// <li><code>FINE</code></li>
		/// <li><code>NORMAL</code></li>
		/// <li><code>RAW</code></li>
		/// <li><code>RAW2.7M</code></li>
		/// </ul>
		/// </remarks>
		public const int TagQualityAndFileFormat = unchecked((int)(0x0004));

		/// <summary>The camera's white balance setting, as an uppercase string.</summary>
		/// <remarks>
		/// The camera's white balance setting, as an uppercase string.  Examples include:
		/// <ul>
		/// <li><code>AUTO</code></li>
		/// <li><code>CLOUDY</code></li>
		/// <li><code>FLASH</code></li>
		/// <li><code>FLUORESCENT</code></li>
		/// <li><code>INCANDESCENT</code></li>
		/// <li><code>PRESET</code></li>
		/// <li><code>PRESET0</code></li>
		/// <li><code>PRESET1</code></li>
		/// <li><code>PRESET3</code></li>
		/// <li><code>SUNNY</code></li>
		/// <li><code>WHITE PRESET</code></li>
		/// <li><code>4350K</code></li>
		/// <li><code>5000K</code></li>
		/// <li><code>DAY WHITE FL</code></li>
		/// <li><code>SHADE</code></li>
		/// </ul>
		/// </remarks>
		public const int TagCameraWhiteBalance = unchecked((int)(0x0005));

		/// <summary>The camera's sharpening setting, as an uppercase string.</summary>
		/// <remarks>
		/// The camera's sharpening setting, as an uppercase string.  Examples include:
		/// <ul>
		/// <li><code>AUTO</code></li>
		/// <li><code>HIGH</code></li>
		/// <li><code>LOW</code></li>
		/// <li><code>NONE</code></li>
		/// <li><code>NORMAL</code></li>
		/// <li><code>MED.H</code></li>
		/// <li><code>MED.L</code></li>
		/// </ul>
		/// </remarks>
		public const int TagCameraSharpening = unchecked((int)(0x0006));

		/// <summary>The camera's auto-focus mode, as an uppercase string.</summary>
		/// <remarks>
		/// The camera's auto-focus mode, as an uppercase string.  Examples include:
		/// <ul>
		/// <li><code>AF-C</code></li>
		/// <li><code>AF-S</code></li>
		/// <li><code>MANUAL</code></li>
		/// <li><code>AF-A</code></li>
		/// </ul>
		/// </remarks>
		public const int TagAfType = unchecked((int)(0x0007));

		/// <summary>The camera's flash setting, as an uppercase string.</summary>
		/// <remarks>
		/// The camera's flash setting, as an uppercase string.  Examples include:
		/// <ul>
		/// <li><code></code></li>
		/// <li><code>NORMAL</code></li>
		/// <li><code>RED-EYE</code></li>
		/// <li><code>SLOW</code></li>
		/// <li><code>NEW_TTL</code></li>
		/// <li><code>REAR</code></li>
		/// <li><code>REAR SLOW</code></li>
		/// </ul>
		/// Note: when TAG_AUTO_FLASH_MODE is blank (whitespace), Nikon Browser displays "Flash Sync Mode: Not Attached"
		/// </remarks>
		public const int TagFlashSyncMode = unchecked((int)(0x0008));

		/// <summary>The type of flash used in the photograph, as a string.</summary>
		/// <remarks>
		/// The type of flash used in the photograph, as a string.  Examples include:
		/// <ul>
		/// <li><code></code></li>
		/// <li><code>Built-in,TTL</code></li>
		/// <li><code>NEW_TTL</code> Nikon Browser interprets as "D-TTL"</li>
		/// <li><code>Built-in,M</code></li>
		/// <li><code>Optional,TTL</code> with speedlight SB800, flash sync mode as "NORMAL"</li>
		/// </ul>
		/// </remarks>
		public const int TagAutoFlashMode = unchecked((int)(0x0009));

		/// <summary>An unknown tag, as a rational.</summary>
		/// <remarks>
		/// An unknown tag, as a rational.  Several values given here:
		/// http://gvsoft.homedns.org/exif/makernote-nikon-type2.html#0x000b
		/// </remarks>
		public const int TagUnknown34 = unchecked((int)(0x000A));

		/// <summary>The camera's white balance bias setting, as an uint16 array having either one or two elements.</summary>
		/// <remarks>
		/// The camera's white balance bias setting, as an uint16 array having either one or two elements.
		/// <ul>
		/// <li><code>0</code></li>
		/// <li><code>1</code></li>
		/// <li><code>-3</code></li>
		/// <li><code>-2</code></li>
		/// <li><code>-1</code></li>
		/// <li><code>0,0</code></li>
		/// <li><code>1,0</code></li>
		/// <li><code>5,-5</code></li>
		/// </ul>
		/// </remarks>
		public const int TagCameraWhiteBalanceFine = unchecked((int)(0x000B));

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
		public const int TagCameraWhiteBalanceRbCoeff = unchecked((int)(0x000C));

		/// <summary>The camera's program shift setting, as an array of four integers.</summary>
		/// <remarks>
		/// The camera's program shift setting, as an array of four integers.
		/// The value, in EV, is calculated as <code>a*b/c</code>.
		/// <ul>
		/// <li><code>0,1,3,0</code> = 0 EV</li>
		/// <li><code>1,1,3,0</code> = 0.33 EV</li>
		/// <li><code>-3,1,3,0</code> = -1 EV</li>
		/// <li><code>1,1,2,0</code> = 0.5 EV</li>
		/// <li><code>2,1,6,0</code> = 0.33 EV</li>
		/// </ul>
		/// </remarks>
		public const int TagProgramShift = unchecked((int)(0x000D));

		/// <summary>The exposure difference, as an array of four integers.</summary>
		/// <remarks>
		/// The exposure difference, as an array of four integers.
		/// The value, in EV, is calculated as <code>a*b/c</code>.
		/// <ul>
		/// <li><code>-105,1,12,0</code> = -8.75 EV</li>
		/// <li><code>-72,1,12,0</code> = -6.00 EV</li>
		/// <li><code>-11,1,12,0</code> = -0.92 EV</li>
		/// </ul>
		/// </remarks>
		public const int TagExposureDifference = unchecked((int)(0x000E));

		/// <summary>The camera's ISO mode, as an uppercase string.</summary>
		/// <remarks>
		/// The camera's ISO mode, as an uppercase string.
		/// <ul>
		/// <li><code>AUTO</code></code></li>
		/// <li><code>MANUAL</code></li>
		/// </ul>
		/// </remarks>
		public const int TagIsoMode = unchecked((int)(0x000F));

		/// <summary>Added during merge of Type2 & Type3.</summary>
		/// <remarks>Added during merge of Type2 & Type3.  May apply to earlier models, such as E990 and D1.</remarks>
		public const int TagDataDump = unchecked((int)(0x0010));

		/// <summary>
		/// Preview to another IFD (?)
		/// <p/>
		/// Details here: http://gvsoft.homedns.org/exif/makernote-nikon-2-tag0x0011.html
		/// // TODO if this is another IFD, decode it
		/// </summary>
		public const int TagPreviewIfd = unchecked((int)(0x0011));

		/// <summary>The flash compensation, as an array of four integers.</summary>
		/// <remarks>
		/// The flash compensation, as an array of four integers.
		/// The value, in EV, is calculated as <code>a*b/c</code>.
		/// <ul>
		/// <li><code>-18,1,6,0</code> = -3 EV</li>
		/// <li><code>4,1,6,0</code> = 0.67 EV</li>
		/// <li><code>6,1,6,0</code> = 1 EV</li>
		/// </ul>
		/// </remarks>
		public const int TagAutoFlashCompensation = unchecked((int)(0x0012));

		/// <summary>The requested ISO value, as an array of two integers.</summary>
		/// <remarks>
		/// The requested ISO value, as an array of two integers.
		/// <ul>
		/// <li><code>0,0</code></li>
		/// <li><code>0,125</code></li>
		/// <li><code>1,2500</code></li>
		/// </ul>
		/// </remarks>
		public const int TagIsoRequested = unchecked((int)(0x0013));

		/// <summary>Defines the photo corner coordinates, in 8 bytes.</summary>
		/// <remarks>
		/// Defines the photo corner coordinates, in 8 bytes.  Treated as four 16-bit integers, they
		/// decode as: top-left (x,y); bot-right (x,y)
		/// - 0 0 49163 53255
		/// - 0 0 3008 2000 (the image dimensions were 3008x2000) (D70)
		/// <ul>
		/// <li><code>0,0,4288,2848</code> The max resolution of the D300 camera</li>
		/// <li><code>0,0,3008,2000</code> The max resolution of the D70 camera</li>
		/// <li><code>0,0,4256,2832</code> The max resolution of the D3 camera</li>
		/// </ul>
		/// </remarks>
		public const int TagImageBoundary = unchecked((int)(0x0016));

		/// <summary>The flash exposure compensation, as an array of four integers.</summary>
		/// <remarks>
		/// The flash exposure compensation, as an array of four integers.
		/// The value, in EV, is calculated as <code>a*b/c</code>.
		/// <ul>
		/// <li><code>0,0,0,0</code> = 0 EV</li>
		/// <li><code>0,1,6,0</code> = 0 EV</li>
		/// <li><code>4,1,6,0</code> = 0.67 EV</li>
		/// </ul>
		/// </remarks>
		public const int TagFlashExposureCompensation = unchecked((int)(0x0017));

		/// <summary>The flash bracket compensation, as an array of four integers.</summary>
		/// <remarks>
		/// The flash bracket compensation, as an array of four integers.
		/// The value, in EV, is calculated as <code>a*b/c</code>.
		/// <ul>
		/// <li><code>0,0,0,0</code> = 0 EV</li>
		/// <li><code>0,1,6,0</code> = 0 EV</li>
		/// <li><code>4,1,6,0</code> = 0.67 EV</li>
		/// </ul>
		/// </remarks>
		public const int TagFlashBracketCompensation = unchecked((int)(0x0018));

		/// <summary>The AE bracket compensation, as a rational number.</summary>
		/// <remarks>
		/// The AE bracket compensation, as a rational number.
		/// <ul>
		/// <li><code>0/0</code></li>
		/// <li><code>0/1</code></li>
		/// <li><code>0/6</code></li>
		/// <li><code>4/6</code></li>
		/// <li><code>6/6</code></li>
		/// </ul>
		/// </remarks>
		public const int TagAeBracketCompensation = unchecked((int)(0x0019));

		/// <summary>Flash mode, as a string.</summary>
		/// <remarks>
		/// Flash mode, as a string.
		/// <ul>
		/// <li><code></code></li>
		/// <li><code>Red Eye Reduction</code></li>
		/// <li><code>D-Lighting</code></li>
		/// <li><code>Distortion control</code></li>
		/// </ul>
		/// </remarks>
		public const int TagFlashMode = unchecked((int)(0x001a));

		public const int TagCropHighSpeed = unchecked((int)(0x001b));

		public const int TagExposureTuning = unchecked((int)(0x001c));

		/// <summary>The camera's serial number, as a string.</summary>
		/// <remarks>
		/// The camera's serial number, as a string.
		/// Note that D200 is always blank, and D50 is always <code>"D50"</code>.
		/// </remarks>
		public const int TagCameraSerialNumber = unchecked((int)(0x001d));

		/// <summary>The camera's color space setting.</summary>
		/// <remarks>
		/// The camera's color space setting.
		/// <ul>
		/// <li><code>1</code> sRGB</li>
		/// <li><code>2</code> Adobe RGB</li>
		/// </ul>
		/// </remarks>
		public const int TagColorSpace = unchecked((int)(0x001e));

		public const int TagVrInfo = unchecked((int)(0x001f));

		public const int TagImageAuthentication = unchecked((int)(0x0020));

		public const int TagUnknown35 = unchecked((int)(0x0021));

		/// <summary>The active D-Lighting setting.</summary>
		/// <remarks>
		/// The active D-Lighting setting.
		/// <ul>
		/// <li><code>0</code> Off</li>
		/// <li><code>1</code> Low</li>
		/// <li><code>3</code> Normal</li>
		/// <li><code>5</code> High</li>
		/// <li><code>7</code> Extra High</li>
		/// <li><code>65535</code> Auto</li>
		/// </ul>
		/// </remarks>
		public const int TagActiveDLighting = unchecked((int)(0x0022));

		public const int TagPictureControl = unchecked((int)(0x0023));

		public const int TagWorldTime = unchecked((int)(0x0024));

		public const int TagIsoInfo = unchecked((int)(0x0025));

		public const int TagUnknown36 = unchecked((int)(0x0026));

		public const int TagUnknown37 = unchecked((int)(0x0027));

		public const int TagUnknown38 = unchecked((int)(0x0028));

		public const int TagUnknown39 = unchecked((int)(0x0029));

		/// <summary>The camera's vignette control setting.</summary>
		/// <remarks>
		/// The camera's vignette control setting.
		/// <ul>
		/// <li><code>0</code> Off</li>
		/// <li><code>1</code> Low</li>
		/// <li><code>3</code> Normal</li>
		/// <li><code>5</code> High</li>
		/// </ul>
		/// </remarks>
		public const int TagVignetteControl = unchecked((int)(0x002a));

		public const int TagUnknown40 = unchecked((int)(0x002b));

		public const int TagUnknown41 = unchecked((int)(0x002c));

		public const int TagUnknown42 = unchecked((int)(0x002d));

		public const int TagUnknown43 = unchecked((int)(0x002e));

		public const int TagUnknown44 = unchecked((int)(0x002f));

		public const int TagUnknown45 = unchecked((int)(0x0030));

		public const int TagUnknown46 = unchecked((int)(0x0031));

		/// <summary>The camera's image adjustment setting, as a string.</summary>
		/// <remarks>
		/// The camera's image adjustment setting, as a string.
		/// <ul>
		/// <li><code>AUTO</code></li>
		/// <li><code>CONTRAST(+)</code></li>
		/// <li><code>CONTRAST(-)</code></li>
		/// <li><code>NORMAL</code></li>
		/// <li><code>B & W</code></li>
		/// <li><code>BRIGHTNESS(+)</code></li>
		/// <li><code>BRIGHTNESS(-)</code></li>
		/// <li><code>SEPIA</code></li>
		/// </ul>
		/// </remarks>
		public const int TagImageAdjustment = unchecked((int)(0x0080));

		/// <summary>The camera's tone compensation setting, as a string.</summary>
		/// <remarks>
		/// The camera's tone compensation setting, as a string.
		/// <ul>
		/// <li><code>NORMAL</code></li>
		/// <li><code>LOW</code></li>
		/// <li><code>MED.L</code></li>
		/// <li><code>MED.H</code></li>
		/// <li><code>HIGH</code></li>
		/// <li><code>AUTO</code></li>
		/// </ul>
		/// </remarks>
		public const int TagCameraToneCompensation = unchecked((int)(0x0081));

		/// <summary>A description of any auxiliary lens, as a string.</summary>
		/// <remarks>
		/// A description of any auxiliary lens, as a string.
		/// <ul>
		/// <li><code>OFF</code></li>
		/// <li><code>FISHEYE 1</code></li>
		/// <li><code>FISHEYE 2</code></li>
		/// <li><code>TELEPHOTO 2</code></li>
		/// <li><code>WIDE ADAPTER</code></li>
		/// </ul>
		/// </remarks>
		public const int TagAdapter = unchecked((int)(0x0082));

		/// <summary>The type of lens used, as a byte.</summary>
		/// <remarks>
		/// The type of lens used, as a byte.
		/// <ul>
		/// <li><code>0x00</code> AF</li>
		/// <li><code>0x01</code> MF</li>
		/// <li><code>0x02</code> D</li>
		/// <li><code>0x06</code> G, D</li>
		/// <li><code>0x08</code> VR</li>
		/// <li><code>0x0a</code> VR, D</li>
		/// <li><code>0x0e</code> VR, G, D</li>
		/// </ul>
		/// </remarks>
		public const int TagLensType = unchecked((int)(0x0083));

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
		public const int TagLens = unchecked((int)(0x0084));

		/// <summary>Added during merge of Type2 & Type3.</summary>
		/// <remarks>Added during merge of Type2 & Type3.  May apply to earlier models, such as E990 and D1.</remarks>
		public const int TagManualFocusDistance = unchecked((int)(0x0085));

		/// <summary>The amount of digital zoom used.</summary>
		public const int TagDigitalZoom = unchecked((int)(0x0086));

		/// <summary>Whether the flash was used in this image.</summary>
		/// <remarks>
		/// Whether the flash was used in this image.
		/// <ul>
		/// <li><code>0</code> Flash Not Used</li>
		/// <li><code>1</code> Manual Flash</li>
		/// <li><code>3</code> Flash Not Ready</li>
		/// <li><code>7</code> External Flash</li>
		/// <li><code>8</code> Fired, Commander Mode</li>
		/// <li><code>9</code> Fired, TTL Mode</li>
		/// </ul>
		/// </remarks>
		public const int TagFlashUsed = unchecked((int)(0x0087));

		/// <summary>The position of the autofocus target.</summary>
		public const int TagAfFocusPosition = unchecked((int)(0x0088));

		/// <summary>The camera's shooting mode.</summary>
		/// <remarks>
		/// The camera's shooting mode.
		/// <p/>
		/// A bit-array with:
		/// <ul>
		/// <li><code>0</code> Single Frame</li>
		/// <li><code>1</code> Continuous</li>
		/// <li><code>2</code> Delay</li>
		/// <li><code>8</code> PC Control</li>
		/// <li><code>16</code> Exposure Bracketing</li>
		/// <li><code>32</code> Auto ISO</li>
		/// <li><code>64</code> White-Balance Bracketing</li>
		/// <li><code>128</code> IR Control</li>
		/// </ul>
		/// </remarks>
		public const int TagShootingMode = unchecked((int)(0x0089));

		public const int TagUnknown20 = unchecked((int)(0x008A));

		/// <summary>Lens stops, as an array of four integers.</summary>
		/// <remarks>
		/// Lens stops, as an array of four integers.
		/// The value, in EV, is calculated as <code>a*b/c</code>.
		/// <ul>
		/// <li><code>64,1,12,0</code> = 5.33 EV</li>
		/// <li><code>72,1,12,0</code> = 6 EV</li>
		/// </ul>
		/// </remarks>
		public const int TagLensStops = unchecked((int)(0x008B));

		public const int TagContrastCurve = unchecked((int)(0x008C));

		/// <summary>The color space as set in the camera, as a string.</summary>
		/// <remarks>
		/// The color space as set in the camera, as a string.
		/// <ul>
		/// <li><code>MODE1</code> = Mode 1 (sRGB)</li>
		/// <li><code>MODE1a</code> = Mode 1 (sRGB)</li>
		/// <li><code>MODE2</code> = Mode 2 (Adobe RGB)</li>
		/// <li><code>MODE3</code> = Mode 2 (sRGB): Higher Saturation</li>
		/// <li><code>MODE3a</code> = Mode 2 (sRGB): Higher Saturation</li>
		/// <li><code>B & W</code> = B & W</li>
		/// </ul>
		/// </remarks>
		public const int TagCameraColorMode = unchecked((int)(0x008D));

		public const int TagUnknown47 = unchecked((int)(0x008E));

		/// <summary>The camera's scene mode, as a string.</summary>
		/// <remarks>
		/// The camera's scene mode, as a string.  Examples include:
		/// <ul>
		/// <li><code>BEACH/SNOW</code></li>
		/// <li><code>CLOSE UP</code></li>
		/// <li><code>NIGHT PORTRAIT</code></li>
		/// <li><code>PORTRAIT</code></li>
		/// <li><code>ANTI-SHAKE</code></li>
		/// <li><code>BACK LIGHT</code></li>
		/// <li><code>BEST FACE</code></li>
		/// <li><code>BEST</code></li>
		/// <li><code>COPY</code></li>
		/// <li><code>DAWN/DUSK</code></li>
		/// <li><code>FACE-PRIORITY</code></li>
		/// <li><code>FIREWORKS</code></li>
		/// <li><code>FOOD</code></li>
		/// <li><code>HIGH SENS.</code></li>
		/// <li><code>LAND SCAPE</code></li>
		/// <li><code>MUSEUM</code></li>
		/// <li><code>PANORAMA ASSIST</code></li>
		/// <li><code>PARTY/INDOOR</code></li>
		/// <li><code>SCENE AUTO</code></li>
		/// <li><code>SMILE</code></li>
		/// <li><code>SPORT</code></li>
		/// <li><code>SPORT CONT.</code></li>
		/// <li><code>SUNSET</code></li>
		/// </ul>
		/// </remarks>
		public const int TagSceneMode = unchecked((int)(0x008F));

		/// <summary>The lighting type, as a string.</summary>
		/// <remarks>
		/// The lighting type, as a string.  Examples include:
		/// <ul>
		/// <li><code></code></li>
		/// <li><code>NATURAL</code></li>
		/// <li><code>SPEEDLIGHT</code></li>
		/// <li><code>COLORED</code></li>
		/// <li><code>MIXED</code></li>
		/// <li><code>NORMAL</code></li>
		/// </ul>
		/// </remarks>
		public const int TagLightSource = unchecked((int)(0x0090));

		/// <summary>Advertised as ASCII, but actually isn't.</summary>
		/// <remarks>
		/// Advertised as ASCII, but actually isn't.  A variable number of bytes (eg. 18 to 533).  Actual number of bytes
		/// appears fixed for a given camera model.
		/// </remarks>
		public const int TagShotInfo = unchecked((int)(0x0091));

		/// <summary>The hue adjustment as set in the camera.</summary>
		/// <remarks>The hue adjustment as set in the camera.  Values observed are either 0 or 3.</remarks>
		public const int TagCameraHueAdjustment = unchecked((int)(0x0092));

		/// <summary>The NEF (RAW) compression.</summary>
		/// <remarks>
		/// The NEF (RAW) compression.  Examples include:
		/// <ul>
		/// <li><code>1</code> Lossy (Type 1)</li>
		/// <li><code>2</code> Uncompressed</li>
		/// <li><code>3</code> Lossless</li>
		/// <li><code>4</code> Lossy (Type 2)</li>
		/// </ul>
		/// </remarks>
		public const int TagNefCompression = unchecked((int)(0x0093));

		/// <summary>The saturation level, as a signed integer.</summary>
		/// <remarks>
		/// The saturation level, as a signed integer.  Examples include:
		/// <ul>
		/// <li><code>+3</code></li>
		/// <li><code>+2</code></li>
		/// <li><code>+1</code></li>
		/// <li><code>0</code> Normal</li>
		/// <li><code>-1</code></li>
		/// <li><code>-2</code></li>
		/// <li><code>-3</code> (B&W)</li>
		/// </ul>
		/// </remarks>
		public const int TagSaturation = unchecked((int)(0x0094));

		/// <summary>The type of noise reduction, as a string.</summary>
		/// <remarks>
		/// The type of noise reduction, as a string.  Examples include:
		/// <ul>
		/// <li><code>OFF</code></li>
		/// <li><code>FPNR</code></li>
		/// </ul>
		/// </remarks>
		public const int TagNoiseReduction = unchecked((int)(0x0095));

		public const int TagLinearizationTable = unchecked((int)(0x0096));

		public const int TagColorBalance = unchecked((int)(0x0097));

		public const int TagLensData = unchecked((int)(0x0098));

		/// <summary>The NEF (RAW) thumbnail size, as an integer array with two items representing [width,height].</summary>
		public const int TagNefThumbnailSize = unchecked((int)(0x0099));

		/// <summary>The sensor pixel size, as a pair of rational numbers.</summary>
		public const int TagSensorPixelSize = unchecked((int)(0x009A));

		public const int TagUnknown10 = unchecked((int)(0x009B));

		public const int TagSceneAssist = unchecked((int)(0x009C));

		public const int TagUnknown11 = unchecked((int)(0x009D));

		public const int TagRetouchHistory = unchecked((int)(0x009E));

		public const int TagUnknown12 = unchecked((int)(0x009F));

		/// <summary>The camera serial number, as a string.</summary>
		/// <remarks>
		/// The camera serial number, as a string.
		/// <ul>
		/// <li><code>NO= 00002539</code></li>
		/// <li><code>NO= -1000d71</code></li>
		/// <li><code>PKG597230621263</code></li>
		/// <li><code>PKG5995671330625116</code></li>
		/// <li><code>PKG49981281631130677</code></li>
		/// <li><code>BU672230725063</code></li>
		/// <li><code>NO= 200332c7</code></li>
		/// <li><code>NO= 30045efe</code></li>
		/// </ul>
		/// </remarks>
		public const int TagCameraSerialNumber2 = unchecked((int)(0x00A0));

		public const int TagImageDataSize = unchecked((int)(0x00A2));

		public const int TagUnknown27 = unchecked((int)(0x00A3));

		public const int TagUnknown28 = unchecked((int)(0x00A4));

		public const int TagImageCount = unchecked((int)(0x00A5));

		public const int TagDeletedImageCount = unchecked((int)(0x00A6));

		/// <summary>The number of total shutter releases.</summary>
		/// <remarks>The number of total shutter releases.  This value increments for each exposure (observed on D70).</remarks>
		public const int TagExposureSequenceNumber = unchecked((int)(0x00A7));

		public const int TagFlashInfo = unchecked((int)(0x00A8));

		/// <summary>The camera's image optimisation, as a string.</summary>
		/// <remarks>
		/// The camera's image optimisation, as a string.
		/// <ul>
		/// <li><code></code></li>
		/// <li><code>NORMAL</code></li>
		/// <li><code>CUSTOM</code></li>
		/// <li><code>BLACK AND WHITE</code></li>
		/// <li><code>LAND SCAPE</code></li>
		/// <li><code>MORE VIVID</code></li>
		/// <li><code>PORTRAIT</code></li>
		/// <li><code>SOFT</code></li>
		/// <li><code>VIVID</code></li>
		/// </ul>
		/// </remarks>
		public const int TagImageOptimisation = unchecked((int)(0x00A9));

		/// <summary>The camera's saturation level, as a string.</summary>
		/// <remarks>
		/// The camera's saturation level, as a string.
		/// <ul>
		/// <li><code></code></li>
		/// <li><code>NORMAL</code></li>
		/// <li><code>AUTO</code></li>
		/// <li><code>ENHANCED</code></li>
		/// <li><code>MODERATE</code></li>
		/// </ul>
		/// </remarks>
		public const int TagSaturation2 = unchecked((int)(0x00AA));

		/// <summary>The camera's digital vari-program setting, as a string.</summary>
		/// <remarks>
		/// The camera's digital vari-program setting, as a string.
		/// <ul>
		/// <li><code></code></li>
		/// <li><code>AUTO</code></li>
		/// <li><code>AUTO(FLASH OFF)</code></li>
		/// <li><code>CLOSE UP</code></li>
		/// <li><code>LANDSCAPE</code></li>
		/// <li><code>NIGHT PORTRAIT</code></li>
		/// <li><code>PORTRAIT</code></li>
		/// <li><code>SPORT</code></li>
		/// </ul>
		/// </remarks>
		public const int TagDigitalVariProgram = unchecked((int)(0x00AB));

		/// <summary>The camera's digital vari-program setting, as a string.</summary>
		/// <remarks>
		/// The camera's digital vari-program setting, as a string.
		/// <ul>
		/// <li><code></code></li>
		/// <li><code>VR-ON</code></li>
		/// <li><code>VR-OFF</code></li>
		/// <li><code>VR-HYBRID</code></li>
		/// <li><code>VR-ACTIVE</code></li>
		/// </ul>
		/// </remarks>
		public const int TagImageStabilisation = unchecked((int)(0x00AC));

		/// <summary>The camera's digital vari-program setting, as a string.</summary>
		/// <remarks>
		/// The camera's digital vari-program setting, as a string.
		/// <ul>
		/// <li><code></code></li>
		/// <li><code>HYBRID</code></li>
		/// <li><code>STANDARD</code></li>
		/// </ul>
		/// </remarks>
		public const int TagAfResponse = unchecked((int)(0x00AD));

		public const int TagUnknown29 = unchecked((int)(0x00AE));

		public const int TagUnknown30 = unchecked((int)(0x00AF));

		public const int TagMultiExposure = unchecked((int)(0x00B0));

		/// <summary>The camera's high ISO noise reduction setting, as an integer.</summary>
		/// <remarks>
		/// The camera's high ISO noise reduction setting, as an integer.
		/// <ul>
		/// <li><code>0</code> Off</li>
		/// <li><code>1</code> Minimal</li>
		/// <li><code>2</code> Low</li>
		/// <li><code>4</code> Normal</li>
		/// <li><code>6</code> High</li>
		/// </ul>
		/// </remarks>
		public const int TagHighIsoNoiseReduction = unchecked((int)(0x00B1));

		public const int TagUnknown31 = unchecked((int)(0x00B2));

		public const int TagUnknown32 = unchecked((int)(0x00B3));

		public const int TagUnknown33 = unchecked((int)(0x00B4));

		public const int TagUnknown48 = unchecked((int)(0x00B5));

		public const int TagPowerUpTime = unchecked((int)(0x00B6));

		public const int TagAfInfo2 = unchecked((int)(0x00B7));

		public const int TagFileInfo = unchecked((int)(0x00B8));

		public const int TagAfTune = unchecked((int)(0x00B9));

		public const int TagUnknown49 = unchecked((int)(0x00BB));

		public const int TagUnknown50 = unchecked((int)(0x00BD));

		public const int TagUnknown51 = unchecked((int)(0x0103));

		public const int TagPrintIm = unchecked((int)(0x0E00));

		/// <summary>Data about changes set by Nikon Capture Editor.</summary>
		/// <remarks>
		/// Data about changes set by Nikon Capture Editor.
		/// Values observed
		/// </remarks>
		public const int TagNikonCaptureData = unchecked((int)(0x0E01));

		public const int TagUnknown52 = unchecked((int)(0x0E05));

		public const int TagUnknown53 = unchecked((int)(0x0E08));

		public const int TagNikonCaptureVersion = unchecked((int)(0x0E09));

		public const int TagNikonCaptureOffsets = unchecked((int)(0x0E0E));

		public const int TagNikonScan = unchecked((int)(0x0E10));

		public const int TagUnknown54 = unchecked((int)(0x0E19));

		public const int TagNefBitDepth = unchecked((int)(0x0E22));

		public const int TagUnknown55 = unchecked((int)(0x0E23));

		[NotNull]
		protected internal static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>();

		static NikonType2MakernoteDirectory()
		{
			_tagNameMap.Put(TagFirmwareVersion, "Firmware Version");
			_tagNameMap.Put(TagIso1, "ISO");
			_tagNameMap.Put(TagQualityAndFileFormat, "Quality & File Format");
			_tagNameMap.Put(TagCameraWhiteBalance, "White Balance");
			_tagNameMap.Put(TagCameraSharpening, "Sharpening");
			_tagNameMap.Put(TagAfType, "AF Type");
			_tagNameMap.Put(TagCameraWhiteBalanceFine, "White Balance Fine");
			_tagNameMap.Put(TagCameraWhiteBalanceRbCoeff, "White Balance RB Coefficients");
			_tagNameMap.Put(TagIsoRequested, "ISO");
			_tagNameMap.Put(TagIsoMode, "ISO Mode");
			_tagNameMap.Put(TagDataDump, "Data Dump");
			_tagNameMap.Put(TagProgramShift, "Program Shift");
			_tagNameMap.Put(TagExposureDifference, "Exposure Difference");
			_tagNameMap.Put(TagPreviewIfd, "Preview IFD");
			_tagNameMap.Put(TagLensType, "Lens Type");
			_tagNameMap.Put(TagFlashUsed, "Flash Used");
			_tagNameMap.Put(TagAfFocusPosition, "AF Focus Position");
			_tagNameMap.Put(TagShootingMode, "Shooting Mode");
			_tagNameMap.Put(TagLensStops, "Lens Stops");
			_tagNameMap.Put(TagContrastCurve, "Contrast Curve");
			_tagNameMap.Put(TagLightSource, "Light source");
			_tagNameMap.Put(TagShotInfo, "Shot Info");
			_tagNameMap.Put(TagColorBalance, "Color Balance");
			_tagNameMap.Put(TagLensData, "Lens Data");
			_tagNameMap.Put(TagNefThumbnailSize, "NEF Thumbnail Size");
			_tagNameMap.Put(TagSensorPixelSize, "Sensor Pixel Size");
			_tagNameMap.Put(TagUnknown10, "Unknown 10");
			_tagNameMap.Put(TagSceneAssist, "Scene Assist");
			_tagNameMap.Put(TagUnknown11, "Unknown 11");
			_tagNameMap.Put(TagRetouchHistory, "Retouch History");
			_tagNameMap.Put(TagUnknown12, "Unknown 12");
			_tagNameMap.Put(TagFlashSyncMode, "Flash Sync Mode");
			_tagNameMap.Put(TagAutoFlashMode, "Auto Flash Mode");
			_tagNameMap.Put(TagAutoFlashCompensation, "Auto Flash Compensation");
			_tagNameMap.Put(TagExposureSequenceNumber, "Exposure Sequence Number");
			_tagNameMap.Put(TagColorMode, "Color Mode");
			_tagNameMap.Put(TagUnknown20, "Unknown 20");
			_tagNameMap.Put(TagImageBoundary, "Image Boundary");
			_tagNameMap.Put(TagFlashExposureCompensation, "Flash Exposure Compensation");
			_tagNameMap.Put(TagFlashBracketCompensation, "Flash Bracket Compensation");
			_tagNameMap.Put(TagAeBracketCompensation, "AE Bracket Compensation");
			_tagNameMap.Put(TagFlashMode, "Flash Mode");
			_tagNameMap.Put(TagCropHighSpeed, "Crop High Speed");
			_tagNameMap.Put(TagExposureTuning, "Exposure Tuning");
			_tagNameMap.Put(TagCameraSerialNumber, "Camera Serial Number");
			_tagNameMap.Put(TagColorSpace, "Color Space");
			_tagNameMap.Put(TagVrInfo, "VR Info");
			_tagNameMap.Put(TagImageAuthentication, "Image Authentication");
			_tagNameMap.Put(TagUnknown35, "Unknown 35");
			_tagNameMap.Put(TagActiveDLighting, "Active D-Lighting");
			_tagNameMap.Put(TagPictureControl, "Picture Control");
			_tagNameMap.Put(TagWorldTime, "World Time");
			_tagNameMap.Put(TagIsoInfo, "ISO Info");
			_tagNameMap.Put(TagUnknown36, "Unknown 36");
			_tagNameMap.Put(TagUnknown37, "Unknown 37");
			_tagNameMap.Put(TagUnknown38, "Unknown 38");
			_tagNameMap.Put(TagUnknown39, "Unknown 39");
			_tagNameMap.Put(TagVignetteControl, "Vignette Control");
			_tagNameMap.Put(TagUnknown40, "Unknown 40");
			_tagNameMap.Put(TagUnknown41, "Unknown 41");
			_tagNameMap.Put(TagUnknown42, "Unknown 42");
			_tagNameMap.Put(TagUnknown43, "Unknown 43");
			_tagNameMap.Put(TagUnknown44, "Unknown 44");
			_tagNameMap.Put(TagUnknown45, "Unknown 45");
			_tagNameMap.Put(TagUnknown46, "Unknown 46");
			_tagNameMap.Put(TagUnknown47, "Unknown 47");
			_tagNameMap.Put(TagSceneMode, "Scene Mode");
			_tagNameMap.Put(TagCameraSerialNumber2, "Camera Serial Number");
			_tagNameMap.Put(TagImageDataSize, "Image Data Size");
			_tagNameMap.Put(TagUnknown27, "Unknown 27");
			_tagNameMap.Put(TagUnknown28, "Unknown 28");
			_tagNameMap.Put(TagImageCount, "Image Count");
			_tagNameMap.Put(TagDeletedImageCount, "Deleted Image Count");
			_tagNameMap.Put(TagSaturation2, "Saturation");
			_tagNameMap.Put(TagDigitalVariProgram, "Digital Vari Program");
			_tagNameMap.Put(TagImageStabilisation, "Image Stabilisation");
			_tagNameMap.Put(TagAfResponse, "AF Response");
			_tagNameMap.Put(TagUnknown29, "Unknown 29");
			_tagNameMap.Put(TagUnknown30, "Unknown 30");
			_tagNameMap.Put(TagMultiExposure, "Multi Exposure");
			_tagNameMap.Put(TagHighIsoNoiseReduction, "High ISO Noise Reduction");
			_tagNameMap.Put(TagUnknown31, "Unknown 31");
			_tagNameMap.Put(TagUnknown32, "Unknown 32");
			_tagNameMap.Put(TagUnknown33, "Unknown 33");
			_tagNameMap.Put(TagUnknown48, "Unknown 48");
			_tagNameMap.Put(TagPowerUpTime, "Power Up Time");
			_tagNameMap.Put(TagAfInfo2, "AF Info 2");
			_tagNameMap.Put(TagFileInfo, "File Info");
			_tagNameMap.Put(TagAfTune, "AF Tune");
			_tagNameMap.Put(TagFlashInfo, "Flash Info");
			_tagNameMap.Put(TagImageOptimisation, "Image Optimisation");
			_tagNameMap.Put(TagImageAdjustment, "Image Adjustment");
			_tagNameMap.Put(TagCameraToneCompensation, "Tone Compensation");
			_tagNameMap.Put(TagAdapter, "Adapter");
			_tagNameMap.Put(TagLens, "Lens");
			_tagNameMap.Put(TagManualFocusDistance, "Manual Focus Distance");
			_tagNameMap.Put(TagDigitalZoom, "Digital Zoom");
			_tagNameMap.Put(TagCameraColorMode, "Colour Mode");
			_tagNameMap.Put(TagCameraHueAdjustment, "Camera Hue Adjustment");
			_tagNameMap.Put(TagNefCompression, "NEF Compression");
			_tagNameMap.Put(TagSaturation, "Saturation");
			_tagNameMap.Put(TagNoiseReduction, "Noise Reduction");
			_tagNameMap.Put(TagLinearizationTable, "Linearization Table");
			_tagNameMap.Put(TagNikonCaptureData, "Nikon Capture Data");
			_tagNameMap.Put(TagUnknown49, "Unknown 49");
			_tagNameMap.Put(TagUnknown50, "Unknown 50");
			_tagNameMap.Put(TagUnknown51, "Unknown 51");
			_tagNameMap.Put(TagPrintIm, "Print IM");
			_tagNameMap.Put(TagUnknown52, "Unknown 52");
			_tagNameMap.Put(TagUnknown53, "Unknown 53");
			_tagNameMap.Put(TagNikonCaptureVersion, "Nikon Capture Version");
			_tagNameMap.Put(TagNikonCaptureOffsets, "Nikon Capture Offsets");
			_tagNameMap.Put(TagNikonScan, "Nikon Scan");
			_tagNameMap.Put(TagUnknown54, "Unknown 54");
			_tagNameMap.Put(TagNefBitDepth, "NEF Bit Depth");
			_tagNameMap.Put(TagUnknown55, "Unknown 55");
		}

		public NikonType2MakernoteDirectory()
		{
			this.SetDescriptor(new NikonType2MakernoteDescriptor(this));
		}

		[NotNull]
		public override string GetName()
		{
			return "Nikon Makernote";
		}

		[NotNull]
		protected internal override Dictionary<int, string> GetTagNameMap()
		{
			return _tagNameMap;
		}
	}
}
